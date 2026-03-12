using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase
{
    public class StaticBatchingRuntime : MonoBehaviour
    {
        void Awake()
        {
            StaticBatchingUtility.Combine(gameObject);
        }

    }
}