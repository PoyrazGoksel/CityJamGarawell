using System;
using System.Collections.Generic;
using System.Linq;
using Components.Buildings;
using Events;
using Extensions.System;
using Extensions.Unity;
using Extensions.Unity.Entities;
using Extensions.Unity.MonoHelper;
using Sirenix.OdinInspector;
using UnityEngine;
using ViewModels;
using Zenject;

namespace UI.Main.Components
{
    public class BuildingQueue : EventListenerMono
    {
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [Inject] private PlayerVM PlayerVM{get;set;}
        [SerializeField] private List<BuildingRowSerialized> _buildingRows;
        [ShowInInspector] private Dictionary<IBuildingRow, IBuilding> _rowBuildingDict = new();
        private List<IGrouping<int, IBuilding>> _groupedBuildings;

        private void Awake()
        {
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
            if(TryGetEmptyRow(out IBuildingRow emptyRow))
            {
                AssignBuilding(arg0, emptyRow);

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

                    AssignBuilding(buildingToAssign, thisRow);

                    index++;
                }
            }
        }

        private void CheckForDestroy()
        {
            IGrouping<int, IBuilding> firstToDestroy = _groupedBuildings?.FirstOrDefault
            (e => e.Count() >= 3);

            if(firstToDestroy != null)
            {
                DestroyGroup(firstToDestroy);
                    
                SortBuildings();
            }
        }

        private void DestroyGroup(IGrouping<int, IBuilding> firstToDestroy)
        {
            Vector3 destroyPos = Vector3.zero;

            firstToDestroy.DoToAll(e =>
            {
                destroyPos +=  e.Transform.position;
                RemoveBuilding(e);
            });

            destroyPos /= firstToDestroy.Count();

            //TODO: To settings
            float destroyPosYOff = 10f;

            destroyPos += destroyPosYOff * PlayerVM.PlayerCam.Up;

            foreach(IBuilding building in firstToDestroy)
            {
                building.DestroyMatch(destroyPos,
                    delegate
                    {
                        building.Transform.gameObject.Destroy();
                    });
            }
        }

        private void RemoveBuilding(IBuilding arg0) => AssignBuilding(arg0, null);

        private void AssignBuilding(IBuilding arg0, IBuildingRow emptyRow)
        {
            if(arg0.Row != null)
            {
                _rowBuildingDict[arg0.Row] = null;
            }

            if(emptyRow != null)
            {
                _rowBuildingDict[emptyRow] = arg0;
            }

            arg0.AssignRow(emptyRow);
        }

        private bool TryGetEmptyRow(out IBuildingRow firstEmpty)
        {
            firstEmpty = _rowBuildingDict.FirstOrDefault(e => e.Value == null).Key;

            return firstEmpty != null;
        }

        protected override void RegisterEvents()
        {
            BuildingEvents.BuildingClicked += OnBuildingClicked;
            BuildingEvents.BuildingArrivedToRow += OnBuildingArrivedToRow;
        }

        private void OnBuildingArrivedToRow(IBuilding arg0)
        {
            CheckForDestroy();
        }

        private void OnBuildingClicked(IBuilding arg0) {TryFitBuilding(arg0);}

        protected override void UnRegisterEvents()
        {
            BuildingEvents.BuildingClicked -= OnBuildingClicked;
            BuildingEvents.BuildingArrivedToRow -= OnBuildingArrivedToRow;
        }
    }

    [Serializable] public class BuildingRowSerialized : SerializedInterface<IBuildingRow> {}
}