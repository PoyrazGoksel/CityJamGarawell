using System.Collections.Generic;
using Components.BuildingQueues;
using Components.Buildings;
using Extensions.System;
using UnityEngine;
using Zenject;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectSettings), menuName = EnvVar.SettingsPath + nameof(ProjectSettings))]
    public class ProjectSettings : ScriptableObjectInstaller<ProjectSettings>
    {
        private static ProjectSettings editorInstance;
        public List<GameObject> BuildingPrefabsByID => _buildingPrefabsByID;
        public List<GameObject> LevelPrefabs => _levelPrefabs;
        public Building.Settings BuildingSettings => _buildingSettings;
        public BuildingRow.Settings BuildingRowSettings => _buildingRowSettings;
        [SerializeField] private List<GameObject> _buildingPrefabsByID = new();
        [SerializeField] private List<GameObject> _levelPrefabs;
        [SerializeField] private Building.Settings _buildingSettings;
        [SerializeField] private BuildingRow.Settings _buildingRowSettings;

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
}