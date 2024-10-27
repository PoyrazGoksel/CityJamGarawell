using Settings;
using UnityEngine.Events;

namespace Events
{
    public class LevelEvents
    {
        public UnityAction<LevelData> LevelLoaded;
        public UnityAction TimeOut;
        public UnityAction NoRowsLeft;
        public UnityAction LevelTaskComplete;
    }
}