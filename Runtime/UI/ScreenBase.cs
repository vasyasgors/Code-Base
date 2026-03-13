using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CodeBase.UI
{


    public abstract class ScreenBase : UIElementBase
    {
        [SerializeField] private Text titileText;

        public void SetTitle(string title)
        {
            if (titileText == null) return;

            titileText.text = title;
        }
    }
}


