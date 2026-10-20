using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    public static class PlatformObjectValidator
    {
        [DidReloadScripts]
        static PlatformObjectValidator()
        {
            UpdateAllPlatformPrefabs();
        }

        public static void UpdateAllPlatformPrefabs()
        {
            // 1. Ищем конфиг окружения
            string[] configGuids = AssetDatabase.FindAssets("t:" + nameof(BuildEnvironmentConfig));
            if (configGuids.Length == 0) return;

            string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            var environmentConfig = AssetDatabase.LoadAssetAtPath<BuildEnvironmentConfig>(configPath);

            if (environmentConfig == null) return;

            DistributionPlatform currentPlatform = environmentConfig.CurrentDistributionPlatform;

            // 2. Находим все PlatformPrefab ассеты
            string[] prefabGuids = AssetDatabase.FindAssets("t:" + nameof(PlatformObject));
            bool anyAssetChanged = false;

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var platformPrefab = AssetDatabase.LoadAssetAtPath<PlatformObject>(assetPath);

                if (platformPrefab == null) continue;

                GameObject targetPrefab = null;

                // 3. Ищем путь префаба для текущей платформы в массиве маппингов
                if (platformPrefab.PlatformMappings != null)
                {
                    for (int i = 0; i < platformPrefab.PlatformMappings.Length; i++)
                    {
                        var mapping = platformPrefab.PlatformMappings[i];
                        if (mapping != null && mapping.platform == currentPlatform)
                        {
                            if (!string.IsNullOrEmpty(mapping.prefabPath))
                            {
                                targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mapping.prefabPath);

                                if (targetPrefab == null)
                                {
                                    Debug.LogError($"[{nameof(PlatformObjectValidator)}] Не удалось загрузить префаб по пути '{mapping.prefabPath}' для платформы {currentPlatform} в ассете {platformPrefab.name}");
                                }
                            }
                            break; // Нашли нужную платформу, выходим из цикла
                        }
                    }
                }

                // 4. Если текущий префаб компиляции отличается от целевого — обновляем его
                if (platformPrefab.GameObject != targetPrefab)
                {
                    platformPrefab.GameObject = targetPrefab;
                    EditorUtility.SetDirty(platformPrefab);
                    anyAssetChanged = true;

                    Debug.Log($"[{nameof(PlatformObjectValidator)}] Ассет '{platformPrefab.name}' перекомпилирован под платформу {currentPlatform}. Активный префаб: {(targetPrefab != null ? targetPrefab.name : "null")}");
                }
            }

            if (anyAssetChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
