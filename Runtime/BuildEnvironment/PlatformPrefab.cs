using System;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [Serializable]
    public class PlatformPathMapping
    {
        public DistributionPlatform platform;
        public string prefabPath;
    }

    [CreateAssetMenu(fileName = "Platform Prefab", menuName = "Configs/Platform Prefab")]
    public class PlatformPrefab : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private PlatformPathMapping[] platformMappings;

        // Публичные свойства для чтения/записи (нужны для редактора и валидатора)
        public GameObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }

        public PlatformPathMapping[] PlatformMappings
        {
            get => platformMappings;
            set => platformMappings = value;
        }

        /// <summary>
        /// Просто возвращает префаб, который валидатор уже подставил для текущей компиляции
        /// </summary>
        public GameObject GetPrefab()
        {
            return prefab;
        }
    }
}
