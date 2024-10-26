using System;
using Components.BuildingQueues;
using DG.Tweening;
using Events;
using Events.Internal;
using Extensions.DoTween;
using Extensions.Unity.MonoHelper;
using Settings;
using UnityEngine;
using Zenject;

namespace Components.Buildings
{
    public partial class Building : EventListenerMono, IBuilding, ITweenContainerBind
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [Inject] private BuildingEventsInternal BuildingEventsInternal{get;set;}
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [SerializeField] private int _id;
        private Transform _transform;
        private Settings _settings;
        public int ID => _id;
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            _settings = ProjectSettings.BuildingSettings;
            TweenContainer = TweenContain.Install(this);
            _transform = transform;
        }

        public void AssignRow(IBuildingRow buildingRow)
        {
            TweenContainer.AddSequence = DOTween.Sequence();
            TweenContainer.AddedSeq.Append(_transform.DOMove(buildingRow.TransEncapsulated.position, _settings.RowAssignAnimDur));
            TweenContainer.AddedSeq.Insert(0f, _transform.DOScale(_settings.BuildingRowScale, _settings.RowAssignAnimDur));

            TweenContainer.AddedSeq.onComplete += buildingRow.BuildingArrived;
        }

        protected override void RegisterEvents()
        {
            BuildingEventsInternal.ColliderMouseUp += OnColliderMouseUp;
        }

        private void OnColliderMouseUp()
        {
            BuildingEvents.BuildingClicked?.Invoke(this);
        }

        protected override void UnRegisterEvents()
        {
            BuildingEventsInternal.ColliderMouseUp -= OnColliderMouseUp;
            TweenContainer.Clear();
        }

        [Serializable]
        public class Settings
        {
            public float BuildingRowScale => _buildingRowScale;
            public float RowAssignAnimDur => _rowAssignAnimDur;
            [SerializeField] private float _buildingRowScale = 0.2f;
            [SerializeField] private float _rowAssignAnimDur = 1f;
        }
    }

    public interface IBuilding
    {
        int ID{get;}

        void AssignRow(IBuildingRow buildingRow);
    }
}