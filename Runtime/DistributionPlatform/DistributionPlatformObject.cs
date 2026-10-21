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

    [CreateAssetMenu(fileName = "Distribution Platform Object", menuName = "Configs/Distribution Platform Object")]
    public class DistributionPlatformObject : ScriptableObject
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private PlatformPathMapping[] platformMappings;

        // Публичные свойства для чтения/записи (нужны для редактора и валидатора)
        public GameObject GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }

        public PlatformPathMapping[] PlatformMappings
        {
            get => platformMappings;
            set => platformMappings = value;
        }

    }
}
