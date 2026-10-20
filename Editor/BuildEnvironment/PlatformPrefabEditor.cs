using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Editor;

namespace CodeBase.Infrastructure
{
    [CustomEditor(typeof(PlatformPrefab))]
    public class PlatformPrefabEditor : UnityEditor.Editor
    {
        // Флаг, указывающий на наличие непримененных изменений
        private bool _isDirty = false;

        public override void OnInspectorGUI()
        {
            var script = (PlatformPrefab)target;

            serializedObject.Update();

            // 1. Поле выбора префаба
            GameObject currentAsset = null;
            if (!string.IsNullOrEmpty(script.PrefabPath))
            {
                currentAsset = AssetDatabase.LoadAssetAtPath<GameObject>(script.PrefabPath);
            }

            EditorGUI.BeginChangeCheck();
            GameObject newAsset = (GameObject)EditorGUILayout.ObjectField("Target Prefab", currentAsset, typeof(GameObject), false);

            if (EditorGUI.EndChangeCheck())
            {
                if (newAsset != null)
                {
                    string path = AssetDatabase.GetAssetPath(newAsset);
                    script.PrefabPath = path;
                }
                else
                {
                    script.PrefabPath = string.Empty;
                }

                EditorUtility.SetDirty(script);
                _isDirty = true; // Фиксируем изменение пути
            }

            // 2. Путь под полем выбора
            EditorGUILayout.LabelField("Prefab Path", script.PrefabPath);

            // 3. Массив платформ
            SerializedProperty platformsProp = serializedObject.FindProperty("platforms");
            if (platformsProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(platformsProp, new GUIContent("Platforms"), true);
                if (EditorGUI.EndChangeCheck())
                {
                    _isDirty = true; // Фиксируем изменение списка платформ
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Поле 'platforms' не найдено.", MessageType.Warning);
            }

            // 4. Сериализованная ссылка на префаб, как в обычном инспекторе
            SerializedProperty loadedPrefabProp = serializedObject.FindProperty("prefab");

            EditorGUI.BeginDisabledGroup(true); // Блокируем изменение для всех элементов внутри группы
            EditorGUILayout.PropertyField(loadedPrefabProp, new GUIContent("Loaded Runtime Prefab"), true);
            EditorGUI.EndDisabledGroup(); // Снимаем блокировку

            EditorGUILayout.Space(15);

            // Кнопка перекомпиляции с динамическим цветом
            Color originalColor = GUI.backgroundColor;
            if (_isDirty)
            {
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f); // Спокойный красный цвет
            }

            if (GUILayout.Button("Apply and Recompile", GUILayout.Height(30)))
            {
                // Применяем все изменения свойств, если они не применились автоматически
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(script);
                AssetDatabase.SaveAssets();

                // Принудительный вызов валидатора (чтобы префаб обновился сразу до компиляции, если нужно)
                // Замечание: Убедитесь, что метод UpdateAllPlatformPrefabs публичный в вашем PlatformPrefabValidator
                PlatformPrefabValidator.UpdateAllPlatformPrefabs();

                // Запрос перекомпиляции скриптов
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

                _isDirty = false;
                Debug.Log($"[{nameof(PlatformPrefabEditor)}] Изменения сохранены, запущен процесс перекомпиляции.");
            }

            GUI.backgroundColor = originalColor;

            // Если были изменены другие стандартные свойства (через ApplyModifiedProperties)
            if (serializedObject.ApplyModifiedProperties())
            {
                _isDirty = true;
            }
        }
    }
}