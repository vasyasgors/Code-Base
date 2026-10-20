using UnityEngine;
using UnityEditor;

namespace CodeBase.Infrastructure.Editor
{
    [CustomEditor(typeof(PlatformPrefab))]
    public class PlatformPrefabEditor : UnityEditor.Editor
    {
        private SerializedProperty _mappingsProp;

        private void OnEnable()
        {
            // Ищем приватное сериализованное поле по его названию в коде
            _mappingsProp = serializedObject.FindProperty("platformMappings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_mappingsProp == null)
            {
                EditorGUILayout.HelpBox("Поле 'platformMappings' не найдено в компоненте. Проверьте имя переменной.", MessageType.Error);
                return;
            }

            // Отображает строку заголовка массива (Platforms List) и стрелочку свернуть/развернуть
            EditorGUILayout.PropertyField(_mappingsProp, new GUIContent("Platforms List"), false);

            if (_mappingsProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Кнопка быстрого добавления элемента, если массив пустой
                if (_mappingsProp.arraySize == 0)
                {
                    if (GUILayout.Button("Add Platform Mapping"))
                    {
                        _mappingsProp.arraySize++;
                    }
                }

                for (int i = 0; i < _mappingsProp.arraySize; i++)
                {
                    SerializedProperty elementProp = _mappingsProp.GetArrayElementAtIndex(i);

                    // Строгое соответствие именам полей в классе PlatformMapping
                    SerializedProperty platformProp = elementProp.FindPropertyRelative("platform");
                    SerializedProperty pathProp = elementProp.FindPropertyRelative("prefabPath");
                    SerializedProperty prefabProp = elementProp.FindPropertyRelative("prefab");

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    // Строка 1: [Платформа] [Окно для префаба]
                    EditorGUILayout.BeginHorizontal();

                    if (platformProp != null)
                    {
                        EditorGUILayout.PropertyField(platformProp, GUIContent.none, GUILayout.Width(130));
                    }

                    if (prefabProp != null && pathProp != null)
                    {
                        // Восстанавливаем префаб по пути из ассетов
                        GameObject currentAsset = null;
                        if (!string.IsNullOrEmpty(pathProp.stringValue))
                        {
                            currentAsset = AssetDatabase.LoadAssetAtPath<GameObject>(pathProp.stringValue);
                        }

                        EditorGUI.BeginChangeCheck();
                        GameObject newAsset = (GameObject)EditorGUILayout.ObjectField(GUIContent.none, currentAsset, typeof(GameObject), false);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (newAsset != null)
                            {
                                string assetPath = AssetDatabase.GetAssetPath(newAsset);
                                pathProp.stringValue = assetPath;
                                prefabProp.objectReferenceValue = newAsset;
                            }
                            else
                            {
                                pathProp.stringValue = string.Empty;
                                prefabProp.objectReferenceValue = null;
                            }
                        }
                    }

                    // Маленькая кнопка "Х" для удаления конкретной строки
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        _mappingsProp.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    // Строка 2: Вывод пути (только для чтения)
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
                    }
                }

                EditorGUI.indentLevel--;
            }

            // Если были изменения — применяем их и сохраняем на диск
            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();

                // Вызываем валидатор для мгновенного обновления рантайм-ссылок
                PlatformPrefabValidator.UpdateAllPlatformPrefabs();
            }
        }
    }
}
