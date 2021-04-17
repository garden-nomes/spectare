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

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPowerupCount = playerController.Powerups;
    }

    void Update()
    {
        if (previousPowerupCount != playerController.Powerups)
        {
            visibleTimer = visibleTime;
            spriteRenderer.enabled = true;
        }

        if (visibleTimer > 0f)
        {
            int frame = Mathf.Min(playerController.Powerups, frames.Length - 1);
            spriteRenderer.sprite = frames[frame];

            visibleTimer -= Time.deltaTime;
            if (visibleTimer <= 0f)
                spriteRenderer.enabled = false;
        }

        previousPowerupCount = playerController.Powerups;
    }
}
