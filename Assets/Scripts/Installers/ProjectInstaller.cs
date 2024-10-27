using Events;
using Extensions.Unity.MonoHelper;
using Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViewModels;

namespace Installers
{
    public class ProjectInstaller : EventListenerInstaller<ProjectInstaller>
    {
        private UIEvents _uiEvents;
        private ProjectEvents _projectEvents;
        private LevelEvents _levelEvents;

        public override void InstallBindings()
        {
            InstallEvents();
            InstallData();
            InstallSettings();
            
            _projectEvents.ProjectInstalled?.Invoke();
        }

        private void InstallEvents()
        {
            _projectEvents = new ProjectEvents();
            Container.BindInstance(_projectEvents).AsSingle();
            
            Container.Bind<BuildingEvents>().AsSingle();
            Container.Bind<InputEvents>().AsSingle();
            
            _uiEvents = new UIEvents();
            Container.BindInstance(_uiEvents).AsSingle();

            _levelEvents = new LevelEvents();
            Container.BindInstance(_levelEvents).AsSingle();
        }

        private void InstallData()
        {
            Container.BindInterfacesAndSelfTo<PlayerVM>().AsSingle();
        }

        private void InstallSettings()
        {
            ProjectSettings newProjSettings = Resources.Load<ProjectSettings>(EnvVar.SettingsPath + nameof(ProjectSettings));

            Container.BindInstance(newProjSettings).AsSingle();
        }

        public override void Start()
        {
            _projectEvents.ProjectStarted?.Invoke();
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene("Main");
        }

        protected override void RegisterEvents()
        {
            _uiEvents.RestartClick += OnRestartClick;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(arg0.name == EnvVar.LoginSceneName)
            {
                LoadMainScene();
            }
        }

        private void OnRestartClick()
        {
            LoadMainScene();
        }

        protected override void UnRegisterEvents()
        {
            _uiEvents.RestartClick -= OnRestartClick;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}