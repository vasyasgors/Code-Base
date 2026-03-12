using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IConfigsProvider : IService
    {
        TConfig GetConfig<TConfig>() where TConfig : ScriptableObject;
    }
}