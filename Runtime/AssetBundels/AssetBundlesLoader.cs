
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CodeBase.Infrastructure
{
    public static class AssetBundlesLoader
    {
        private static AssetBundlesConfigs AssetBundlesConfigs;

        private static string GetAssetBundlesPath(bool remote)
        {
            if (remote == true)
            {
                if (Application.isEditor == true) return AssetBundlesConfigs.AssetBundleRemoteURLStandaloneWindows;
                if (Application.platform == RuntimePlatform.WebGLPlayer) return AssetBundlesConfigs.AssetBundleRemoteURLWebGL;
            }

            if (remote == false)
            {
                if (Application.isEditor == true) return AssetBundlesConfigs.AssetBundleDirectoryStandaloneWindows;
                if (Application.platform == RuntimePlatform.WebGLPlayer) return AssetBundlesConfigs.AssetBundleDirectoryWebGL;
            }

            return "";
        }

        public static void SetConfig(AssetBundlesConfigs assetBundlesConfigs)
        {
            AssetBundlesConfigs = assetBundlesConfigs;
        }

        public static IEnumerator LoadAssetBundle(string bundleName, uint version, Action<AssetBundle> onSuccess, bool remote = false)
        {
            string bundleUrl = GetAssetBundlesPath(remote);

            if (remote == true)
            {
                using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl + "/" + bundleName, version))
                {
                    yield return uwr.SendWebRequest();

                    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);


                    if (onSuccess != null)
                        onSuccess(assetBundle);
                }
            }

            if (remote == false)
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleUrl + "/" + bundleName);

                if (onSuccess != null)
                    onSuccess(assetBundle);

                yield break;
            }
        }

        public static IEnumerator LoadAssetBundleFromRawBytes(string bundleName, Action<AssetBundle> onSuccess, bool remote = false)
        {
            string bundleUrl = GetAssetBundlesPath(remote);

            if (remote == true)
            {
                using (UnityWebRequest uwr = UnityWebRequest.Get(bundleUrl + "/" + bundleName))
                {
                    yield return uwr.SendWebRequest();

                    var bundle = AssetBundle.LoadFromMemory(uwr.downloadHandler.data);

                    if (onSuccess != null)
                        onSuccess(bundle);
                }
            }

            if (remote == false)
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleUrl + "/" + bundleName);
                if (onSuccess != null)
                    onSuccess(assetBundle);

                yield break;
            }
        }

        /*
        public static IEnumerator LoadAssetBundleFile(string url, uint version, Action<AssetBundle> onSuccess)
        {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, version))
            {
                yield return uwr.SendWebRequest();

                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);


                if (onSuccess != null)
                    onSuccess(assetBundle);
            }

        }*/
    }
}