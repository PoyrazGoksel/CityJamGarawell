using System;
using System.Collections.Generic;
using Extensions.Unity.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public class LevelData
    {
        public GameObject LevelPrefab => _levelPrefab;
        public float LevelTimerMins => _levelTimerMins;
        public TaskBuildings TaskBuildings => _taskBuildings;
        [SerializeField] private GameObject _levelPrefab;
        [SerializeField] private float _levelTimerMins = 4;
        [InfoBox("Key = Building ID, Value = Remaining Buildings")]
        [SerializeField] private TaskBuildings _taskBuildings;
    }

    [Serializable]
    public class TaskBuildings : UnityDictionary<int, int>
    {
        public TaskBuildings GetClone()
        {
            TaskBuildings clone = new();

            foreach(KeyValuePair<int, int> pair in this)
            {
                clone.Add(pair.Key, pair.Value);
            }
            
            return clone;
        }
    }
}