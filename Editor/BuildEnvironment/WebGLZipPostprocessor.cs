using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CodeBase.Infrastructure.Editor
{
    public static class WebGLZipPostprocessor
    {
        [PostProcessBuild]
        public static void CreateZip(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.WebGL)
                return;

            BuildEnvironmentConfig config = BuildEnvironmentConfigEditor.Load();
            if (config == null) return;

            EnvironmentSettings currentSettings = config.GetCurrentSettings();
            if (currentSettings == null || !currentSettings.CreateZipArchive)
            {
                Debug.Log("[BuildPipeline] Создание WebGL zip архива пропущено.");
                return;
            }

            string platformName = config.GetCurrentDistributionPlatformName();
            string version = PlayerSettings.bundleVersion;
            string gameName = PlayerSettings.productName;

            platformName = SanitizeFileName(platformName);
            version = SanitizeFileName(version);
            gameName = SanitizeFileName(gameName);

            string parentDirectory = Directory.GetParent(pathToBuiltProject).FullName;
            string archivePath = Path.Combine(parentDirectory, $"{platformName}_{version}_{gameName}.zip");

            if (File.Exists(archivePath))
                File.Delete(archivePath);

            ZipFile.CreateFromDirectory(pathToBuiltProject, archivePath);
            Debug.Log($"WebGL zip создана: {archivePath}");
        }

        private static string SanitizeFileName(string name)
        {
            return Regex.Replace(name, @"[\\/:*?""<>|]", "_");
        }
    }
}
