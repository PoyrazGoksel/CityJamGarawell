using System.IO;
using Events;
using Extensions.System;
using Extensions.Unity.Utils;
using Models;
using Zenject;

namespace ViewModels
{
    public class PlayerVM : EventListenerClass
    {
        [Inject] private ProjectEvents ProjectEvents{get;set;}
        private PlayerModel _playerModel;
        public int Level => _playerModel.Level;

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
            }

            ProjectEvents.PlayerLoaded?.Invoke();
        }

        private void Load()
        {
            string json = JsonUtilityWithCall.ReadToEnd(EnvVar.PlayerSavePath);
            _playerModel = JsonUtilityWithCall.FromJson<PlayerModel>(json);
        }

        private void CreateDefault()
        {
            _playerModel = new PlayerModel();
            _playerModel.Defaults();
        }

        protected override void RegisterEvents() {}

        protected override void UnRegisterEvents() {}
    }
}