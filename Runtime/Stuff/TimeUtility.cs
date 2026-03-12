using System;
using UnityEngine;
using System.Linq;
using System.IO;


namespace CodeBase
{
    public static class TimeUtility
    {
        public static string GetFormattedTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000) / 100;
            return string.Format("{0:00}:{1:00}.{2:0}", minutes, seconds, milliseconds);
        }
    }
}

