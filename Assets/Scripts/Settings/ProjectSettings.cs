using System.Collections.Generic;
using Components;
using Components.Buildings;
using Sirenix.OdinInspector;
using UI.Main.Components;
using UnityEngine;
using Zenject;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectSettings), menuName = EnvVar.SettingsPath + nameof(ProjectSettings))]
    public class ProjectSettings : ScriptableObjectInstaller<ProjectSettings>
    {
        private static ProjectSettings editorInstance;
        public List<BuildingData> BuildingDatasByID => _buildingDatasByID;
        public List<LevelData> Levels => _levels;
        public Building.Settings BuildingSettings => _buildingSettings;
        public BuildingQueue.Settings BuildingQueueSettings => _buildingQueueSettings;
        public PlayerCam.Settings PlayerCamSettings => _playerCamSettings;
        [SerializeField] private List<LevelData> _levels;
        [SerializeField] private List<BuildingData> _buildingDatasByID = new();
        [SerializeField] private Building.Settings _buildingSettings;
        [SerializeField] private BuildingQueue.Settings _buildingQueueSettings;
        [SerializeField] private PlayerCam.Settings _playerCamSettings;

        [Button]
        private void AssignListIndexToBuildingIDs(bool areYouSure)
        {
            if(areYouSure == false) return;
            
            for(int i = 0; i < _buildingDatasByID.Count; i ++)
            {
                GameObject buildingGo = _buildingDatasByID[i].Prefab;
                
                buildingGo.GetComponent<IBuildingEditor>().SetID(i);
            }
        }

        public static ProjectSettings GetEditorInstance()
        {
            editorInstance ??= Resources.Load<ProjectSettings>
            (EnvVar.SettingsPath + nameof(ProjectSettings));

            return editorInstance;
        }
    }
}