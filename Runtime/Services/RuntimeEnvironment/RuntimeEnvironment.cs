using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public static class RuntimeEnvironment
    {
        private const RuntimeEnvironmentType EditorRuntimeEnvironment = RuntimeEnvironmentType.Moblie;

        public static RuntimeEnvironmentType CurrentRuntimeEnvironment
        {
            get
            {
                return GetCurrentRuntimeEnvironment();
            }
        }

        private static RuntimeEnvironmentType GetCurrentRuntimeEnvironment()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor) return EditorRuntimeEnvironment;

            if (Application.platform == RuntimePlatform.IPhonePlayer) return RuntimeEnvironmentType.Moblie;

            if (Application.platform == RuntimePlatform.Android) return RuntimeEnvironmentType.Moblie;

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                IGame game = ServiceContainer.Single<IGame>();

                if (game == null) return RuntimeEnvironmentType.Standalone;

                if (game != null)
                {
                    if (game.IsMobile == true) return RuntimeEnvironmentType.Moblie;

                    if (game.IsMobile == false) return RuntimeEnvironmentType.Standalone;
                }
            }

            return RuntimeEnvironmentType.Unknown;
        }
    }
}