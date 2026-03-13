using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CodeBase.UI
{


    public abstract class ScreenBase : UIElementBase
    {
        [SerializeField] private Text titileText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button hideButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Close);
            hideButton?.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            closeButton?.onClick.RemoveAllListeners();
            hideButton?.onClick.RemoveAllListeners();
        }

        public void SetTitle(string title)
        {
            if (titileText == null) return;

            titileText.text = title;
        }
     
    }
}


