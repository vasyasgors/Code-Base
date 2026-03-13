using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CodeBase.UI
{


    public abstract class ScreenBase : UIElementBase
    {
        [SerializeField] private Text titileText;
        [SerializeField] private Button CloseButton;

        private void Awake()
        {
            CloseButton?.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            CloseButton?.onClick.RemoveAllListeners();
        }

        public void SetTitle(string title)
        {
            if (titileText == null) return;

            titileText.text = title;
        }
     
    }
}


