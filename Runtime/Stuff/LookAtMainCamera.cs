using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    private Transform target;

    private void Update()
    {
        if (target == null)
            target = Camera.main.transform;

        if (target == null) return;

        transform.LookAt(target);
    }
}
