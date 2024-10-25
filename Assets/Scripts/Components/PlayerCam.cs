using Events;
using Extensions.Unity.MonoHelper;
using Zenject;

namespace Components
{
    public class PlayerCam : EventListenerMono
    {
        [Inject] private ProjectEvents ProjectEvents{get;set;}

        private void Start()
        {
            ProjectEvents.PlayerCamStart?.Invoke(this);
        }

        protected override void RegisterEvents() {}

        protected override void UnRegisterEvents() {}
    }
}