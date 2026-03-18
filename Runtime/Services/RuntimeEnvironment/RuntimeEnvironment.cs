using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{

    public static class RuntimeEnvironment
    {
#if UNITY_EDITOR
        private static RuntimeEnvironmentType EditorRuntimeEnvironment 
        {
            get
            {
                if (RuntimeEnvironmentMenu.IsDesktopEnabled == true) return RuntimeEnvironmentType.Standalone;
                if (RuntimeEnvironmentMenu.IsMobileEnabled == true) return RuntimeEnvironmentType.Mobile;

                return RuntimeEnvironmentType.Unknown;
            }
        }


#endif

        public static RuntimeEnvironmentType CurrentRuntimeEnvironment
        {
            get
            {
                return GetCurrentRuntimeEnvironment();
            }
        }

        private static RuntimeEnvironmentType GetCurrentRuntimeEnvironment()
        {
#if UNITY_EDITOR
            if (Application.platform == RuntimePlatform.WindowsEditor) return EditorRuntimeEnvironment;
#endif

            if (Application.platform == RuntimePlatform.IPhonePlayer) return RuntimeEnvironmentType.Mobile;

            if (Application.platform == RuntimePlatform.Android) return RuntimeEnvironmentType.Mobile;

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                IGame game = ServiceContainer.Single<IGame>();

                if (game == null) return RuntimeEnvironmentType.Standalone;

                if (game != null)
                {
                    if (game.IsMobile == true) return RuntimeEnvironmentType.Mobile;

                    if (game.IsMobile == false) return RuntimeEnvironmentType.Standalone;
                }
            }

            return RuntimeEnvironmentType.Unknown;
        }
    }
}