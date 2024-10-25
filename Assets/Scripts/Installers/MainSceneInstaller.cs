using Components;
using Extensions.System;
using Settings;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
    {
        private Level _currLevel;
        [Inject] private PlayerVM PlayerVM{get;set;}
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        
        public override void InstallBindings()
        {
            
        }

        private void Awake()
        {
            if(FindObjectOfType<Level>())
            {
                Debug.LogWarning("There is a level prefab before scene awake!");
            }
            
            TryLoadLevel();
        }

        private void TryLoadLevel()
        {
            int effectiveLevel = PlayerVM.Level % ProjectSettings.LevelPrefabs.Count;
            
            if(ProjectSettings.LevelPrefabs.IsInRange(PlayerVM.Level))
            {
                //Loading fake progress
            }
            
            LoadLevelPrefab(ProjectSettings.LevelPrefabs[effectiveLevel]);
        }

        private void LoadLevelPrefab(GameObject projectSettingsLevelPrefab)
        {
            _currLevel = Container.InstantiatePrefab(projectSettingsLevelPrefab).GetComponent<Level>();
        }
    }
}