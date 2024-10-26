using System;
using DG.Tweening;
using Extensions.DoTween;
using Extensions.Unity.MonoHelper;
using Extensions.Unity.Utils;
using Settings;
using UnityEngine;
using Zenject;

namespace Components.BuildingQueues
{
    public class BuildingRow : EventListenerMono, IBuildingRow, ITweenContainerBind
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        private Transform _transform;
        private Settings _settings;
        public ETransform TransEncapsulated{get;private set;}
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            _settings = ProjectSettings.BuildingRowSettings;
            _transform = transform;
            TweenContainer = TweenContain.Install(this);
            TransEncapsulated = new ETransform(transform);
        }

        public void BuildingArrived()
        {
            TweenContainer.AddSequence = DOTween.Sequence();
            
            Tween downTween = _transform.DOLocalMoveY(_settings.BuildingArrivedSagAmount, _settings.SagAnimDurHalf);
            Tween upTween = _transform.DOLocalMoveY(0, _settings.SagAnimDurHalf);

            TweenContainer.AddedSeq.Append(downTween);
            TweenContainer.AddedSeq.Append(upTween);
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
        ETransform TransEncapsulated{get;}

        void BuildingArrived();
    }
}