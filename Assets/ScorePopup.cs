using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    public float showTime = 3f;
    public Vector3 move;

    void Start()
    {
        StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        var initialPosition = transform.position;

        for (float t = 0f; t < 1f; t += Time.deltaTime / showTime)
        {
            float invT = 1f - t;
            float cubicEaseOut = 1f - invT * invT * invT;
            transform.position = initialPosition + move * cubicEaseOut;
            yield return null;
        }

        GameObject.Destroy(gameObject);
    }
}
