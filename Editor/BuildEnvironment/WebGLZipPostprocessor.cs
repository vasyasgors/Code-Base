using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Infrastructure;

namespace CodeBase.Infrastructure.Editor
{
    public static class WebGLZipPostprocessor
    {
        private const string ConfigPath = "Assets/Resources/GameBuildConfig.asset";

        [PostProcessBuild]
        public static void CreateZip(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.WebGL)
                return;

            var config = AssetDatabase.LoadAssetAtPath<GameBuildConfig>(ConfigPath);

            string platformName = config != null ? config.GetVariantName() : "WebGL";
            string version = config != null ? PlayerSettings.bundleVersion : PlayerSettings.bundleVersion;
            string gameName = config != null ? PlayerSettings.productName : PlayerSettings.productName;

            platformName = SanitizeFileName(platformName);
            version = SanitizeFileName(version);
            gameName = SanitizeFileName(gameName);

            string parentDirectory = Directory.GetParent(pathToBuiltProject).FullName;
            string archivePath = Path.Combine(parentDirectory, $"{platformName}_{version}_{gameName}.zip");

            if (File.Exists(archivePath))
                File.Delete(archivePath);

            ZipFile.CreateFromDirectory(pathToBuiltProject, archivePath);
            Debug.Log($"WebGL zip created: {archivePath}");
        }

        private static string SanitizeFileName(string name)
        {
            return Regex.Replace(name, @"[\\/:*?""<>|]", "_");
        }
    }
}
