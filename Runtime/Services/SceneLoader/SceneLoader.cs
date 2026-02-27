using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infractructure
{
    public class SceneLoader : ISceneLoader
    {
        private ICoroutineRunner coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner)
        {
            this.coroutineRunner = coroutineRunner;
        }

        public void Load(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(name, loadSceneMode);
        }

        public void LoadAsync(string name, LoadSceneMode loadSceneMode, Action onLoaded = null)
        {
            coroutineRunner.StartCoroutine(StartLoadAsync(name, loadSceneMode, onLoaded));
        }

        private IEnumerator StartLoadAsync(string name, LoadSceneMode loadSceneMode, Action onLoaded = null)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name, loadSceneMode);

            while (asyncOperation.isDone == false)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}
