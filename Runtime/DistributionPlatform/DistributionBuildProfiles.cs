using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Distribution Build Profiles", menuName = "Configs/Distribution Build Profiles")]
    public class DistributionBuildProfiles : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";
        public const string PlatfromSDKPostfix = "";

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

        // Изменено: Метод теперь возвращает список дефайнов для всех выбранных SDK
        public List<string> GetPlatformSDKDefines()
        {
            var current = GetCurrentSettings();
            var defines = new List<string>();

            if (current != null && current.Sdks != null && current.Sdks.Count > 0)
            {
                foreach (var sdk in current.Sdks)
                {
                    defines.Add(sdk.ToString().ToUpper() + PlatfromSDKPostfix);
                }
            }
            else
            {
                defines.Add("LOCAL" + PlatfromSDKPostfix);
            }

            return defines;
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
                    // Изменено: генерация лейбла с учетом нескольких SDK
                    string sdksLabel = env.Sdks != null && env.Sdks.Count > 0 ? string.Join(", ", env.Sdks) : "None";
                    env.label = $"[{env.Platform}] : [{sdksLabel}]";
                }

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
                // Изменено: Удалена строчка RuntimeConfig.CurrentPlatformSDK = activeSettings.Sdk;
                RuntimeConfig.CurrentPlatform = activeSettings.Platform;
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
