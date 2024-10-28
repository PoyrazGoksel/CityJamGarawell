using Events;
using Extensions.Unity.MonoHelper;
using Extensions.Unity.Utils;
using Zenject;

namespace UI.Main.Components
{
    public class WinPanel : UIPanel
    {
        [Inject] private LevelEvents LevelEvents{get;set;}
        
        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            LevelEvents.LevelSuccess += OnLevelSuccess;
        }

        private void OnLevelSuccess()
        {
            SetActive(true);
            EDebug.Method();
        }

        protected override void UnRegisterEvents()
        {
            base.UnRegisterEvents();
            LevelEvents.LevelSuccess -= OnLevelSuccess;
        }
    }
}