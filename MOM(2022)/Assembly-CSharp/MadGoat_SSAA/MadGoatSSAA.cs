// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MadGoat_SSAA.MadGoatSSAA
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MadGoat_SSAA;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class MadGoatSSAA : MonoBehaviour
{
    public Mode renderMode;

    public float multiplier = 1f;

    public float multiplierVertical = 1f;

    public bool fssaaAlpha;

    public SsaaProfile SSAA_X2 = new SsaaProfile(1.5f, useDownsampling: true, Filter.BILINEAR, 0.8f, 0.5f);

    public SsaaProfile SSAA_X4 = new SsaaProfile(2f, useDownsampling: true, Filter.BICUBIC, 0.725f, 0.95f);

    public SsaaProfile SSAA_HALF = new SsaaProfile(0.5f, useDownsampling: true, Filter.NEAREST_NEIGHBOR, 0f, 0f);

    public SSAAMode ssaaMode;

    public bool ssaaUltra;

    [Range(0f, 1f)]
    public float fssaaIntensity = 1f;

    public RenderTextureFormat textureFormat = RenderTextureFormat.ARGBHalf;

    public bool useShader = true;

    public Filter filterType = Filter.BILINEAR;

    public float sharpness = 0.8f;

    public float sampleDistance = 1f;

    public bool useVsyncTarget;

    public int targetFramerate = 60;

    public float minMultiplier = 0.5f;

    public float maxMultiplier = 1.5f;

    public string screenshotPath = "Assets/SuperSampledSceenshots/";

    public string namePrefix = "SSAA";

    public bool useProductName;

    public ImageFormat imageFormat;

    [Range(0f, 100f)]
    public int JPGQuality = 90;

    public bool EXR32;

    private Shader _FXAA_FSS;

    private Material _FXAA_FSS_Mat;

    private CommandBuffer fssCb;

    private RenderTexture fxaaFlip;

    private RenderTexture fxaaFlop;

    private Shader _grabAlpha;

    private Material _grabAlphaMat;

    private CommandBuffer grabAlphaCB;

    private CommandBuffer pasteAlphaCB;

    public RenderTexture grabAlphaRT;

    public RenderTexture pasteAlphaRT;

    [SerializeField]
    protected Camera currentCamera;

    protected Camera renderCamera;

    protected GameObject renderCameraObject;

    protected MadGoatSSAA_InternalRenderer SSAA_Internal;

    private Rect tempRect;

    private Texture2D _sphereTemp;

    protected FramerateSampler FpsData = new FramerateSampler();

    public DebugData dbgData;

    public bool mouseCompatibilityMode;

    public bool exposeInternalRender;

    public bool flipImageFix;

    public RenderTexture targetTexture;

    public GameObject madGoatDebugger;

    public ScreenshotSettings screenshotSettings = new ScreenshotSettings();

    public PanoramaSettings panoramaSettings = new PanoramaSettings(1024, 1);

    protected Shader FXAA_FSS
    {
        get
        {
            if (this._FXAA_FSS == null)
            {
                this._FXAA_FSS = Shader.Find("Hidden/SSAA/FSS");
            }
            return this._FXAA_FSS;
        }
    }

    protected Material FXAA_FSS_Mat
    {
        get
        {
            if (this._FXAA_FSS_Mat == null)
            {
                this._FXAA_FSS_Mat = new Material(this.FXAA_FSS);
            }
            return this._FXAA_FSS_Mat;
        }
    }

    protected CommandBuffer FssCb
    {
        get
        {
            return this.fssCb;
        }
        set
        {
            this.fssCb = value;
        }
    }

    protected RenderTexture FxaaFlip
    {
        get
        {
            return this.fxaaFlip;
        }
        set
        {
            this.fxaaFlip = value;
        }
    }

    protected RenderTexture FxaaFlop
    {
        get
        {
            return this.fxaaFlop;
        }
        set
        {
            this.fxaaFlop = value;
        }
    }

    protected Shader GrabAlpha
    {
        get
        {
            if (this._grabAlpha == null)
            {
                this._grabAlpha = Shader.Find("Hidden/SSAA_Alpha");
            }
            return this._grabAlpha;
        }
    }

    protected Material GrabAlphaMat
    {
        get
        {
            if (this._grabAlphaMat == null)
            {
                this._grabAlphaMat = new Material(this.GrabAlpha);
            }
            return this._grabAlphaMat;
        }
    }

    protected CommandBuffer GrabAlphaCB
    {
        get
        {
            return this.grabAlphaCB;
        }
        set
        {
            this.grabAlphaCB = value;
        }
    }

    protected CommandBuffer PasteAlphaCB
    {
        get
        {
            return this.pasteAlphaCB;
        }
        set
        {
            this.pasteAlphaCB = value;
        }
    }

    private Texture2D sphereTemp
    {
        get
        {
            if (this._sphereTemp != null)
            {
                return this._sphereTemp;
            }
            this._sphereTemp = new Texture2D(2, 2);
            return this._sphereTemp;
        }
    }

    private string getName => (this.useProductName ? Application.productName : this.namePrefix) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssff") + "_" + this.panoramaSettings.panoramaSize + "p";

    private void OnEnable()
    {
        if (this.dbgData == null)
        {
            this.dbgData = new DebugData(this);
        }
        this.currentCamera = base.GetComponent<Camera>();
        this.Init();
        base.StartCoroutine(this.AdaptiveTask());
        base.StartCoroutine(this.OnEndCameraRender());
        this.SSAA_Internal.OnMainEnable();
    }

    private void Update()
    {
        if (!this.currentCamera.targetTexture)
        {
            Debug.LogWarning("Something went wrong with the target texture. Restarting SSAA...");
            this.Refresh();
            return;
        }
        this.currentCamera.targetTexture.filterMode = ((this.filterType != 0 || !this.useShader) ? FilterMode.Trilinear : FilterMode.Point);
        this.renderCameraObject.hideFlags = ((!this.exposeInternalRender) ? (HideFlags.HideInHierarchy | HideFlags.HideInInspector) : HideFlags.None);
        this.renderCamera.enabled = this.currentCamera.enabled;
        int layer = this.renderCamera.gameObject.layer;
        this.renderCamera.CopyFrom(this.currentCamera, null);
        this.renderCamera.cullingMask = (this.mouseCompatibilityMode ? (-1) : 0);
        this.renderCamera.clearFlags = this.currentCamera.clearFlags;
        this.renderCamera.targetTexture = this.targetTexture;
        this.renderCamera.depth = this.currentCamera.depth;
        this.renderCamera.gameObject.layer = layer;
        this.SSAA_Internal.Multiplier = this.multiplier;
        this.SSAA_Internal.Sharpness = this.sharpness;
        this.SSAA_Internal.UseShader = this.useShader;
        this.SSAA_Internal.SampleDistance = this.sampleDistance;
        this.SSAA_Internal.OnMainFilterChanged(this.filterType);
        this.FpsData.Update();
        this.SendDbgInfo();
    }

    private void OnDisable()
    {
        this.SSAA_Internal.OnMainDisable();
        this.SSAA_Internal.enabled = false;
        this.currentCamera.targetTexture.Release();
        this.currentCamera.targetTexture = null;
        this.renderCamera.enabled = false;
        if (MadGoatSSAA_Utils.DetectSRP())
        {
            return;
        }
        if (new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_FSS") != null)
        {
            this.currentCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_FSS"));
            this.FssCb.Clear();
        }
        if (new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Grab_Alpha") != null)
        {
            this.currentCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Grab_Alpha"));
            this.GrabAlphaCB.Clear();
            this.currentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.AfterImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Apply_Alpha"));
            this.PasteAlphaCB.Clear();
        }
    }

    private void OnPreRender()
    {
        this.OnBeginCameraRender(this.currentCamera);
    }

    protected virtual void OnBeginCameraRender(Camera cam)
    {
        if (cam != this.currentCamera || !base.enabled)
        {
            return;
        }
        this.currentCamera.aspect = (float)Screen.width * this.currentCamera.rect.width / ((float)Screen.height * this.currentCamera.rect.height);
        if (this.screenshotSettings.takeScreenshot)
        {
            this.SetupScreenshotRender(this.screenshotSettings.screenshotMultiplier, compatibilityMode: false);
            return;
        }
        if ((float)Screen.width * this.multiplier != (float)this.currentCamera.targetTexture.width || (float)Screen.height * ((this.renderMode == Mode.PerAxisScale) ? this.multiplierVertical : this.multiplier) != (float)this.currentCamera.targetTexture.height)
        {
            this.SetupRender();
            if (new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Grab_Alpha") != null)
            {
                this.currentCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Grab_Alpha"));
                this.GrabAlphaCB.Clear();
                this.currentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.AfterImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Apply_Alpha"));
                this.PasteAlphaCB.Clear();
            }
        }
        this.currentCamera.targetTexture.Release();
        this.currentCamera.targetTexture.Create();
        this.DoAlphaCommandBuffer();
        if (this.multiplier > 1f && this.ssaaUltra && this.renderMode != Mode.AdaptiveResolution && !MadGoatSSAA_Utils.DetectSRP())
        {
            this.DoFSSCommandBuffer();
        }
        else if (!MadGoatSSAA_Utils.DetectSRP() && new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_FSS") != null)
        {
            this.currentCamera.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, this.FssCb);
            this.FssCb.Clear();
        }
        this.SSAA_Internal.OnMainRender();
    }

    protected virtual IEnumerator OnEndCameraRender()
    {
        yield return new WaitForEndOfFrame();
        this.SSAA_Internal.OnMainRenderEnded();
        if (base.enabled)
        {
            base.StartCoroutine(this.OnEndCameraRender());
        }
    }

    protected virtual void Init()
    {
        if (this.renderCameraObject == null)
        {
            if ((bool)base.GetComponentInChildren<MadGoatSSAA_InternalRenderer>())
            {
                this.SSAA_Internal = base.GetComponentInChildren<MadGoatSSAA_InternalRenderer>();
                this.renderCameraObject = this.SSAA_Internal.gameObject;
                this.renderCamera = this.renderCameraObject.GetComponent<Camera>();
            }
            else
            {
                this.renderCameraObject = new GameObject("RenderCameraObject");
                this.renderCameraObject.transform.SetParent(base.transform);
                this.renderCameraObject.transform.position = Vector3.zero;
                this.renderCameraObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                this.renderCamera = this.renderCameraObject.AddComponent<Camera>();
                this.SSAA_Internal = this.renderCameraObject.AddComponent<MadGoatSSAA_InternalRenderer>();
            }
            this.renderCameraObject.hideFlags = ((!this.exposeInternalRender) ? (HideFlags.HideInHierarchy | HideFlags.HideInInspector) : HideFlags.None);
            this.SSAA_Internal.Current = this.renderCamera;
            this.SSAA_Internal.Main = this.currentCamera;
            this.SSAA_Internal.enabled = true;
            this.renderCamera.CopyFrom(this.currentCamera);
            this.renderCamera.cullingMask = 0;
            this.renderCamera.clearFlags = CameraClearFlags.Nothing;
            this.renderCamera.enabled = true;
        }
        else
        {
            this.SSAA_Internal.enabled = true;
            this.renderCamera.enabled = true;
        }
        this.currentCamera.targetTexture = new RenderTexture(1024, 1024, 24, this.textureFormat);
        this.currentCamera.targetTexture.Create();
        if (!MadGoatSSAA_Utils.DetectSRP())
        {
            this.FssCb = new CommandBuffer();
            this.GrabAlphaCB = new CommandBuffer();
            this.PasteAlphaCB = new CommandBuffer();
        }
    }

    protected void SendDbgInfo()
    {
        if (Application.isPlaying && (bool)this.madGoatDebugger)
        {
            string value = "SSAA: Render Res:" + this.GetResolution() + " [x" + this.dbgData.multiplier + "] [FSSAA:" + this.dbgData.fssaa + "] [Mode: " + this.dbgData.renderMode.ToString() + "]";
            this.madGoatDebugger.SendMessage("SsaaListener", value);
        }
    }

    private void DoFSSCommandBuffer()
    {
        if (MadGoatSSAA_Utils.DetectSRP())
        {
            return;
        }
        this.FXAA_FSS_Mat.SetVector("_QualitySettings", new Vector3(1f, 0.063f, 0.0312f));
        this.FXAA_FSS_Mat.SetVector("_ConsoleSettings", new Vector4(0.5f, 2f, 0.125f, 0.04f));
        this.FXAA_FSS_Mat.SetFloat("_Intensity", this.fssaaIntensity);
        if (new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_FSS") == null)
        {
            this.FssCb.name = "SSAA_FSS";
            this.FssCb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            if ((bool)this.FxaaFlip)
            {
                this.FxaaFlip.Release();
            }
            if ((bool)this.FxaaFlop)
            {
                this.FxaaFlop.Release();
            }
            this.FxaaFlip = new RenderTexture(this.currentCamera.targetTexture.width, this.currentCamera.targetTexture.height, 1, RenderTextureFormat.ARGBHalf);
            this.FxaaFlop = new RenderTexture(this.currentCamera.targetTexture.width, this.currentCamera.targetTexture.height, 1, RenderTextureFormat.ARGBHalf);
            RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(this.FxaaFlip);
            RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(this.FxaaFlop);
            this.FssCb.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier);
            this.FssCb.Blit(renderTargetIdentifier, renderTargetIdentifier2, this._FXAA_FSS_Mat, 0);
            this.FssCb.Blit(renderTargetIdentifier2, BuiltinRenderTextureType.CameraTarget);
            this.currentCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, this.FssCb);
        }
    }

    private void DoAlphaCommandBuffer()
    {
        if (!MadGoatSSAA_Utils.DetectSRP() && new List<CommandBuffer>(this.currentCamera.GetCommandBuffers(CameraEvent.BeforeImageEffects)).Find((CommandBuffer x) => x.name == "SSAA_Grab_Alpha") == null)
        {
            this.GrabAlphaCB.name = "SSAA_Grab_Alpha";
            this.PasteAlphaCB.name = "SSAA_Apply_Alpha";
            this.GrabAlphaCB.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            this.PasteAlphaCB.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            if ((bool)this.grabAlphaRT)
            {
                this.grabAlphaRT.Release();
            }
            this.grabAlphaRT = new RenderTexture(this.currentCamera.targetTexture.width, this.currentCamera.targetTexture.height, 1, RenderTextureFormat.ARGBHalf);
            RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(this.grabAlphaRT);
            this.GrabAlphaCB.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier, this.GrabAlphaMat, 0);
            if ((bool)this.pasteAlphaRT)
            {
                this.pasteAlphaRT.Release();
            }
            this.pasteAlphaRT = new RenderTexture(this.currentCamera.targetTexture.width, this.currentCamera.targetTexture.height, 1, RenderTextureFormat.ARGBHalf);
            RenderTargetIdentifier renderTargetIdentifier2 = new RenderTargetIdentifier(this.pasteAlphaRT);
            this.PasteAlphaCB.SetGlobalTexture("_MainTexA", renderTargetIdentifier);
            this.PasteAlphaCB.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier2);
            this.PasteAlphaCB.Blit(renderTargetIdentifier2, BuiltinRenderTextureType.CameraTarget, this.GrabAlphaMat, 1);
            this.currentCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, this.GrabAlphaCB);
            this.currentCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, this.PasteAlphaCB);
        }
    }

    private void RenderPanorama()
    {
        base.enabled = false;
        int num = this.panoramaSettings.panoramaSize * this.panoramaSettings.panoramaMultiplier;
        Cubemap cubemap = new Cubemap(num, TextureFormat.ARGB32, mipChain: false);
        RenderTexture temporary = RenderTexture.GetTemporary(this.panoramaSettings.panoramaSize, this.panoramaSettings.panoramaSize, 24, RenderTextureFormat.ARGB32);
        this.renderCamera.CopyFrom(this.currentCamera, null);
        this.SSAA_Internal.enabled = false;
        this.currentCamera.RenderToCubemap(cubemap);
        string text = this.screenshotPath + "\\" + this.getName + "\\";
        new FileInfo(text).Directory.Create();
        for (int i = 0; i < 6; i++)
        {
            this.sphereTemp.Reinitialize(num, num);
            this.sphereTemp.SetPixels(this.Rotate90(this.Rotate90(cubemap.GetPixels((CubemapFace)i), num), num));
            this.sphereTemp.Apply();
            if (this.panoramaSettings.panoramaMultiplier == 1)
            {
                if (this.imageFormat == ImageFormat.PNG)
                {
                    CubemapFace cubemapFace = (CubemapFace)i;
                    File.WriteAllBytes(text + "Face_" + cubemapFace.ToString() + ".png", this.sphereTemp.EncodeToPNG());
                }
                else if (this.imageFormat == ImageFormat.JPG)
                {
                    CubemapFace cubemapFace = (CubemapFace)i;
                    File.WriteAllBytes(text + "Face_" + cubemapFace.ToString() + ".jpg", this.sphereTemp.EncodeToJPG(this.JPGQuality));
                }
                else
                {
                    CubemapFace cubemapFace = (CubemapFace)i;
                    File.WriteAllBytes(text + "Face_" + cubemapFace.ToString() + ".exr", this.sphereTemp.EncodeToEXR(this.EXR32 ? Texture2D.EXRFlags.OutputAsFloat : Texture2D.EXRFlags.None));
                }
                continue;
            }
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = true;
            if (!this.panoramaSettings.useFilter)
            {
                Graphics.Blit(this.sphereTemp, temporary);
            }
            else
            {
                this.SSAA_Internal.MaterialBilinear.SetFloat("_ResizeWidth", num);
                this.SSAA_Internal.MaterialBilinear.SetFloat("_ResizeHeight", num);
                this.SSAA_Internal.MaterialBilinear.SetFloat("_Sharpness", this.panoramaSettings.sharpness);
                Graphics.Blit(this.sphereTemp, temporary, this.SSAA_Internal.MaterialBilinear, 0);
            }
            RenderTexture.active = temporary;
            Texture2D texture2D = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.ARGB32, mipChain: true, linear: true);
            texture2D.ReadPixels(new Rect(0f, 0f, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
            if (this.imageFormat == ImageFormat.PNG)
            {
                CubemapFace cubemapFace = (CubemapFace)i;
                File.WriteAllBytes(text + "\\Face_" + cubemapFace.ToString() + ".png", texture2D.EncodeToPNG());
            }
            else if (this.imageFormat == ImageFormat.JPG)
            {
                CubemapFace cubemapFace = (CubemapFace)i;
                File.WriteAllBytes(text + "\\Face_" + cubemapFace.ToString() + ".jpg", texture2D.EncodeToJPG(this.JPGQuality));
            }
            else
            {
                CubemapFace cubemapFace = (CubemapFace)i;
                File.WriteAllBytes(text + "\\Face_" + cubemapFace.ToString() + ".exr", texture2D.EncodeToEXR(this.EXR32 ? Texture2D.EXRFlags.OutputAsFloat : Texture2D.EXRFlags.None));
            }
            GL.sRGBWrite = sRGBWrite;
        }
        this.sphereTemp.Reinitialize(2, 2);
        this.sphereTemp.Apply();
        RenderTexture.ReleaseTemporary(temporary);
        this.SSAA_Internal.enabled = true;
        base.enabled = true;
    }

    private void SetupAdaptive(int fps)
    {
        int num = (this.useVsyncTarget ? Screen.currentResolution.refreshRate : this.targetFramerate);
        if (fps < num - 5)
        {
            this.multiplier = Mathf.Clamp(this.multiplier - 0.1f, this.minMultiplier, this.maxMultiplier);
        }
        else if (fps > num + 10)
        {
            this.multiplier = Mathf.Clamp(this.multiplier + 0.1f, this.minMultiplier, this.maxMultiplier);
        }
    }

    private void SetupRender()
    {
        try
        {
            this.currentCamera.targetTexture.Release();
            this.currentCamera.targetTexture.width = (int)((float)Screen.width * this.multiplier);
            this.currentCamera.targetTexture.height = (int)((float)Screen.height * ((this.renderMode == Mode.PerAxisScale) ? this.multiplierVertical : this.multiplier));
            this.currentCamera.targetTexture.Create();
        }
        catch (Exception message)
        {
            Debug.LogError("Something went wrong. SSAA has been set to off");
            Debug.LogError(message);
            this.SetAsSSAA(SSAAMode.SSAA_OFF);
        }
    }

    private void SetupScreenshotRender(float mul, bool compatibilityMode)
    {
        try
        {
            this.currentCamera.aspect = this.screenshotSettings.outputResolution.x / this.screenshotSettings.outputResolution.y;
            this.currentCamera.targetTexture.Release();
            this.currentCamera.targetTexture.width = (int)(this.screenshotSettings.outputResolution.x * mul);
            this.currentCamera.targetTexture.height = (int)(this.screenshotSettings.outputResolution.y * mul);
            this.currentCamera.targetTexture.Create();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    protected IEnumerator AdaptiveTask()
    {
        yield return new WaitForSeconds(2f);
        if (this.renderMode == Mode.AdaptiveResolution)
        {
            this.SetupAdaptive(this.FpsData.CurrentFps);
        }
        if (base.enabled)
        {
            base.StartCoroutine(this.AdaptiveTask());
        }
    }

    private Color[] Rotate90(Color[] source, int n)
    {
        Color[] array = new Color[n * n];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                array[i * n + j] = source[(n - j - 1) * n + i];
            }
        }
        return array;
    }

    public void Refresh()
    {
        base.enabled = false;
        base.enabled = true;
        this.currentCamera.rect = new Rect(0f, 0f, 1f, 1f);
    }

    public void SetAsSSAA(SSAAMode mode)
    {
        this.renderMode = Mode.SSAA;
        this.ssaaMode = mode;
        switch (mode)
        {
        case SSAAMode.SSAA_OFF:
            this.multiplier = 1f;
            this.useShader = false;
            break;
        case SSAAMode.SSAA_HALF:
            this.multiplier = this.SSAA_HALF.multiplier;
            this.useShader = this.SSAA_HALF.useFilter;
            this.sharpness = this.SSAA_HALF.sharpness;
            this.filterType = this.SSAA_HALF.filterType;
            this.sampleDistance = this.SSAA_HALF.sampleDistance;
            break;
        case SSAAMode.SSAA_X2:
            this.multiplier = this.SSAA_X2.multiplier;
            this.useShader = this.SSAA_X2.useFilter;
            this.sharpness = this.SSAA_X2.sharpness;
            this.filterType = this.SSAA_X2.filterType;
            this.sampleDistance = this.SSAA_X2.sampleDistance;
            break;
        case SSAAMode.SSAA_X4:
            this.multiplier = this.SSAA_X4.multiplier;
            this.useShader = this.SSAA_X4.useFilter;
            this.sharpness = this.SSAA_X4.sharpness;
            this.filterType = this.SSAA_X4.filterType;
            this.sampleDistance = this.SSAA_X4.sampleDistance;
            break;
        }
    }

    public void SetAsScale(int percent)
    {
        percent = Mathf.Clamp(percent, 50, 200);
        this.renderMode = Mode.ResolutionScale;
        this.multiplier = (float)percent / 100f;
        this.SetDownsamplingSettings(use: false);
    }

    public void SetAsScale(int percent, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        percent = Mathf.Clamp(percent, 50, 200);
        this.renderMode = Mode.ResolutionScale;
        this.multiplier = (float)percent / 100f;
        this.SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
    }

    public void SetAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate)
    {
        if (minMultiplier < 0.1f)
        {
            minMultiplier = 0.1f;
        }
        if (maxMultiplier < minMultiplier)
        {
            maxMultiplier = minMultiplier + 0.1f;
        }
        this.minMultiplier = minMultiplier;
        this.maxMultiplier = maxMultiplier;
        this.targetFramerate = targetFramerate;
        this.useVsyncTarget = false;
        this.SetDownsamplingSettings(use: false);
    }

    public void SetAsAdaptive(float minMultiplier, float maxMultiplier)
    {
        if (minMultiplier < 0.1f)
        {
            minMultiplier = 0.1f;
        }
        if (maxMultiplier < minMultiplier)
        {
            maxMultiplier = minMultiplier + 0.1f;
        }
        this.minMultiplier = minMultiplier;
        this.maxMultiplier = maxMultiplier;
        this.useVsyncTarget = true;
        this.SetDownsamplingSettings(use: false);
    }

    public void SetAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        if (minMultiplier < 0.1f)
        {
            minMultiplier = 0.1f;
        }
        if (maxMultiplier < minMultiplier)
        {
            maxMultiplier = minMultiplier + 0.1f;
        }
        this.minMultiplier = minMultiplier;
        this.maxMultiplier = maxMultiplier;
        this.targetFramerate = targetFramerate;
        this.useVsyncTarget = false;
        this.SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
    }

    public void SetAsAdaptive(float minMultiplier, float maxMultiplier, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        if (minMultiplier < 0.1f)
        {
            minMultiplier = 0.1f;
        }
        if (maxMultiplier < minMultiplier)
        {
            maxMultiplier = minMultiplier + 0.1f;
        }
        this.minMultiplier = minMultiplier;
        this.maxMultiplier = maxMultiplier;
        this.useVsyncTarget = true;
        this.SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
    }

    public void SetAsCustom(float Multiplier)
    {
        if (Multiplier < 0.1f)
        {
            Multiplier = 0.1f;
        }
        this.renderMode = Mode.Custom;
        this.multiplier = Multiplier;
        this.SetDownsamplingSettings(use: false);
    }

    public void SetAsCustom(float Multiplier, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        if (Multiplier < 0.1f)
        {
            Multiplier = 0.1f;
        }
        this.renderMode = Mode.Custom;
        this.multiplier = Multiplier;
        this.SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
    }

    public virtual void SetAsAxisBased(float MultiplierX, float MultiplierY)
    {
        if (MultiplierX < 0.1f)
        {
            MultiplierX = 0.1f;
        }
        if (MultiplierY < 0.1f)
        {
            MultiplierY = 0.1f;
        }
        this.renderMode = Mode.PerAxisScale;
        this.multiplier = MultiplierX;
        this.multiplierVertical = MultiplierY;
        this.SetDownsamplingSettings(use: false);
    }

    public virtual void SetAsAxisBased(float MultiplierX, float MultiplierY, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        if (MultiplierX < 0.1f)
        {
            MultiplierX = 0.1f;
        }
        if (MultiplierY < 0.1f)
        {
            MultiplierY = 0.1f;
        }
        this.renderMode = Mode.PerAxisScale;
        this.multiplier = MultiplierX;
        this.multiplierVertical = MultiplierY;
        this.SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
    }

    public void SetDownsamplingSettings(bool use)
    {
        this.useShader = use;
        this.filterType = (use ? Filter.BILINEAR : Filter.NEAREST_NEIGHBOR);
        this.sharpness = (use ? 0.85f : 0f);
        this.sampleDistance = (use ? 0.9f : 0f);
    }

    public void SetDownsamplingSettings(Filter FilterType, float sharpnessfactor, float sampledist)
    {
        this.useShader = true;
        this.filterType = FilterType;
        this.sharpness = Mathf.Clamp(sharpnessfactor, 0f, 1f);
        this.sampleDistance = Mathf.Clamp(sampledist, 0.5f, 1.5f);
    }

    public void SetUltra(bool enabled)
    {
        this.ssaaUltra = enabled;
    }

    public void SetUltraIntensity(float intensity)
    {
        this.fssaaIntensity = Mathf.Clamp01(intensity);
    }

    public virtual void TakeScreenshot(string path, Vector2 Size, int multiplier)
    {
        this.screenshotSettings.takeScreenshot = true;
        this.screenshotSettings.outputResolution = Size;
        this.screenshotSettings.screenshotMultiplier = multiplier;
        this.screenshotPath = path;
        this.screenshotSettings.useFilter = false;
    }

    public virtual void TakeScreenshot(string path, Vector2 Size, int multiplier, float sharpness)
    {
        this.screenshotSettings.takeScreenshot = true;
        this.screenshotSettings.outputResolution = Size;
        this.screenshotSettings.screenshotMultiplier = multiplier;
        this.screenshotPath = path;
        this.screenshotSettings.useFilter = true;
        this.screenshotSettings.sharpness = Mathf.Clamp(sharpness, 0f, 1f);
    }

    public virtual void TakePanorama(string path, int size)
    {
        this.panoramaSettings.useFilter = false;
        this.panoramaSettings.panoramaSize = size;
        this.panoramaSettings.panoramaMultiplier = 1;
        this.screenshotPath = path;
        this.RenderPanorama();
    }

    public virtual void TakePanorama(string path, int size, int multiplier)
    {
        this.panoramaSettings.useFilter = false;
        this.panoramaSettings.panoramaSize = size;
        this.panoramaSettings.panoramaMultiplier = multiplier;
        this.screenshotPath = path;
        this.RenderPanorama();
    }

    public virtual void TakePanorama(string path, int size, int multiplier, float sharpness)
    {
        this.panoramaSettings.useFilter = true;
        this.panoramaSettings.panoramaSize = size;
        this.panoramaSettings.panoramaMultiplier = multiplier;
        this.panoramaSettings.sharpness = sharpness;
        this.screenshotPath = path;
        this.RenderPanorama();
    }

    public virtual void SetScreenshotModuleToPNG()
    {
        this.imageFormat = ImageFormat.PNG;
    }

    public virtual void SetScreenshotModuleToJPG(int quality)
    {
        this.imageFormat = ImageFormat.JPG;
        this.JPGQuality = Mathf.Clamp(1, 100, quality);
    }

    public virtual void SetScreenshotModuleToEXR(bool EXR32)
    {
        this.imageFormat = ImageFormat.EXR;
        this.EXR32 = EXR32;
    }

    public virtual string GetResolution()
    {
        return (int)((float)Screen.width * this.multiplier) + "x" + (int)((float)Screen.height * this.multiplier);
    }

    public static void SetAllAsSSAA(SSAAMode mode)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsSSAA(mode);
        }
    }

    public static void SetAllAsScale(int percent)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsScale(percent);
        }
    }

    public static void SetAllAsScale(int percent, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsScale(percent, FilterType, sharpnessfactor, sampledist);
        }
    }

    public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsAdaptive(minMultiplier, maxMultiplier, targetFramerate);
        }
    }

    public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsAdaptive(minMultiplier, maxMultiplier);
        }
    }

    public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsAdaptive(minMultiplier, maxMultiplier, targetFramerate, FilterType, sharpnessfactor, sampledist);
        }
    }

    public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsAdaptive(minMultiplier, maxMultiplier, FilterType, sharpnessfactor, sampledist);
        }
    }

    public static void SetAllAsCustom(float Multiplier)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsCustom(Multiplier);
        }
    }

    public static void SetAllAsCustom(float Multiplier, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsCustom(Multiplier, FilterType, sharpnessfactor, sampledist);
        }
    }

    public static void SetAllAsAxisBased(float MultiplierX, float MultiplierY)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsAxisBased(MultiplierX, MultiplierY);
        }
    }

    public static void SetAllAsAxisBased(float MultiplierX, float MultiplierY, Filter FilterType, float sharpnessfactor, float sampledist)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetAsAxisBased(MultiplierX, MultiplierY, FilterType, sharpnessfactor, sampledist);
        }
    }

    public static void SetAllDownsamplingSettings(bool use)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetDownsamplingSettings(use);
        }
    }

    public static void SetAllDownsamplingSettings(Filter FilterType, float sharpnessfactor, float sampledist)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
        }
    }

    public static void SetAllUltra(bool enabled)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetUltra(enabled);
        }
    }

    public static void SetAllUltraIntensity(float intensity)
    {
        MadGoatSSAA[] array = global::UnityEngine.Object.FindObjectsOfType<MadGoatSSAA>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetUltraIntensity(intensity);
        }
    }

    public virtual Ray ScreenPointToRay(Vector3 position)
    {
        return this.renderCamera.ScreenPointToRay(position);
    }

    public virtual Vector3 ScreenToViewportPoint(Vector3 position)
    {
        return this.renderCamera.ScreenToViewportPoint(position);
    }

    public virtual Vector3 ScreenToWorldPoint(Vector3 position)
    {
        return this.renderCamera.ScreenToWorldPoint(position);
    }

    public virtual Vector3 WorldToScreenPoint(Vector3 position)
    {
        return this.renderCamera.WorldToScreenPoint(position);
    }

    public virtual Vector3 ViewportToScreenPoint(Vector3 position)
    {
        return this.renderCamera.ViewportToScreenPoint(position);
    }
}
