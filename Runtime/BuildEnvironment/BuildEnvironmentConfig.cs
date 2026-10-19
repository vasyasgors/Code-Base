using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [System.Serializable]
    public class EnvironmentSettings
    {
        [Tooltip("Понятное название для отображения в списке и выпадающем меню")]
        public string label;

        [Header("Target Pair")]
        public PlatformSDK Sdk;
        public DistributionPlatform Platform;

        [Header("Features")]
        public bool ExcludeStreamingAssets;
        public bool CreateZipArchive;

        [Header("Assets Exclusion")]
        [Tooltip("Пути к папкам относительно ассетов, которые нужно исключить из билда (например: Assets/SecretFolder)")]
        public List<string> ExcludeFolderPaths = new List<string>();
    }

    [CreateAssetMenu(fileName = "Build Environment Config", menuName = "Configs/Build Environment Config")]
    public class BuildEnvironmentConfig : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";
        public const string PlatfromSDKPostfix = "_SDK";

        [HideInInspector]
        public int ActiveEnvironmentIndex = 0; // Хранит индекс выбранного окружения из списка

        [Header("All Environment Configurations")]
        public List<EnvironmentSettings> Environments = new List<EnvironmentSettings>();

        // Возвращает текущее активное окружение
        public EnvironmentSettings GetCurrentSettings()
        {
            if (Environments == null || Environments.Count == 0) return null;

            // Защита от выхода за границы массива
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

        public string GetCurrentDistributionPlatformName()
        {
            var current = GetCurrentSettings();
            return current != null ? current.Platform.ToString() : "UnknownPlatform";
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
            }
        }
#endif
    }


}
