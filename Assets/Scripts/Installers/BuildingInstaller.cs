using Events.Internal;
using Zenject;

namespace Installers
{
    public class BuildingInstaller : MonoInstaller<BuildingInstaller>
    {
        public override void InstallBindings()
        {
            InstallEvents();
        }

        private void InstallEvents()
        {
            Container.Bind<BuildingEventsInternal>().AsSingle();
        }

        public override void Start() {}
    }
}