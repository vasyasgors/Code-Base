using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public abstract class ScreenBase : MonoBehaviour
    {
        public event UnityAction Closed;

        [SerializeField] private Text titileText;

        protected virtual void OnClose() { }

        public void SetTitle(string title)
        {
            if (titileText == null) return;

            titileText.text = title;
        }

        public void Close()
        {
            Closed?.Invoke();
            OnClose();
            Destroy(gameObject);
        }


    }
}


