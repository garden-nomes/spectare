using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteAnimation animation;
    [SerializeField] private bool randomizeStart = false;
    public SpriteAnimation Animation
    {
        get => animation;
        set => SetAnimation(value, true);
    }

    private float timer;
    private SpriteRenderer spriteRenderer;
    private bool isCompletedThisFrame;
    public bool IsCompletedThisFrame => isCompletedThisFrame;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = 0f;

        if (randomizeStart)
            timer = Random.value * ((float) animation.frames.Length) / animation.fps;
    }

    void Update()
    {
        int frame = Mathf.FloorToInt(timer * animation.fps) % animation.frames.Length;
        spriteRenderer.sprite = animation.frames[frame];

        timer += Time.deltaTime;

        int nextFrame = Mathf.FloorToInt(timer * animation.fps % animation.frames.Length);
        isCompletedThisFrame = frame > 0 && nextFrame == 0;
    }

    public void SetAnimation(SpriteAnimation animation, bool restartTimer = true)
    {
        if (animation == this.animation)
            return;

        this.animation = animation;

        if (restartTimer)
            timer = 0f;
    }
}
