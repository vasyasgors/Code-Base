using CodeBase.Infractructure;
using System;
using UnityEngine.SceneManagement;


namespace CodeBase.Infractructure
{
    public interface ISceneLoader : IService
    {
        void LoadAsync(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single, Action onLoaded = null);
        void Load(string name, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
    }
}