using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace MadGoat_SSAA
{
    public class MadGoatSSAA_VR : MadGoatSSAA
    {
        [SerializeField]
        private Shader _bilinearshader;

        [SerializeField]
        private Shader _bicubicshader;

        [SerializeField]
        private Shader _neighborshader;

        [SerializeField]
        private Shader _defaultshader;

        private Material material_bl;

        private Material material_bc;

        private Material material_nn;

        private Material material_def;

        private Material material_current;

        private Material material_old;

        private bool ssaaUltraOld;

        private int resSumOld;

        private int resSumCurrent;

        private int postVolumePass;

        private int postVolumePassOld;

        private CommandBuffer processBuffer;

        private LayerMask oldPostLayer;

        private RenderTexture buff;

        public Shader bilinearshader
        {
            get
            {
                if (this._bilinearshader == null)
                {
                    this._bilinearshader = Shader.Find("Hidden/SSAA_Bilinear");
                }
                return this._bilinearshader;
            }
        }

        public Shader bicubicshader
        {
            get
            {
                if (this._bicubicshader == null)
                {
                    this._bicubicshader = Shader.Find("Hidden/SSAA_Bicubic");
                }
                return this._bicubicshader;
            }
        }

        public Shader neighborshader
        {
            get
            {
                if (this._neighborshader == null)
                {
                    this._neighborshader = Shader.Find("Hidden/SSAA_Nearest");
                }
                return this._neighborshader;
            }
        }

        public Shader defaultshader
        {
            get
            {
                if (this._defaultshader == null)
                {
                    this._defaultshader = Shader.Find("Hidden/SSAA_Def");
                }
                return this._defaultshader;
            }
        }

        public Material Material_bc => this.material_bc;

        public Material Material_bl => this.material_bl;

        public Material Material_nn => this.material_nn;

        public Material Material_def => this.material_def;

        public Material Material_current
        {
            get
            {
                return this.material_current;
            }
            set
            {
                this.material_current = value;
            }
        }

        public Material Material_old
        {
            get
            {
                return this.material_old;
            }
            set
            {
                this.material_old = value;
            }
        }

        public int PostVolumePass
        {
            get
            {
                return this.postVolumePass;
            }
            set
            {
                this.postVolumePass = value;
            }
        }

        public int PostVolumePassOld
        {
            get
            {
                return this.postVolumePassOld;
            }
            set
            {
                this.postVolumePassOld = value;
            }
        }

        protected override void Init()
        {
            if (base.currentCamera == null)
            {
                base.currentCamera = base.GetComponent<Camera>();
            }
            XRSettings.eyeTextureResolutionScale = base.multiplier;
            this.material_bl = new Material(this.bilinearshader);
            this.material_bc = new Material(this.bicubicshader);
            this.material_nn = new Material(this.neighborshader);
            this.material_def = new Material(this.defaultshader);
            this.Material_current = this.material_def;
            this.Material_old = this.Material_current;
            if (MadGoatSSAA_Utils.DetectSRP())
            {
                this.SetupSRPCB();
            }
            else
            {
                this.SetupStandardCB();
            }
        }

        private void OnEnable()
        {
            if (base.dbgData == null)
            {
                base.dbgData = new DebugData(this);
            }
            this.Init();
            base.StartCoroutine(base.AdaptiveTask());
        }

        private void Update()
        {
            base.FpsData.Update();
            base.SendDbgInfo();
        }

        private void OnDisable()
        {
            XRSettings.eyeTextureResolutionScale = 1f;
            if (MadGoatSSAA_Utils.DetectSRP())
            {
                this.ClearSRPCB();
            }
            else
            {
                this.ClearStandardCB();
            }
        }

        protected override void OnBeginCameraRender(Camera cam)
        {
            if (cam != base.currentCamera || !base.enabled)
            {
                return;
            }
            try
            {
                this.ChangeMaterial(base.filterType);
                if (MadGoatSSAA_Utils.DetectSRP())
                {
                    this.UpdateSRPCB();
                }
                else
                {
                    this.UpdateSdanrdardCB();
                }
            }
            catch (Exception message)
            {
                Debug.LogError("Something went wrong. SSAA has been set to off and the plugin was disabled");
                Debug.LogError(message);
                base.SetAsSSAA(SSAAMode.SSAA_OFF);
                base.enabled = false;
            }
        }

        public void ChangeMaterial(Filter Type)
        {
            this.Material_old = this.Material_current;
            this.PostVolumePassOld = this.PostVolumePass;
            switch (Type)
            {
            case Filter.NEAREST_NEIGHBOR:
                this.Material_current = this.Material_nn;
                this.PostVolumePass = 1;
                break;
            case Filter.BILINEAR:
                this.Material_current = this.Material_bl;
                this.PostVolumePass = 2;
                break;
            case Filter.BICUBIC:
                this.Material_current = this.Material_bc;
                this.PostVolumePass = 3;
                break;
            }
            if ((!base.useShader || base.multiplier == 1f) && this.Material_current != this.Material_def)
            {
                this.Material_current = this.Material_def;
                this.PostVolumePass = 0;
            }
            this.resSumCurrent = XRSettings.eyeTextureWidth + XRSettings.eyeTextureWidth;
            if (this.Material_current != this.Material_old || this.ssaaUltraOld != base.ssaaUltra || this.resSumOld != this.resSumCurrent)
            {
                this.resSumOld = this.resSumCurrent;
                this.ssaaUltraOld = base.ssaaUltra;
                this.Material_old = this.Material_current;
                this.PostVolumePassOld = this.PostVolumePass;
                this.ClearStandardCB();
                this.SetupStandardCB();
            }
        }

        private void SetupStandardCB()
        {
            this.processBuffer = new CommandBuffer();
            if (new List<CommandBuffer>(base.currentCamera.GetCommandBuffers(CameraEvent.AfterEverything)).Find((CommandBuffer x) => x.name == "SSAA_VR_APPLY") == null)
            {
                this.Material_current.SetOverrideTag("RenderType", "Opaque");
                this.Material_current.SetInt("_SrcBlend", 1);
                this.Material_current.SetInt("_DstBlend", 0);
                this.Material_current.SetInt("_ZWrite", 1);
                this.Material_current.renderQueue = -1;
                if ((bool)this.buff)
                {
                    this.buff.Release();
                }
                this.buff = new RenderTexture(XRSettings.eyeTextureWidth * 2, XRSettings.eyeTextureHeight, 24, RenderTextureFormat.ARGBHalf);
                RenderTargetIdentifier renderTargetIdentifier = new RenderTargetIdentifier(this.buff);
                this.buff.vrUsage = XRSettings.eyeTextureDesc.vrUsage;
                this.processBuffer.Clear();
                this.processBuffer.name = "SSAA_VR_APPLY";
                this.processBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                this.processBuffer.Blit(BuiltinRenderTextureType.CameraTarget, renderTargetIdentifier, this.Material_current, 0);
                this.processBuffer.Blit(renderTargetIdentifier, BuiltinRenderTextureType.CameraTarget, (base.multiplier > 1f && base.ssaaUltra && base.renderMode != Mode.AdaptiveResolution) ? base.FXAA_FSS_Mat : this.Material_def, 0);
                base.currentCamera.AddCommandBuffer(CameraEvent.AfterEverything, this.processBuffer);
            }
        }

        private void UpdateSdanrdardCB()
        {
            this.Material_current.SetOverrideTag("RenderType", "Opaque");
            this.Material_current.SetInt("_SrcBlend", 1);
            this.Material_current.SetInt("_DstBlend", 0);
            this.Material_current.SetInt("_ZWrite", 1);
            this.Material_current.renderQueue = -1;
            this.Material_current.SetFloat("_ResizeWidth", XRSettings.eyeTextureWidth);
            this.Material_current.SetFloat("_ResizeHeight", XRSettings.eyeTextureHeight);
            this.Material_current.SetFloat("_Sharpness", base.sharpness);
            this.Material_current.SetFloat("_SampleDistance", base.sampleDistance);
            base.FXAA_FSS_Mat.SetVector("_QualitySettings", new Vector3(1f, 0.063f, 0.0312f));
            base.FXAA_FSS_Mat.SetVector("_ConsoleSettings", new Vector4(0.5f, 2f, 0.125f, 0.04f));
            base.FXAA_FSS_Mat.SetFloat("_Intensity", base.fssaaIntensity);
        }

        private void ClearStandardCB()
        {
            if (new List<CommandBuffer>(base.currentCamera.GetCommandBuffers(CameraEvent.AfterEverything)).Find((CommandBuffer x) => x.name == "SSAA_VR_APPLY") != null)
            {
                base.currentCamera.RemoveCommandBuffer(CameraEvent.AfterEverything, this.processBuffer);
            }
        }

        private void SetupSRPCB()
        {
        }

        private void UpdateSRPCB()
        {
        }

        private void ClearSRPCB()
        {
        }

        public override void SetAsAxisBased(float MultiplierX, float MultiplierY)
        {
            Debug.LogWarning("NOT SUPPORTED IN VR MODE.\nX axis will be used as global multiplier instead.");
            base.SetAsAxisBased(MultiplierX, MultiplierY);
        }

        public override void SetAsAxisBased(float MultiplierX, float MultiplierY, Filter FilterType, float sharpnessfactor, float sampledist)
        {
            Debug.LogWarning("NOT SUPPORTED IN VR MODE.\nX axis will be used as global multiplier instead.");
            base.SetAsAxisBased(MultiplierX, MultiplierY, FilterType, sharpnessfactor, sampledist);
        }

        public override Ray ScreenPointToRay(Vector3 position)
        {
            return base.currentCamera.ScreenPointToRay(position);
        }

        public override Vector3 ScreenToWorldPoint(Vector3 position)
        {
            return base.currentCamera.ScreenToWorldPoint(position);
        }

        public override Vector3 ScreenToViewportPoint(Vector3 position)
        {
            return base.currentCamera.ScreenToViewportPoint(position);
        }

        public override Vector3 WorldToScreenPoint(Vector3 position)
        {
            return base.currentCamera.WorldToScreenPoint(position);
        }

        public override Vector3 ViewportToScreenPoint(Vector3 position)
        {
            return base.currentCamera.ViewportToScreenPoint(position);
        }

        public override void TakeScreenshot(string path, Vector2 Size, int multiplier)
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void TakeScreenshot(string path, Vector2 Size, int multiplier, float sharpness)
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void TakePanorama(string path, int size)
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void TakePanorama(string path, int size, int multiplier)
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void TakePanorama(string path, int size, int multiplier, float sharpness)
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void SetScreenshotModuleToPNG()
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void SetScreenshotModuleToJPG(int quality)
        {
            Debug.LogWarning("Not available in VR mode");
        }

        public override void SetScreenshotModuleToEXR(bool EXR32)
        {
            Debug.LogWarning("Not available in VR mode");
        }
    }
}
