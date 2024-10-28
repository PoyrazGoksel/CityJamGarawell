using Components.Buildings;
using Events;
using Settings;
using UnityEngine;
using Zenject;

namespace Components
{
    public class Level : MonoBehaviour
    {
        [Inject] private LevelEvents LevelEvents{get;set;}
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [SerializeField] private TaskBuildings _reqBuildings = new();

        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        private void TryRemoveReqBuilding(IBuilding arg0)
        {
            if(_reqBuildings.ContainsKey(arg0.ID))
            {
                _reqBuildings[arg0.ID] --;

                if(_reqBuildings[arg0.ID] <= 0)
                {
                    _reqBuildings.Remove(arg0.ID);
                }
            }

            if(_reqBuildings.Count == 0)
            {
                Debug.LogWarning("WinCond: Task Complete");
                LevelEvents.LevelSuccess?.Invoke();
            }
        }

        private void RegisterEvents()
        {
            BuildingEvents.PreBuildingDestroy += OnPreBuildingDestroy;
            LevelEvents.LevelLoaded += OnLevelLoaded;
            LevelEvents.NoRowsLeft += OnNoRowsLeft;
            LevelEvents.TimeOut += OnTimeOut;
        }

        private void OnTimeOut()
        {
            LevelEvents.LevelFail?.Invoke();
        }

        private void OnNoRowsLeft()
        {
            LevelEvents.LevelFail?.Invoke();
        }

        private void OnLevelLoaded(LevelData arg0)
        {
            _reqBuildings = arg0.TaskBuildings.GetClone();
        }

        private void OnPreBuildingDestroy(IBuilding arg0)
        {
            TryRemoveReqBuilding(arg0);
        }

        private void UnRegisterEvents()
        {
            BuildingEvents.PreBuildingDestroy -= OnPreBuildingDestroy;
            LevelEvents.LevelLoaded -= OnLevelLoaded;
            LevelEvents.NoRowsLeft -= OnNoRowsLeft;
            LevelEvents.TimeOut -= OnTimeOut;
        }
    }
}