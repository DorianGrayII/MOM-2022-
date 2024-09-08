// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AnimateShader
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AnimateShader : MonoBehaviour
{
    public float valueRight;

    public float valueLeft;

    public bool animateSingle;

    public float valueSingle;

    public string varName;

    private Material m;

    private void Awake()
    {
        this.m = base.GetComponent<MaskableGraphic>().material;
    }

    private void OnDidApplyAnimationProperties()
    {
        if (!this.animateSingle)
        {
            Vector4 value = new Vector4(this.valueLeft, this.valueRight);
            this.m.SetVector(this.varName, value);
        }
        else
        {
            this.m.SetFloat(this.varName, this.valueSingle);
        }
    }
}
