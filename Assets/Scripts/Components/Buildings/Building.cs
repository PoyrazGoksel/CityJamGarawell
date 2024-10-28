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
    public partial class Building : EventListenerMono, IBuilding, ITweenContainerBind
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [Inject] private BuildingEventsInternal BuildingEventsInternal{get;set;}
        [Inject] private BuildingEvents BuildingEvents{get;set;}
        [Inject] private PlayerVM PlayerVM{get;set;}
        public Bounds Bounds => _myCollider.bounds;
        [SerializeField] private int _id;
        [SerializeField] private float _uiSize;
        [SerializeField] private Collider _myCollider;
        private Settings _settings;
        private Sequence _arriveAnimSeq;
        private Sequence _moveSeq;
        public int ID => _id;
        public Transform Transform{get;private set;}
        public IBuildingRow Row{get;private set;}
        public bool IsMoving{get;private set;}
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            _settings = ProjectSettings.BuildingSettings;
            TweenContainer = TweenContain.Install(this);
            Transform = transform;
        }

        public void AssignRow(IBuildingRow buildingRow, bool terrainPick = false, TweenCallback onComplete = null)
        {
            if(Transform)
            {
                Transform.parent = PlayerVM.PlayerCam.Transform;
            }

            Row = buildingRow;
 
            if (Row == null)
            {
                // Queued for destroy
                return;
            }

            if(_moveSeq.IsActive()) _moveSeq.Kill();
            
            _moveSeq = DOTween.Sequence();
            TweenContainer.AddSequence = DOTween.Sequence();

            IsMoving = true;
            
            if(terrainPick)
            {
                TweenContainer.AddedSeq.Insert
                (
                    0f,
                    Transform
                    .DOMove
                    (
                        Transform.position + Vector3.up * 10f,
                        _settings.RowAssignAnimDur * 0.5f
                    )
                );
                
                TweenContainer.AddedSeq.Insert
                (
                    _settings.RowAssignAnimDur * 0.5f,
                    Transform
                    .DOLocalMove
                    (
                        PlayerVM.GetLocalUnderCam(Row.Transform.position),
                        _settings.RowAssignAnimDur * 0.5f
                    )
                );
                
                TweenContainer.AddedSeq.Insert
                (
                    _settings.RowAssignAnimDur * 0.5f,
                    Transform
                    .DOScale
                    (
                        Vector3.one * _uiSize,
                        _settings.RowAssignAnimDur * 0.5f
                    )
                );
            }
            else
            {
                TweenContainer.AddedSeq.Insert
                (
                    0f,
                    Transform
                    .DOLocalMove
                    (
                        PlayerVM.GetLocalUnderCam(Row.Transform.position),
                        _settings.RowAssignAnimDur
                    )
                );
                
                TweenContainer.AddedSeq.Insert
                (
                    0f,
                    Transform
                    .DOScale
                    (
                        Vector3.one * _uiSize,
                        _settings.RowAssignAnimDur
                    )
                );
            }

            TweenContainer.AddedSeq.SetEase(_settings.AssignAnimEase);

            TweenContainer.AddedSeq.onComplete += delegate
            {
                IsMoving = false;
                onComplete?.Invoke();
            };
        }

        public void DestroyMatch
        (Vector3 destroyPos, TweenCallback onComplete = null)
        {
            TweenContainer.AddSequence = DOTween.Sequence();
            
            TweenContainer.AddedSeq.Append
            (Transform.DOLocalMove(PlayerVM.GetLocalUnderCam(destroyPos), _settings.DestroyAnimDur));

            TweenContainer.AddedSeq.SetEase(_settings.DestroyAnimEase);
            TweenContainer.AddedSeq.onComplete += delegate
            {
                onComplete?.Invoke();
            };
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
            public Ease AssignAnimEase => _assignAnimEase;
            public Ease DestroyAnimEase => _destroyAnimEase;
            [SerializeField] private float _rowAssignAnimDur = 1f;
            [SerializeField] private float _destroyAnimDur = 0.5f;
            [SerializeField] private Ease _assignAnimEase = Ease.Linear;
            [SerializeField] private Ease _destroyAnimEase = Ease.Linear;
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
        bool IsMoving{get;}

        void AssignRow(IBuildingRow buildingRow,bool terrainPick = false, TweenCallback onComplete = null);

        void DestroyMatch(Vector3 destroyPos, TweenCallback onComplete = null);
    }
}