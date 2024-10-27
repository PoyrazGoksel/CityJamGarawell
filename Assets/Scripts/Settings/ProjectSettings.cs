using System;
using System.Collections.Generic;
using Components.Buildings;
using Extensions.System;
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
        public List<GameObject> BuildingPrefabsByID => _buildingPrefabsByID;
        public List<LevelData> Levels => _levels;
        public Building.Settings BuildingSettings => _buildingSettings;
        public BuildingRow.Settings BuildingRowSettings => _buildingRowSettings;
        [SerializeField] private List<LevelData> _levels;
        [SerializeField] private List<GameObject> _buildingPrefabsByID = new();
        [SerializeField] private Building.Settings _buildingSettings;
        [SerializeField] private BuildingRow.Settings _buildingRowSettings;

        [Button]
        private void AssignListIndexToBuildingIDs(bool areYouSure)
        {
            if(areYouSure == false) return;
            
            for(int i = 0; i < _buildingPrefabsByID.Count; i ++)
            {
                GameObject buildingGo = _buildingPrefabsByID[i];
                
                buildingGo.GetComponent<IBuildingEditor>().SetID(i);
            }
        }

        public static ProjectSettings GetEditorInstance()
        {
            editorInstance ??= Resources.Load<ProjectSettings>
            (EnvVar.SettingsPath + nameof(ProjectSettings));

            return editorInstance;
        }

        public GameObject GetBuilding(int buildingID)
        {
            return _buildingPrefabsByID.IsInRange(buildingID) ? _buildingPrefabsByID[buildingID] : null;
        }
    }

    [Serializable]
    public class LevelData
    {
        public GameObject LevelPrefab => _levelPrefab;
        public float LevelTimerMins => _levelTimerMins;
        [SerializeField] private GameObject _levelPrefab;
        [SerializeField] private float _levelTimerMins = 4;
    }
}