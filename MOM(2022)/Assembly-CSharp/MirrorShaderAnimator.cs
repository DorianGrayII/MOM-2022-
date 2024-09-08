using System;
using UnityEngine;
using UnityEngine.UI;

public class MirrorShaderAnimator : MonoBehaviour
{
    public float twist;
    private float lastTwist = float.MaxValue;
    private Material m;
    private MaskableGraphic graphic;

    private void Start()
    {
        this.graphic = base.GetComponent<MaskableGraphic>();
        this.m = this.graphic.material;
    }

    private void Update()
    {
        if (this.twist != this.lastTwist)
        {
            this.lastTwist = this.twist;
            this.m.SetFloat("_TwistUvAmount", this.twist);
        }
    }
}

