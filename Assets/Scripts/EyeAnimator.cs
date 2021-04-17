using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EyeAnimatorState
{
    Closed,
    Opening,
    Closing,
    Open,
}

public class EyeAnimator : MonoBehaviour
{
    public SpriteAnimation outlineAnimation;
    public SpriteAnimation backdropAnimation;

    public SpriteRenderer outlineRenderer;
    public SpriteRenderer backdropRenderer;
    public SpriteMask pupilMask;

    private EyeAnimatorState state = EyeAnimatorState.Closed;
    public EyeAnimatorState State => state;
    private int frameCount;
    private float fps;
    private int currentFrame;

    void Start()
    {
        Debug.Assert(outlineAnimation.frames.Length == backdropAnimation.frames.Length);
        frameCount = outlineAnimation.frames.Length;
        fps = outlineAnimation.fps;
        currentFrame = frameCount - 1;
        SetFrame(currentFrame);
    }

    [ContextMenu("Open")]
    public void Open()
    {
        StartCoroutine(OpenCoroutine());
    }

    [ContextMenu("Close")]
    public void Close()
    {
        StartCoroutine(CloseCoroutine());
    }

    public IEnumerator OpenCoroutine()
    {
        state = EyeAnimatorState.Opening;

        while (state == EyeAnimatorState.Opening && currentFrame > 0)
        {
            currentFrame--;
            SetFrame(currentFrame);
            yield return new WaitForSeconds(1f / fps);
        }

        if (state == EyeAnimatorState.Opening)
            state = EyeAnimatorState.Open;
    }

    public IEnumerator CloseCoroutine()
    {
        state = EyeAnimatorState.Closing;

        while (state == EyeAnimatorState.Closing && currentFrame < frameCount - 1)
        {
            currentFrame++;
            SetFrame(currentFrame);
            yield return new WaitForSeconds(1f / fps);
        }

        if (state == EyeAnimatorState.Closing)
            state = EyeAnimatorState.Closed;
    }

    void SetFrame(int frame)
    {
        outlineRenderer.sprite = outlineAnimation.frames[frame];
        backdropRenderer.sprite = backdropAnimation.frames[frame];
        pupilMask.sprite = backdropAnimation.frames[frame];
    }
}
