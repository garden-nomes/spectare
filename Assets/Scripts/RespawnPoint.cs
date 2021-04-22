using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float step = Mathf.PI / 8f;
        for (float a = 0; a < Mathf.PI * 2f; a += step)
        {
            Vector3 from = transform.position + new Vector3(Mathf.Cos(a), Mathf.Sin(a)) * 0.5f;
            Vector3 to = transform.position + new Vector3(Mathf.Cos(a + step), Mathf.Sin(a + step)) * 0.5f;
            Gizmos.DrawLine(from, to);
        }
    }
#endif
}
