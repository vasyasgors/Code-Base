using UnityEngine;

namespace CodeBase.Infractructure
{
    public abstract class Context : MonoBehaviour
    {
        [SerializeField] protected MonoInstaller[] monoInstallers;

        protected void Install()
        {
            if (monoInstallers == null) return;

            for (int i = 0; i < monoInstallers.Length; i++)
            {
                monoInstallers[i]?.Install();
            }
        }

    }


}

