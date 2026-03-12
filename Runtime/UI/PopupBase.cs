using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public abstract class PopupBase : MonoBehaviour
    {
        [SerializeField] protected Text text;

        public abstract void Show(string message, float showedTime);
        public abstract void Show(string message);
    }
}