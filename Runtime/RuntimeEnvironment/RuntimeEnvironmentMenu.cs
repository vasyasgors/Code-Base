
#if UNITY_EDITOR
using UnityEditor;

namespace CodeBase.Infrastructure
{
    public static class RuntimeEnvironmentMenu
    {
        private const string MENU_NAME = "Tools/Editor Runtime Environment";
        private const string DESKTOP_KEY = "RuntimeEnvironmentDesktop";
        private const string MOBILE_KEY = "RuntimeEnvironmentMobile";

        public static bool IsDesktopEnabled
        {
            get => EditorPrefs.GetBool(DESKTOP_KEY, true);
            set => EditorPrefs.SetBool(DESKTOP_KEY, value);
        }

        public static bool IsMobileEnabled
        {
            get => EditorPrefs.GetBool(MOBILE_KEY, false);
            set => EditorPrefs.SetBool(MOBILE_KEY, value);
        }

        [MenuItem(MENU_NAME + "/Desktop")]
        private static void ToggleDesktop()
        {
            IsDesktopEnabled = !IsDesktopEnabled;
            IsMobileEnabled = false;  
        }

        [MenuItem(MENU_NAME + "/Mobile")]
        private static void ToggleMobile()
        {
            IsMobileEnabled = !IsMobileEnabled;
            IsDesktopEnabled = false;
        }
   
        [MenuItem(MENU_NAME + "/Desktop", true)]  
        private static bool DesktopValidate() => !IsDesktopEnabled;

        [MenuItem(MENU_NAME + "/Mobile", true)]  
        private static bool MobileValidate() => !IsMobileEnabled;

    }
}

#endif