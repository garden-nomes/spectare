using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Sprite pressedSprite;
    public Gate gate;

    private bool isPressed;

    void Start()
    {
        isPressed = false;
    }

    public void Press()
    {
        if (!isPressed)
        {
            GetComponent<SpriteRenderer>().sprite = pressedSprite;

            if (gate != null)
                gate.Open();

            isPressed = true;
        }
    }
}
