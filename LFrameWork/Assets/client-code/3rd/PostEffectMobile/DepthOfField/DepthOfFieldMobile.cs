using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quality
{
    height = 1,
    middle = 2,
    low = 4,
}
[ExecuteInEditMode]
public class DepthOfFieldMobile : PostEffectsBase
{
    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnDisable()
    {
        GetComponent<Camera>().depthTextureMode &= ~DepthTextureMode.Depth;
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material)
        {
            Graphics.Blit(src, dest, material);
        }
    }
}
