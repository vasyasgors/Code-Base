using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    public class PackageManagerManagerWindow : EditorWindow
    {
        private PackageManagerPresets _presets;
        private Vector2 _scrollPosition;

        private HashSet<string> _targetGitUrls = new HashSet<string>();
        private HashSet<string> _excludedGitUrls = new HashSet<string>();

        // Переменная для хранения состояния галочки обновления
        private bool _updateAlreadyInstalled = false;

        private HashSet<int> _selectedCollectionIndices = new HashSet<int>();


        [MenuItem("Tools/Package Collections Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageManagerManagerWindow>("Package Manager Presets");
            window.minSize = new Vector2(450, 500);
        }

        private void OnEnable()
        {
            LoadOrCreateConfig();
        }

        private void LoadOrCreateConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(PackageManagerPresets));

            if (guids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _presets = AssetDatabase.LoadAssetAtPath<PackageManagerPresets>(assetPath);
                return;
            }
            Debug.Log($"[{nameof(PackageManagerManagerWindow)}] Пресет не найден.");
        }

        private void OnGUI()
        {
            if (_presets == null) LoadOrCreateConfig();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Коллекции пакетов", EditorStyles.boldLabel);

            string currentPath = AssetDatabase.GetAssetPath(_presets);
            EditorGUILayout.LabelField($"Файл конфигурации: {currentPath}", EditorStyles.miniLabel);

            if (GUILayout.Button("Фокус на файл конфигурации ассета", GUILayout.Width(250)))
            {
                EditorGUIUtility.PingObject(_presets);
            }

            EditorGUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            SerializedObject serializedPresets = new SerializedObject(_presets);
            SerializedProperty collectionsProp = serializedPresets.FindProperty("collections");

            for (int i = 0; i < _presets.collections.Count; i++)
            {
                var collection = _presets.collections[i];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();


                // --- ВСТАВЬТЕ ЭТОТ КОД: ---
                SerializedProperty elementProp = collectionsProp.GetArrayElementAtIndex(i);
                // Считываем состояние галочки из временной коллекции в оперативной памяти окна
                bool isSelected = _selectedCollectionIndices.Contains(i);
                bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

                // Если пользователь нажал на галочку, обновляем данные только в памяти окна
                if (newSelected != isSelected)
                {
                    if (newSelected) _selectedCollectionIndices.Add(i);
                    else _selectedCollectionIndices.Remove(i);
                }

                // ------------------------------------------------------------------------


                EditorGUILayout.PropertyField(elementProp, new GUIContent(collection.collectionName ?? "Новая коллекция"), true);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    collectionsProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            if (GUILayout.Button("+ Добавить новую коллекцию", GUILayout.Height(25)))
            {
                int newIndex = collectionsProp.arraySize;
                collectionsProp.InsertArrayElementAtIndex(newIndex);
                var newElement = collectionsProp.GetArrayElementAtIndex(newIndex);
                newElement.FindPropertyRelative("collectionName").stringValue = "Новая коллекция";
                newElement.FindPropertyRelative("gitUrls").ClearArray();
                newElement.FindPropertyRelative("isSelected").boolValue = false;
            }

            EditorGUILayout.Space(15);

            // --- ДОБАВЛЕНА ГАЛОЧКА ПЕРЕД КНОПКОЙ ---
            _updateAlreadyInstalled = EditorGUILayout.ToggleLeft("Обновить уже установленные пакеты (перепроверить ссылки)", _updateAlreadyInstalled);
            EditorGUILayout.Space(5);
            // ----------------------------------------

            GUI.backgroundColor = new Color(0.3f, 1f, 0.4f);
            if (GUILayout.Button("Обновить и Загрузить (Синхронизировать)", GUILayout.Height(35)))
            {
                ExecuteSynchronization();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndScrollView();

            if (serializedPresets.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_presets);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.Space(10);
        }

        private void ExecuteSynchronization()
        {
            if (EditorApplication.isCompiling)
            {
                Debug.LogWarning("[Package Sync] Редактор занят компиляцией. Пожалуйста, подождите.");
                return;
            }

            EditorUtility.DisplayProgressBar("Синхронизация пакетов", "Получение списка установленных пакетов...", 0.1f);

            _targetGitUrls.Clear();
            _excludedGitUrls.Clear();

            // --- ИЗМЕНЕНИЕ ТУТ: Сортируем ссылки на основе нашего runtime-списка индексов ---
            for (int i = 0; i < _presets.collections.Count; i++)
            {
                var col = _presets.collections[i];
                bool isSelected = _selectedCollectionIndices.Contains(i);

                if (isSelected)
                {
                    foreach (var url in col.gitUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                        _targetGitUrls.Add(url.Trim());
                }
                else
                {
                    foreach (var url in col.gitUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                        _excludedGitUrls.Add(url.Trim());
                }
            }

            var listRequest = Client.List(true, false);
            while (!listRequest.IsCompleted) { }

            if (listRequest.Status != StatusCode.Success)
            {
                Debug.LogError($"[Package Sync] Ошибка получения списка: {listRequest.Error.message}");
                EditorUtility.ClearProgressBar();
                return;
            }

            List<string> toRemove = new List<string>();
            List<string> toAdd = new List<string>();

            foreach (var p in listRequest.Result)
            {
                string gitUrlFromPackage = string.Empty;
                if (p.source == PackageSource.Git)
                {
                    if (p.repository != null && !string.IsNullOrEmpty(p.repository.url))
                        gitUrlFromPackage = p.repository.url;
                    else if (!string.IsNullOrEmpty(p.packageId) && p.packageId.Contains("@"))
                        gitUrlFromPackage = p.packageId.Split('@')[1];
                }

                bool isInExcludedList = _excludedGitUrls.Contains(p.name) ||
                                       (!string.IsNullOrEmpty(gitUrlFromPackage) && _excludedGitUrls.Any(url => gitUrlFromPackage.Contains(url)));

                bool isInTargetList = _targetGitUrls.Contains(p.name) ||
                                     (!string.IsNullOrEmpty(gitUrlFromPackage) && _targetGitUrls.Any(url => gitUrlFromPackage.Contains(url)));

                if (isInExcludedList && !isInTargetList)
                    toRemove.Add(p.name);
            }

            foreach (var targetUrl in _targetGitUrls)
            {
                // Проверяем, установлен ли уже пакет
                bool isInstalled = listRequest.Result.Any(p =>
                {
                    if (p.name == targetUrl) return true;
                    if (p.source == PackageSource.Git)
                    {
                        string gitUrl = (p.repository != null) ? p.repository.url : "";
                        if (string.IsNullOrEmpty(gitUrl) && p.packageId.Contains("@"))
                            gitUrl = p.packageId.Split('@')[1];
                        return !string.IsNullOrEmpty(gitUrl) && gitUrl.Contains(targetUrl);
                    }
                    return false;
                });

                // --- ИСПРАВЛЕНИЕ ЛОГИКИ ОБНОВЛЕНИЯ ---
                // Если пакет не установлен ИЛИ если включена галочка "Обновить уже установленные"
                if (!isInstalled || _updateAlreadyInstalled)
                {
                    toAdd.Add(targetUrl);
                }
                // -------------------------------------
            }

            int totalOperations = toRemove.Count + toAdd.Count;
            int currentOperation = 0;

            if (totalOperations == 0)
            {
                Debug.Log("[Package Sync] Все пакеты уже соответствуют выбранным коллекциям.");
                EditorUtility.ClearProgressBar();
                return;
            }

            // 1. Удаление лишних
            foreach (var packId in toRemove)
            {
                currentOperation++;
                float progress = (float)currentOperation / totalOperations;
                EditorUtility.DisplayProgressBar("Синхронизация пакетов", $"Удаление пакета: {packId}...", progress);

                var removeRequest = Client.Remove(packId);
                while (!removeRequest.IsCompleted) { }

                if (removeRequest.Status == StatusCode.Success)
                    Debug.Log($"[Package Sync] Успешно удален: {packId}");
                else
                    Debug.LogError($"[Package Sync] Ошибка удаления {packId}: {removeRequest.Error.message}");
            }

            // 2. Установка новых / Обновление существующих
            foreach (var packUrl in toAdd)
            {
                currentOperation++;
                float progress = (float)currentOperation / totalOperations;
                EditorUtility.DisplayProgressBar("Синхронизация пакетов", $"Обработка пакета: {packUrl}...", progress);

                var addRequest = Client.Add(packUrl);
                while (!addRequest.IsCompleted) { }
                if (addRequest.Status == StatusCode.Success)
                    Debug.Log($"[Package Sync] Успешно обработан/обновлен: {packUrl}");
                else
                    Debug.LogError($"[Package Sync] Ошибка установки/обновления {packUrl}: {addRequest.Error.message}");
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("[Package Sync] Синхронизация коллекций пакетов полностью завершена!");
        }
    }
}