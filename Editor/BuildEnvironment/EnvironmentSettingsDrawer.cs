using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [CustomPropertyDrawer(typeof(EnvironmentSettings))]
    public class EnvironmentSettingsDrawer : PropertyDrawer
    {
        private string[] _displayNames;
        private string[] _internalPaths;
        private bool _templatesLoaded = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LoadAvailableTemplates();
            EditorGUI.BeginProperty(position, label, property);

            // Получаем понятное имя для заголовка складной группы (Foldout)
            var labelProp = property.FindPropertyRelative("label");
            string headerText = string.IsNullOrEmpty(labelProp?.stringValue) ? label.text : labelProp.stringValue;

            // Отрисовываем стандартный Foldout для элемента массива
            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, headerText, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Создаем изолированную копию свойства, чтобы безопасно итерироваться по внутренним полям
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();

                float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                bool enterChildren = true;

                // Проходим по всем свойствам класса EnvironmentSettings
                while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    enterChildren = false; // Нам нужен только первый уровень вложенности

                    if (iterator.name == "WebGlTemplate")
                    {
                        // Вычисляем позицию для кастомного выпадающего списка
                        float height = EditorGUIUtility.singleLineHeight;
                        Rect rect = new Rect(position.x, currentY, position.width, height);

                        int currentIndex = Mathf.Max(0, System.Array.IndexOf(_internalPaths, iterator.stringValue));
                        int newIndex = EditorGUI.Popup(rect, "WebGL Template", currentIndex, _displayNames);

                        if (newIndex != currentIndex)
                        {
                            iterator.stringValue = _internalPaths[newIndex];
                        }

                        currentY += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                    else
                    {
                        // Все остальные поля рисуем стандартно, учитывая их высоту (например, для массивов папок)
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

        // Unity необходимо знать точную высоту элемента в пикселях, чтобы элементы массива не накладывались друг на друга
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

        // Поиск WebGL шаблонов через внутреннюю рефлексию Unity
        private void LoadAvailableTemplates()
        {
            if (_templatesLoaded) return;

            var type = typeof(PlayerSettings).Assembly.GetType("UnityEditor.WebGL.WebGlTemplateUtils");
            if (type != null)
            {
                var method = type.GetMethod("GetTemplates", BindingFlags.Public | BindingFlags.Static);
                if (method != null)
                {
                    var templates = (object[])method.Invoke(null, null);
                    _displayNames = new string[templates.Length];
                    _internalPaths = new string[templates.Length];

                    for (int i = 0; i < templates.Length; i++)
                    {
                        var t = templates[i];
                        string name = t.GetType().GetProperty("name")?.GetValue(t)?.ToString();
                        string path = t.GetType().GetProperty("path")?.GetValue(t)?.ToString();

                        _displayNames[i] = name ?? path;
                        _internalPaths[i] = path;
                    }
                    _templatesLoaded = true;
                    return;
                }
            }

            _displayNames = new[] { "Default", "Minimal" };
            _internalPaths = new[] { "APPLICATION:Default", "APPLICATION:Minimal" };
            _templatesLoaded = true;
        }
    }
}
