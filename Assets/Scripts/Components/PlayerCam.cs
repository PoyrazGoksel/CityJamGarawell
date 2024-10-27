using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Components
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCam : EventListenerMono
    {
        [Inject] private ProjectEvents ProjectEvents{get;set;}
        [Inject] private PlayerVM PlayerVM{get;set;}
        [Inject] private InputEvents InputEvents{get;set;}
        //TODO: To settings
        private float _zoomSpeed = 0.1f;
        private float _panSpeed = 0.5f;
        private float _moveSpeed = 0.02f;
        public Vector3 Up => _transform.up;
        private Transform _transform;
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _transform = transform;
            PlayerVM.SetPlayerCam(this);
        }

        private void Start()
        {
            ProjectEvents.PlayerCamStart?.Invoke(this);
        }

        protected override void RegisterEvents()
        {
            InputEvents.ZoomDelta += OnZoomDelta;
            InputEvents.PanDelta += OnPanDelta;
        }

        private void OnZoomDelta(float arg0)
        {
            Vector3 pos = _transform.position;
            pos -= _transform.forward * arg0 * _zoomSpeed;
            
            //TODO: Clamp
            _transform.position = pos;
        }

        private void OnPanDelta(Vector3 arg0)
        {
            _camera.transform.Translate(-arg0.x * _panSpeed * Time.deltaTime, -arg0.y * _panSpeed * Time.deltaTime, 0);
        }

        protected override void UnRegisterEvents()
        {
            InputEvents.ZoomDelta -= OnZoomDelta;
            InputEvents.PanDelta -= OnPanDelta;
        }
    }
}