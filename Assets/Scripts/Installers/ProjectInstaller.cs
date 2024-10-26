using Events;
using Settings;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        private ProjectEvents _projectEvents;
        
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
        }

        private void InstallData()
        {
            Container.Bind<PlayerVM>().AsSingle();
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
    }
}