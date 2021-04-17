using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        var position = target.position;
        position.z = transform.position.z;
        transform.position = position;
    }
}
