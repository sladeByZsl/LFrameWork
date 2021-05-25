﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrightnessSaturationAndContrast : PostEffectsBase
{
    public Shader briSatConShader;
    private Material briSatConMaterial;

    //亮度
    [Range(0.0f, 3.0f)]
    public float brightness = 1.0f;

    //饱和度
    [Range(0.0f, 3.0f)]
    public float saturation = 1.0f;

    //对比度
    [Range(0.0f, 3.0f)]
    public float contrast = 1.0f;
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material!=null)
        {
            material.SetFloat("_Brightness",brightness);
            material.SetFloat("_Saturation", saturation);
            material.SetFloat("_Contrast", contrast);
            Graphics.Blit(src,dest,material);
        }
        else
        {
            Graphics.Blit(src,dest,material);
        }
    }
}
