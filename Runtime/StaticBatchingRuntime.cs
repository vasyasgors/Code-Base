using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// переименовать в  runtime static batching 
public class StaticBatchingRuntime : MonoBehaviour
{
    void Awake()
    {
        StaticBatchingUtility.Combine(gameObject);
    }

}
