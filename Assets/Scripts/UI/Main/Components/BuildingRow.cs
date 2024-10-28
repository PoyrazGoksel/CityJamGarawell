using Components.Buildings;
using Settings;
using UnityEngine;
using Zenject;

namespace UI.Main.Components
{
    public class BuildingRow : MonoBehaviour, IBuildingRow
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        public Transform Transform{get;private set;}
        public IBuilding Building{get;private set;}

        public void SetBuilding(IBuilding building)
        {
            Building = building;
        }

        private void Awake()
        {
            Transform = transform;
        }
    }

    public interface IBuildingRow
    {
        Transform Transform{get;}
        IBuilding Building{get;}

        void SetBuilding(IBuilding building);
    }
}