using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Numeral : MonoBehaviour
{
    public Sprite[] numeralSprites;

    private Image image;
    private SpriteRenderer spriteRenderer;
    private bool hasSpriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasSpriteRenderer = spriteRenderer != null;

        if (!hasSpriteRenderer)
            image = GetComponent<Image>();
    }

    public int Digit
    {
        set
        {
            if (hasSpriteRenderer)
                spriteRenderer.sprite = numeralSprites[value % 10];
            else
                image.sprite = numeralSprites[value % 10];
        }
    }

    public bool Enabled
    {
        set
        {
            if (hasSpriteRenderer)
                spriteRenderer.enabled = value;
            else
                image.enabled = value;
        }
    }
}
