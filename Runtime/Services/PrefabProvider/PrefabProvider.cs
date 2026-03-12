using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class PrefabProvider : MonoBehaviour, IPrefabProvider
    {
        [Header("Prefabs (Must be unique)")]
        [SerializeField] private GameObject[] prefabs;

        public TPrefab GetPrefab<TPrefab>() =>
            FindPrefabInArray<TPrefab>(prefabs);

        private TPrefab FindPrefabInArray<TPrefab>(GameObject[] prefabArray)
        {
            for (int i = 0; i < prefabArray.Length; i++)
            {
                TPrefab prefab = prefabArray[i].GetComponent<TPrefab>();

                if (prefab != null)
                    return prefab;
            }

            throw new NullReferenceException($"Type { typeof(TPrefab) } not found in { prefabArray } prefabs list!");
        }

    }
}
