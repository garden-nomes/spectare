using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Bounds bounds;

    private Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 position = transform.position;

        // lock to target
        if (target != null)
        {
            position.x = target.position.x;
            position.y = target.position.y;
        }

        // confine within bounds
        if (bounds != null)
        {
            float vsize = camera.orthographicSize;
            float hsize = camera.aspect * vsize;
            position.x = Mathf.Max(bounds.min.x, position.x - hsize) + hsize;
            position.x = Mathf.Min(bounds.max.x, position.x + hsize) - hsize;
            position.y = Mathf.Max(bounds.min.y, position.y - vsize) + vsize;
            position.y = Mathf.Min(bounds.max.y, position.y + vsize) - vsize;
        }

        transform.position = position;
    }
}
