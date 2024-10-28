using System;
using Components.Buildings;
using Events;
using Extensions.System;
using Extensions.Unity.MonoHelper;
using Settings;
using Sirenix.OdinInspector;
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
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        [Inject] private LevelEvents LevelEvents{get;set;}
        public Vector3 Up => Transform.up;
        public Transform Transform{get;private set;}
        private Camera _camera;
        private Settings _settings;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Transform = transform;
            PlayerVM.SetPlayerCam(this);
            _settings = ProjectSettings.PlayerCamSettings;
        }

        private void Start()
        {
            ProjectEvents.PlayerCamStart?.Invoke(this);
        }
        [Button]
        private void FitBoundsPerspective()
        {
            Bounds levelBounds = new();
            
            FindObjectsOfType<Building>().DoToAll(e =>
            {
                levelBounds.Encapsulate(e.Bounds);
            });

            float verticalFOV = _camera.fieldOfView * Mathf.Deg2Rad;
            float boundsHeight = levelBounds.size.y;
            float distance = boundsHeight * 0.5f / Mathf.Tan(verticalFOV * 0.5f);

            float boundsWidth = levelBounds.size.x;
            float horizontalFOV = 2 * Mathf.Atan(Mathf.Tan(verticalFOV * 0.5f) * _camera.aspect);
            float horizontalDistance = boundsWidth * 0.5f / Mathf.Tan(horizontalFOV * 0.5f);

            distance = Mathf.Max(distance, horizontalDistance);

            Vector3 boundsCenter = levelBounds.center;
            
            Transform.position = boundsCenter - Transform.forward * distance;
        }
        
        protected override void RegisterEvents()
        {
            InputEvents.ZoomDelta += OnZoomDelta;
            InputEvents.PanDelta += OnPanDelta;
            LevelEvents.LevelLoaded += OnLevelLoaded;
        }

        private void OnZoomDelta(float arg0)
        {
            Vector3 pos = Transform.position;
            pos -= Transform.forward * arg0 * _settings.ZoomSpeed;
            
            //TODO: Clamp
            Transform.position = pos;
        }

        private void OnPanDelta(Vector3 arg0)
        {
            _camera.transform.Translate(-arg0.x * _settings.PanSpeed * Time.deltaTime, -arg0.y * _settings.PanSpeed * Time.deltaTime, 0);
        }

        private void OnLevelLoaded(LevelData arg0)
        {
            FitBoundsPerspective();
        }

        protected override void UnRegisterEvents()
        {
            InputEvents.ZoomDelta -= OnZoomDelta;
            InputEvents.PanDelta -= OnPanDelta;
            LevelEvents.LevelLoaded -= OnLevelLoaded;
        }

        [Serializable]
        public class Settings
        {
            public float ZoomSpeed => _zoomSpeed;
            public float PanSpeed => _panSpeed;
            [SerializeField] private float _zoomSpeed = 0.1f;
            [SerializeField] private float _panSpeed = 0.5f;
        }
    }
}