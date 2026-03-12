using CodeBase.Infrastructure;
using UnityEngine;


namespace CodeBase.UI
{
    public interface IUIFactory : IService
    {
        public Transform UIRoot { get; }

        TScreen CreateScreen<TScreen>() where TScreen : ScreenBase;
        TPanel CreatePanel<TPanel>() where TPanel : PanelBase;
        TPopup CreatePopup<TPopup>() where TPopup : PopupBase;
        TPrefab CreateElement<TPrefab>() where TPrefab : MonoBehaviour;
    }

}