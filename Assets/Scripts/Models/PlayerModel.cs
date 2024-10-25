using System;
using Extensions.Unity.Utils;

namespace Models
{
    [Serializable]
    public class PlayerModel : IJsonCallBackReceiver
    {
        public int Level = 0;

        public void Defaults()
        {
            Level = 0;
        }
        
        public void OnBeforeSerialize() {}

        public void OnAfterDeserialize() {}
    }
}