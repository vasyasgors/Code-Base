using UnityEngine;
using System.Collections;

namespace CodeBase.Infractructure
{
    public interface ICoroutineRunner : IService
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}
