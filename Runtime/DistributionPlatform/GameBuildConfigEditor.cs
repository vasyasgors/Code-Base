using CodeBase.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;

#endif

using UnityEngine;

namespace CodeBase.Infrastructure
{

#if UNITY_EDITOR
    [CustomEditor(typeof(DistributionPlatformConfig))]
    public class GameBuildConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();



            DrawDefaultInspector();

        
            EditorGUILayout.Space();


            if (GUILayout.Button("Apply Platform Defines"))
            {
                serializedObject.ApplyModifiedProperties();

                var config = (DistributionPlatformConfig)target;

                ApplyDefines(config);

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
            }

            serializedObject.ApplyModifiedProperties();
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
            Debug.Log($"Applied define: {config.GetDistributionPlatformDefine()}");
        }

        private static DistributionPlatformConfig Load()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(DistributionPlatformConfig));
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
                .Where(s => !s.StartsWith(DistributionPlatformConfig.PlatfromSDKPrefix))
                .ToList();

            existing.Add(config.GetDistributionPlatformDefine());
            existing.Add(config.GetPlatformSDKDefine());

            PlayerSettings.SetScriptingDefineSymbols(target, existing.ToArray());
        }

    }

#endif

}
