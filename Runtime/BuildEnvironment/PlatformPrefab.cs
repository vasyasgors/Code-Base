using System;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Platform Prefab", menuName = "Configs/Platform Prefab")]
    public class PlatformPrefab : ScriptableObject
    {
        [SerializeField] private string path;
        [SerializeField] private DistributionPlatform[] platforms;

        [SerializeField] private GameObject prefab;

        public string PrefabPath
        {
            get => path;
            set => path = value;
        }

        public DistributionPlatform[] Platforms => platforms;

        public GameObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }

        public GameObject GetPrefab()
        {
            if (prefab == null)
                throw new NullReferenceException( $"Prefab is null in {name}. Path: {path}");

            return prefab;
        }

        public bool ContainsPlatform(DistributionPlatform platform)
        {
            if (platforms == null)
                return false;

            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] == platform)
                    return true;
            }

            return false;
        }
    }
}