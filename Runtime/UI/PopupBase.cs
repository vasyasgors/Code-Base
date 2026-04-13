using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public abstract class PopupBase : MonoBehaviour
    {
        [SerializeField] protected Text text;

        public abstract PopupBase Show(string message, float showedTime);
        public abstract PopupBase Show(string message);
    }
}