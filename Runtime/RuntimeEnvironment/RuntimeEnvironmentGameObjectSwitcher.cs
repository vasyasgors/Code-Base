using UnityEngine;

namespace CodeBase.Infrastructure
{
    [DefaultExecutionOrder(-9000)]
    public class RuntimeEnvironmentGameObjectSwitcher : MonoBehaviour
    {
        public GameObject[] mobileGameObjects;
        public GameObject[] standaloneGameObjexts;

        private void Awake()
        {
            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Mobile)
            {
                SetActiveArrayObjects(mobileGameObjects, true);
                SetActiveArrayObjects(standaloneGameObjexts, false);
            }

            if (RuntimeEnvironment.CurrentRuntimeEnvironment == RuntimeEnvironmentType.Standalone)
            {
                SetActiveArrayObjects(mobileGameObjects, false);
                SetActiveArrayObjects(standaloneGameObjexts, true);
            }
        }

        private void SetActiveArrayObjects(GameObject[] gameObjects, bool enabled)
        {
            for (int i = 0; i < gameObjects.Length; i++) gameObjects[i].SetActive(enabled);
        }
    }
}
