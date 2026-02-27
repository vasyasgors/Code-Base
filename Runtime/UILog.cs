using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILog : MonoBehaviour
{
    private const uint qsize = 20;  

    private Queue myLogQueue = new Queue();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(30, 0, Screen.width * 0.7f, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

   
}
