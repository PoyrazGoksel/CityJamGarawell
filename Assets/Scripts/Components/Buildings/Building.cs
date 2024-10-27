using System;
using DG.Tweening;
using Events;
using Events.Internal;
using Extensions.DoTween;
using Extensions.Unity.MonoHelper;
using Settings;
using UI.Main.Components;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Components.Buildings
{
    public partial class Building : EventListenerMono, IBuilding, ITweenContainerBind, IBuildingEditor
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [Inject] private BuildingEventsInternal BuildingEventsInternal{get;set;}
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [Inject] private PlayerVM PlayerVM{get;set;}
        [SerializeField] private int _id;
        [SerializeField] private float _uiSize;
        private Settings _settings;
        public int ID => _id;
        public Transform Transform{get;private set;}
        public IBuildingRow Row{get;private set;}
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            _settings = ProjectSettings.BuildingSettings;
            TweenContainer = TweenContain.Install(this);
            Transform = transform;
        }

        public void AssignRow(IBuildingRow buildingRow, TweenCallback onComplete = null)
        {
            Transform.parent = PlayerVM.PlayerCam.transform;
            
            Row = buildingRow;

            if(Row == null)
            {
                //Queued for destroy
                return;
            }
            
            TweenContainer.AddSequence = DOTween.Sequence();
            TweenContainer.AddedSeq.Append(Transform.DOLocalMove(PlayerVM.GetLocalUnderCam(Row.Transform.position), _settings.RowAssignAnimDur));
            TweenContainer.AddedSeq.Insert(0f, Transform.DOScale(Vector3.one * _uiSize, _settings.RowAssignAnimDur));

            TweenContainer.AddedSeq.onComplete += delegate
            {
                Row.BuildingArrived();
                BuildingEvents.BuildingArrivedToRow?.Invoke(this);
            };
             

            if(onComplete != null)
            {
                TweenContainer.AddedSeq.onComplete += onComplete;
            }
        }

        public void DestroyMatch
        (Vector3 destroyPos, TweenCallback onComplete = null)
        {
            TweenContainer.AddTween = Transform.DOLocalMove(PlayerVM.GetLocalUnderCam(destroyPos), _settings.DestroyAnimDur);

            TweenContainer.AddedTween.onComplete += onComplete;
        }

        protected override void RegisterEvents()
        {
            BuildingEventsInternal.ColliderMouseUp += OnColliderMouseUp;
        }

        private void OnColliderMouseUp()
        {
            if(Row != null) return;
            
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
            public float RowAssignAnimDur => _rowAssignAnimDur;
            public float DestroyAnimDur => _destroyAnimDur;
            [SerializeField] private float _rowAssignAnimDur = 1f;
            [SerializeField] private float _destroyAnimDur = 0.5f;
        }
    }

    public interface IBuildingEditor
    {
        void SetID(int id);
    }

    public interface IBuilding
    {
        int ID{get;}
        Transform Transform{get;}
        IBuildingRow Row{get;}

        void AssignRow(IBuildingRow buildingRow, TweenCallback onComplete = null);

        void DestroyMatch(Vector3 destroyPos, TweenCallback onComplete = null);
    }
}