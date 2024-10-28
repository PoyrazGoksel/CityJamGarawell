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
        [ReadOnly][SerializeField] private List<BuildingRowSerialized> _buildingRows;
        private Settings _settings;
        private int _incBuildingCount;
        [ShowInInspector] private List<IBuilding> _lastOrder = new();
        [ShowInInspector] private BuildingRows BuildingRows{get;set;}
        [ShowInInspector] private List<IBuilding> Buildings => BuildingRows.Select(e => e.Building).ToList();

        private void Awake()
        {
            _settings = ProjectSettings.BuildingQueueSettings;
            CreateBuildingQueue();
        }

        private void CreateBuildingQueue()
        {
            BuildingRows = new BuildingRows();
            
            foreach(BuildingRowSerialized rowSerialized in _buildingRows)
            {
                BuildingRows.Add(rowSerialized.Value());
            }
        }

        private bool TryFitBuilding(IBuilding arg0)
        {
            if(BuildingRows.BuildingCount() == BuildingRows.Count - 1)
            {
                int incomingMatchCount = BuildingRows.GetSameIDBuildingCount(arg0);

                if(incomingMatchCount <= 2)
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
            _lastOrder = BuildingRows.GetBuildingsGroupedByID().SelectMany(group => group).ToList();

            for(int i = 0; i < _lastOrder.Count; i ++)
            {
                AssignBuilding(BuildingRows[i], _lastOrder[i]);
            }

            DestroyMatches();
        }

        private void DestroyMatches()
        {
            List<IBuilding> matches = new();

            int lastID = -1;
            bool didDestroy = false;

            for(int i = 0; i < BuildingRows.Count; i ++)
            {
                IBuildingRow row = BuildingRows[i];

                if(row.Building == null) continue;

                if(lastID == -1)
                {
                    lastID = row.Building.ID;
                    matches.Add(row.Building);

                    continue;
                }

                if(row.Building.IsMoving)
                {
                    matches.Clear();
                    lastID = -1;

                    continue;
                }

                if(lastID == row.Building.ID)
                {
                    matches.Add(row.Building);

                    if(matches.Count % MinMatchCount == 0)
                    {
                        didDestroy = true;

                        Vector3 destroyPos = Vector3.zero;

                        foreach(IBuilding building in matches)
                        {
                            destroyPos += building.Transform.position;
                            RemoveBuilding(building);
                        }

                        destroyPos /= MinMatchCount;

                        destroyPos += _settings.DestroyAnimOffY * PlayerVM.PlayerCam.Up;

                        foreach(IBuilding building in matches)
                        {
                            building.DestroyMatch
                            (
                                destroyPos,
                                delegate
                                {
                                    BuildingEvents.PreBuildingDestroy?.Invoke(building);
                                    building.Transform.gameObject.Destroy();

                                    Instantiate
                                    (
                                        _settings.DestroyParticlePrefab,
                                        destroyPos,
                                        Quaternion.identity
                                    );
                                }
                            );
                        }

                        matches.Clear();
                    }
                }
                else
                {
                    matches.Clear();
                    lastID = row.Building.ID;
                    matches.Add(row.Building);
                }
            }

            if(didDestroy) SortBuildings();
        }

        private void RemoveBuilding(IBuilding arg0) => AssignBuilding(null, arg0);

        private void AssignBuilding(IBuildingRow newRow, IBuilding building, bool terrainPick = false)
        {
            if(building.Row?.Building == building) building.Row.SetBuilding(null);
            
            newRow?.SetBuilding(building);

            if(terrainPick)
            {
                building.AssignRow
                (
                    newRow,
                    true,
                    DestroyMatches
                );
            }
            else
            {
                building.AssignRow(newRow);
            }
        }

        private bool TryGetEmptyRow(out IBuildingRow firstEmpty)
        {
            firstEmpty = BuildingRows.FirstOrDefault(e => e.Building == null);

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

    [Serializable]
    public class BuildingRows : List<IBuildingRow>
    {
        public int BuildingCount() {return this.Count(e => e.Building != null);}

        public int GetSameIDBuildingCount(IBuilding arg0)
        {
            return this.Count(e => e.Building?.ID == arg0.ID);
        }

        public List<IGrouping<int, IBuilding>> GetBuildingsGroupedByID()
        {
            return this
            .Select(e => e.Building)
            .NotNull()
            .GroupBy(e => e.ID)
            .OrderBy(group => group.Key)
            .ToList();
        }
    }
}