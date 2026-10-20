using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using CodeBase.Infrastructure;


namespace CodeBase.Infrastructure
{
    [CustomEditor(typeof(PlatformPrefab))]
    public class PlatformPrefabEditor : UnityEditor.Editor
    {
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
            }

            // 2. Путь под полем выбора
            EditorGUILayout.LabelField("Prefab Path", script.PrefabPath);

            // 3. Массив платформ
            SerializedProperty platformsProp = serializedObject.FindProperty("platforms");
            if (platformsProp != null)
            {
                EditorGUILayout.PropertyField(platformsProp, new GUIContent("Platforms"), true);
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


            serializedObject.ApplyModifiedProperties();
        }
    }
}