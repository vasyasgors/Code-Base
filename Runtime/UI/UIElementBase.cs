using UnityEngine;
using UnityEngine.Events;

namespace CodeBase.UI
{
    public abstract class UIElementBase : MonoBehaviour
    {
        public event UnityAction Showed;
        public event UnityAction Hided;
        public event UnityAction Closed;

   
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        protected virtual void OnClose() { }

      
       
        public void Show()
        {
            gameObject.SetActive(true);

            Showed?.Invoke();
            OnShow();
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            Hided?.Invoke();
            OnHide();
        }

        public void Close()
        {
            Closed?.Invoke();
            OnClose();
            Destroy(gameObject);
        }

    }
}


