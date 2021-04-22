using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public Vector3 pan;

    void Update()
    {
        transform.position += pan * Time.deltaTime;
    }
}
