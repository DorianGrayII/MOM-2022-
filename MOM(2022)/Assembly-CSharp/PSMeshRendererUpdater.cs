using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class PSMeshRendererUpdater : MonoBehaviour
{
    private class ParticleStartInfo
    {
        public ParticleSystem.MinMaxCurve StartSize;

        public ParticleSystem.MinMaxCurve StartSpeed;
    }

    public GameObject MeshObject;

    public float StartScaleMultiplier = 1f;

    public Color Color = Color.black;

    private const string materialName = "MeshEffect";

    private List<Material[]> rendererMaterials = new List<Material[]>();

    private List<Material[]> skinnedMaterials = new List<Material[]>();

    public bool IsActive = true;

    public float FadeTime = 1.5f;

    private bool currentActiveStatus;

    private bool needUpdateAlpha;

    private Color oldColor = Color.black;

    private float currentAlphaTime;

    private string[] colorProperties = new string[9] { "_TintColor", "_Color", "_EmissionColor", "_BorderColor", "_ReflectColor", "_RimColor", "_MainColor", "_CoreColor", "_FresnelColor" };

    private float alpha;

    private float prevAlpha;

    private Dictionary<string, float> startAlphaColors;

    private bool previousActiveStatus;

    private bool needUpdate;

    private bool needLastUpdate;

    private Dictionary<ParticleSystem, ParticleStartInfo> startParticleParameters;

    private void OnEnable()
    {
        this.alpha = 0f;
        this.prevAlpha = 0f;
        this.IsActive = true;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        if (this.startAlphaColors == null)
        {
            this.InitStartAlphaColors();
        }
        if (this.IsActive && this.alpha < 1f)
        {
            this.alpha += Time.deltaTime / this.FadeTime;
        }
        if (!this.IsActive && this.alpha > 0f)
        {
            this.alpha -= Time.deltaTime / this.FadeTime;
        }
        if (this.alpha > 0f && this.alpha < 1f)
        {
            this.needUpdate = true;
        }
        else
        {
            this.needUpdate = false;
            this.alpha = Mathf.Clamp01(this.alpha);
            if (Mathf.Abs(this.prevAlpha - this.alpha) >= Mathf.Epsilon)
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

    private void InitStartAlphaColors()
    {
        this.startAlphaColors = new Dictionary<string, float>();
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer in componentsInChildren)
        {
            Material[] materials = renderer.materials;
            for (int j = 0; j < materials.Length; j++)
            {
                if (materials[j].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(renderer.GetHashCode().ToString(), j, materials[j]);
                }
            }
        }
        SkinnedMeshRenderer[] componentsInChildren2 = base.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
        {
            Material[] materials2 = skinnedMeshRenderer.materials;
            for (int k = 0; k < materials2.Length; k++)
            {
                if (materials2[k].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(skinnedMeshRenderer.GetHashCode().ToString(), k, materials2[k]);
                }
            }
        }
        Light[] componentsInChildren3 = base.GetComponentsInChildren<Light>(includeInactive: true);
        for (int l = 0; l < componentsInChildren3.Length; l++)
        {
            ME_LightCurves component = componentsInChildren3[l].GetComponent<ME_LightCurves>();
            float value = 1f;
            if (component != null)
            {
                value = component.GraphIntensityMultiplier;
            }
            this.startAlphaColors.Add(componentsInChildren3[l].GetHashCode().ToString() + l, value);
        }
        componentsInChildren = this.MeshObject.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer2 in componentsInChildren)
        {
            Material[] materials3 = renderer2.materials;
            for (int m = 0; m < materials3.Length; m++)
            {
                if (materials3[m].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(renderer2.GetHashCode().ToString(), m, materials3[m]);
                }
            }
        }
        componentsInChildren2 = this.MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
        foreach (SkinnedMeshRenderer skinnedMeshRenderer2 in componentsInChildren2)
        {
            Material[] materials4 = skinnedMeshRenderer2.materials;
            for (int n = 0; n < materials4.Length; n++)
            {
                if (materials4[n].name.Contains("MeshEffect"))
                {
                    this.GetStartAlphaByProperties(skinnedMeshRenderer2.GetHashCode().ToString(), n, materials4[n]);
                }
            }
        }
    }

    private void InitStartParticleParameters()
    {
        this.startParticleParameters = new Dictionary<ParticleSystem, ParticleStartInfo>();
        ParticleSystem[] componentsInChildren = this.MeshObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
        foreach (ParticleSystem particleSystem in componentsInChildren)
        {
            this.startParticleParameters.Add(particleSystem, new ParticleStartInfo
            {
                StartSize = particleSystem.main.startSize,
                StartSpeed = particleSystem.main.startSpeed
            });
        }
    }

    private void UpdateVisibleStatus()
    {
        Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer in componentsInChildren)
        {
            Material[] materials = renderer.materials;
            for (int j = 0; j < materials.Length; j++)
            {
                if (materials[j].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer.GetHashCode().ToString(), j, materials[j], this.alpha);
                }
            }
        }
        componentsInChildren = base.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer2 in componentsInChildren)
        {
            Material[] materials2 = renderer2.materials;
            for (int k = 0; k < materials2.Length; k++)
            {
                if (materials2[k].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer2.GetHashCode().ToString(), k, materials2[k], this.alpha);
                }
            }
        }
        componentsInChildren = this.MeshObject.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer3 in componentsInChildren)
        {
            Material[] materials3 = renderer3.materials;
            for (int l = 0; l < materials3.Length; l++)
            {
                if (materials3[l].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer3.GetHashCode().ToString(), l, materials3[l], this.alpha);
                }
            }
        }
        componentsInChildren = this.MeshObject.GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (Renderer renderer4 in componentsInChildren)
        {
            Material[] materials4 = renderer4.materials;
            for (int m = 0; m < materials4.Length; m++)
            {
                if (materials4[m].name.Contains("MeshEffect"))
                {
                    this.UpdateAlphaByProperties(renderer4.GetHashCode().ToString(), m, materials4[m], this.alpha);
                }
            }
        }
        ME_LightCurves[] componentsInChildren2 = base.GetComponentsInChildren<ME_LightCurves>(includeInactive: true);
        for (int i = 0; i < componentsInChildren2.Length; i++)
        {
            componentsInChildren2[i].enabled = this.IsActive;
        }
        Light[] componentsInChildren3 = base.GetComponentsInChildren<Light>(includeInactive: true);
        for (int n = 0; n < componentsInChildren3.Length; n++)
        {
            if (!this.IsActive)
            {
                float num = this.startAlphaColors[componentsInChildren3[n].GetHashCode().ToString() + n];
                componentsInChildren3[n].intensity = this.alpha * num;
            }
        }
        ParticleSystem[] componentsInChildren4 = base.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
        foreach (ParticleSystem particleSystem in componentsInChildren4)
        {
            if (!this.IsActive && !particleSystem.isStopped)
            {
                particleSystem.Stop();
            }
            if (this.IsActive && particleSystem.isStopped)
            {
                particleSystem.Play();
            }
        }
        ME_TrailRendererNoise[] componentsInChildren5 = base.GetComponentsInChildren<ME_TrailRendererNoise>();
        for (int i = 0; i < componentsInChildren5.Length; i++)
        {
            componentsInChildren5[i].IsActive = this.IsActive;
        }
    }

    private void UpdateAlphaByProperties(string rendName, int materialNumber, Material mat, float alpha)
    {
        string[] array = this.colorProperties;
        foreach (string text in array)
        {
            if (mat.HasProperty(text))
            {
                float num = this.startAlphaColors[rendName + materialNumber + text.ToString()];
                Color color = mat.GetColor(text);
                color.a = alpha * num;
                mat.SetColor(text, color);
            }
        }
    }

    private void GetStartAlphaByProperties(string rendName, int materialNumber, Material mat)
    {
        string[] array = this.colorProperties;
        foreach (string text in array)
        {
            if (mat.HasProperty(text))
            {
                string key = rendName + materialNumber + text.ToString();
                if (!this.startAlphaColors.ContainsKey(key))
                {
                    this.startAlphaColors.Add(rendName + materialNumber + text.ToString(), mat.GetColor(text).a);
                }
            }
        }
    }

    public void UpdateColor(Color color)
    {
        if (!(this.MeshObject == null))
        {
            ME_ColorHelper.HSBColor hSBColor = ME_ColorHelper.ColorToHSV(color);
            ME_ColorHelper.ChangeObjectColorByHUE(this.MeshObject, hSBColor.H);
        }
    }

    public void UpdateColor(float HUE)
    {
        if (!(this.MeshObject == null))
        {
            ME_ColorHelper.ChangeObjectColorByHUE(this.MeshObject, HUE);
        }
    }

    public void UpdateMeshEffect()
    {
        base.transform.localPosition = Vector3.zero;
        base.transform.localRotation = default(Quaternion);
        this.rendererMaterials.Clear();
        this.skinnedMaterials.Clear();
        if (!(this.MeshObject == null))
        {
            this.UpdatePSMesh(this.MeshObject);
            this.AddMaterialToMesh(this.MeshObject);
        }
    }

    private void CheckScaleIncludedParticles()
    {
    }

    public void UpdateMeshEffect(GameObject go)
    {
        this.rendererMaterials.Clear();
        this.skinnedMaterials.Clear();
        if (go == null)
        {
            Debug.Log("You need set a gameObject");
            return;
        }
        this.MeshObject = go;
        this.UpdatePSMesh(this.MeshObject);
        this.AddMaterialToMesh(this.MeshObject);
    }

    private void UpdatePSMesh(GameObject go)
    {
        if (this.startParticleParameters == null)
        {
            this.InitStartParticleParameters();
        }
        ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
        MeshRenderer componentInChildren = go.GetComponentInChildren<MeshRenderer>();
        SkinnedMeshRenderer componentInChildren2 = go.GetComponentInChildren<SkinnedMeshRenderer>();
        Light[] componentsInChildren2 = base.GetComponentsInChildren<Light>();
        float num = 1f;
        float num2 = 1f;
        if (componentInChildren != null)
        {
            num = componentInChildren.bounds.size.magnitude;
            num2 = componentInChildren.transform.lossyScale.magnitude;
        }
        if (componentInChildren2 != null)
        {
            num = componentInChildren2.bounds.size.magnitude;
            num2 = componentInChildren2.transform.lossyScale.magnitude;
        }
        ParticleSystem[] array = componentsInChildren;
        foreach (ParticleSystem particleSystem in array)
        {
            particleSystem.transform.gameObject.SetActive(value: false);
            ParticleSystem.ShapeModule shape = particleSystem.shape;
            if (shape.enabled)
            {
                if (componentInChildren != null)
                {
                    shape.shapeType = ParticleSystemShapeType.MeshRenderer;
                    shape.meshRenderer = componentInChildren;
                }
                if (componentInChildren2 != null)
                {
                    shape.shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
                    shape.skinnedMeshRenderer = componentInChildren2;
                }
            }
            ParticleSystem.MainModule main = particleSystem.main;
            ParticleStartInfo particleStartInfo = this.startParticleParameters[particleSystem];
            main.startSize = this.UpdateParticleParam(particleStartInfo.StartSize, main.startSize, num / num2 * this.StartScaleMultiplier);
            main.startSpeed = this.UpdateParticleParam(particleStartInfo.StartSpeed, main.startSpeed, num / num2 * this.StartScaleMultiplier);
            particleSystem.transform.gameObject.SetActive(value: true);
        }
        if (componentInChildren != null)
        {
            Light[] array2 = componentsInChildren2;
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i].transform.position = componentInChildren.bounds.center;
            }
        }
        if (componentInChildren2 != null)
        {
            Light[] array2 = componentsInChildren2;
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i].transform.position = componentInChildren2.bounds.center;
            }
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

    private void AddMaterialToMesh(GameObject go)
    {
        ME_MeshMaterialEffect componentInChildren = base.GetComponentInChildren<ME_MeshMaterialEffect>();
        if (!(componentInChildren == null))
        {
            MeshRenderer componentInChildren2 = go.GetComponentInChildren<MeshRenderer>();
            SkinnedMeshRenderer componentInChildren3 = go.GetComponentInChildren<SkinnedMeshRenderer>();
            if (componentInChildren2 != null)
            {
                this.rendererMaterials.Add(componentInChildren2.sharedMaterials);
                componentInChildren2.sharedMaterials = this.AddToSharedMaterial(componentInChildren2.sharedMaterials, componentInChildren);
            }
            if (componentInChildren3 != null)
            {
                this.skinnedMaterials.Add(componentInChildren3.sharedMaterials);
                componentInChildren3.sharedMaterials = this.AddToSharedMaterial(componentInChildren3.sharedMaterials, componentInChildren);
            }
        }
    }

    private Material[] AddToSharedMaterial(Material[] sharedMaterials, ME_MeshMaterialEffect meshMatEffect)
    {
        if (meshMatEffect.IsFirstMaterial)
        {
            return new Material[1] { meshMatEffect.Material };
        }
        List<Material> list = sharedMaterials.ToList();
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

    private void OnDestroy()
    {
        if (this.MeshObject == null)
        {
            return;
        }
        MeshRenderer[] componentsInChildren = this.MeshObject.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] componentsInChildren2 = this.MeshObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (this.rendererMaterials.Count == componentsInChildren.Length)
            {
                componentsInChildren[i].sharedMaterials = this.rendererMaterials[i];
            }
            List<Material> list = componentsInChildren[i].sharedMaterials.ToList();
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].name.Contains("MeshEffect"))
                {
                    list.RemoveAt(j);
                }
            }
            componentsInChildren[i].sharedMaterials = list.ToArray();
        }
        for (int k = 0; k < componentsInChildren2.Length; k++)
        {
            if (this.skinnedMaterials.Count == componentsInChildren2.Length)
            {
                componentsInChildren2[k].sharedMaterials = this.skinnedMaterials[k];
            }
            List<Material> list2 = componentsInChildren2[k].sharedMaterials.ToList();
            for (int l = 0; l < list2.Count; l++)
            {
                if (list2[l].name.Contains("MeshEffect"))
                {
                    list2.RemoveAt(l);
                }
            }
            componentsInChildren2[k].sharedMaterials = list2.ToArray();
        }
        this.rendererMaterials.Clear();
        this.skinnedMaterials.Clear();
    }
}
