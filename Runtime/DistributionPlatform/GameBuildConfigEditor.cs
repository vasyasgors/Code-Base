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
    [CustomEditor(typeof(BuildEnvironmentConfig))]
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

                var config = (BuildEnvironmentConfig)target;

                ApplyDefines(config);

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
            }

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Tools/Build Environment Config")]
        public static void OpenConfig()
        {
            var config = Load();
            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);
        }

     

        private static BuildEnvironmentConfig Load()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(BuildEnvironmentConfig));
            var config = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<BuildEnvironmentConfig>(path))
                .FirstOrDefault(x => x != null);

            if (config != null)
                return config;

            Debug.LogError("[Distribution Platform Config] not found!");
            return null;
        }

        private static void ApplyDefines(BuildEnvironmentConfig config)
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var existing = PlayerSettings.GetScriptingDefineSymbols(target)
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith(BuildEnvironmentConfig.DistributionPlatformPrefix))
                .Where(s => !s.EndsWith(BuildEnvironmentConfig.PlatfromSDKPostfix))
                .ToList();

            existing.Add(config.GetDistributionPlatformDefine());
            existing.Add(config.GetPlatformSDKDefine());

            PlayerSettings.SetScriptingDefineSymbols(target, existing.ToArray());
        }

    }

#endif

}
