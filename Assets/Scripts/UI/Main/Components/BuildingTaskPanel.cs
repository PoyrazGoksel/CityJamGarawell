using System.Collections.Generic;
using System.Linq;
using Components.Buildings;
using Events;
using Extensions.Unity.MonoHelper;
using Settings;
using UnityEngine;
using Zenject;

namespace UI.Main.Components
{
    public class BuildingTaskPanel : UIPanel
    {
        [Inject] private LevelEvents LevelEvents{get;set;}
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [SerializeField] private GameObject _buildingTaskPrefab;
        private Transform _transform;
        private List<BuildingTask> _buildingTasks = new();
        private void Awake() {_transform = transform;}

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            LevelEvents.LevelLoaded += OnLevelLoaded;
            BuildingEvents.PreBuildingDestroy += OnPreBuildingDestroy;
        }

        private void OnPreBuildingDestroy(IBuilding arg0)
        {
            BuildingTask firstTask = _buildingTasks.FirstOrDefault(e => e.ID == arg0.ID);

            if(firstTask)
            {
                firstTask.Decrease();

                if(firstTask.Remaining <= 0)
                {
                    _buildingTasks.Remove(firstTask);
                    firstTask.Complete();
                }
            }
        }

        private void OnLevelLoaded(LevelData arg0)
        {
            TaskBuildings task = arg0.TaskBuildings;

            Debug.LogWarning(ProjectSettings.BuildingDatasByID.Count);
            
            foreach(KeyValuePair<int, int> idToCount in task)
            {
                GameObject buildingTaskGo = Instantiate(_buildingTaskPrefab, _transform);
                BuildingTask buildingTask = buildingTaskGo.GetComponent<BuildingTask>();

                int thisID = idToCount.Key;
                buildingTask.SetTask(thisID, ProjectSettings.BuildingDatasByID[thisID].Preview, idToCount.Value);
                
                _buildingTasks.Add(buildingTask);
            }
        }

        protected override void UnRegisterEvents()
        {
            base.UnRegisterEvents();
            LevelEvents.LevelLoaded -= OnLevelLoaded;
            BuildingEvents.PreBuildingDestroy += OnPreBuildingDestroy;
        }
    }
}