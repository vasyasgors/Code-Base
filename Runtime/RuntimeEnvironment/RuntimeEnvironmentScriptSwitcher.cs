using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    [DefaultExecutionOrder(-9000)]
    public class RuntimeEnvironmentScriptSwitcher : MonoBehaviour
    {
        public MonoBehaviour[] mobileScripts;
        public MonoBehaviour[] standaloneScripts;

        private void Awake()
        {
            if(RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Mobile)
            {
                SetEnabledArrayScripts(mobileScripts, true);
                SetEnabledArrayScripts(standaloneScripts, false);
            }

            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Standalone)
            {
                SetEnabledArrayScripts(mobileScripts, false);
                SetEnabledArrayScripts(standaloneScripts, true);
            }
        }

        private void SetEnabledArrayScripts(MonoBehaviour[] monoBehaviours, bool enabled)
        {
            for (int i = 0; i < monoBehaviours.Length; i++) monoBehaviours[i].enabled = enabled;
        }
    }
}
