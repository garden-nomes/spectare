using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteRepeat : MonoBehaviour
{
    public float height = 4f;
    public float spacing = 20f;

    void Update()
    {
        float cameraBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;

        if (cameraBottom > transform.position.y + height)
            transform.position += Vector3.up * spacing;
    }
}
