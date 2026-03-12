#if UNITY_EDITOR

using UnityEditor;
using System.IO;


namespace CodeBase.Infrastructure
{
    public class AssetBundlesBuilder
    {


        [MenuItem("Assets/Build AssetBundles for StandaloneWindows")]
        public static void BuildAllAssetBundlesStandaloneWindows()
        {
            if (!Directory.Exists(AssetBundlesConfigs.Instance.AssetBundleDirectoryStandaloneWindows)) Directory.CreateDirectory(AssetBundlesConfigs.Instance.AssetBundleDirectoryStandaloneWindows);

            BuildPipeline.BuildAssetBundles(AssetBundlesConfigs.Instance.AssetBundleDirectoryStandaloneWindows, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }

        [MenuItem("Assets/Build AssetBundles for WebGL")]
        public static void BuildAllAssetBundlesWegGL()
        {
            if (!Directory.Exists(AssetBundlesConfigs.Instance.AssetBundleDirectoryWebGL)) Directory.CreateDirectory(AssetBundlesConfigs.Instance.AssetBundleDirectoryWebGL);

            BuildPipeline.BuildAssetBundles(AssetBundlesConfigs.Instance.AssetBundleDirectoryWebGL, BuildAssetBundleOptions.None, BuildTarget.WebGL);
        }
    }

}

#endif



