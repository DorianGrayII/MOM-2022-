using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ME_ColorHelper
{
    private const float TOLERANCE = 0.0001f;
    private static string[] colorProperties;

    static ME_ColorHelper()
    {
        string[] textArray1 = new string[10];
        textArray1[0] = "_TintColor";
        textArray1[1] = "_Color";
        textArray1[2] = "_EmissionColor";
        textArray1[3] = "_BorderColor";
        textArray1[4] = "_ReflectColor";
        textArray1[5] = "_RimColor";
        textArray1[6] = "_MainColor";
        textArray1[7] = "_CoreColor";
        textArray1[8] = "_FresnelColor";
        textArray1[9] = "_CutoutColor";
        colorProperties = textArray1;
    }

    public static void ChangeObjectColorByHUE(GameObject go, float hue)
    {
        int num;
        string[] colorProperties;
        int num2;
        Material[] materialArray2;
        int num3;
        Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
        for (num = 0; num < componentsInChildren.Length; num++)
        {
            Renderer renderer = componentsInChildren[num];
            Material[] materialArray = Application.isPlaying ? renderer.materials : renderer.sharedMaterials;
            if (materialArray.Length != 0)
            {
                colorProperties = ME_ColorHelper.colorProperties;
                num2 = 0;
                while (num2 < colorProperties.Length)
                {
                    string name = colorProperties[num2];
                    materialArray2 = materialArray;
                    num3 = 0;
                    while (true)
                    {
                        if (num3 >= materialArray2.Length)
                        {
                            num2++;
                            break;
                        }
                        Material mat = materialArray2[num3];
                        if ((mat != null) && mat.HasProperty(name))
                        {
                            setMatHUEColor(mat, name, hue);
                        }
                        num3++;
                    }
                }
            }
        }
        ParticleSystemRenderer[] rendererArray2 = go.GetComponentsInChildren<ParticleSystemRenderer>(true);
        for (num = 0; num < rendererArray2.Length; num++)
        {
            ParticleSystemRenderer renderer2 = rendererArray2[num];
            Material trailMaterial = renderer2.trailMaterial;
            if (trailMaterial != null)
            {
                Material material1 = new Material(trailMaterial);
                material1.name = trailMaterial.name + " (Instance)";
                trailMaterial = material1;
                renderer2.trailMaterial = trailMaterial;
                colorProperties = ME_ColorHelper.colorProperties;
                num2 = 0;
                while (num2 < colorProperties.Length)
                {
                    string name = colorProperties[num2];
                    if ((trailMaterial != null) && trailMaterial.HasProperty(name))
                    {
                        setMatHUEColor(trailMaterial, name, hue);
                    }
                    num2++;
                }
            }
        }
        SkinnedMeshRenderer[] rendererArray3 = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (num = 0; num < rendererArray3.Length; num++)
        {
            SkinnedMeshRenderer renderer3 = rendererArray3[num];
            Material[] materialArray3 = Application.isPlaying ? renderer3.materials : renderer3.sharedMaterials;
            if (materialArray3.Length != 0)
            {
                colorProperties = ME_ColorHelper.colorProperties;
                num2 = 0;
                while (num2 < colorProperties.Length)
                {
                    string name = colorProperties[num2];
                    materialArray2 = materialArray3;
                    num3 = 0;
                    while (true)
                    {
                        if (num3 >= materialArray2.Length)
                        {
                            num2++;
                            break;
                        }
                        Material mat = materialArray2[num3];
                        if ((mat != null) && mat.HasProperty(name))
                        {
                            setMatHUEColor(mat, name, hue);
                        }
                        num3++;
                    }
                }
            }
        }
        Projector[] projectorArray = go.GetComponentsInChildren<Projector>(true);
        for (num = 0; num < projectorArray.Length; num++)
        {
            Projector projector = projectorArray[num];
            if (!projector.material.name.EndsWith("(Instance)"))
            {
                Material material5 = new Material(projector.material);
                material5.name = projector.material.name + " (Instance)";
                projector.material = material5;
            }
            Material material = projector.material;
            if (material != null)
            {
                foreach (string str4 in ME_ColorHelper.colorProperties)
                {
                    if ((material != null) && material.HasProperty(str4))
                    {
                        projector.material = setMatHUEColor(material, str4, hue);
                    }
                }
            }
        }
        Light[] lightArray = go.GetComponentsInChildren<Light>(true);
        for (num = 0; num < lightArray.Length; num++)
        {
            Light light1 = lightArray[num];
            HSBColor hsbColor = ColorToHSV(light1.color);
            hsbColor.H = hue;
            light1.color = HSVToColor(hsbColor);
        }
        ParticleSystem[] systemArray = go.GetComponentsInChildren<ParticleSystem>(true);
        num = 0;
        while (num < systemArray.Length)
        {
            ParticleSystem system1 = systemArray[num];
            ParticleSystem.MainModule main = system1.main;
            ParticleSystem.MainModule module3 = system1.main;
            HSBColor hsbColor = ColorToHSV(module3.startColor.color);
            hsbColor.H = hue;
            main.startColor = HSVToColor(hsbColor);
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = system1.colorOverLifetime;
            ParticleSystem.MinMaxGradient color = colorOverLifetime.color;
            Gradient gradient = colorOverLifetime.color.gradient;
            ParticleSystem.MinMaxGradient gradient3 = colorOverLifetime.color;
            GradientColorKey[] colorKeys = gradient3.gradient.colorKeys;
            float num4 = 0f;
            hsbColor = ColorToHSV(colorKeys[0].color);
            num4 = Math.Abs((float) (ColorToHSV(colorKeys[1].color).H - hsbColor.H));
            hsbColor.H = hue;
            colorKeys[0].color = HSVToColor(hsbColor);
            int index = 1;
            while (true)
            {
                if (index >= colorKeys.Length)
                {
                    gradient.colorKeys = colorKeys;
                    color.gradient = gradient;
                    colorOverLifetime.color = color;
                    num++;
                    break;
                }
                hsbColor = ColorToHSV(colorKeys[index].color);
                hsbColor.H = Mathf.Repeat(hsbColor.H + num4, 1f);
                colorKeys[index].color = HSVToColor(hsbColor);
                index++;
            }
        }
    }

    public static unsafe HSBColor ColorToHSV(Color color)
    {
        HSBColor color2 = new HSBColor(0f, 0f, 0f, color.a);
        float r = color.r;
        float g = color.g;
        float b = color.b;
        float num4 = Mathf.Max(r, Mathf.Max(g, b));
        if (num4 > 0f)
        {
            float num5 = Mathf.Min(r, Mathf.Min(g, b));
            float num6 = num4 - num5;
            if (num4 <= num5)
            {
                color2.H = 0f;
            }
            else
            {
                color2.H = (Math.Abs((float) (g - num4)) >= 0.0001f) ? ((Math.Abs((float) (b - num4)) >= 0.0001f) ? ((b <= g) ? (((g - b) / num6) * 60f) : ((((g - b) / num6) * 60f) + 360f)) : ((((r - g) / num6) * 60f) + 240f)) : ((((b - r) / num6) * 60f) + 120f);
                if (color2.H < 0f)
                {
                    color2.H += 360f;
                }
            }
            float* singlePtr1 = &color2.H;
            singlePtr1[0] *= 0.002777778f;
            color2.S = (num6 / num4) * 1f;
            color2.B = num4;
        }
        return color2;
    }

    public static Color ConvertRGBColorByHUE(Color rgbColor, float hue)
    {
        float b = ColorToHSV(rgbColor).B;
        if (b < 0.0001f)
        {
            b = 0.0001f;
        }
        HSBColor hsbColor = ColorToHSV(rgbColor / b);
        hsbColor.H = hue;
        Color color2 = HSVToColor(hsbColor) * b;
        color2.a = rgbColor.a;
        return color2;
    }

    public static Color HSVToColor(HSBColor hsbColor)
    {
        float b = hsbColor.B;
        float num2 = hsbColor.B;
        float num3 = hsbColor.B;
        if (Math.Abs(hsbColor.S) > 0.0001f)
        {
            float num4 = hsbColor.B;
            float num5 = hsbColor.B * hsbColor.S;
            float num6 = hsbColor.B - num5;
            float num7 = hsbColor.H * 360f;
            if (num7 < 60f)
            {
                b = num4;
                num2 = ((num7 * num5) / 60f) + num6;
                num3 = num6;
            }
            else if (num7 < 120f)
            {
                b = ((-(num7 - 120f) * num5) / 60f) + num6;
                num2 = num4;
                num3 = num6;
            }
            else if (num7 < 180f)
            {
                b = num6;
                num2 = num4;
                num3 = (((num7 - 120f) * num5) / 60f) + num6;
            }
            else if (num7 < 240f)
            {
                b = num6;
                num2 = ((-(num7 - 240f) * num5) / 60f) + num6;
                num3 = num4;
            }
            else if (num7 < 300f)
            {
                b = (((num7 - 240f) * num5) / 60f) + num6;
                num2 = num6;
                num3 = num4;
            }
            else if (num7 <= 360f)
            {
                b = num4;
                num2 = num6;
                num3 = ((-(num7 - 360f) * num5) / 60f) + num6;
            }
            else
            {
                b = 0f;
                num2 = 0f;
                num3 = 0f;
            }
        }
        return new Color(Mathf.Clamp01(b), Mathf.Clamp01(num2), Mathf.Clamp01(num3), hsbColor.A);
    }

    private static Material setMatAlphaColor(Material mat, string name, float alpha)
    {
        Color color = mat.GetColor(name);
        color.a = alpha;
        mat.SetColor(name, color);
        return mat;
    }

    private static Material setMatHUEColor(Material mat, string name, float hueColor)
    {
        Color color = ConvertRGBColorByHUE(mat.GetColor(name), hueColor);
        mat.SetColor(name, color);
        return mat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HSBColor
    {
        public float H;
        public float S;
        public float B;
        public float A;
        public HSBColor(float h, float s, float b, float a)
        {
            this.H = h;
            this.S = s;
            this.B = b;
            this.A = a;
        }
    }
}

