using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PSMeshRendererUpdater : MonoBehaviour
{
    public GameObject MeshObject;
    public float StartScaleMultiplier = 1f;
    public UnityEngine.Color Color = UnityEngine.Color.black;
    private const string materialName = "MeshEffect";
    private List<Material[]> rendererMaterials = new List<Material[]>();
    private List<Material[]> skinnedMaterials = new List<Material[]>();
    public bool IsActive = true;
    public float FadeTime = 1.5f;
    private bool currentActiveStatus;
    private bool needUpdateAlpha;
    private UnityEngine.Color oldColor = UnityEngine.Color.black;
    private float currentAlphaTime;
    private string[] colorProperties;
    private float alpha;
    private float prevAlpha;
    private Dictionary<string, float> startAlphaColors;
    private bool previousActiveStatus;
    private bool needUpdate;
    private bool needLastUpdate;
    private Dictionary<ParticleSystem, ParticleStartInfo> startParticleParameters;

    public PSMeshRendererUpdater()
    {
        string[] textArray1 = new string[9];
        textArray1[0] = "_TintColor";
        textArray1[1] = "_Color";
        textArray1[2] = "_EmissionColor";
        textArray1[3] = "_BorderColor";
        textArray1[4] = "_ReflectColor";
        textArray1[5] = "_RimColor";
        textArray1[6] = "_MainColor";
        textArray1[7] = "_CoreColor";
        textArray1[8] = "_FresnelColor";
        this.colorProperties = textArray1;
    }

    private void AddMaterialToMesh(GameObject go)
    {
        ME_MeshMaterialEffect componentInChildren = base.GetComponentInChildren<ME_MeshMaterialEffect>();
        if (componentInChildren != null)
        {
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
            SkinnedMeshRenderer renderer2 = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                this.rendererMaterials.Add(renderer.sharedMaterials);
                renderer.sharedMaterials = this.AddToSharedMaterial(renderer.sharedMaterials, componentInChildren);
            }
            if (renderer2 != null)
            {
                this.skinnedMaterials.Add(renderer2.sharedMaterials);
                renderer2.sharedMaterials = this.AddToSharedMaterial(renderer2.sharedMaterials, componentInChildren);
            }
        }
    }

    private Material[] AddToSharedMaterial(Material[] sharedMaterials, ME_MeshMaterialEffect meshMatEffect)
    {
        if (meshMatEffect.IsFirstMaterial)
        {
            return new Material[] { meshMatEffect.Material };
        }
        List<Material> list = Enumerable.ToList<Material>(sharedMaterials);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].name.Contains("MeshEffect"))
            {
                list.RemoveAt(i);
            }
        }
        list.Add(meshMatEffect.Material);
        return list.ToArray();
    }

    private void CheckScaleIncludedParticles()
    {
    }

    private void GetStartAlphaByProperties(string rendName, int materialNumber, Material mat)
    {
        foreach (string str in this.colorProperties)
        {
            if (mat.HasProperty(str))
            {
                string key = rendName + materialNumber.ToString() + str.ToString();
                if (!this.startAlphaColors.ContainsKey(key))
                {
                    this.startAlphaColors.Add(rendName + materialNumber.ToString() + str.ToString(), mat.GetColor(str).a);
                }
            }
        }
    }

    private void InitStartAlphaColors()
    {
        this.startAlphaColors = new Dictionary<string, float>();
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
        int index = 0;
        while (index < componentsInChildren.Length)
        {
            Renderer renderer = componentsInChildren[index];
            Material[] materials = renderer.materials;
            int num2 = 0;
            while (true)
            {
                if (num2 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num2].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(renderer.GetHashCode().ToString(), num2, materials[num2]);
                }
                num2++;
            }
        }
        SkinnedMeshRenderer[] rendererArray2 = base.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        index = 0;
        while (index < rendererArray2.Length)
        {
            SkinnedMeshRenderer renderer2 = rendererArray2[index];
            Material[] materials = renderer2.materials;
            int num4 = 0;
            while (true)
            {
                if (num4 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num4].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(renderer2.GetHashCode().ToString(), num4, materials[num4]);
                }
                num4++;
            }
        }
        Light[] lightArray = base.GetComponentsInChildren<Light>(true);
        for (int i = 0; i < lightArray.Length; i++)
        {
            ME_LightCurves component = lightArray[i].GetComponent<ME_LightCurves>();
            float graphIntensityMultiplier = 1f;
            if (component != null)
            {
                graphIntensityMultiplier = component.GraphIntensityMultiplier;
            }
            this.startAlphaColors.Add(lightArray[i].GetHashCode().ToString() + i.ToString(), graphIntensityMultiplier);
        }
        componentsInChildren = this.MeshObject.GetComponentsInChildren<Renderer>(true);
        index = 0;
        while (index < componentsInChildren.Length)
        {
            Renderer renderer3 = componentsInChildren[index];
            Material[] materials = renderer3.materials;
            int num7 = 0;
            while (true)
            {
                if (num7 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num7].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(renderer3.GetHashCode().ToString(), num7, materials[num7]);
                }
                num7++;
            }
        }
        rendererArray2 = this.MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        index = 0;
        while (index < rendererArray2.Length)
        {
            SkinnedMeshRenderer renderer4 = rendererArray2[index];
            Material[] materials = renderer4.materials;
            int num8 = 0;
            while (true)
            {
                if (num8 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num8].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(renderer4.GetHashCode().ToString(), num8, materials[num8]);
                }
                num8++;
            }
        }
    }

    private void InitStartParticleParameters()
    {
        this.startParticleParameters = new Dictionary<ParticleSystem, ParticleStartInfo>();
        foreach (ParticleSystem system in this.MeshObject.GetComponentsInChildren<ParticleSystem>(true))
        {
            ParticleStartInfo info1 = new ParticleStartInfo();
            info1.StartSize = system.main.startSize;
            ParticleSystem.MainModule main = system.main;
            info1.StartSpeed = main.startSpeed;
            this.startParticleParameters.Add(system, info1);
        }
    }

    private void OnDestroy()
    {
        if (this.MeshObject != null)
        {
            MeshRenderer[] componentsInChildren = this.MeshObject.GetComponentsInChildren<MeshRenderer>();
            SkinnedMeshRenderer[] rendererArray2 = this.MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            int index = 0;
            while (index < componentsInChildren.Length)
            {
                if (this.rendererMaterials.Count == componentsInChildren.Length)
                {
                    componentsInChildren[index].sharedMaterials = this.rendererMaterials[index];
                }
                List<Material> list = Enumerable.ToList<Material>(componentsInChildren[index].sharedMaterials);
                int num2 = 0;
                while (true)
                {
                    if (num2 >= list.Count)
                    {
                        componentsInChildren[index].sharedMaterials = list.ToArray();
                        index++;
                        break;
                    }
                    if (list[num2].name.Contains("MeshEffect"))
                    {
                        list.RemoveAt(num2);
                    }
                    num2++;
                }
            }
            int num3 = 0;
            while (num3 < rendererArray2.Length)
            {
                if (this.skinnedMaterials.Count == rendererArray2.Length)
                {
                    rendererArray2[num3].sharedMaterials = this.skinnedMaterials[num3];
                }
                List<Material> list2 = Enumerable.ToList<Material>(rendererArray2[num3].sharedMaterials);
                int num4 = 0;
                while (true)
                {
                    if (num4 >= list2.Count)
                    {
                        rendererArray2[num3].sharedMaterials = list2.ToArray();
                        num3++;
                        break;
                    }
                    if (list2[num4].name.Contains("MeshEffect"))
                    {
                        list2.RemoveAt(num4);
                    }
                    num4++;
                }
            }
            this.rendererMaterials.Clear();
            this.skinnedMaterials.Clear();
        }
    }

    private void OnEnable()
    {
        this.alpha = 0f;
        this.prevAlpha = 0f;
        this.IsActive = true;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (this.startAlphaColors == null)
            {
                this.InitStartAlphaColors();
            }
            if (this.IsActive && (this.alpha < 1f))
            {
                this.alpha += Time.deltaTime / this.FadeTime;
            }
            if (!this.IsActive && (this.alpha > 0f))
            {
                this.alpha -= Time.deltaTime / this.FadeTime;
            }
            if ((this.alpha > 0f) && (this.alpha < 1f))
            {
                this.needUpdate = true;
            }
            else
            {
                this.needUpdate = false;
                this.alpha = Mathf.Clamp01(this.alpha);
                if (Mathf.Abs((float) (this.prevAlpha - this.alpha)) >= Mathf.Epsilon)
                {
                    this.UpdateVisibleStatus();
                }
            }
            this.prevAlpha = this.alpha;
            if (this.needUpdate)
            {
                this.UpdateVisibleStatus();
            }
            if (this.Color != this.oldColor)
            {
                this.oldColor = this.Color;
                this.UpdateColor(this.Color);
            }
        }
    }

    private void UpdateAlphaByProperties(string rendName, int materialNumber, Material mat, float alpha)
    {
        foreach (string str in this.colorProperties)
        {
            if (mat.HasProperty(str))
            {
                float num2 = this.startAlphaColors[rendName + materialNumber.ToString() + str.ToString()];
                UnityEngine.Color color = mat.GetColor(str);
                color.a = alpha * num2;
                mat.SetColor(str, color);
            }
        }
    }

    public void UpdateColor(float HUE)
    {
        if (this.MeshObject != null)
        {
            ME_ColorHelper.ChangeObjectColorByHUE(this.MeshObject, HUE);
        }
    }

    public void UpdateColor(UnityEngine.Color color)
    {
        if (this.MeshObject != null)
        {
            ME_ColorHelper.HSBColor color2 = ME_ColorHelper.ColorToHSV(color);
            ME_ColorHelper.ChangeObjectColorByHUE(this.MeshObject, color2.H);
        }
    }

    public void UpdateMeshEffect()
    {
        base.transform.localPosition = Vector3.zero;
        Quaternion quaternion = new Quaternion();
        base.transform.localRotation = quaternion;
        this.rendererMaterials.Clear();
        this.skinnedMaterials.Clear();
        if (this.MeshObject != null)
        {
            this.UpdatePSMesh(this.MeshObject);
            this.AddMaterialToMesh(this.MeshObject);
        }
    }

    public void UpdateMeshEffect(GameObject go)
    {
        this.rendererMaterials.Clear();
        this.skinnedMaterials.Clear();
        if (go == null)
        {
            Debug.Log("You need set a gameObject");
        }
        else
        {
            this.MeshObject = go;
            this.UpdatePSMesh(this.MeshObject);
            this.AddMaterialToMesh(this.MeshObject);
        }
    }

    private ParticleSystem.MinMaxCurve UpdateParticleParam(ParticleSystem.MinMaxCurve startParam, ParticleSystem.MinMaxCurve currentParam, float scale)
    {
        if (currentParam.mode == ParticleSystemCurveMode.TwoConstants)
        {
            currentParam.constantMin = startParam.constantMin * scale;
            currentParam.constantMax = startParam.constantMax * scale;
        }
        else if (currentParam.mode == ParticleSystemCurveMode.Constant)
        {
            currentParam.constant = startParam.constant * scale;
        }
        return currentParam;
    }

    private void UpdatePSMesh(GameObject go)
    {
        Bounds bounds;
        Light[] lightArray2;
        if (this.startParticleParameters == null)
        {
            this.InitStartParticleParameters();
        }
        MeshRenderer componentInChildren = go.GetComponentInChildren<MeshRenderer>();
        SkinnedMeshRenderer renderer2 = go.GetComponentInChildren<SkinnedMeshRenderer>();
        Light[] componentsInChildren = base.GetComponentsInChildren<Light>();
        float magnitude = 1f;
        float magnitude = 1f;
        if (componentInChildren != null)
        {
            magnitude = componentInChildren.bounds.size.magnitude;
            magnitude = componentInChildren.transform.lossyScale.magnitude;
        }
        if (renderer2 != null)
        {
            magnitude = renderer2.bounds.size.magnitude;
            magnitude = renderer2.transform.lossyScale.magnitude;
        }
        ParticleSystem[] systemArray = base.GetComponentsInChildren<ParticleSystem>();
        int index = 0;
        while (index < systemArray.Length)
        {
            ParticleSystem system = systemArray[index];
            system.transform.gameObject.SetActive(false);
            ParticleSystem.ShapeModule shape = system.shape;
            if (shape.enabled)
            {
                if (componentInChildren != null)
                {
                    shape.shapeType = ParticleSystemShapeType.MeshRenderer;
                    shape.meshRenderer = componentInChildren;
                }
                if (renderer2 != null)
                {
                    shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                    shape.skinnedMeshRenderer = renderer2;
                }
            }
            ParticleSystem.MainModule main = system.main;
            ParticleStartInfo info = this.startParticleParameters[system];
            main.startSize = this.UpdateParticleParam(info.StartSize, main.startSize, (magnitude / magnitude) * this.StartScaleMultiplier);
            main.startSpeed = this.UpdateParticleParam(info.StartSpeed, main.startSpeed, (magnitude / magnitude) * this.StartScaleMultiplier);
            system.transform.gameObject.SetActive(true);
            index++;
        }
        if (componentInChildren != null)
        {
            lightArray2 = componentsInChildren;
            index = 0;
            while (index < lightArray2.Length)
            {
                bounds = componentInChildren.bounds;
                lightArray2[index].transform.position = bounds.center;
                index++;
            }
        }
        if (renderer2 != null)
        {
            lightArray2 = componentsInChildren;
            for (index = 0; index < lightArray2.Length; index++)
            {
                bounds = renderer2.bounds;
                lightArray2[index].transform.position = bounds.center;
            }
        }
    }

    private void UpdateVisibleStatus()
    {
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
        int index = 0;
        while (index < componentsInChildren.Length)
        {
            Renderer renderer = componentsInChildren[index];
            Material[] materials = renderer.materials;
            int num2 = 0;
            while (true)
            {
                if (num2 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num2].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer.GetHashCode().ToString(), num2, materials[num2], this.alpha);
                }
                num2++;
            }
        }
        componentsInChildren = base.GetComponentsInChildren<Renderer>(true);
        index = 0;
        while (index < componentsInChildren.Length)
        {
            Renderer renderer2 = componentsInChildren[index];
            Material[] materials = renderer2.materials;
            int num4 = 0;
            while (true)
            {
                if (num4 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num4].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer2.GetHashCode().ToString(), num4, materials[num4], this.alpha);
                }
                num4++;
            }
        }
        componentsInChildren = this.MeshObject.GetComponentsInChildren<Renderer>(true);
        index = 0;
        while (index < componentsInChildren.Length)
        {
            Renderer renderer3 = componentsInChildren[index];
            Material[] materials = renderer3.materials;
            int num5 = 0;
            while (true)
            {
                if (num5 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num5].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer3.GetHashCode().ToString(), num5, materials[num5], this.alpha);
                }
                num5++;
            }
        }
        componentsInChildren = this.MeshObject.GetComponentsInChildren<Renderer>(true);
        index = 0;
        while (index < componentsInChildren.Length)
        {
            Renderer renderer4 = componentsInChildren[index];
            Material[] materials = renderer4.materials;
            int num6 = 0;
            while (true)
            {
                if (num6 >= materials.Length)
                {
                    index++;
                    break;
                }
                if (materials[num6].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer4.GetHashCode().ToString(), num6, materials[num6], this.alpha);
                }
                num6++;
            }
        }
        ME_LightCurves[] curvesArray = base.GetComponentsInChildren<ME_LightCurves>(true);
        for (index = 0; index < curvesArray.Length; index++)
        {
            curvesArray[index].enabled = this.IsActive;
        }
        Light[] lightArray = base.GetComponentsInChildren<Light>(true);
        for (int i = 0; i < lightArray.Length; i++)
        {
            if (!this.IsActive)
            {
                float num8 = this.startAlphaColors[lightArray[i].GetHashCode().ToString() + i.ToString()];
                lightArray[i].intensity = this.alpha * num8;
            }
        }
        ParticleSystem[] systemArray = base.GetComponentsInChildren<ParticleSystem>(true);
        for (index = 0; index < systemArray.Length; index++)
        {
            ParticleSystem system = systemArray[index];
            if (!this.IsActive && !system.isStopped)
            {
                system.Stop();
            }
            if (this.IsActive && system.isStopped)
            {
                system.Play();
            }
        }
        ME_TrailRendererNoise[] noiseArray = base.GetComponentsInChildren<ME_TrailRendererNoise>();
        for (index = 0; index < noiseArray.Length; index++)
        {
            noiseArray[index].IsActive = this.IsActive;
        }
    }

    private class ParticleStartInfo
    {
        public ParticleSystem.MinMaxCurve StartSize;
        public ParticleSystem.MinMaxCurve StartSpeed;
    }
}

