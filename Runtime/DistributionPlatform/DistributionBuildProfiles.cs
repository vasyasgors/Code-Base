using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Distribution Build Profiles", menuName = "Configs/Distribution Build Profiles")]
    public class DistributionBuildProfiles : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";
        public const string PlatfromSDKPostfix = "_SDK";

        [HideInInspector]
        public int ActiveEnvironmentIndex = 0;

        [Header("Runtime Configuration Link")]
        public RuntimeDistributionPlatfromConfig RuntimeConfig;

        [Space(10)]
        public List<DistributionBuildProfile> Profiles = new List<DistributionBuildProfile>();

        public DistributionBuildProfile GetCurrentSettings()
        {
            if (Profiles == null || Profiles.Count == 0) return null;

            if (ActiveEnvironmentIndex < 0 || ActiveEnvironmentIndex >= Profiles.Count)
            {
                ActiveEnvironmentIndex = 0;
            }

            return Profiles[ActiveEnvironmentIndex];
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
            if (Profiles == null) return;

            for (int i = 0; i < Profiles.Count; i++)
            {
                var env = Profiles[i];
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
            if (activeSettings != null && RuntimeConfig != null)
            {
                RuntimeConfig.CurrentPlatformSDK = activeSettings.Sdk;
                RuntimeConfig.CurrentDistributionPlatform = activeSettings.Platform;
                UnityEditor.EditorUtility.SetDirty(RuntimeConfig);
            }
        }

        private bool AssetDatabaseIsValidFolder(string assetPath)
        {
            return System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), assetPath));
        }
#endif
    }
}
