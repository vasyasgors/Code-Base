using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace CodeBase.Infrastructure.Editor
{
    [CustomPropertyDrawer(typeof(DistributionBuildProfile))]
    public class DistributionBuildSettingsDrawer : PropertyDrawer
    {
        private string[] _displayNames;
        private string[] _internalPaths;
        private bool _templatesLoaded = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LoadAvailableTemplates();
            EditorGUI.BeginProperty(position, label, property);

            var labelProp = property.FindPropertyRelative("label");
            string headerText = string.IsNullOrEmpty(labelProp?.stringValue) ? label.text : labelProp.stringValue;

            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, headerText, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();

                float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                bool enterChildren = true;

                while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    enterChildren = false;

                    if (iterator.name == "WebGlTemplate")
                    {
                        float height = EditorGUIUtility.singleLineHeight;
                        Rect rect = new Rect(position.x, currentY, position.width, height);

                        int currentIndex = Mathf.Max(0, System.Array.IndexOf(_internalPaths, iterator.stringValue));

                        // Если сохраненного пути нет в массиве (например, проект только открыли), ставим Default
                        if (currentIndex == 0 && iterator.stringValue != _internalPaths[0])
                        {
                            currentIndex = System.Array.IndexOf(_internalPaths, "APPLICATION:Default");
                            if (currentIndex < 0) currentIndex = 0;
                        }

                        int newIndex = EditorGUI.Popup(rect, "WebGL Template", currentIndex, _displayNames);

                        if (newIndex != currentIndex)
                        {
                            iterator.stringValue = _internalPaths[newIndex];
                        }

                        currentY += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                    else
                    {
                        float height = EditorGUI.GetPropertyHeight(iterator, true);
                        Rect rect = new Rect(position.x, currentY, position.width, height);

                        EditorGUI.PropertyField(rect, iterator, true);

                        currentY += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            float totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;

                if (iterator.name == "WebGlTemplate")
                {
                    totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                else
                {
                    totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return totalHeight;
        }

        // Прямой поиск шаблонов на диске и в ассетах
        private void LoadAvailableTemplates()
        {
            if (_templatesLoaded) return;

            List<string> displays = new List<string>();
            List<string> paths = new List<string>();

            // 1. Добавляем встроенные шаблоны Unity по умолчанию
            displays.Add("Default");
            paths.Add("APPLICATION:Default");

            displays.Add("Minimal");
            paths.Add("APPLICATION:Minimal");

            // 2. Ищем кастомные шаблоны в папке Assets/WebGLTemplates
            string customTemplatesPath = Path.Combine(Application.dataPath, "WebGLTemplates");

            if (Directory.Exists(customTemplatesPath))
            {
                // Получаем все подпапки (каждая подпапка — это отдельный шаблон)
                string[] templateDirectories = Directory.GetDirectories(customTemplatesPath);

                foreach (string dir in templateDirectories)
                {
                    string folderName = Path.GetFileName(dir);

                    // Проверяем, что это действительно шаблон (внутри должен быть index.html)
                    if (File.Exists(Path.Combine(dir, "index.html")))
                    {
                        displays.Add($"{folderName} (Custom)");
                        paths.Add($"PROJECT:{folderName}"); // Префикс для кастомных шаблонов проекта
                    }
                }
            }

            _displayNames = displays.ToArray();
            _internalPaths = paths.ToArray();
            _templatesLoaded = false; // Позволяет обновлять список при открытии/закрытии элементов, если добавились папки
        }
    }
}