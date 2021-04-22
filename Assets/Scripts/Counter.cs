using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public Numeral[] numerals;
    public float animateTime = .25f;

    [SerializeField] private int count;
    private int displayCount;
    public int Count { get => count; set => AnimateTo(value); }

    void Start()
    {
        displayCount = count;
    }

    void Update()
    {
        int maxCount = Mathf.FloorToInt(Mathf.Pow(10f, (float) numerals.Length)) - 1;
        int tmpDisplayCount = Mathf.Min(maxCount, displayCount);

        for (int i = 0; i < numerals.Length; i++)
        {
            if (tmpDisplayCount > 0)
            {
                numerals[i].Enabled = true;
                numerals[i].Digit = tmpDisplayCount % 10;

                tmpDisplayCount /= 10;
            }
            else
                numerals[i].Enabled = false;
        }
    }

    public void AnimateTo(int to)
    {
        count = to;
        StopAllCoroutines();
        StartCoroutine(AnimateToCoroutine(to));
    }

    public void JumpTo(int to)
    {
        StopAllCoroutines();
        count = to;
        displayCount = to;
    }

    IEnumerator AnimateToCoroutine(int to)
    {
        int from = displayCount;

        for (float t = 0f; t < 1f; t += Time.deltaTime / animateTime)
        {
            float smoothT = t * t * (3f - 2f * t);
            displayCount = from + Mathf.FloorToInt((float) (to - from) * smoothT);
            yield return null;
        }

        displayCount = to;
    }
}
