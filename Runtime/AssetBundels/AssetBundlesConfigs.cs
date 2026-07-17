using System.IO;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Asset Bundles Configs", menuName = "Configs/Asset Bundles Configs")]
    public class AssetBundlesConfigs : ScriptableObject
    {
        public string AssetBundleDirectoryStandaloneWindows;
        public string AssetBundleDirectoryWebGL;

        public string AssetBundleRemoteURLStandaloneWindows = "https://storage.yandexcloud.net/unityassets/";
        public string AssetBundleRemoteURLWebGL = "https://storage.yandexcloud.net/unityassets/";

        private static AssetBundlesConfigs _instance;

        public static AssetBundlesConfigs Instance
        {
            get
            {
                if (_instance == null) _instance = LoadConfig();
                return _instance;
            }
        }

        private static AssetBundlesConfigs LoadConfig()
        {
            var settings = Resources.LoadAll<AssetBundlesConfigs>("")[0];

            if (settings != null)
                return settings;
            else
                Debug.LogError("Asset bundles config not found!");

            return settings;
        }


    }
}


