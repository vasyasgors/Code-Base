using CodeBase.Infrastructure;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [CustomEditor(typeof(DistributionBuildProfiles))]
    public class DistributionBuildProfilesEditor : UnityEditor.Editor
    {
        private bool _isDirty = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var config = (DistributionBuildProfiles)target;

            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Selected profile", EditorStyles.boldLabel);

            if (config.Profiles != null && config.Profiles.Count > 0)
            {
                string[] options = config.Profiles.Select(e => e.label).ToArray();
                int currentIndex = config.ActiveEnvironmentIndex;
                int newIndex = EditorGUILayout.Popup("Active Environment Target", currentIndex, options);

                if (newIndex != currentIndex)
                {
                    config.ActiveEnvironmentIndex = newIndex;
                    _isDirty = true;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Добавьте хотя бы одно окружение в список выше!", MessageType.Warning);
            }

            if (EditorGUI.EndChangeCheck())
            {
                _isDirty = true;
            }

            EditorGUILayout.Space(10);

            Color originalColor = GUI.backgroundColor;
            if (_isDirty)
            {
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f);
            }

            if (GUILayout.Button("Apply Defines and Recompile", GUILayout.Height(30)))
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);

                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                _isDirty = false;

                Debug.Log($"[BuildEnvironmentConfig] Настройки применены! Шаблон: {config.GetCurrentSettings()?.WebGlTemplate}");
            }

            GUI.backgroundColor = originalColor;

            if (serializedObject.ApplyModifiedProperties())
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);
            }
        }

        private void SyncTargets(DistributionBuildProfiles config)
        {
            var activeSettings = config.GetCurrentSettings();
            if (activeSettings != null && config.RuntimeConfig != null)
            {
                // Изменено: Удалена строчка переноса SDK в RuntimeConfig
                config.RuntimeConfig.CurrentPlatform = activeSettings.Platform;
                EditorUtility.SetDirty(config.RuntimeConfig);
            }
        }

        private void ApplyConfigAndSave(DistributionBuildProfiles config)
        {
            ApplyDefines(config);
            ApplyWebGLSettings(config);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static void ApplyWebGLSettings(DistributionBuildProfiles config)
        {
            var activeSettings = config.GetCurrentSettings();
            if (activeSettings == null) return;

            PlayerSettings.WebGL.decompressionFallback = activeSettings.DecompressionFallback;

            if (!string.IsNullOrEmpty(activeSettings.WebGlTemplate))
            {
                PlayerSettings.WebGL.template = activeSettings.WebGlTemplate;
            }
        }

        private static void ApplyDefines(DistributionBuildProfiles config)
        {
            var target = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var existing = PlayerSettings.GetScriptingDefineSymbols(target)
                .Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith(DistributionBuildProfiles.DistributionPlatformPrefix))
                .ToList();

            existing.Add(config.GetDistributionPlatformDefine());

            // Изменено: Добавляем все дефайны из списка SDK активного профиля
            existing.AddRange(config.GetPlatformSDKDefines());

            PlayerSettings.SetScriptingDefineSymbols(target, existing.ToArray());
        }

        [MenuItem("Tools/Distribution Build Profils")]
        public static void OpenConfig()
        {
            var config = Load();
            if (config != null)
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
        }

        public static DistributionBuildProfiles Load()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(DistributionBuildProfiles));
            var config = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath<DistributionBuildProfiles>(path))
                .FirstOrDefault(x => x != null);

            if (config == null) Debug.LogError("[Build Environment Config] не найден в проекте!");
            return config;
        }
    }
}
