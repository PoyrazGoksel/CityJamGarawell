using System;
using Components.Buildings;
using DG.Tweening;
using Extensions.DoTween;
using Extensions.Unity.MonoHelper;
using Settings;
using UnityEngine;
using Zenject;

namespace UI.Main.Components
{
    public class BuildingRow : EventListenerMono, IBuildingRow, ITweenContainerBind
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [SerializeField] private Transform _mySlotImgTrans;
        private Settings _settings;
        private Vector3 _myImgInitPos;
        public Transform Transform{get;private set;}
        public IBuilding Building{get;private set;}
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            _myImgInitPos = _mySlotImgTrans.position;

            Transform = transform;
            _settings = ProjectSettings.BuildingRowSettings;
            TweenContainer = TweenContain.Install(this);
        }

        public void BuildingArrived()
        {
            TweenContainer.AddSequence = DOTween.Sequence();
            
            Tween downTween = _mySlotImgTrans.DOLocalMoveY(_myImgInitPos.y + _settings.BuildingArrivedSagAmount, _settings.SagAnimDurHalf);
            Tween upTween = _mySlotImgTrans.DOLocalMoveY(_myImgInitPos.y, _settings.SagAnimDurHalf);

            TweenContainer.AddedSeq.Append(downTween);
            TweenContainer.AddedSeq.Append(upTween);
        }

        public void SetBuilding(IBuilding building)
        {
            Building = building;
        }

        protected override void RegisterEvents() {}

        protected override void UnRegisterEvents()
        {
            TweenContainer.Clear();
        }

        [Serializable]
        public class Settings
        {
            public float BuildingArrivedSagAmount => _buildingArrivedSagAmount;
            public float SagAnimDurHalf => _sagAnimDurHalf;
            [SerializeField] private float _buildingArrivedSagAmount = -10f;
            [SerializeField] private float _sagAnimDurHalf = 0.5f;
        }
    }

    public interface IBuildingRow
    {
        Transform Transform{get;}
        IBuilding Building{get;}

        void BuildingArrived();
    }
}