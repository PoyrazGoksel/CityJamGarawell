using System;
using System.Collections.Generic;
using System.Linq;
using Components.Buildings;
using Events;
using Extensions.System;
using Extensions.Unity;
using Extensions.Unity.Entities;
using Extensions.Unity.MonoHelper;
using Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using ViewModels;
using Zenject;

namespace UI.Main.Components
{
    public class BuildingQueue : EventListenerMono
    {
        private const int MinMatchCount = 3;
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [Inject] private PlayerVM PlayerVM{get;set;}
        [Inject] private LevelEvents LevelEvents{get;set;}
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [SerializeField] private List<BuildingRowSerialized> _buildingRows;
        [ShowInInspector] private Dictionary<IBuildingRow, IBuilding> _rowBuildingDict = new();
        private List<IGrouping<int, IBuilding>> _groupedBuildings;
        private Settings _settings;

        private void Awake()
        {
            _settings = ProjectSettings.BuildingQueueSettings;
            CreateBuildingQueue();
        }

        private void CreateBuildingQueue()
        {
            foreach(IBuildingRow buildingRowSerialized in _buildingRows.Select(e => e.Value()))
            {
                if(_rowBuildingDict.TryAdd(buildingRowSerialized, null)) {}
            }
        }

        private bool TryFitBuilding(IBuilding arg0)
        {
            if(_rowBuildingDict.Values.Count == _rowBuildingDict.Count.ToIndex())
            {
                int incomingMatch = _rowBuildingDict.Values.Count(e => e.ID == arg0.ID);

                if(incomingMatch <= 2)
                {
                    Debug.LogWarning("FailCond: NoRows");
                    LevelEvents.NoRowsLeft?.Invoke();
                }
            }
            
            if(TryGetEmptyRow(out IBuildingRow emptyRow))
            {
                AssignBuilding(emptyRow, arg0, true);

                SortBuildings();
                
                return true;
            }
            
            return false;
        }

        private void SortBuildings()
        {
            List<IBuildingRow> keysList = _rowBuildingDict.Keys.ToList();

            _groupedBuildings = _rowBuildingDict.Values.NotNull().GroupBy(e => e.ID)
            .OrderBy(group => group.Key)
            .ToList();

            int index = 0;

            List<KeyValuePair<IBuildingRow, IBuilding>> newOrder = new();
            
            foreach (IGrouping<int, IBuilding> group in _groupedBuildings)
            {
                foreach (IBuilding buildingToAssign in group.OrderBy(b => b.ID))
                {
                    IBuildingRow thisRow = keysList[index];

                    IBuilding currentBuilding = _rowBuildingDict[thisRow];

                    if(currentBuilding != null)
                    {
                        index ++;
                        continue;
                    }

                    newOrder.Add(new KeyValuePair<IBuildingRow, IBuilding>(thisRow, buildingToAssign));
                    
                    index++;
                }
            }

            foreach(KeyValuePair<IBuildingRow, IBuilding> pair in newOrder)
            {
                AssignBuilding(pair.Key, pair.Value);
            }
        }

        private void CheckForDestroy()
        {
            IGrouping<int, IBuilding> firstToDestroy = _groupedBuildings?.FirstOrDefault
            (e => e.Count() >= MinMatchCount);

            if(firstToDestroy != null)
            {
                DestroyGroup(firstToDestroy);
                
                SortBuildings();
            }
        }

        private void DestroyGroup(IGrouping<int, IBuilding> firstToDestroy)
        {
            List<IBuilding> destroyList = new();
            List<IBuilding> destroyListCurrent = new();

            int lastID = -1;
            
            foreach(KeyValuePair<IBuildingRow,IBuilding> pair in _rowBuildingDict)
            {
                if(lastID == -1)
                {
                    lastID = pair.Value.ID;
                    break;
                }

                if(lastID == pair.Value.ID)
                {
                    destroyListCurrent.Add(pair.Value);

                    if(destroyListCurrent.Count % 3 == 0)
                    {
                        Vector3 destroyPos = Vector3.zero;
                        
                        foreach(IBuilding building in destroyListCurrent)
                        {
                            destroyPos += building.Transform.position;
                            RemoveBuilding(building);
                        }
                        
                        destroyPos /= firstToDestroy.Count();

                        destroyPos += _settings.DestroyAnimOffY * PlayerVM.PlayerCam.Up;
                        
                        foreach(IBuilding building in destroyList)
                        {
                            IBuilding building1 = building;

                            building.DestroyMatch(destroyPos,
                                delegate
                                {
                                    BuildingEvents.PreBuildingDestroy?.Invoke(building1);
                                    building1.Transform.gameObject.Destroy();

                                    Instantiate
                                    (_settings.DestroyParticlePrefab, destroyPos, Quaternion.identity);
                                });
                        }
                        
                        destroyList.AddRange(destroyListCurrent);
                        destroyListCurrent.Clear();
                    }
                }
                else
                {
                    lastID = pair.Value.ID;
                    destroyListCurrent.Clear();
                }
            }
        }

        private void RemoveBuilding(IBuilding arg0) => AssignBuilding(null, arg0);

        private int _incBuildingCount;
        
        private void AssignBuilding(IBuildingRow emptyRow, IBuilding arg0, bool terrainPick = false)
        {
            if(arg0.Row != null) _rowBuildingDict[arg0.Row] = null;

            if(emptyRow != null) _rowBuildingDict[emptyRow] = arg0;

            if(terrainPick)
            {
                _incBuildingCount ++;
                arg0.AssignRow
                (
                    emptyRow,
                    true,
                    delegate
                    {
                        _incBuildingCount --;
                        
                        if(_incBuildingCount == 0)
                        {
                            CheckForDestroy();
                        }
                    }
                );
            }
            else
            {
                arg0.AssignRow(emptyRow);
            }
        }

        private bool TryGetEmptyRow(out IBuildingRow firstEmpty)
        {
            firstEmpty = _rowBuildingDict.FirstOrDefault(e => e.Value == null).Key;

            return firstEmpty != null;
        }

        protected override void RegisterEvents()
        {
            BuildingEvents.BuildingClicked += OnBuildingClicked;
        }

        private void OnBuildingClicked(IBuilding arg0) {TryFitBuilding(arg0);}

        protected override void UnRegisterEvents()
        {
            BuildingEvents.BuildingClicked -= OnBuildingClicked;
        }

        [Serializable]
        public class Settings
        {
            public GameObject DestroyParticlePrefab => _destroyParticlePrefab;
            public float DestroyAnimOffY => _destroyAnimOffY;
            [SerializeField] private GameObject _destroyParticlePrefab;
            [SerializeField] private float _destroyAnimOffY = 10f;
        }
    }

    [Serializable] public class BuildingRowSerialized : SerializedInterface<IBuildingRow> {}
}