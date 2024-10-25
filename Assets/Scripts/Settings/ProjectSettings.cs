using System.Collections.Generic;
using Extensions.System;
using UnityEngine;
using Zenject;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectSettings), menuName = EnvVar.SettingsPath + nameof(ProjectSettings))]
    public class ProjectSettings : ScriptableObjectInstaller<ProjectSettings>
    {
        public List<GameObject> BuildingPrefabsByID => BuildingPrefabsByID;
        [SerializeField] private List<GameObject> _buildingPrefabsByID = new();
        private static ProjectSettings _editorInstance;
        [SerializeField] private List<GameObject> _levelPrefabs;
        public List<GameObject> LevelPrefabs => _levelPrefabs;

        public static ProjectSettings GetEditorInstance()
        {
            _editorInstance ??= Resources.Load<ProjectSettings>
            (EnvVar.SettingsPath + nameof(ProjectSettings));

            return _editorInstance;
        }

        public GameObject GetBuilding(int buildingID)
        {
            return _buildingPrefabsByID.IsInRange(buildingID) ? _buildingPrefabsByID[buildingID] : null;
        }
    }
}