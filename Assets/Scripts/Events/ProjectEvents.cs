using Components;
using Extensions.System;
using UnityEngine.Events;

namespace Events
{
    public class ProjectEvents
    {
        public RPCEvent ProjectInstalled = new();
        public RPCEvent ProjectStarted = new();
        public UnityAction<PlayerCam> PlayerCamStart;
        public RPCEvent PlayerLoaded;
        public RPCEvent PlayerCreated;
    }
}