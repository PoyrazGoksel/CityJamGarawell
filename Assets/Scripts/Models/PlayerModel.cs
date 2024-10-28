using System;
using Extensions.Unity.Utils;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class PlayerModel : IJsonCallBackReceiver
    {
        [SerializeField] public int Level = 0;

        public void Defaults()
        {
            Level = 0;
        }
        
        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize() {}
    }
}