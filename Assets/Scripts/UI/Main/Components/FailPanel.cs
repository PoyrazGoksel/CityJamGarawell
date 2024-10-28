using Events;
using Extensions.Unity.MonoHelper;
using Zenject;

namespace UI.Main.Components
{
    public class FailPanel : UIPanel
    {
        [Inject] private LevelEvents LevelEvents{get;set;}

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            LevelEvents.LevelFail += OnLevelFail;
        }

        private void OnLevelFail()
        {
            SetActive(true);
        }

        protected override void UnRegisterEvents()
        {
            base.UnRegisterEvents();
            LevelEvents.LevelFail -= OnLevelFail;
        }
    }
}