using System.IO;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Asset Bundles Configs", menuName = "Configs/Asset Bundles Configs")]
    public class AssetBundlesConfigs : ScriptableObject
    {
        private const string Path = @"Assets/Resources/LocalizationSettings.asset";

        public string AssetBundleDirectoryStandaloneWindows = "D:/Repositories/vasyasgors.github.io/UnityAssetBundles/" + Application.productName + "/StandaloneWindows";
        public string AssetBundleDirectoryWebGL = "D:/Repositories/vasyasgors.github.io/UnityAssetBundles/" + Application.productName + "/WebGL";

        public string AssetBundleRemoteURLStandaloneWindows = "https://storage.yandexcloud.net/unityassets/" + Application.productName + "/StandaloneWindows";
        public string AssetBundleRemoteURLWebGL = "https://storage.yandexcloud.net/unityassets/" + Application.productName + "/WebGL";

        private static AssetBundlesConfigs _instance;

        public static AssetBundlesConfigs Instance
        {
            get
            {
                if (_instance == null) _instance = LoadSettings();
                return _instance;
            }
        }

        private static AssetBundlesConfigs LoadSettings()
        {
            var settings = Resources.Load<AssetBundlesConfigs>(System.IO.Path.GetFileNameWithoutExtension(Path));

            if (settings != null)
                return settings;
            else
                Debug.LogError("Asset bundles config not found!");

            return settings;
        }


    }
}


