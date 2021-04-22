using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public Gate gate;
    public SpriteAnimation unlockAnimation;

    [ContextMenu("Unlock")]
    public void Unlock()
    {
        StartCoroutine(UnlockCoroutine());
    }

    IEnumerator UnlockCoroutine()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < unlockAnimation.frames.Length; i++)
        {
            spriteRenderer.sprite = unlockAnimation.frames[i];
            yield return new WaitForSecondsRealtime(1f / unlockAnimation.fps);
        }

        gate.Open();
        gameObject.SetActive(false);
    }
}
