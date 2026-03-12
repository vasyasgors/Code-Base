using UnityEngine;

namespace CodeBase.UI
{
    public abstract class PanelBase : MonoBehaviour
    {
        public void Close()
        {
            Destroy(gameObject);
        }
    }
}


