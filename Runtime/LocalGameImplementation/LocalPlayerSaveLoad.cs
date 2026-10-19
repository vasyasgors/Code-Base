using UnityEngine;
using CodeBase.Infrastructure;
using System.Threading.Tasks;
using System;

namespace CodeBase
{

    public class LocalPlayerSaveLoad : IPlayerProgressSaveLoad, IAfterSaveEvent
    {
        private const string ProgressKey = "Progress";

        private IPlayerProgressProviderBase playerProgressProvider;

        public event Action AfterSave;

        public bool IsFirstLoad => playerProgressProvider.SaveVersion == 0;

        public LocalPlayerSaveLoad(IPlayerProgressProviderBase playerProgressProvider)
        {
            this.playerProgressProvider = playerProgressProvider;
        }

        public void Save()
        {
            playerProgressProvider.IncrementSaveVerstion();

            PlayerPrefs.SetString(ProgressKey, playerProgressProvider.GetProgressJSON());

            AfterSave?.Invoke();
        }

        public void Load()
        {
            string progress = PlayerPrefs.GetString(ProgressKey, "{}");

            if (progress == "{}")
            {
                playerProgressProvider.SetDefaultProgress();
            }
            else
            {
                playerProgressProvider.SetProgressJSON(progress);
            }
        }


        public async Task SaveAsync()
        {
            Save();
            await Task.Yield();

            AfterSave?.Invoke();

        }

        public async Task LoadAsync()
        {
            Load();
            await Task.Yield();
        }



    }
}


