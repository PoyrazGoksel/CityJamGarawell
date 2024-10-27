using Components;
using Events;
using Extensions.System;
using Settings;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
    {
        [Inject] private PlayerVM PlayerVM{get;set;}
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [Inject] private LevelEvents LevelEvents{get;set;}
        private Level _currLevel;
        private LevelData _currLevelSettings;

        public override void InstallBindings()
        {
            
        }

        private void Awake()
        {
            if(FindObjectOfType<Level>())
            {
                Debug.LogWarning("There is a level prefab before scene awake!");
            }
        }

        public override void Start()
        {
            TryLoadLevel();
        }

        private void TryLoadLevel()
        {
            int effectiveLevel = PlayerVM.Level % ProjectSettings.Levels.Count;
            
            if(ProjectSettings.Levels.IsInRange(PlayerVM.Level))
            {
                //Loading fake progress
            }

            _currLevelSettings = ProjectSettings.Levels[effectiveLevel];
            LoadLevelPrefab(_currLevelSettings.LevelPrefab);
        }

        private void LoadLevelPrefab(GameObject projectSettingsLevelPrefab)
        {
            _currLevel = Container.InstantiatePrefab(projectSettingsLevelPrefab).GetComponent<Level>();
            
            LevelEvents.LevelLoaded?.Invoke(_currLevelSettings);
        }
    }
}