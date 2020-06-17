﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontGradient : BaseMeshEffect
{
    public enum Blend
    {
        Override,
        Add,
        Multiply
    }

    public enum Type
    {
        Horizontal,
        Vertical,
        Middle
    }


    [SerializeField]
    Type _gradientType;


    [SerializeField]
    Blend _blendMode = Blend.Multiply;


    [SerializeField]
    [Range(-1, 1)]
    float _offset = 0f;


    [SerializeField]
    UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient()
    { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };


    #region Properties
    public Blend BlendMode
    {
        get { return _blendMode; }
        set { _blendMode = value; }
    }


    public UnityEngine.Gradient EffectGradient
    {
        get { return _effectGradient; }
        set { _effectGradient = value; }
    }


    public Type GradientType
    {
        get { return _gradientType; }
        set { _gradientType = value; }
    }


    public float Offset
    {
        get { return _offset; }
        set { _offset = value; }
    }
    #endregion

    void VerticalModify(List<UIVertex> _vertexList, VertexHelper helper)
    {
        int nCount = _vertexList.Count;
        float bottom = _vertexList[0].position.y;
        float top = _vertexList[0].position.y;
        float y = 0f;
        for (int i = nCount - 1; i >= 1; --i)
        {
            y = _vertexList[i].position.y;


            if (y > top) top = y;
            else if (y < bottom) bottom = y;
        }
        float height = 1f / (top - bottom);
        UIVertex vertex = new UIVertex();


        for (int i = 0; i < helper.currentVertCount; i++)
        {
            helper.PopulateUIVertex(ref vertex, i);


            vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate((vertex.position.y - bottom) * height - Offset));


            helper.SetUIVertex(vertex, i);
        }
    }
     void MiddleModify(List<UIVertex> _vertexList, VertexHelper helper)
    {
        int nCount = _vertexList.Count;
        float left = _vertexList[0].position.x;
        float right = _vertexList[0].position.x;
        float x = 0f;
        for (int i = nCount - 1; i >= 1; --i)
        {
            x = _vertexList[i].position.x;


            if (x > right) right = x;
            else if (x < left) left = x;
        }
        float width = 1f / (right - left);
        float middle = (right + left) / 2.0f;
        UIVertex vertex = new UIVertex();


        for (int i = 0; i < helper.currentVertCount; i++)
        {
            helper.PopulateUIVertex(ref vertex, i);


            vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate(Math.Abs(vertex.position.x - middle) * width * 2 - Offset));


            helper.SetUIVertex(vertex, i);
        }
    }
    void HorizontalModify(List<UIVertex> _vertexList, VertexHelper helper)
    {
        int nCount = _vertexList.Count;
        float left = _vertexList[0].position.x;
         float right = _vertexList[0].position.x;
         float x = 0f;
        for (int i = nCount - 1; i >= 1; --i)
        {
            x = _vertexList[i].position.x;
 
 
            if (x > right) right = x;
            else if (x<left) left = x;
        }
        float width = 1f / (right - left);
        UIVertex vertex = new UIVertex();
 
 
        for (int i = 0; i<helper.currentVertCount; i++)
        {
            helper.PopulateUIVertex(ref vertex, i);
 
 
            vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate((vertex.position.x - left) * width - Offset));
 
 
            helper.SetUIVertex(vertex, i);
        }

}

    public override void ModifyMesh(VertexHelper helper)
    {
        if (!IsActive() || helper.currentVertCount == 0)
            return;


        List<UIVertex> _vertexList = new List<UIVertex>();


        helper.GetUIVertexStream(_vertexList);


        int nCount = _vertexList.Count;
        switch (GradientType)
        {
            case Type.Horizontal:
                HorizontalModify(_vertexList, helper);
                break;
            case Type.Vertical:
                VerticalModify(_vertexList, helper);
                break;
            case Type.Middle:
                MiddleModify(_vertexList, helper);
                break;

        }
                
    }
    Color BlendColor(Color colorA, Color colorB)
    {
        switch (BlendMode)
        {
            default: return colorB;
            case Blend.Add: return colorA + colorB;
            case Blend.Multiply: return colorA * colorB;
        }
    }
}
