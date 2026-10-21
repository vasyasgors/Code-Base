using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    public static class DistributionPlatformObjectValidator
    {
        [DidReloadScripts]
        static DistributionPlatformObjectValidator()
        {
            UpdateAllPlatformPrefabs();
        }

        public static void UpdateAllPlatformPrefabs()
        {
            // 1. Ищем конфиг окружения
            string[] configGuids = AssetDatabase.FindAssets("t:" + nameof(DistributionBuildProfiles));
            if (configGuids.Length == 0) return;

            string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            var environmentConfig = AssetDatabase.LoadAssetAtPath<DistributionBuildProfiles>(configPath);

            if (environmentConfig == null) return;

            // ПРОВЕРКА: Проверяем, что ссылка на рантайм-конфиг существует
            if (environmentConfig.RuntimeConfig == null)
            {
                Debug.LogWarning($"[{nameof(DistributionPlatformObjectValidator)}] Ссылка на RuntimeConfig не задана в {environmentConfig.name}. Валидация префабов пропущена.");
                return;
            }

            // Изменено: Берем текущую платформу из вынесенного RuntimeConfig
            DistributionPlatform currentPlatform = environmentConfig.RuntimeConfig.CurrentDistributionPlatform;

            // 2. Находим все PlatformPrefab ассеты
            string[] prefabGuids = AssetDatabase.FindAssets("t:" + nameof(DistributionPlatformObject));
            bool anyAssetChanged = false;

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var platformPrefab = AssetDatabase.LoadAssetAtPath<DistributionPlatformObject>(assetPath);

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
                                    Debug.LogError($"[{nameof(DistributionPlatformObjectValidator)}] Не удалось загрузить префаб по пути '{mapping.prefabPath}' для платформы {currentPlatform} в ассете {platformPrefab.name}");
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

                    Debug.Log($"[{nameof(DistributionPlatformObjectValidator)}] Ассет '{platformPrefab.name}' перекомпилирован под платформу {currentPlatform}. Активный префаб: {(targetPrefab != null ? targetPrefab.name : "null")}");
                }
            }

            if (anyAssetChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
