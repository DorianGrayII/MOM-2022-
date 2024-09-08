// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AllIn1Shader
using System.Collections.Generic;
using System.IO;
using AllIn1SpriteShader;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("AllIn1SpriteShader/AddAllIn1Shader")]
public class AllIn1Shader : MonoBehaviour
{
    public enum ShaderTypes
    {
        Default = 0,
        ScaledTime = 1,
        MaskedUI = 2,
        Urp2dRenderer = 3,
        Invalid = 4
    }

    private enum AfterSetAction
    {
        Clear = 0,
        CopyMaterial = 1,
        Reset = 2
    }

    public ShaderTypes shaderTypes = ShaderTypes.Invalid;

    private Material currMaterial;

    private Material prevMaterial;

    private bool matAssigned;

    private bool destroyed;

    [Range(1f, 20f)]
    public float normalStrenght = 5f;

    [Range(0f, 3f)]
    public int normalSmoothing = 1;

    [HideInInspector]
    public bool computingNormal;

    private bool needToWait;

    private int waitingCycles;

    private int timesWeWaited;

    private SpriteRenderer normalMapSr;

    private Renderer normalMapRenderer;

    private bool isSpriteRenderer;

    private string path;

    private string subPath;

    public void MakeNewMaterial(bool getShaderTypeFromPrefs, string shaderName = "AllIn1SpriteShader")
    {
        this.SetMaterial(AfterSetAction.Clear, getShaderTypeFromPrefs, shaderName);
    }

    public void MakeCopy()
    {
        this.SetMaterial(AfterSetAction.CopyMaterial, getShaderTypeFromPrefs: false, this.GetStringFromShaderType());
    }

    private void ResetAllProperties(bool getShaderTypeFromPrefs, string shaderName)
    {
        this.SetMaterial(AfterSetAction.Reset, getShaderTypeFromPrefs, shaderName);
    }

    private string GetStringFromShaderType()
    {
        if (this.shaderTypes == ShaderTypes.Default)
        {
            return "AllIn1SpriteShader";
        }
        if (this.shaderTypes == ShaderTypes.ScaledTime)
        {
            return "AllIn1SpriteShaderScaledTime";
        }
        if (this.shaderTypes == ShaderTypes.MaskedUI)
        {
            return "AllIn1SpriteShaderUiMask";
        }
        if (this.shaderTypes == ShaderTypes.Urp2dRenderer)
        {
            return "AllIn1Urp2dRenderer";
        }
        return "AllIn1SpriteShader";
    }

    private void SetMaterial(AfterSetAction action, bool getShaderTypeFromPrefs, string shaderName)
    {
        Shader shader = Resources.Load(shaderName, typeof(Shader)) as Shader;
        if (getShaderTypeFromPrefs)
        {
            switch (PlayerPrefs.GetInt("allIn1DefaultShader"))
            {
            case 1:
                shader = Resources.Load("AllIn1SpriteShaderScaledTime", typeof(Shader)) as Shader;
                break;
            case 2:
                shader = Resources.Load("AllIn1SpriteShaderUiMask", typeof(Shader)) as Shader;
                break;
            case 3:
                shader = Resources.Load("AllIn1Urp2dRenderer", typeof(Shader)) as Shader;
                break;
            }
        }
        if (!Application.isPlaying && Application.isEditor && shader != null)
        {
            bool flag = false;
            if (base.GetComponent<Renderer>() != null)
            {
                flag = true;
                this.prevMaterial = new Material(base.GetComponent<Renderer>().sharedMaterial);
                this.currMaterial = new Material(shader);
                base.GetComponent<Renderer>().sharedMaterial = this.currMaterial;
                base.GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
                this.matAssigned = true;
                this.DoAfterSetAction(action);
            }
            else
            {
                Graphic component = base.GetComponent<Graphic>();
                if (component != null)
                {
                    flag = true;
                    this.prevMaterial = new Material(component.material);
                    this.currMaterial = new Material(shader);
                    component.material = this.currMaterial;
                    component.material.hideFlags = HideFlags.None;
                    this.matAssigned = true;
                    this.DoAfterSetAction(action);
                }
            }
            if (!flag)
            {
                this.MissingRenderer();
            }
            else
            {
                this.SetSceneDirty();
            }
        }
        else if (shader == null)
        {
            Debug.LogError("Make sure the AllIn1SpriteShader shader variants are inside the Resource folder!");
        }
    }

