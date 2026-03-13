using UnityEngine;
using CodeBase.Infrastructure;

namespace CodeBase
{
    public class CameraFarByRuntimeEnvironment : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private float distanceOnMobile;
        [SerializeField] private float distanceOnStandalone;


        void Start()
        {
            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Mobile)
            {
                camera.farClipPlane = distanceOnMobile;
            }

            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Standalone)
            {
                camera.farClipPlane = distanceOnStandalone;

            }
        }
    }
}
