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

        // Папка по умолчанию, ЕСЛИ ассет вообще не найден ни в одной папке (включая Packages)
        //private const string FallbackConfigPath = "Assets/DeveloperPresets/PackageManagerPresets.asset";
        private PackageManagerPresets _presets;
        private Vector2 _scrollPosition;

        private HashSet<string> _targetGitUrls = new HashSet<string>();
        private HashSet<string> _excludedGitUrls = new HashSet<string>();

        // Очереди для пакетного менеджера
        private Queue<string> _packagesToAdd = new Queue<string>();
        private Queue<string> _packagesToRemove = new Queue<string>();
        private Request _currentRequest;
        private string _currentActionName;

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
            // 1. Ищем ассет по типу по всему проекту, включая папку Packages
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(PackageManagerPresets));

            if (guids.Length > 0)
            {
                // Если нашли хотя бы один, загружаем его по первому GUID
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _presets = AssetDatabase.LoadAssetAtPath<PackageManagerPresets>(assetPath);
                return;
            }

            // 2. Если ассет нигде не найден (ни в Assets, ни в Packages), создаем дефолтный в Assets
            //_presets = CreateInstance<PackageManagerPresets>();
            //string directory = Path.GetDirectoryName(FallbackConfigPath);
            //if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

           // AssetDatabase.CreateAsset(_presets, FallbackConfigPath);
           // AssetDatabase.SaveAssets();
            Debug.Log($"[{nameof(PackageManagerManagerWindow)}] Пресет не найден:");
        }

        private void OnGUI()
        {
            if (_presets == null) LoadOrCreateConfig();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Коллекции пакетов", EditorStyles.boldLabel);

            // Показываем текущий путь, где лежит файл пресетов (теперь он может быть в Packages/)
            string currentPath = AssetDatabase.GetAssetPath(_presets);
            EditorGUILayout.LabelField($"Файл конфигурации: {currentPath}", EditorStyles.miniLabel);

            if (GUILayout.Button("Фокус на файл конфигурации ассета", GUILayout.Width(250)))
            {
                EditorGUIUtility.PingObject(_presets);
            }

            EditorGUILayout.Space(10);

            // Отображение прогресса выполнения пакетных операций
            if (_currentRequest != null)
            {
                EditorGUILayout.HelpBox($"Выполняется: {_currentActionName}...", MessageType.Info);
                Repaint();
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Отрисовка коллекций
            SerializedObject serializedPresets = new SerializedObject(_presets);
            SerializedProperty collectionsProp = serializedPresets.FindProperty("collections");

            for (int i = 0; i < _presets.collections.Count; i++)
            {
                var collection = _presets.collections[i];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                // --- ИСПРАВЛЕНИЕ ТУТ ---
                // Получаем доступ к свойству isSelected конкретного элемента в массиве через SerializedProperty
                SerializedProperty elementProp = collectionsProp.GetArrayElementAtIndex(i);
                SerializedProperty isSelectedProp = elementProp.FindPropertyRelative("isSelected");

                // Отрисовываем чекбокс, привязанный к системе сериализации Unity
                isSelectedProp.boolValue = EditorGUILayout.Toggle(isSelectedProp.boolValue, GUILayout.Width(20));
                // -----------------------

                EditorGUILayout.PropertyField(elementProp, new GUIContent(collection.collectionName ?? "Новая коллекция"), true);

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    // Для удаления элемента из SerializedProperty лучше использовать методы самого свойства, 
                    // чтобы Unity корректно зафиксировала изменение структуры данных
                    collectionsProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            if (GUILayout.Button("+ Добавить новую коллекцию", GUILayout.Height(25)))
            {
                // Добавляем элемент также через SerializedProperty, чтобы не сбивать индекс
                int newIndex = collectionsProp.arraySize;
                collectionsProp.InsertArrayElementAtIndex(newIndex);
                var newElement = collectionsProp.GetArrayElementAtIndex(newIndex);
                newElement.FindPropertyRelative("collectionName").stringValue = "Новая коллекция";
                newElement.FindPropertyRelative("gitUrls").ClearArray();
                newElement.FindPropertyRelative("isSelected").boolValue = false;
            }

            EditorGUILayout.EndScrollView();

            // Применяем изменения. Если что-то поменялось (включая галочку), Unity сама вызовет SetDirty и сохранит файл.
            if (serializedPresets.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(_presets);
                AssetDatabase.SaveAssets();
            }
        }

        private void StartSynchronization()
        {
            // Собираем все ссылки из ВЫБРАННЫХ коллекций
            HashSet<string> targetGitUrls = new HashSet<string>();
            foreach (var col in _presets.collections.Where(c => c.isSelected))
            {
                foreach (var url in col.gitUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    targetGitUrls.Add(url.Trim());
                }
            }

        

            // Собираем все ссылки из НЕВЫБРАННЫХ коллекций
            HashSet<string> excludedGitUrls = new HashSet<string>();
            foreach (var col in _presets.collections.Where(c => !c.isSelected))
            {
                foreach (var url in col.gitUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    excludedGitUrls.Add(url.Trim());
                }
            }

            Debug.Log($"[Package Sync] Запрос списка установленных пакетов...");

            _packagesToAdd.Clear();
            _packagesToRemove.Clear();

            // Инициализируем запрос списка асинхронно
            _currentActionName = "Получение списка установленных пакетов";
            _currentRequest = Client.List(true, false);

            // Подписываемся на апдейт, обработка начнется асинхронно в ProgressQueueProcessing
            EditorApplication.update -= ProgressQueueProcessing; // Защита от дублирования
            EditorApplication.update += ProgressQueueProcessing;
        }

        private void ProgressQueueProcessing()
        {
            if (_currentRequest != null)
            {
                if (!_currentRequest.IsCompleted) return; // Ждем завершения текущей операции

                if (_currentRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"[Package Sync] Успешно выполнено: {_currentActionName}");

                    // --- ВСТАВЛЯТЬ СЮДА ---
                    // Если завершился запрос списка пакетов, обрабатываем его и наполняем очереди
                    if (_currentRequest is ListRequest listRequest)
                    {
                        ProcessListResult(listRequest.Result, _targetGitUrls, _excludedGitUrls);
                    }
                    // ----------------------
                }
                else
                {
                    Debug.LogError($"[Package Sync] Ошибка при {_currentActionName}: {_currentRequest.Error.message}");

                    // Если ошибка произошла на этапе получения списка, прерываем всю синхронизацию
                    if (_currentActionName.Contains("получение списка") || _currentRequest is ListRequest)
                    {
                        _currentRequest = null;
                        EditorApplication.update -= ProgressQueueProcessing;
                        return;
                    }
                }

                _currentRequest = null;
            }

            // Сначала удаляем всё лишнее
            if (_packagesToRemove.Count > 0)
            {
                string packId = _packagesToRemove.Dequeue();
                _currentActionName = $"Удаление {packId}";
                _currentRequest = Client.Remove(packId);
                return;
            }

            // Затем накатываем то, чего не хватает
            if (_packagesToAdd.Count > 0)
            {
                string packUrl = _packagesToAdd.Dequeue();
                _currentActionName = $"Установка {packUrl}";
                _currentRequest = Client.Add(packUrl);
                return;
            }

            // Очереди пусты — отписываемся от тиков редактора
            EditorApplication.update -= ProgressQueueProcessing;
            Debug.Log("[Package Sync] Синхронизация коллекций пакетов полностью завершена!");
        }

        // Указываем явный полный тип UnityEditor.PackageManager.PackageCollection, чтобы избежать конфликта с вашим классом
        private void ProcessListResult(UnityEditor.PackageManager.PackageCollection installedPackages, HashSet<string> targetGitUrls, HashSet<string> excludedGitUrls)
        {
            // Проверяем, какие из ИСКЛЮЧЕННЫХ пакетов сейчас установлены (чтобы удалить их)
            foreach (var p in installedPackages)
            {
                string gitUrlFromPackage = string.Empty;
                if (p.source == PackageSource.Git)
                {
                    if (p.repository != null && !string.IsNullOrEmpty(p.repository.url))
                    {
                        gitUrlFromPackage = p.repository.url;
                    }
                    else if (!string.IsNullOrEmpty(p.packageId) && p.packageId.Contains("@"))
                    {
                        gitUrlFromPackage = p.packageId.Split('@')[1]; // Восстановлен правильный индекс среза [1]
                    }
                }

                bool isInExcludedList = excludedGitUrls.Contains(p.name) ||
                                       (!string.IsNullOrEmpty(gitUrlFromPackage) && excludedGitUrls.Any(url => gitUrlFromPackage.Contains(url)));

                bool isInTargetList = targetGitUrls.Contains(p.name) ||
                                     (!string.IsNullOrEmpty(gitUrlFromPackage) && targetGitUrls.Any(url => gitUrlFromPackage.Contains(url)));

                if (isInExcludedList && !isInTargetList)
                {
                    _packagesToRemove.Enqueue(p.name);
                }
            }

            // Проверяем, каких целевых пакетов не хватает
            foreach (var targetUrl in targetGitUrls)
            {
                bool isInstalled = installedPackages.Any(p =>
                {
                    if (p.name == targetUrl) return true;

                    if (p.source == PackageSource.Git)
                    {
                        string gitUrl = (p.repository != null) ? p.repository.url : "";
                        if (string.IsNullOrEmpty(gitUrl) && p.packageId.Contains("@"))
                        {
                            gitUrl = p.packageId.Split('@')[1]; // Восстановлен индекс [1]
                        }
                        return !string.IsNullOrEmpty(gitUrl) && gitUrl.Contains(targetUrl);
                    }
                    return false;
                });

                if (!isInstalled)
                {
                    _packagesToAdd.Enqueue(targetUrl);
                }
            }
        }
    }
}