using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [System.Serializable]
    public class EnvironmentSettings
    {
        [Tooltip("Понятное название для выпадающего меню")]
        public string label;

        public PlatformSDK Sdk;
        public DistributionPlatform Platform;
        public bool CreateZipArchive;
        public bool DecompressionFallback;
        public string WebGlTemplate = "APPLICATION:Default";

        [Header("Exclusion Configuration")]
        [Tooltip("Перетащите сюда папки из окна Project, которые нужно исключить из сборки.")]
        public List<UnityEngine.Object> ExcludedFolders = new List<UnityEngine.Object>();
    }
}
