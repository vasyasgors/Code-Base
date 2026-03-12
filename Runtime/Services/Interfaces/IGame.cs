
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace CodeBase.Infrastructure
{
    public interface IGame : IService
    {
        event UnityAction PageVisibilityOn;
        event UnityAction PageVisibilityOff;

        Task InitAsync();

        void GameplayReady();
        void GameplayStart();
        void GameplayStop();


        bool IsMobile { get; }
    }

}





