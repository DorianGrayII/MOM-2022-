using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class RaycastTarget : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    public override void SetMaterialDirty()
    {
    }

    public override void SetVerticesDirty()
    {
    }
}

