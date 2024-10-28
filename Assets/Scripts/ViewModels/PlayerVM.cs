using System.IO;
using Components;
using Events;
using Extensions.System;
using Extensions.Unity.Utils;
using Models;
using UnityEngine;
using Zenject;

namespace ViewModels
{
    public class PlayerVM : EventListenerClass
    {
        [Inject] private ProjectEvents ProjectEvents{get;set;}
        [Inject] private LevelEvents LevelEvents{get;set;}
        public int Level => _playerModel.Level;
        public PlayerCam PlayerCam{get;private set;}
        private PlayerModel _playerModel;

        public override void Initialize()
        {
            base.Initialize();

            LoadOrCreate();
        }

        private void LoadOrCreate()
        {
            if(File.Exists(EnvVar.PlayerSavePath))
            {
                Load();
            }
            else
            {
                CreateDefault();
                ProjectEvents.PlayerCreated?.Invoke();
                Save();
            }

            ProjectEvents.PlayerLoaded?.Invoke();
        }

        private void Load()
        {
            string json = JsonUtilityWithCall.ReadToEnd(EnvVar.PlayerSavePath);
            _playerModel = JsonUtilityWithCall.FromJson<PlayerModel>(json);
        }

        private void Save()
        {
            Debug.LogWarning($"EnvVar.PlayerSavePath {EnvVar.PlayerSavePath}");
            string json = JsonUtilityWithCall.ToJson(_playerModel);
            JsonUtilityWithCall.WriteToEnd(json, EnvVar.PlayerSavePath);
        }
        
        private void CreateDefault()
        {
            _playerModel = new PlayerModel();
            _playerModel.Defaults();
        }

        public void SetPlayerCam(PlayerCam playerCam) {PlayerCam = playerCam;}

        public Vector3 GetLocalUnderCam(Vector3 worldPos)
        {
            return PlayerCam.Transform.InverseTransformPoint(worldPos);
        }

        protected override void RegisterEvents()
        {
            LevelEvents.LevelSuccess += OnLevelSuccess;
        }

        private void OnLevelSuccess()
        {
            _playerModel.Level ++;
            Save();
        }

        protected override void UnRegisterEvents()
        {
            LevelEvents.LevelSuccess -= OnLevelSuccess;
        }
    }
}