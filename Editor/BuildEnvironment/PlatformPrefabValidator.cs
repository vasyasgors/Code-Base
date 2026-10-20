using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{

    public static class PlatformPrefabValidator
    {
        // Статический конструктор вызывается автоматически при каждом запуске Unity и после каждой перекомпиляции
        [DidReloadScripts]
        static PlatformPrefabValidator()
        {
            UpdateAllPlatformPrefabs();
        }

        public static void UpdateAllPlatformPrefabs()
        {
            // 1. Получаем текущую платформу из BuildEnvironmentConfig
            string[] configGuids = AssetDatabase.FindAssets("t:" + nameof(BuildEnvironmentConfig));
            if (configGuids.Length == 0)
            {
                Debug.LogWarning($"[{nameof(PlatformPrefabValidator)}] {nameof(BuildEnvironmentConfig)} не найден в проекте. Пропуск обновления префабов.");
                return;
            }

            // Берем первый найденный конфиг окружения
            string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            var environmentConfig = AssetDatabase.LoadAssetAtPath<BuildEnvironmentConfig>(configPath);

            if (environmentConfig == null) return;

            // Предполагаем, что у вашего конфига есть свойство или публичное поле CurrentDistributionPlatform
            DistributionPlatform currentPlatform = environmentConfig.CurrentDistributionPlatform;

            // 2. Находим все ассеты типа PlatformPrefab
            string[] prefabGuids = AssetDatabase.FindAssets("t:" + nameof(PlatformPrefab));
            bool anyAssetChanged = false;

            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var platformPrefab = AssetDatabase.LoadAssetAtPath<PlatformPrefab>(assetPath);

                if (platformPrefab == null) continue;

                GameObject targetPrefab = null;

                // 3. Проверяем, содержит ли префаб текущую платформу
                if (platformPrefab.ContainsPlatform(currentPlatform))
                {
                    if (!string.IsNullOrEmpty(platformPrefab.PrefabPath))
                    {
                        targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(platformPrefab.PrefabPath);

                        if (targetPrefab == null)
                        {
                            Debug.LogError($"[{nameof(PlatformPrefabValidator)}] Не удалось загрузить префаб по пути '{platformPrefab.PrefabPath}' для ассета {platformPrefab.name}");
                        }
                    }
                }

                // 4. Если значение поля изменилось, обновляем его и помечаем объект "грязным"
                if (platformPrefab.Prefab != targetPrefab)
                {
                    platformPrefab.Prefab = targetPrefab;

                    // Помечаем объект как измененный, чтобы Unity сохранила изменения на диск
                    EditorUtility.SetDirty(platformPrefab);
                    anyAssetChanged = true;

                    Debug.Log($"[{nameof(PlatformPrefabValidator)}] Ассет '{platformPrefab.name}' обновлен. Поле Prefab установлено в: {(targetPrefab != null ? targetPrefab.name : "null")}");
                }
            }

            // 5. Если хоть один ассет изменился, принудительно сохраняем базу данных ассетов
            if (anyAssetChanged)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
