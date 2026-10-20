using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [System.Serializable]
    public class EnvironmentSettings
    {
        [Tooltip("Понятное название для выпадающего меню")]
        public string label;

        [Header("Target Pair")]
        public PlatformSDK Sdk;
        public DistributionPlatform Platform;

        [Header("Features")]
        [Tooltip("Создавать ли ZIP-архив после сборки WebGL проекта")]
        public bool CreateZipArchive;

        [Header("Exclusion Configuration")]
        [Tooltip("Перетащите сюда папки из окна Project, которые нужно исключить из сборки.")]
        // Используем базовый Object, который доступен в Runtime
        public List<UnityEngine.Object> ExcludedFolders = new List<UnityEngine.Object>();
    }
}
