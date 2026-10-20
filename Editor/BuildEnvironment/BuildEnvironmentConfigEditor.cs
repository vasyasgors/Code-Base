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
        private bool _isDirty = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var config = (BuildEnvironmentConfig)target;

            // Включаем отслеживание изменений перед отрисовкой инспектора
            EditorGUI.BeginChangeCheck();

            // 1. Рисуем полностью стандартный инспектор (включая массив Environments и наш Drawer)
            DrawDefaultInspector();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Active Environment Selection", EditorStyles.boldLabel);

            // 2. Отрисовка выпадающего меню выбора текущего активного окружения
            if (config.Environments != null && config.Environments.Count > 0)
            {
                string[] options = config.Environments.Select(e => e.label).ToArray();
                int currentIndex = config.ActiveEnvironmentIndex;
                int newIndex = EditorGUILayout.Popup("Active Environment Target", currentIndex, options);

                if (newIndex != currentIndex)
                {
                    config.ActiveEnvironmentIndex = newIndex;
                    _isDirty = true; // Переключение активного окружения тоже делает конфиг "грязным"
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Добавьте хотя бы одно окружение в список выше!", MessageType.Warning);
            }

            // Если хоть одно поле, массив, размер массива или папка изменились во время отрисовки:
            if (EditorGUI.EndChangeCheck())
            {
                _isDirty = true; // Подсвечиваем кнопку красным
            }

            EditorGUILayout.Space(10);

            // 3. Подсветка кнопки применения изменений
            Color originalColor = GUI.backgroundColor;
            if (_isDirty)
            {
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f); // Спокойный красный цвет
            }

            if (GUILayout.Button("Apply Defines and Recompile", GUILayout.Height(30)))
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);

                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
                _isDirty = false; // Сбрасываем флаг, так как все применили

                Debug.Log($"[BuildEnvironmentConfig] Дефайны и настройки WebGL успешно применены! Текущий шаблон: {config.GetCurrentSettings()?.WebGlTemplate}");
            }

            GUI.backgroundColor = originalColor;

            // Применяем изменения в сериализованный объект
            if (serializedObject.ApplyModifiedProperties())
            {
                // Вызываем синхронизацию и сохранение ассета, чтобы изменения в инспекторе не пропали при перезапуске Unity
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
            ApplyWebGLSettings(config); // Записываем Decompression Fallback и Template в PlayerSettings
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static void ApplyWebGLSettings(BuildEnvironmentConfig config)
        {
            var activeSettings = config.GetCurrentSettings();
            if (activeSettings == null) return;

            PlayerSettings.WebGL.decompressionFallback = activeSettings.DecompressionFallback;

            if (!string.IsNullOrEmpty(activeSettings.WebGlTemplate))
            {
                PlayerSettings.WebGL.template = activeSettings.WebGlTemplate;
            }
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

            if (config == null) Debug.LogError("[Build Environment Config] не найден в проекте!");
            return config;
        }
    }
}
