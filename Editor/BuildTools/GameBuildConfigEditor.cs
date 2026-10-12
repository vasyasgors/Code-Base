using CodeBase.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [CustomEditor(typeof(DistributionPlatformConfig))]
    public class GameBuildConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUI.BeginChangeCheck();
           
            DrawDefaultInspector();


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                var config = (DistributionPlatformConfig) target;
                ApplyDefines(config);
                EditorUtility.SetDirty(config);
                

                AssetDatabase.SaveAssets();
            }
            else
            {
                serializedObject.ApplyModifiedProperties();
            }
        }


        [MenuItem("Tools/Distribution Platforms/Open Config")]
        public static void OpenConfig()
        {
            var config = Load();
            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);
        }

        [MenuItem("Tools/Distribution Platforms/Apply Config Defines")]
        public static void ApplyConfigDefines()
        {
            var config = Load();
            ApplyDefines(config);
            Debug.Log($"Applied define: {config.GetDefineSymbol()}");
        }

        private static DistributionPlatformConfig Load()
        {
            var guids = AssetDatabase.FindAssets("t:DistributionPlatformConfig");
            var config = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<DistributionPlatformConfig>(path))
                .FirstOrDefault(x => x != null);

            if (config != null)
                return config;

            Debug.LogError("[Distribution Platform Config] not found!");
            return null;
        }

        private static void ApplyDefines(DistributionPlatformConfig config)
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var existing = PlayerSettings.GetScriptingDefineSymbols(target)
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith(DistributionPlatformConfig.DistributionPlatformPrefix))
                .ToList();

            existing.Add(config.GetDefineSymbol());
            PlayerSettings.SetScriptingDefineSymbols(target, existing.ToArray());
        }

    }
}
