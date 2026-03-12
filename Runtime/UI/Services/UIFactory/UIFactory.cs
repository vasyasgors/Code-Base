using CodeBase.Infrastructure;
using System.Threading.Tasks;
using UnityEngine;


namespace CodeBase.UI
{
    public class UIFactory : IUIFactory
    {
        private const string UIRootGameObjectName = "UI Root";

        public Transform UIRoot { get; set; }

        private IUIPrefabProvider uIPrefabProvider;

        public UIFactory(IUIPrefabProvider uIPrefabProvider)
        {
            this.uIPrefabProvider = uIPrefabProvider;
        }

        public TScreen CreateScreen<TScreen>() where TScreen : ScreenBase
        {
            return InstantiateToUIRoot<TScreen>(uIPrefabProvider.GetScreenPrefab<TScreen>() as TScreen);
        }
        public TPanel CreatePanel<TPanel>() where TPanel : PanelBase
        {
            return InstantiateToUIRoot<TPanel>(uIPrefabProvider.GetPanelPrefab<TPanel>() as TPanel);
        }

        public TPrefab CreateElement<TPrefab>() where TPrefab : MonoBehaviour
        {
            return InstantiateToUIRoot<TPrefab>(uIPrefabProvider.GetPrefab<TPrefab>() as TPrefab);
        }

        private TUIElement InstantiateToUIRoot<TUIElement>(TUIElement prefab) where TUIElement : MonoBehaviour
        {
            if (UIRoot == null) UIRoot = new GameObject(UIRootGameObjectName).transform;

            TUIElement element = GameObject.Instantiate(prefab);
            element.transform.SetParent(UIRoot);
            return element;
        }

        public TPopup CreatePopup<TPopup>() where TPopup : PopupBase
        {
            return InstantiateToUIRoot<TPopup>(uIPrefabProvider.GetPupupPrefab<TPopup>() as TPopup);

        }
    }


}