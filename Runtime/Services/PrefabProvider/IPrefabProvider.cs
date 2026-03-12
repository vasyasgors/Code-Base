
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IPrefabProvider : IService
    {
        public TPrefab GetPrefab<TPrefab>();
    }
}