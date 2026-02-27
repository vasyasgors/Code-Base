using UnityEditor;
using UnityEngine;

namespace CodeBase.Editor
{
    public class PlayerPrefsCleaner 
    {
        [MenuItem("Tools/Clear Prefs")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
         
        }

        [MenuItem("Tools/Clear Cache")]
        public static void ClearCache()
        {
            Caching.ClearCache();
        }
    }
}
