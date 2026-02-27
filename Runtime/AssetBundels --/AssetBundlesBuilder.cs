#if UNITY_EDITOR

using UnityEditor;
using System.IO;

public class AssetBundlesBuilder
{
  

    [MenuItem("Assets/Build AssetBundles for StandaloneWindows")]
    public static void BuildAllAssetBundlesStandaloneWindows()
    {
        if (!Directory.Exists(AssetBundlesConfigs.AssetBundleDirectoryStandaloneWindows)) Directory.CreateDirectory(AssetBundlesConfigs.AssetBundleDirectoryStandaloneWindows);

        BuildPipeline.BuildAssetBundles(AssetBundlesConfigs.AssetBundleDirectoryStandaloneWindows, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Assets/Build AssetBundles for WebGL")]
    public static void BuildAllAssetBundlesWegGL()
    {
        if (!Directory.Exists(AssetBundlesConfigs.AssetBundleDirectoryWebGL)) Directory.CreateDirectory(AssetBundlesConfigs.AssetBundleDirectoryWebGL);

        BuildPipeline.BuildAssetBundles(AssetBundlesConfigs.AssetBundleDirectoryWebGL, BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }
}


#endif



