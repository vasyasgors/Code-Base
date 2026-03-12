
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{

    public class GameStateMachineTicker : MonoBehaviour, IService
    {
        private IGameStateSwither gameStateSwither;

        public void SetStateMachine(IGameStateSwither gameStateSwither)
        {
            this.gameStateSwither = gameStateSwither;
        }

        private void Update()
        {
            gameStateSwither.UpdateTick();
        }
    }
}
