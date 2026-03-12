
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class LevelStateMachineTicker : MonoBehaviour, IService
    {
        private ILevelStateSwitcher levelStateSwitcher;


        public void SetStateMachine(ILevelStateSwitcher levelStateSwitcher)
        {
            this.levelStateSwitcher = levelStateSwitcher;
        }

        private void Update()
        {
            levelStateSwitcher.UpdateTick();
        }
    }

}