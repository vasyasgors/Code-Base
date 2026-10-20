using UnityEngine;
using UnityEditor;

namespace CodeBase.Infrastructure.Editor
{
    [CustomEditor(typeof(PlatformObject))]
    public class PlatformObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty _mappingsProp;
        private SerializedProperty _loadedPrefabProp;

        // Флаг наличия непримененных изменений для смены цвета кнопки
        private bool _isDirty = false;

        private void OnEnable()
        {
            _mappingsProp = serializedObject.FindProperty("platformMappings");

            // ИСПРАВЛЕНИЕ: В PlatformObject поле называется "gameObject", а не "prefab"
            _loadedPrefabProp = serializedObject.FindProperty("gameObject");
            _isDirty = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 1. Текущий загруженный префаб компиляции (только для чтения)
            if (_loadedPrefabProp != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(_loadedPrefabProp, new GUIContent("Active Runtime Prefab"), true);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space(10);
            }

            if (_mappingsProp == null)
            {
                EditorGUILayout.HelpBox("Поле 'platformMappings' не найдено.", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("Platform Prefab Mappings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // ИСПРАВЛЕНИЕ: Если массив пустой, принудительно раскрываем его, чтобы пользователь видел структуру
            if (_mappingsProp.arraySize == 0)
            {
                _mappingsProp.isExpanded = true;
            }

            // Отрисовка шапки списка платформ
            EditorGUILayout.PropertyField(_mappingsProp, new GUIContent("Platforms List"), false);

            if (_mappingsProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                // ИСПРАВЛЕНИЕ: Кнопка добавления теперь гарантированно видна, если в массиве нет элементов
                if (_mappingsProp.arraySize == 0)
                {
                    EditorGUILayout.Space(5);
                    if (GUILayout.Button("+ Add First Platform Mapping", GUILayout.Height(25)))
                    {
                        _mappingsProp.arraySize++;
                        _isDirty = true;
                    }
                    EditorGUILayout.Space(5);
                }

                for (int i = 0; i < _mappingsProp.arraySize; i++)
                {
                    SerializedProperty elementProp = _mappingsProp.GetArrayElementAtIndex(i);
                    SerializedProperty platformProp = elementProp.FindPropertyRelative("platform");
                    SerializedProperty pathProp = elementProp.FindPropertyRelative("prefabPath");

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();

                    // [Платформа]
                    if (platformProp != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(platformProp, GUIContent.none, GUILayout.Width(130));
                        if (EditorGUI.EndChangeCheck()) _isDirty = true;
                    }

                    // [Окно для перетаскивания префаба]
                    if (pathProp != null)
                    {
                        GameObject currentAsset = null;
                        if (!string.IsNullOrEmpty(pathProp.stringValue))
                        {
                            currentAsset = AssetDatabase.LoadAssetAtPath<GameObject>(pathProp.stringValue);
                        }

                        EditorGUI.BeginChangeCheck();
                        GameObject newAsset = (GameObject)EditorGUILayout.ObjectField(GUIContent.none, currentAsset, typeof(GameObject), false);

                        if (EditorGUI.EndChangeCheck())
                        {
                            pathProp.stringValue = newAsset != null ? AssetDatabase.GetAssetPath(newAsset) : string.Empty;
                            _isDirty = true;
                        }
                    }

                    // Кнопка удаления элемента
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        _mappingsProp.DeleteArrayElementAtIndex(i);
                        _isDirty = true;
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    // [Путь] под окном перетаскивания (только для чтения)
                    if (pathProp != null && !string.IsNullOrEmpty(pathProp.stringValue))
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField("Path", pathProp.stringValue);
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }

                if (_mappingsProp.arraySize > 0)
                {
                    if (GUILayout.Button("Add New Element", GUILayout.Width(150)))
                    {
                        _mappingsProp.arraySize++;
                        _isDirty = true;
                    }
                }

                EditorGUI.indentLevel--;
            }

            // Проверяем, изменились ли свойства через стандартный инспектор Unity
            if (serializedObject.ApplyModifiedProperties())
            {
                _isDirty = true;
            }

            EditorGUILayout.Space(15);

            // Кнопка перекомпиляции с динамическим цветом
            Color originalColor = GUI.backgroundColor;
            if (_isDirty)
            {
                GUI.backgroundColor = new Color(1f, 0.3f, 0.3f); // Спокойный красный цвет
            }

            if (GUILayout.Button("Apply and Recompile", GUILayout.Height(30)))
            {
                // Принудительно сохраняем все изменения свойств
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();

                // Вызываем валидатор (обновляем поле префаба прямо сейчас)
                PlatformObjectValidator.UpdateAllPlatformPrefabs();

                // Запрос полной перекомпиляции скриптов Unity
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

                _isDirty = false;
                Debug.Log($"[{nameof(PlatformObjectEditor)}] Изменения путей сохранены, запущен процесс валидации и перекомпиляции.");
            }

            GUI.backgroundColor = originalColor;
        }
    }
}
