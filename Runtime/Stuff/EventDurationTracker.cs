using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase
{
    public static class EventDurationTracker<TKey> where TKey : Enum
    {
        private static readonly Dictionary<TKey, DateTime> startTimes = new Dictionary<TKey, DateTime>();

        public static void Start(TKey id)
        {
            startTimes[id] = DateTime.UtcNow;
        }

        public static int GetElapsedSeconds(TKey id, bool clearTime = true)
        {
            if (!startTimes.TryGetValue(id, out DateTime startTime))
            {
                Debug.LogWarning($"[EventTimerRegistry] Timer '{id}' was not started.");
                return -1;
            }

            if (clearTime == true)
                startTimes.Remove(id);

            return (int)(DateTime.UtcNow - startTime).TotalSeconds;
        }

        public static bool IsRunning(TKey id)
        {
            return startTimes.ContainsKey(id);
        }

        public static void Cancel(TKey id)
        {
            startTimes.Remove(id);
        }
    }
}