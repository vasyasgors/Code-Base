using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IConfigsProvider : IService
    {
        void Load();
        TConfig GetConfig<TConfig>() where TConfig : ScriptableObject;
    }
}