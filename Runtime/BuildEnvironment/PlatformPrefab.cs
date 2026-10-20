using System;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [Serializable]
    public class PlatformMapping
    {
        // Делаем поля публичными (или public свойства), чтобы редактор и валидатор имели к ним прямой доступ
        public DistributionPlatform platform;
        public string prefabPath;
        public GameObject prefab;
    }

    [CreateAssetMenu(fileName = "Platform Prefab", menuName = "Configs/Platform Prefab")]
    public class PlatformPrefab : ScriptableObject
    {
        // Поле должно называться точно так же, как в FindProperty в редакторе
        [SerializeField] private PlatformMapping[] platformMappings;

        public PlatformMapping[] PlatformMappings
        {
            get => platformMappings;
            set => platformMappings = value;
        }

        public GameObject GetPrefabForPlatform(DistributionPlatform platform)
        {
            if (platformMappings == null) return null;

            for (int i = 0; i < platformMappings.Length; i++)
            {
                if (platformMappings[i] != null && platformMappings[i].platform == platform)
                {
                    return platformMappings[i].prefab;
                }
            }
            return null;
        }
    }
}
