using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    public float initialDelay = 0f;
    public float rate = 1f;
    public float ratio = .5f;

    void Start()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        var renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            renderer.enabled = true;
            yield return new WaitForSeconds(rate * ratio);
            renderer.enabled = false;
            yield return new WaitForSeconds(rate * (1f - ratio));
        }
    }
}
