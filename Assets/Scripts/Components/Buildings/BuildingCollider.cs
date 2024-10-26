using Events.Internal;
using UnityEngine;
using Zenject;

namespace Components.Buildings
{
    [RequireComponent(typeof(BoxCollider))]
    public partial class BuildingCollider : MonoBehaviour
    {
        [Inject] private BuildingEventsInternal BuildingEventsInternal{get;set;}

        private void OnMouseUp()
        {
            BuildingEventsInternal.ColliderMouseUp?.Invoke();
        }
    }
}