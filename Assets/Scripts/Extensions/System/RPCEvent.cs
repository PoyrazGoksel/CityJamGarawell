using UnityEngine.Events;

namespace Extensions.System
{
    public class RPCEvent
    {
        private bool _isTriggered;

        public RPCEvent()
        {
            On_Event += OnEvent;
        }

        public static RPCEvent operator +(RPCEvent thisEvent, UnityAction action)
        {
            thisEvent.On_Event += action;
            if(thisEvent._isTriggered) { action?.Invoke(); }
            return thisEvent;
        }

        public static RPCEvent operator -(RPCEvent thisEvent, UnityAction action)
        {
            thisEvent.On_Event -= action;
            return thisEvent;
        }

        private event UnityAction On_Event;

        private void OnEvent()
        {
            _isTriggered = true;
        }

        public virtual void Invoke() {On_Event?.Invoke();}
    }
    public class RPCEvent<T>
    {
        private bool _isTriggered;
        private T _typeCache;

        public RPCEvent()
        {
            On_Event += OnEvent;
        }

        public static RPCEvent<T> operator +(RPCEvent<T> thisEvent, UnityAction<T> action)
        {
            thisEvent.On_Event += action;
            if(thisEvent._isTriggered) { action?.Invoke(thisEvent._typeCache); }
            return thisEvent;
        }

        public static RPCEvent<T> operator -(RPCEvent<T> thisEvent, UnityAction<T> action)
        {
            thisEvent.On_Event -= action;
            return thisEvent;
        }

        private event UnityAction<T> On_Event;

        private void OnEvent(T type)
        {
            _typeCache = type;
            _isTriggered = true;
        }

        public void Invoke(T type) {On_Event?.Invoke(type);}
    }
}