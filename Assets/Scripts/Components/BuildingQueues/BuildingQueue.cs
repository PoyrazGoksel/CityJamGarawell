using System;
using System.Collections.Generic;
using System.Linq;
using Components.Buildings;
using Events;
using Extensions.Unity.Entities;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using Zenject;

namespace Components.BuildingQueues
{
    public class BuildingQueue : EventListenerMono
    {
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        public List<BuildingRowSerialized> BuildingRows => _buildingRows;
        [SerializeField] private List<BuildingRowSerialized> _buildingRows;
        private readonly Dictionary<IBuildingRow,IBuilding> _currBuildings = new();


        private void Awake()
        {
            CreateBuildingQueue();
        }

        private void CreateBuildingQueue()
        {
            foreach(IBuildingRow buildingRowSerialized in _buildingRows.Select(e => e.Value()))
            {
                if(_currBuildings.TryAdd(buildingRowSerialized, null))
                {
                    
                }
            }
        }

        private bool TryFitBuilding(IBuilding arg0)
        {
            if(TryGetEmptyRow(out IBuildingRow emptyRow))
            {
                AssignRowToBuilding(arg0, emptyRow);
                
                var rowsSorted = _currBuildings.Where(e=> e.Value != null).GroupBy(e => e.Value.ID, e => e).ToList();

                foreach(IGrouping<int, KeyValuePair<IBuildingRow, IBuilding>> pairs in rowsSorted)
                {
                    foreach(KeyValuePair<IBuildingRow,IBuilding> pair in pairs)
                    {
                        AssignRowToBuilding(pair.Value, pair.Key); 
                    }
                }
                return true;
            }

            return false;
        }

        private void AssignRowToBuilding(IBuilding arg0, IBuildingRow emptyRow)
        {
            _currBuildings[emptyRow] = arg0;
            arg0.AssignRow(emptyRow);
        }

        private bool TryGetEmptyRow(out IBuildingRow firstEmpty)
        {
            firstEmpty = _currBuildings.FirstOrDefault(e => e.Value == null).Key;
            
            return firstEmpty != null;
        }

        protected override void RegisterEvents()
        {
            BuildingEvents.BuildingClicked += OnBuildingClicked;
        }

        private void OnBuildingClicked(IBuilding arg0)
        {
            TryFitBuilding(arg0);
        }

        protected override void UnRegisterEvents()
        {
            BuildingEvents.BuildingClicked -= OnBuildingClicked;
        }
    }

    [Serializable] public class BuildingRowSerialized : SerializedInterface<IBuildingRow> {}
}