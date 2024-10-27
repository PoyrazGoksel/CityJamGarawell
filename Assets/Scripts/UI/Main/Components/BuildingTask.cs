using Extensions.Unity;
using Extensions.Unity.MonoHelper;
using TMPro;
using UnityEngine;

namespace UI.Main.Components
{
    public class BuildingTask : UIIMG
    {
        public int Remaining{get;private set;}
        public int ID{get;private set;}
        private Sprite _preview;
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        public void SetTask(int id, Sprite preview, int count)
        {
            ID = id;
            _preview = preview;
            Remaining = count;

            _myImg.sprite = _preview;
            RenderTxt(Remaining);
        }

        private void RenderTxt(int count) {_textMeshProUGUI.text = count.ToString();}

        public void Decrease()
        {
            Remaining --;

            if(Remaining <= 0)
            {
                Remaining = 0;
            }
            
            RenderTxt(Remaining);
        }

        public void Complete()
        {
            gameObject.Destroy();
        }
    }
}