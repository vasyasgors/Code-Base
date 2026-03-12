using UnityEngine;
using System.Collections;

namespace CodeBase.Infrastructure
{
    public interface ICoroutineRunner : IService
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}
