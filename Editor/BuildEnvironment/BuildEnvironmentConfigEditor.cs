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
        // Флаг, указывающий, что конфиг был изменен, но дефайны еще не применили с перекомпиляцией
        private bool _isDirty = false;

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
                    _isDirty = true; // Фиксируем изменение, но НЕ компилируем
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Добавьте хотя бы одно окружение в список выше!", MessageType.Warning);
            }

            EditorGUILayout.Space(10);

            // Если есть непримененные изменения, красим кнопку в красный цвет
            Color originalColor = GUI.backgroundColor;
            if (_isDirty)
            {
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f); // Спокойный красный цвет
            }

            // Кнопка "Apply Defines"
            if (GUILayout.Button("Apply Defines and Recompil", GUILayout.Height(30)))
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);

                // Принудительная перекомпиляция только по кнопке
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

                _isDirty = false; // Изменения успешно применены

                Debug.Log($"[BuildEnvironmentConfig] Дефайны успешно применены! SDK: {config.GetPlatformSDKDefine()}, Платформа: {config.GetDistributionPlatformDefine()}");
            }

            // Возвращаем оригинальный цвет GUI, чтобы не красить элементы ниже
            GUI.backgroundColor = originalColor;

            if (serializedObject.ApplyModifiedProperties())
            {
                SyncTargets(config);
                ApplyConfigAndSave(config);
                _isDirty = true; // Фиксируем изменения из дефолтного инспектора, но НЕ компилируем
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