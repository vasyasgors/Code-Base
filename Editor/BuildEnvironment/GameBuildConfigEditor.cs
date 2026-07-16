using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [CustomEditor(typeof(GameBuildConfig))]
    public class GameBuildConfigEditor : UnityEditor.Editor
    {
        //SerializedProperty currentDistributionPlatform;

        void OnEnable()
        {
            //currentDistributionPlatform = serializedObject.FindProperty("currentDistributionPlatform");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUI.BeginChangeCheck();
           
            DrawDefaultInspector();


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                var config = (GameBuildConfig) target;
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

        private static GameBuildConfig Load()
        {
            var guids = AssetDatabase.FindAssets("t:GameBuildConfig");
            var config = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<GameBuildConfig>(path))
                .FirstOrDefault(x => x != null);

            if (config != null)
                return config;

            config = ScriptableObject.CreateInstance<GameBuildConfig>();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return config;
        }

        private static void ApplyDefines(GameBuildConfig config)
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var existing = PlayerSettings.GetScriptingDefineSymbols(target)
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith(GameBuildConfig.DistributionPlatformPrefix))
                .ToList();

            existing.Add(config.GetDefineSymbol());
            PlayerSettings.SetScriptingDefineSymbols(target, existing.ToArray());
        }

    }
}
