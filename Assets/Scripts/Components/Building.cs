using Extensions.Unity.MonoHelper;
using UnityEngine;

namespace Components
{
    public abstract partial class Building : EventListenerMono
    {
        [SerializeField] private int _id;
        public int ID => _id;
    }
}