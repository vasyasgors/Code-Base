using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase
{
    public class RuntimeStaticBatching : MonoBehaviour
    {
        void Awake()
        {
            StaticBatchingUtility.Combine(gameObject);
        }

    }
}