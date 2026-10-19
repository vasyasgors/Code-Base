using CodeBase.Infrastructure;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [CustomEditor(typeof(BuildEnvironmentConfig))]
    public class BuildEnvironmentConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var config = (BuildEnvironmentConfig)target;

            // Рисуем стандартный инспектор Unity со всеми массивами
            DrawDefaultInspector();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Active Environment Selection", EditorStyles.boldLabel);

            // Отрисовка выпадающего меню выбора текущего активного окружения
            if (config.Environments != null && config.Environments.Count > 0)
            {
                string[] options = config.Environments.Select(e => e.label).ToArray();
                int currentIndex = config.ActiveEnvironmentIndex;
                int newIndex = EditorGUILayout.Popup("Active Environment Target", currentIndex, options);

                if (newIndex != currentIndex)
                {
                    config.ActiveEnvironmentIndex = newIndex;
                    SyncTargets(config);
                    ApplyConfigAndSave(config);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Добавьте хотя бы одно окружение в список выше!", MessageType.Warning);
            }

            EditorGUILayout.Space(10);

            // Кнопка "Apply Defines"
            if (GUILayout.Button("Apply Defines", GUILayout.Height(30)))
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);
                Debug.Log($"[BuildEnvironmentConfig] Дефайны успешно применены! SDK: {config.GetPlatformSDKDefine()}, Платформа: {config.GetDistributionPlatformDefine()}");
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);
            }
        }

        private void SyncTargets(BuildEnvironmentConfig config)
        {
            var activeSettings = config.GetCurrentSettings();
            if (activeSettings != null)
            {
                config.CurrentPlatformSDK = activeSettings.Sdk;
                config.CurrentDistributionPlatform = activeSettings.Platform;
            }
        }

        private void ApplyConfigAndSave(BuildEnvironmentConfig config)
        {
            ApplyDefines(config);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Build Environment Settings")]
        public static void OpenConfig()
        {
            var config = Load();
            if (config != null)
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
        }

        public static BuildEnvironmentConfig Load()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(BuildEnvironmentConfig));
            var config = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<BuildEnvironmentConfig>(path))
                .FirstOrDefault(x => x != null);

            if (config == null)
                Debug.LogError("[Build Environment Config] не найден в проекте!");

            return config;
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
}
