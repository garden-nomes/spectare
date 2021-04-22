using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupIndicator : MonoBehaviour
{
    public Sprite[] frames;
    public PlayerController playerController;
    public float visibleTime = 2f;

    private SpriteRenderer spriteRenderer;
    private float visibleTimer = 0f;
    private float previousPowerupCount = 0;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPowerupCount = gameManager.PowerupCount;
    }

    void Update()
    {
        if (previousPowerupCount != gameManager.PowerupCount)
        {
            visibleTimer = visibleTime;
            spriteRenderer.enabled = true;
        }

        if (visibleTimer > 0f)
        {
            int frame = Mathf.Min(gameManager.PowerupCount, frames.Length - 1);
            spriteRenderer.sprite = frames[frame];

            visibleTimer -= Time.deltaTime;
            if (visibleTimer <= 0f)
                spriteRenderer.enabled = false;
        }

        previousPowerupCount = gameManager.PowerupCount;
    }
}
