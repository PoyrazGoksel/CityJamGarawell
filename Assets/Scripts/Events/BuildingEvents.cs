using Components.Buildings;
using UnityEngine.Events;

namespace Events
{
    public class BuildingEvents
    {
        public UnityAction<IBuilding> BuildingClicked;
        public UnityAction<bool> SortingRows;
        public UnityAction<IBuilding> BuildingArrivedToRow;
    }
}