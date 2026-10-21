using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object; // Защита от неоднозначности типов

namespace CodeBase.Infrastructure.Editor
{
    public class AssetExclusionPreprocessor : IPreprocessBuildWithReport
    {
        private static List<(string origin, string hidden)> _hiddenFoldersRegistry = new List<(string origin, string hidden)>();
        private static bool _isMonitoring = false;

        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            _hiddenFoldersRegistry.Clear();

            DistributionBuildProfiles config = DistributionBuildProfilesEditor.Load();
            if (config == null) return;

            DistributionBuildProfile currentSettings = config.GetCurrentSettings();
            if (currentSettings == null) return;

            if (currentSettings.ExcludedFolders != null)
            {
                foreach (Object folderAsset in currentSettings.ExcludedFolders)
                {
                    if (folderAsset == null) continue;

                    string relativePath = AssetDatabase.GetAssetPath(folderAsset);
                    if (string.IsNullOrWhiteSpace(relativePath)) continue;

                    string absoluteOriginPath = GetAbsoluteSystemPath(relativePath);
                    string absoluteHiddenPath = absoluteOriginPath + "~";

                    HideFolder(absoluteOriginPath, absoluteHiddenPath);
                }
            }

            if (_hiddenFoldersRegistry.Count > 0)
            {
                AssetDatabase.Refresh();
                StartBuildMonitoring();
            }
        }

        private static async void StartBuildMonitoring()
        {
            if (_isMonitoring) return;
            _isMonitoring = true;

            while (BuildPipeline.isBuildingPlayer)
            {
                await Task.Delay(500);
            }

            ExecuteCleanup();
            _isMonitoring = false;
        }

        private static void ExecuteCleanup()
        {
            bool restoredAnything = false;

            // ШАГ 1: Восстановление из оперативной памяти сессии
            for (int i = _hiddenFoldersRegistry.Count - 1; i >= 0; i--)
            {
                var folderPair = _hiddenFoldersRegistry[i];

                // Оборачиваем в try-catch каждую итерацию отдельно, чтобы цикл не прерывался
                try
                {
                    if (Directory.Exists(folderPair.hidden))
                    {
                        RestoreFolderData(folderPair.hidden, folderPair.origin);
                        restoredAnything = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[BuildPipeline] Ошибка восстановления папки из реестра ({folderPair.hidden}): {e.Message}");
                }
            }
            _hiddenFoldersRegistry.Clear();

            // ШАГ 2: БЕЗОПАСНЫЙ СЕЙВ-ПЛАН (Чтение конфига напрямую, если память сбросилась)
            DistributionBuildProfiles config = DistributionBuildProfilesEditor.Load();
            if (config != null)
            {
                DistributionBuildProfile currentSettings = config.GetCurrentSettings();
                if (currentSettings != null && currentSettings.ExcludedFolders != null)
                {
                    foreach (Object folderAsset in currentSettings.ExcludedFolders)
                    {
                        if (folderAsset == null) continue;

                        // Оборачиваем в try-catch проверку каждой папки из ассета
                        try
                        {
                            string relativePath = AssetDatabase.GetAssetPath(folderAsset);
                            if (string.IsNullOrWhiteSpace(relativePath)) continue;

                            string absoluteOriginPath = GetAbsoluteSystemPath(relativePath);
                            string absoluteHiddenPath = absoluteOriginPath + "~";

                            if (Directory.Exists(absoluteHiddenPath))
                            {
                                RestoreFolderData(absoluteHiddenPath, absoluteOriginPath);
                                restoredAnything = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"[BuildPipeline] Ошибка восстановления папки из сейв-плана ({folderAsset.name}): {e.Message}");
                        }
                    }
                }
            }

            if (restoredAnything)
            {
                AssetDatabase.Refresh();
                Debug.Log("[BuildPipeline] Процесс восстановления завершен. Доступные исключенные директории возвращены в проект.");
            }
        }

        private void HideFolder(string origin, string hidden)
        {
            try
            {
                if (Directory.Exists(origin))
                {
                    Directory.Move(origin, hidden);

                    string metaOrigin = origin + ".meta";
                    string metaHidden = hidden + ".meta";
                    if (File.Exists(metaOrigin))
                    {
                        Directory.Move(metaOrigin, metaHidden);
                    }

                    _hiddenFoldersRegistry.Add((origin, hidden));
                    Debug.Log($"[BuildPipeline] Папка временно скрыта: {origin}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[BuildPipeline] Не удалось скрыть папку ({origin}): {e.Message}");
            }
        }

        private static void RestoreFolderData(string hidden, string origin)
        {
            Directory.Move(hidden, origin);

            string metaHidden = hidden + ".meta";
            string metaOrigin = origin + ".meta";
            if (File.Exists(metaHidden))
            {
                Directory.Move(metaHidden, metaOrigin);
            }
        }

        private static string GetAbsoluteSystemPath(string relativePath)
        {
            string absolute = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));
            if (absolute.EndsWith("/") || absolute.EndsWith("\\"))
            {
                absolute = absolute.Substring(0, absolute.Length - 1);
            }
            return absolute;
        }
    }
}
