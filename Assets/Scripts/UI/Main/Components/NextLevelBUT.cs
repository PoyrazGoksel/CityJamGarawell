using Events;
using Extensions.Unity.MonoHelper;
using Zenject;

namespace UI.Main.Components
{
    public class NextLevelBUT : UIButtonTMP
    {
        [Inject] private UIEvents UIEvents{get;set;}

        protected override void OnClick()
        {
            UIEvents.NextLevelClick?.Invoke();
        }
    }
}