using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnimationSet
{
    public SpriteAnimation defaultAnimation;
    public SpriteAnimation withDoubleJumpAnimation;
}

public class PlayerAnimator : MonoBehaviour
{
    public PlayerAnimationSet idleAnimation;
    public PlayerAnimationSet runAnimation;
    public PlayerAnimationSet jumpAnimation;
    public PlayerAnimationSet fallAnimation;
    public PlayerAnimationSet wallDragAnimation;
    public PlayerAnimationSet landAnimation;
    public SpriteAnimation deathAnimation;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private PlayerAnimationSet currentAnimation;
    private float timer = 0f;
    private bool isInCoroutine = false;
    private bool hadDoubleJumpLastFrame = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        currentAnimation = idleAnimation;
        playerController.onDeath += () => StartCoroutine(DeathCoroutine());
    }

    void Update()
    {
        if (isInCoroutine)
            return;

        if (playerController.IsGrounded)
        {
            if (playerController.velocity.x == 0f)
                SetAnimation(idleAnimation);
            else
                SetAnimation(runAnimation);
        }
        else
        {
            if (playerController.IsLeftTouching || playerController.IsRightTouching)
                SetAnimation(wallDragAnimation);
            else if (playerController.velocity.y > 0f)
                SetAnimation(jumpAnimation);
            else
                SetAnimation(fallAnimation);
        }

        if (!playerController.hasDoubleJump)
        {
            if (hadDoubleJumpLastFrame)
                StartCoroutine(GainOrLoseDoubleJumpCoroutine(false));
            else
                UpdateSprite(currentAnimation.defaultAnimation);
        }
        else
        {
            if (!hadDoubleJumpLastFrame)
                StartCoroutine(GainOrLoseDoubleJumpCoroutine(true));
            else
                UpdateSprite(currentAnimation.withDoubleJumpAnimation);
        }

        hadDoubleJumpLastFrame = playerController.hasDoubleJump;

        timer += Time.deltaTime;
    }

    void UpdateSprite(SpriteAnimation animation)
    {
        int frame = Mathf.FloorToInt(timer * animation.fps) % animation.frames.Length;
        spriteRenderer.sprite = animation.frames[frame];
    }

    void SetAnimation(PlayerAnimationSet animation)
    {
        if (animation == currentAnimation)
            return;

        timer = 0f;
        currentAnimation = animation;
    }

    IEnumerator GainOrLoseDoubleJumpCoroutine(bool hasDoubleJump)
    {
        isInCoroutine = true;
        int initialSortingOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = 999;
        Time.timeScale = 0f;
        UpdateSprite(hasDoubleJump ? currentAnimation.defaultAnimation : currentAnimation.withDoubleJumpAnimation);

        if (hasDoubleJump)
            GameObject.FindObjectOfType<DoubleJumpUnlockedDisplay>().Show();

        yield return new WaitForSecondsRealtime(0.2f);

        bool isBig = !hasDoubleJump;
        for (float i = 0; i < 10; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            UpdateSprite(isBig ? currentAnimation.withDoubleJumpAnimation : currentAnimation.defaultAnimation);
            isBig = !isBig;
        }

        UpdateSprite(hasDoubleJump ? currentAnimation.withDoubleJumpAnimation : currentAnimation.defaultAnimation);

        yield return new WaitForSecondsRealtime(0.2f);

        Time.timeScale = 1f;
        spriteRenderer.sortingOrder = initialSortingOrder;
        isInCoroutine = false;
    }

    IEnumerator DeathCoroutine()
    {
        isInCoroutine = true;
        spriteRenderer.sortingOrder = 999;
        Time.timeScale = 0f;

        for (int i = 0; i < deathAnimation.frames.Length; i++)
        {
            spriteRenderer.sprite = deathAnimation.frames[i];
            yield return new WaitForSecondsRealtime(1f / deathAnimation.fps);
        }

        yield return new WaitForSecondsRealtime(.2f);
        Time.timeScale = 1f;
        GameObject.Destroy(gameObject);
    }
}
