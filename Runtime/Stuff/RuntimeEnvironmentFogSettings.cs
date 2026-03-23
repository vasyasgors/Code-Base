using CodeBase.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase
{
    public class RuntimeEnvironmentFogSettings : MonoBehaviour 
    {
        [SerializeField] private float fogStartDistanceOnMobile;
        [SerializeField] private float fogEndDistanceOnMobile;
        [SerializeField] private float fogStartDistanceOnStandalone;
        [SerializeField] private float fogEndDistanceOnStandalone;

        void Start()
        {
            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Mobile)
            {
                RenderSettings.fogStartDistance = fogStartDistanceOnMobile;
                RenderSettings.fogEndDistance = fogEndDistanceOnMobile;
            }


            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Standalone)
            {
                RenderSettings.fogStartDistance = fogStartDistanceOnStandalone;
                RenderSettings.fogEndDistance = fogEndDistanceOnStandalone;
            }
        }
    }
}
