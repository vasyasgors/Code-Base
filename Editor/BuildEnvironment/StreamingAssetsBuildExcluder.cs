using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    public class FollderExclusionPreprocessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private static List<(string origin, string hidden)> _hiddenFoldersRegistry = new List<(string origin, string hidden)>();

        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            _hiddenFoldersRegistry.Clear();

            BuildEnvironmentConfig config = BuildEnvironmentConfigEditor.Load();
            if (config == null) return;

            EnvironmentSettings currentSettings = config.GetCurrentSettings();
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
            }
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.result == BuildResult.Cancelled)
            {
                Debug.LogWarning("[BuildPipeline] Сборка ОТМЕНЕНА. Восстановление ресурсов...");
            }
            else if (report.summary.result == BuildResult.Failed)
            {
                Debug.LogError("[BuildPipeline] Сборка ЗАВЕРШИЛАСЬ ОШИБКОЙ. Восстановление ресурсов...");
            }

            bool restoredAnything = false;

            for (int i = _hiddenFoldersRegistry.Count - 1; i >= 0; i--)
            {
                var folderPair = _hiddenFoldersRegistry[i];
                if (Directory.Exists(folderPair.hidden))
                {
                    RestoreFolderData(folderPair.hidden, folderPair.origin);
                    restoredAnything = true;
                }
            }
            _hiddenFoldersRegistry.Clear();

            BuildEnvironmentConfig config = BuildEnvironmentConfigEditor.Load();
            if (config != null)
            {
                EnvironmentSettings currentSettings = config.GetCurrentSettings();
                if (currentSettings != null && currentSettings.ExcludedFolders != null)
                {
                    foreach (Object folderAsset in currentSettings.ExcludedFolders)
                    {
                        if (folderAsset == null) continue;

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
                }
            }

            if (restoredAnything)
            {
                AssetDatabase.Refresh();
                Debug.Log("[BuildPipeline] Все исключенные директории успешно возвращены в проект.");
            }
        }

        private void HideFolder(string origin, string hidden)
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

        private void RestoreFolderData(string hidden, string origin)
        {
            Directory.Move(hidden, origin);

            string metaHidden = hidden + ".meta";
            string metaOrigin = origin + ".meta";
            if (File.Exists(metaHidden))
            {
                Directory.Move(metaHidden, metaOrigin);
            }
        }

        private string GetAbsoluteSystemPath(string relativePath)
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
