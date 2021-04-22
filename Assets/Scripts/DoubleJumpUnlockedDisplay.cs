using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpUnlockedDisplay : MonoBehaviour
{
    public float displayTime = 4f;
    public GameObject doubleJumpUnlockedDisplay;
    public float flashRate = 0.5f;
    public float flashRatio = 2f / 3f;

    void Start()
    {
        doubleJumpUnlockedDisplay.SetActive(false);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        StopAllCoroutines();
        StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        for (float t = 0f; t < displayTime; t += flashRate)
        {
            doubleJumpUnlockedDisplay.SetActive(true);
            yield return new WaitForSecondsRealtime(flashRate * flashRatio);
            doubleJumpUnlockedDisplay.SetActive(false);
            yield return new WaitForSecondsRealtime(flashRate * (1f - flashRatio));
        }

        doubleJumpUnlockedDisplay.SetActive(false);
    }
}
