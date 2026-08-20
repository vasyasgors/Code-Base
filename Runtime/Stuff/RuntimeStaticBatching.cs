using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class RuntimeStaticBatching : MonoBehaviour
    {

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StaticBatchingUtility.Combine(gameObject);

            SceneManager.sceneLoaded -= OnSceneLoaded;     
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }


    }
}