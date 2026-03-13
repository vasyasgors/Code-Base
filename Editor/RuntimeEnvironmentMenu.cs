using UnityEditor;

namespace CodeBase.Infrastructure
{
    public static class RuntimeEnvironmentMenu
    {
        private const string MENU_NAME = "Tools/Editor Runtime Environment";
        private const string DESKTOP_KEY = "RuntimeEnvironmentDesktop";
        private const string MOBILE_KEY = "RuntimeEnvironmentMobile";

        private static bool IsDesktopEnabled
        {
            get => EditorPrefs.GetBool(DESKTOP_KEY, true);
            set => EditorPrefs.SetBool(DESKTOP_KEY, value);
        }

        private static bool IsMobileEnabled
        {
            get => EditorPrefs.GetBool(MOBILE_KEY, false);
            set => EditorPrefs.SetBool(MOBILE_KEY, value);
        }

        [MenuItem(MENU_NAME + "/Desktop %d")]
        private static void ToggleDesktop()
        {
            IsDesktopEnabled = !IsDesktopEnabled;
            IsMobileEnabled = false; 
            UpdateRuntimeEnvironment();
        }

        [MenuItem(MENU_NAME + "/Mobile %m")]
        private static void ToggleMobile()
        {
            IsMobileEnabled = !IsMobileEnabled;
            IsDesktopEnabled = false; 
            UpdateRuntimeEnvironment();
        }

        [MenuItem(MENU_NAME + "/Desktop", true)]
        private static bool DesktopValidate() => IsDesktopEnabled;

        [MenuItem(MENU_NAME + "/Mobile", true)]
        private static bool MobileValidate() => IsMobileEnabled;

        private static void UpdateRuntimeEnvironment()
        {
            RuntimeEnvironmentType newType = IsDesktopEnabled ? RuntimeEnvironmentType.Standalone : RuntimeEnvironmentType.Mobile;
            RuntimeEnvironment.EditorRuntimeEnvironment = newType; 

            // Обновляем состояние в Project Settings
            AssetDatabase.SaveAssets();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}