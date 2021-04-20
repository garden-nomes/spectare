using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ScaleUiWithPixelPerfectCamera : MonoBehaviour
{
    public PixelPerfectCamera pixelPerfectCamera;

    private CanvasScaler canvasScaler;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
    }

    void Update()
    {
        canvasScaler.scaleFactor = pixelPerfectCamera.pixelRatio;
    }
}
