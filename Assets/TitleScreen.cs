using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public string gameScene;

    void Update()
    {
        bool isAnyKeyDown = Keyboard.current.anyKey.wasPressedThisFrame;

        bool isAnyButtonDown = false;
        if (Gamepad.current != null)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is ButtonControl button && button.isPressed && !control.synthetic)
                    isAnyButtonDown = true;
            }
        }

        if (isAnyKeyDown || isAnyButtonDown)
            SceneManager.LoadScene(gameScene);
    }
}
