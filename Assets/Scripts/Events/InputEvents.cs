using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class InputEvents
    {
        public UnityAction<float> ZoomDelta;
        public UnityAction<Vector3> PanDelta;
    }
}