using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Infrastructure;
using System.Linq;

namespace CodeBase.Infrastructure.Editor
{
    public static class WebGLZipPostprocessor
    {
        [PostProcessBuild]
        public static void CreateZip(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.WebGL)
                return;

            var guids = AssetDatabase.FindAssets("t:GameBuildConfig");
            var config = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<GameBuildConfig>(path))
                .FirstOrDefault(x => x != null);


            string platformName = config != null ? config.GetVariantName() : EditorUserBuildSettings.activeBuildTarget.ToString();
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