    private void DoAfterSetAction(AfterSetAction action)
    {
        switch (action)
        {
        case AfterSetAction.Clear:
            this.ClearAllKeywords();
            break;
        case AfterSetAction.CopyMaterial:
            this.currMaterial.CopyPropertiesFromMaterial(this.prevMaterial);
            break;
        }
    }

    public void TryCreateNew()
    {
        bool flag = false;
        Renderer component = base.GetComponent<Renderer>();
        if (component != null)
        {
            flag = true;
            if (component != null && component.sharedMaterial != null && component.sharedMaterial.name.Contains("AllIn1"))
            {
                this.ResetAllProperties(getShaderTypeFromPrefs: false, this.GetStringFromShaderType());
                this.ClearAllKeywords();
            }
            else
            {
                this.CleanMaterial();
                this.MakeNewMaterial(getShaderTypeFromPrefs: false, this.GetStringFromShaderType());
            }
        }
        else
        {
            Graphic component2 = base.GetComponent<Graphic>();
            if (component2 != null)
            {
                flag = true;
                if (component2.material.name.Contains("AllIn1"))
                {
                    this.ResetAllProperties(getShaderTypeFromPrefs: false, this.GetStringFromShaderType());
                    this.ClearAllKeywords();
                }
                else
                {
                    this.MakeNewMaterial(getShaderTypeFromPrefs: false, this.GetStringFromShaderType());
                }
            }
        }
        if (!flag)
        {
            this.MissingRenderer();
        }
        this.SetSceneDirty();
    }

    public void ClearAllKeywords()
    {
        this.SetKeyword("RECTSIZE_ON");
        this.SetKeyword("OFFSETUV_ON");
        this.SetKeyword("CLIPPING_ON");
        this.SetKeyword("POLARUV_ON");
        this.SetKeyword("TWISTUV_ON");
        this.SetKeyword("ROTATEUV_ON");
        this.SetKeyword("FISHEYE_ON");
        this.SetKeyword("PINCH_ON");
        this.SetKeyword("SHAKEUV_ON");
        this.SetKeyword("WAVEUV_ON");
        this.SetKeyword("ROUNDWAVEUV_ON");
        this.SetKeyword("DOODLE_ON");
        this.SetKeyword("ZOOMUV_ON");
        this.SetKeyword("FADE_ON");
        this.SetKeyword("TEXTURESCROLL_ON");
        this.SetKeyword("GLOW_ON");
        this.SetKeyword("OUTBASE_ON");
        this.SetKeyword("ONLYOUTLINE_ON");
        this.SetKeyword("OUTTEX_ON");
        this.SetKeyword("OUTDIST_ON");
        this.SetKeyword("DISTORT_ON");
        this.SetKeyword("WIND_ON");
        this.SetKeyword("GRADIENT_ON");
        this.SetKeyword("GRADIENT2COL_ON");
        this.SetKeyword("RADIALGRADIENT_ON");
        this.SetKeyword("COLORSWAP_ON");
        this.SetKeyword("HSV_ON");
        this.SetKeyword("HITEFFECT_ON");
        this.SetKeyword("PIXELATE_ON");
        this.SetKeyword("NEGATIVE_ON");
        this.SetKeyword("GRADIENTCOLORRAMP_ON");
        this.SetKeyword("COLORRAMP_ON");
        this.SetKeyword("GREYSCALE_ON");
        this.SetKeyword("POSTERIZE_ON");
        this.SetKeyword("BLUR_ON");
        this.SetKeyword("MOTIONBLUR_ON");
        this.SetKeyword("GHOST_ON");
        this.SetKeyword("ALPHAOUTLINE_ON");
        this.SetKeyword("INNEROUTLINE_ON");
        this.SetKeyword("ONLYINNEROUTLINE_ON");
        this.SetKeyword("HOLOGRAM_ON");
        this.SetKeyword("CHROMABERR_ON");
        this.SetKeyword("GLITCH_ON");
        this.SetKeyword("FLICKER_ON");
        this.SetKeyword("SHADOW_ON");
        this.SetKeyword("SHINE_ON");
        this.SetKeyword("CONTRAST_ON");
        this.SetKeyword("OVERLAY_ON");
        this.SetKeyword("OVERLAYMULT_ON");
        this.SetKeyword("ALPHACUTOFF_ON");
        this.SetKeyword("ALPHAROUND_ON");
        this.SetKeyword("CHANGECOLOR_ON");
        this.SetKeyword("CHANGECOLOR2_ON");
        this.SetKeyword("CHANGECOLOR3_ON");
        this.SetKeyword("FOG_ON");
        this.SetSceneDirty();
    }

