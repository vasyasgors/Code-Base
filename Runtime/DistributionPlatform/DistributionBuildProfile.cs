using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [System.Serializable]
    public class DistributionBuildProfile
    {
        public string label;
        public PlatformSDK Sdk;
        public DistributionPlatform Platform;
        public bool CreateZipArchive;
        public bool DecompressionFallback;
        public string WebGlTemplate = "APPLICATION:Default";
        public List<UnityEngine.Object> ExcludedFolders = new List<UnityEngine.Object>();
    }
}
