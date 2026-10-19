using CodeBase.Infrastructure;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace CodeBase
{
    public class LocalGame : IGame
    {
        public bool IsMobile => true;

        public event UnityAction PageVisibilityOn;
        public event UnityAction PageVisibilityOff;

        public void GameplayReady()
        {

        }

        public void GameplayStart()
        {

        }

        public void GameplayStop()
        {

        }

        public long GetServerTimeMillisecond()
        {
            return 0;
        }

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public void UnlockAchievement(string id)
        {

        }
    }
}


