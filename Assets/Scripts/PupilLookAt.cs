using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupilLookAt : MonoBehaviour
{
    [SerializeField] private BoxCollider2D bounds;
    public Vector3 target;

    void Update()
    {
        if (target != null)
        {
            Vector3 toTarget = (target - transform.parent.position).normalized;
            toTarget.x *= bounds.bounds.size.x * 0.5f;
            toTarget.y *= bounds.bounds.size.y * 0.5f;
            toTarget.x = Mathf.Round(toTarget.x * 8f) / 8f;
            toTarget.y = Mathf.Round(toTarget.y * 8f) / 8f;
            transform.localPosition = toTarget;
        }
    }
}