    private void SetKeyword(string keyword, bool state = false)
    {
        if (this.destroyed)
        {
            return;
        }
        if (this.currMaterial == null)
        {
            this.FindCurrMaterial();
            if (this.currMaterial == null)
            {
                this.MissingRenderer();
                return;
            }
        }
        if (!state)
        {
            this.currMaterial.DisableKeyword(keyword);
        }
        else
        {
            this.currMaterial.EnableKeyword(keyword);
        }
    }

    private void FindCurrMaterial()
    {
        if (base.GetComponent<Renderer>() != null)
        {
            this.currMaterial = base.GetComponent<Renderer>().sharedMaterial;
            this.matAssigned = true;
            return;
        }
        Graphic component = base.GetComponent<Graphic>();
        if (component != null)
        {
            this.currMaterial = component.material;
            this.matAssigned = true;
        }
    }

    public void CleanMaterial()
    {
        Renderer component = base.GetComponent<Renderer>();
        if (component != null)
        {
            component.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            this.matAssigned = false;
        }
        else
        {
            Graphic component2 = base.GetComponent<Graphic>();
            if (component2 != null)
            {
                component2.material = new Material(Shader.Find("Sprites/Default"));
                this.matAssigned = false;
            }
        }
        this.SetSceneDirty();
    }

    public void SaveMaterial()
    {
    }

    private void SaveMaterialWithOtherName(string path, int i = 1)
    {
        int num = i;
        string fileName = string.Concat(path + "_" + num, ".mat");
        if (File.Exists(fileName))
        {
            num++;
            this.SaveMaterialWithOtherName(path, num);
        }
        else
        {
            this.DoSaving(fileName);
        }
    }

    private void DoSaving(string fileName)
    {
    }

    public void SetSceneDirty()
    {
    }

    private void MissingRenderer()
    {
    }

    public void ToggleSetAtlasUvs(bool activate)
    {
        SetAtlasUvs setAtlasUvs = base.GetComponent<SetAtlasUvs>();
        if (activate)
        {
            if (setAtlasUvs == null)
            {
                setAtlasUvs = base.gameObject.AddComponent<SetAtlasUvs>();
            }
            setAtlasUvs.GetAndSetUVs();
            this.SetKeyword("ATLAS_ON", state: true);
        }
        else
        {
            if (setAtlasUvs != null)
            {
                setAtlasUvs.ResetAtlasUvs();
                Object.DestroyImmediate(setAtlasUvs);
            }
            this.SetKeyword("ATLAS_ON");
        }
        this.SetSceneDirty();
    }

    public void ApplyMaterialToHierarchy()
    {
        Renderer component = base.GetComponent<Renderer>();
        Graphic component2 = base.GetComponent<Graphic>();
        Material material = null;
        if (component != null)
        {
            material = component.sharedMaterial;
        }
        else
        {
            if (!(component2 != null))
            {
                this.MissingRenderer();
                return;
            }
            material = component2.material;
        }
        List<Transform> transforms = new List<Transform>();
        this.GetAllChildren(base.transform, ref transforms);
        foreach (Transform item in transforms)
        {
            component = item.gameObject.GetComponent<Renderer>();
            if (component != null)
            {
                component.material = material;
                continue;
            }
            component2 = item.gameObject.GetComponent<Graphic>();
            if (component2 != null)
            {
                component2.material = material;
            }
        }
    }

    public void CheckIfValidTarget()
    {
        Renderer component = base.GetComponent<Renderer>();
        Graphic component2 = base.GetComponent<Graphic>();
        if (component == null && component2 == null)
        {
            this.MissingRenderer();
        }
    }

    private void GetAllChildren(Transform parent, ref List<Transform> transforms)
    {
        foreach (Transform item in parent)
        {
            transforms.Add(item);
            this.GetAllChildren(item, ref transforms);
        }
    }

    public void RenderToImage()
    {
    }

    public void RenderAndSaveTexture(Material targetMaterial, Texture targetTexture)
    {
    }

