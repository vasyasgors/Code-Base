using System;
using UnityEngine;

namespace CodeBase.UI
{
    public class UIPrefabProvider : MonoBehaviour, IUIPrefabProvider
    {
        [Header("UI Windows (Must be unique)")]
        [SerializeField] private ScreenBase[] screenPrefabs;

        [Header("UI Panel (Must be unique)")]
        [SerializeField] private PanelBase[] panelsPrefabs;

        [Header("UI popup (Must be unique)")]
        [SerializeField] private PopupBase[] popupPrefabs;

        [Header("UI Elements (Must be unique)")]
        [SerializeField] private GameObject[] uiElementsPrefabs;


        public TScreen GetScreenPrefab<TScreen>() where TScreen : ScreenBase
        {
            for (int i = 0; i < screenPrefabs.Length; i++)
            {
                if (screenPrefabs[i] is TScreen) return screenPrefabs[i] as TScreen;
            }

            throw new NullReferenceException($"Type { typeof(TScreen) } not found in { screenPrefabs } prefabs list!");
        }


        public TPanel GetPanelPrefab<TPanel>() where TPanel : PanelBase
        {
            for (int i = 0; i < panelsPrefabs.Length; i++)
            {
                if (panelsPrefabs[i] is TPanel) return panelsPrefabs[i] as TPanel;
            }

            throw new NullReferenceException($"Type { typeof(TPanel) } not found in { panelsPrefabs } prefabs list!");
        }

        public TUIPrefab GetPrefab<TUIPrefab>()
        {
            return FindPrefabInArray<TUIPrefab>(uiElementsPrefabs);
        }


        private TPrefab FindPrefabInArray<TPrefab>(GameObject[] prefabArray)
        {
            for (int i = 0; i < prefabArray.Length; i++)
            {
                TPrefab prefab = prefabArray[i].GetComponent<TPrefab>();

                if (prefab != null)
                    return prefab;
            }

            throw new NullReferenceException($"Type { typeof(TPrefab) } not found in { prefabArray } prefabs list!");
        }

        public TPopup GetPupupPrefab<TPopup>() where TPopup : PopupBase
        {
            for (int i = 0; i < popupPrefabs.Length; i++)
            {
                if (popupPrefabs[i] is TPopup) return popupPrefabs[i] as TPopup;
            }

            throw new NullReferenceException($"Type { typeof(TPopup) } not found in { popupPrefabs } prefabs list!");
        }
    }
}