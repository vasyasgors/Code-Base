using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{

    public class GlobalTicker : MonoBehaviour, IGlobalTicker
    {
        private List<ITickable> tickables = new List<ITickable>();
        private List<IFixedTickable> fixedTickables = new List<IFixedTickable>();

        private void Update()
        {
            for (int i = 0; i < tickables.Count; i++)
            {
                if (tickables[i] == null) continue;

                tickables[i]?.Tick();
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < fixedTickables.Count; i++)
            {
                if (fixedTickables[i] == null) continue;

                fixedTickables[i]?.FixedTick();
            }
        }

        public void AddTickable(ITickable tickable)
        {
            tickables.Add(tickable);
        }

        public void RemoveTickable(ITickable tickable)
        {
            tickables.Remove(tickable);
        }


        public void AddFixedTickable(IFixedTickable fixedTickable)
        {
            fixedTickables.Add(fixedTickable);
        }

        public void RemoveFixedTickable(IFixedTickable fixedTickable)
        {
            fixedTickables.Remove(fixedTickable);
        }
        public void ClearTickable()
        {
            tickables.Clear();
        }

        public void ClearFixedTickable()
        {
            fixedTickables.Clear();
        }

        public static GlobalTicker CreateInstance()
        {
            GameObject gameObject = new GameObject("Global Ticker");
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<GlobalTicker>();
        }


    }

}