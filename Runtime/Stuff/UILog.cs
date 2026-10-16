using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase
{

    public class UILog : MonoBehaviour
    {
        private const uint qsize = 20;
        private Queue<string> myLogQueue = new Queue<string>();

        [Header("Настройки текста")]
        [SerializeField] private int fontSize = 18;
        [SerializeField] private Color textColor = Color.white;

        [Header("Настройки фона")]
        [SerializeField] private bool showBackground = true;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);

        [Header("Настройки видимости")]
        [SerializeField] private bool isVisible = true;

        [Header("Позиция окна")]
        [SerializeField] private Rect windowRect = new Rect(30, 30, 0, 0);

        private GUIStyle logStyle;
        private Texture2D bgTexture;
        private Vector2 scrollPosition;
        private int windowId;

        private void Start()
        {
            windowId = GetHashCode();
            LoadSettings();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            if (bgTexture != null)
            {
                Destroy(bgTexture);
            }
            SaveSettings();
        }

        private void OnApplicationQuit()
        {
            SaveSettings();
        }

        private void OnGUI()
        {
            if (logStyle == null)
            {
                logStyle = new GUIStyle(GUI.skin.label);
            }
            logStyle.fontSize = fontSize;
            logStyle.normal.textColor = textColor;

            if (showBackground)
            {
                if (bgTexture == null)
                {
                    bgTexture = new Texture2D(1, 1);
                }
                bgTexture.SetPixel(0, 0, backgroundColor);
                bgTexture.Apply();
                logStyle.normal.background = bgTexture;
            }
            else
            {
                logStyle.normal.background = null;
            }

            float panelWidth = Screen.width * 0.7f;
            float maxHeight = Screen.height * 0.5f;
            float totalWidth = panelWidth + 120;

            windowRect.width = totalWidth;
            windowRect.height = isVisible ? maxHeight : 75;

            windowRect = GUI.Window(windowId, windowRect, DrawWindowContents, "", GUIStyle.none);

            windowRect.x = Mathf.Clamp(windowRect.x, 0, Screen.width - windowRect.width);
            windowRect.y = Mathf.Clamp(windowRect.y, 0, Screen.height - windowRect.height);
        }

        private void DrawWindowContents(int id)
        {
            float panelWidth = Screen.width * 0.7f;
            float maxHeight = Screen.height * 0.5f;
            float buttonsStartX = panelWidth + 10;

            GUI.DragWindow(new Rect(0, 0, panelWidth, isVisible ? maxHeight : 75));

            string buttonText = isVisible ? "Hide Logs" : "Show Logs";
            if (GUI.Button(new Rect(buttonsStartX, 0, 100, 30), buttonText))
            {
                isVisible = !isVisible;
                SaveSettings();
            }

            if (GUI.Button(new Rect(buttonsStartX, 35, 100, 30), "Clear"))
            {
                myLogQueue.Clear();
            }

            if (isVisible)
            {
                GUILayout.BeginArea(new Rect(0, 0, panelWidth, maxHeight));

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(panelWidth), GUILayout.Height(maxHeight));

                GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()), logStyle);

                GUILayout.EndScrollView();

                GUILayout.EndArea();
            }
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            myLogQueue.Enqueue("[" + type + "] : " + logString);
            if (type == LogType.Exception)
                myLogQueue.Enqueue(stackTrace);

            while (myLogQueue.Count > qsize)
                myLogQueue.Dequeue();

            scrollPosition.y = float.MaxValue;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("UILog_WindowX", windowRect.x);
            PlayerPrefs.SetFloat("UILog_WindowY", windowRect.y);
            PlayerPrefs.SetInt("UILog_IsVisible", isVisible ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey("UILog_WindowX"))
            {
                windowRect.x = PlayerPrefs.GetFloat("UILog_WindowX");
                windowRect.y = PlayerPrefs.GetFloat("UILog_WindowY");
                isVisible = PlayerPrefs.GetInt("UILog_IsVisible", 1) == 1;
            }
        }
    }

}