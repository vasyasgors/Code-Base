using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    public static class PlatformPrefabValidator
    {
        [DidReloadScripts]
        static PlatformPrefabValidator()
        {
            UpdateAllPlatformPrefabs();
        }

        public static void UpdateAllPlatformPrefabs()
        {
            string[] configGuids = AssetDatabase.FindAssets("t:" + nameof(BuildEnvironmentConfig));
            if (configGuids.Length == 0)
            {
                return;
            }

            string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            var environmentConfig = AssetDatabase.LoadAssetAtPath<BuildEnvironmentConfig>(configPath);

            if (environmentConfig == null) return;

            DistributionPlatform currentPlatform = environmentConfig.CurrentDistributionPlatform;

            string[] prefabGuids = AssetDatabase.FindAssets("t:" + nameof(PlatformPrefab));
            bool anyAssetChanged = false;

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var platformPrefab = AssetDatabase.LoadAssetAtPath<PlatformPrefab>(assetPath);

                if (platformPrefab == null || platformPrefab.PlatformMappings == null) continue;

                bool assetModified = false;

                for (int i = 0; i < platformPrefab.PlatformMappings.Length; i++)
                {
                    var mapping = platformPrefab.PlatformMappings[i];
                    if (mapping == null) continue;

                    GameObject loadedPrefab = null;
                    if (!string.IsNullOrEmpty(mapping.prefabPath))
                    {
                        loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mapping.prefabPath);

                        if (loadedPrefab == null)
                        {
                            Debug.LogError($"[{nameof(PlatformPrefabValidator)}] Не удалось загрузить префаб по пути '{mapping.prefabPath}' для платформы {mapping.platform} в ассете {platformPrefab.name}");
                        }
                    }

                    if (mapping.prefab != loadedPrefab)
                    {
                        mapping.prefab = loadedPrefab;
                        assetModified = true;
                    }
                }

                if (assetModified)
                {
                    EditorUtility.SetDirty(platformPrefab);
                    anyAssetChanged = true;
                }
            }

            if (anyAssetChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
