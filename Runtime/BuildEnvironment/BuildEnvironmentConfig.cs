using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [System.Serializable]
    public class EnvironmentSettings
    {
        [Tooltip("Понятное название для выпадающего меню")]
        public string label;

        [Header("Target Pair")]
        public PlatformSDK Sdk;
        public DistributionPlatform Platform;

        [Header("Features")]
        [Tooltip("Создавать ли ZIP-архив после сборки WebGL проекта")]
        public bool CreateZipArchive;

        [Header("Exclusion Configuration")]
        [Tooltip("Перетащите сюда папки из окна Project, которые нужно исключить из сборки.")]
        // Используем базовый Object, который доступен в Runtime
        public List<UnityEngine.Object> ExcludedFolders = new List<UnityEngine.Object>();
    }

    [CreateAssetMenu(fileName = "Build Environment Config", menuName = "Configs/Build Environment Config")]
    public class BuildEnvironmentConfig : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";
        public const string PlatfromSDKPostfix = "_SDK";

        [HideInInspector]
        public int ActiveEnvironmentIndex = 0;

        [Header("Current Synchronized Targets")]
        public PlatformSDK CurrentPlatformSDK = PlatformSDK.Local;
        public DistributionPlatform CurrentDistributionPlatform = DistributionPlatform.YandexGames;

        [Header("All Environment Configurations")]
        public List<EnvironmentSettings> Environments = new List<EnvironmentSettings>();

        public EnvironmentSettings GetCurrentSettings()
        {
            if (Environments == null || Environments.Count == 0) return null;

            if (ActiveEnvironmentIndex < 0 || ActiveEnvironmentIndex >= Environments.Count)
            {
                ActiveEnvironmentIndex = 0;
            }

            return Environments[ActiveEnvironmentIndex];
        }

        public string GetPlatformSDKDefine()
        {
            var current = GetCurrentSettings();
            return current != null ? current.Sdk.ToString().ToUpper() + PlatfromSDKPostfix : "LOCAL" + PlatfromSDKPostfix;
        }

        public string GetDistributionPlatformDefine()
        {
            var current = GetCurrentSettings();
            return current != null ? DistributionPlatformPrefix + current.Platform.ToString().ToUpper() : DistributionPlatformPrefix + "ANY";
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Environments == null) return;

            for (int i = 0; i < Environments.Count; i++)
            {
                var env = Environments[i];
                if (string.IsNullOrEmpty(env.label) || env.label.StartsWith("[") || env.label.Contains("+"))
                {
                    env.label = $"[{env.Sdk}] + [{env.Platform}]";
                }

                // ВАЛИДАЦИЯ: Защита от перетаскивания файлов вместо папок
                if (env.ExcludedFolders != null)
                {
                    for (int j = 0; j < env.ExcludedFolders.Count; j++)
                    {
                        var obj = env.ExcludedFolders[j];
                        if (obj != null)
                        {
                            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
                            // Если это не папка, обнуляем элемент
                            if (!string.IsNullOrEmpty(path) && !AssetDatabaseIsValidFolder(path))
                            {
                                Debug.LogWarning($"[BuildEnvironment] Объект '{obj.name}' не является папкой и не может быть добавлен в список исключений!");
                                env.ExcludedFolders[j] = null;
                            }
                        }
                    }
                }
            }

            var activeSettings = GetCurrentSettings();
            if (activeSettings != null)
            {
                CurrentPlatformSDK = activeSettings.Sdk;
                CurrentDistributionPlatform = activeSettings.Platform;
            }
        }

        private bool AssetDatabaseIsValidFolder(string assetPath)
        {
            return System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), assetPath));
        }
#endif
    }
}
