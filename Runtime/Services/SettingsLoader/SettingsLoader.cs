using UnityEngine;

namespace CodeBase.Infrastructure
{
    public sealed class SettingsLoader : MonoBehaviour, ISettingsLoader
    {
        [SerializeField] private SettingHandler[] Settings;

        private void Start()
        {
            for (int i = 0; i < Settings.Length; i++)
            {
                Settings[i].Load();
                Settings[i].Apply();
            }
        }
    }
}