    private string GetNewValidPath(string path, int i = 1)
    {
        int num = i;
        string result = string.Concat(path + "_" + num, ".png");
        if (File.Exists(result))
        {
            num++;
            result = this.GetNewValidPath(path, num);
        }
        return result;
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

    protected virtual void OnEditorUpdate()
    {
        if (!this.computingNormal)
        {
            return;
        }
        if (this.needToWait)
        {
            this.waitingCycles++;
            if (this.waitingCycles > 5)
            {
                this.needToWait = false;
                this.timesWeWaited++;
            }
            return;
        }
        if (this.timesWeWaited == 1)
        {
            this.SetNewNormalTexture2();
        }
        if (this.timesWeWaited == 2)
        {
            this.SetNewNormalTexture3();
        }
        if (this.timesWeWaited == 3)
        {
            this.SetNewNormalTexture4();
        }
        this.needToWait = true;
    }

    public void CreateAndAssignNormalMap()
    {
    }

    private void SetNewNormalTexture()
    {
        this.computingNormal = false;
    }

    private void SetNewNormalTexture2()
    {
    }

    private void SetNewNormalTexture3()
    {
    }

    private void SetNewNormalTexture4()
    {
    }

    private Texture2D CreateNormalMap(Texture2D t, float normalMult = 5f, int normalSmooth = 0)
    {
        Color[] array = new Color[t.width * t.height];
        Texture2D texture2D = new Texture2D(t.width, t.height, TextureFormat.RGB24, mipChain: false, linear: false);
        Vector3 rhs = new Vector3(0.3333f, 0.3333f, 0.3333f);
        for (int i = 0; i < t.height; i++)
        {
            for (int j = 0; j < t.width; j++)
            {
                Color pixel = t.GetPixel(j - 1, i - 1);
                Vector3 lhs = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j, i - 1);
                Vector3 lhs2 = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j + 1, i - 1);
                Vector3 lhs3 = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j - 1, i);
                Vector3 lhs4 = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j + 1, i);
                Vector3 lhs5 = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j - 1, i + 1);
                Vector3 lhs6 = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j, i + 1);
                Vector3 lhs7 = new Vector3(pixel.r, pixel.g, pixel.g);
                pixel = t.GetPixel(j + 1, i + 1);
                Vector3 lhs8 = new Vector3(pixel.r, pixel.g, pixel.g);
                float num = Vector3.Dot(lhs, rhs);
                float num2 = Vector3.Dot(lhs2, rhs);
                float num3 = Vector3.Dot(lhs3, rhs);
                float num4 = Vector3.Dot(lhs4, rhs);
                float num5 = Vector3.Dot(lhs5, rhs);
                float num6 = Vector3.Dot(lhs6, rhs);
                float num7 = Vector3.Dot(lhs7, rhs);
                float num8 = Vector3.Dot(lhs8, rhs);
                float x = (num - num3) * 0.25f + (num4 - num5) * 0.5f + (num6 - num8) * 0.25f;
                float y = (num - num6) * 0.25f + (num2 - num7) * 0.5f + (num3 - num8) * 0.25f;
                Vector2 vector = new Vector2(x, y) * normalMult;
                Vector3 normalized = new Vector3(vector.x, vector.y, 1f).normalized;
                Color color = new Color(normalized.x * 0.5f + 0.5f, normalized.y * 0.5f + 0.5f, normalized.z * 0.5f + 0.5f, 1f);
                array[j + i * t.width] = color;
            }
        }
        if ((float)normalSmooth > 0f)
        {
            for (int k = 0; k < t.height; k++)
            {
                for (int l = 0; l < t.width; l++)
                {
                    float num9 = 0f;
                    Color color2 = array[l + k * t.width];
                    num9 += 1f;
                    if (l - normalSmooth > 0)
                    {
                        if (k - normalSmooth > 0)
                        {
                            color2 += array[l - normalSmooth + (k - normalSmooth) * t.width];
                            num9 += 1f;
                        }
                        color2 += array[l - normalSmooth + k * t.width];
                        num9 += 1f;
                        if (k + normalSmooth < t.height)
                        {
                            color2 += array[l - normalSmooth + (k + normalSmooth) * t.width];
                            num9 += 1f;
                        }
                    }
                    if (k - normalSmooth > 0)
                    {
                        color2 += array[l + (k - normalSmooth) * t.width];
                        num9 += 1f;
                    }
                    if (k + normalSmooth < t.height)
                    {
                        color2 += array[l + (k + normalSmooth) * t.width];
                        num9 += 1f;
                    }
                    if (l + normalSmooth < t.width)
                    {
                        if (k - normalSmooth > 0)
                        {
                            color2 += array[l + normalSmooth + (k - normalSmooth) * t.width];
                            num9 += 1f;
                        }
                        color2 += array[l + normalSmooth + k * t.width];
                        num9 += 1f;
                        if (k + normalSmooth < t.height)
                        {
                            color2 += array[l + normalSmooth + (k + normalSmooth) * t.width];
                            num9 += 1f;
                        }
                    }
                    array[l + k * t.width] = color2 / num9;
                }
            }
        }
        texture2D.SetPixels(array);
        texture2D.Apply();
        return texture2D;
    }
}
