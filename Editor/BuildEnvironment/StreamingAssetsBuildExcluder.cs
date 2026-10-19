using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    public class AssetExclusionPreprocessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private readonly string _streamingAssetsOrigin = Path.Combine(Application.dataPath, "StreamingAssets");
        private readonly string _streamingAssetsHidden = Path.Combine(Application.dataPath, "StreamingAssets~");

        // Реестр для штатного возврата папок во время одной сессии билда
        private static List<(string origin, string hidden)> _hiddenFoldersRegistry = new List<(string origin, string hidden)>();

        public int callbackOrder => int.MinValue;

        public void OnPreprocessBuild(BuildReport report)
        {
            _hiddenFoldersRegistry.Clear();

            BuildEnvironmentConfig config = BuildEnvironmentConfigEditor.Load();
            if (config == null) return;

            EnvironmentSettings currentSettings = config.GetCurrentSettings();
            if (currentSettings == null) return;

            // 1. Исключение StreamingAssets
            if (currentSettings.ExcludeStreamingAssets)
            {
                HideFolder(_streamingAssetsOrigin, _streamingAssetsHidden);
            }

            // 2. Исключение списка кастомных папок
            if (currentSettings.ExcludeFolderPaths != null)
            {
                foreach (string relativePath in currentSettings.ExcludeFolderPaths)
                {
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
            bool restoredAnything = false;

            // ШАГ 1: Штатный возврат папок из оперативной памяти
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

            // ШАГ 2: ЗАПАСНОЙ ПЛАН (На случай краша Unity, когда память очистилась)
            // Проверяем StreamingAssets
            if (Directory.Exists(_streamingAssetsHidden))
            {
                RestoreFolderData(_streamingAssetsHidden, _streamingAssetsOrigin);
                restoredAnything = true;
            }

            // Проверяем все кастомные директории ресурсов из конфига
            BuildEnvironmentConfig config = BuildEnvironmentConfigEditor.Load();
            if (config != null)
            {
                EnvironmentSettings currentSettings = config.GetCurrentSettings();
                if (currentSettings != null && currentSettings.ExcludeFolderPaths != null)
                {
                    foreach (string relativePath in currentSettings.ExcludeFolderPaths)
                    {
                        if (string.IsNullOrWhiteSpace(relativePath)) continue;

                        string absoluteOriginPath = GetAbsoluteSystemPath(relativePath);
                        string absoluteHiddenPath = absoluteOriginPath + "~";

                        // Если реестр стерся, но на диске физически осталась скрытая папка — возвращаем
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
                Debug.Log($"[BuildPipeline] Папка временно исключена: {origin}");
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
            Debug.Log($"[BuildPipeline] Папка успешно возвращена: {origin}");
        }

        // Хелпер для получения чистого абсолютного пути без дублирующихся слешей
        private string GetAbsoluteSystemPath(string relativePath)
        {
            string absolute = Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativePath));
            if (absolute.EndsWith("/") || absolute.EndsWith("\\"))
            {
                absolute = absolute.Substring(0, absolute.Length - 1);
            }
            return absolute;
        }
    }
}
