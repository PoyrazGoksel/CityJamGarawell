using Extensions.DoTween;
using Settings;
using UnityEngine;
using Zenject;

namespace UI.Main.Components
{
    public class BuildingRow : MonoBehaviour, IBuildingRow
    {
        [Inject] private ProjectSettings ProjectSettings{get;set;}
        public Transform Transform{get;private set;}
        public ITweenContainer TweenContainer{get;set;}

        private void Awake()
        {
            Transform = transform;
        }
    }

    public interface IBuildingRow
    {
        Transform Transform{get;}
    }
}