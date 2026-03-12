using CodeBase.Infrastructure;

namespace CodeBase.UI
{
    public interface IUIPrefabProvider : IService
    {
        public TUIPrefab GetPrefab<TUIPrefab>();
        public TWindow GetScreenPrefab<TWindow>() where TWindow : ScreenBase;
        public TPanel GetPanelPrefab<TPanel>() where TPanel : PanelBase;
        public TPopup GetPupupPrefab<TPopup>() where TPopup : PopupBase;
    }
}
