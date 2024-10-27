using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class BuildingData
    {
        public GameObject Prefab => _prefab;
        public Sprite Preview => _preview;
        [PreviewField][SerializeField] private GameObject _prefab;
        [PreviewField][SerializeField] private Sprite _preview;
    }
}