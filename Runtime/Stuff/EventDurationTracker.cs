using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventDurationTracker
{
    private static readonly Dictionary<string, DateTime> startTimes = new Dictionary<string, DateTime>();

    public static void Start(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("[EventTimerService] Start: id не может быть пустым.");
            return;
        }

        startTimes[id] = DateTime.UtcNow;
    }

    public static int GetElapsedSeconds(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("[EventTimerService] GetElapsedSeconds: id не может быть пустым.");
            return -1;
        }

        if (!startTimes.TryGetValue(id, out DateTime startTime))
        {
            Debug.LogWarning($"[EventTimerService] GetElapsedSeconds: таймер с id '{id}' не был запущен.");
            return -1;
        }

        int elapsedSeconds = (int)(DateTime.UtcNow - startTime).TotalSeconds;

        startTimes.Remove(id);

        return elapsedSeconds;
    }

    public static bool IsRunning(string id)
    {
        return !string.IsNullOrEmpty(id) && startTimes.ContainsKey(id);
    }

    public static void Cancel(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            startTimes.Remove(id);
        }
    }
}