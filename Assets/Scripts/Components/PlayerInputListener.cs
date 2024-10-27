using UnityEngine;
using Events;
using Extensions.Unity.MonoHelper;
using Extensions.Unity.Utils;
using Zenject;

namespace Components
{
    public class PlayerInputListener : EventListenerMono
    {
        [Inject] private BuildingEvents BuildingEvents { get; set; }
        [Inject] private InputEvents InputEvents{get;set;}
        private RoutineHelper _inputRoutine;
        private Vector2 _lastPanPosition;
        private Vector2 _panDelta;
        private Vector3 _prevMousePos;

        private void Awake()
        {
            _prevMousePos = Input.mousePosition;
            _inputRoutine = new RoutineHelper(this, null, InputUpdate);
        }

        private void Start()
        {
            _inputRoutine.StartCoroutine();
        }

        private void InputUpdate()
        {
#if UNITY_EDITOR
            if(Input.GetMouseButtonDown(0))
            {
                _prevMousePos = Input.mousePosition;
            }
            else if(Input.GetMouseButton(0))
            {
                Vector3 currMousePos = Input.mousePosition;
                
                _panDelta = currMousePos - _prevMousePos;

                InputEvents.PanDelta?.Invoke(_panDelta);

                _prevMousePos = currMousePos;
            }
#endif
            if (Input.touchCount == 1)
            {
                Pan();
            }
            else if (Input.touchCount == 2)
            {
                Zoom();
            }
        }

        private void Zoom()
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            InputEvents.ZoomDelta?.Invoke(difference);
        }

        private void Pan()
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                _panDelta = touch.position - _lastPanPosition;

                InputEvents.PanDelta?.Invoke(_panDelta);

                _lastPanPosition = touch.position;
            }
        }

        protected override void RegisterEvents()
        {
            BuildingEvents.SortingRows += OnSortingRows;
        }

        private void OnSortingRows(bool isSorting)
        {
            if (isSorting)
            {
                _inputRoutine.StopCoroutine();
            }
            else
            {
                _inputRoutine.StartCoroutine();
            }
        }

        protected override void UnRegisterEvents()
        {
            BuildingEvents.SortingRows -= OnSortingRows;
        }
    }
}