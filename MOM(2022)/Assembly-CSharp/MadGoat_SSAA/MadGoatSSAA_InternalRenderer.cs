using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace MadGoat_SSAA
{
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class MadGoatSSAA_InternalRenderer : MonoBehaviour
    {
        private float multiplier;

        private float sharpness;

        private bool useShader;

        private float sampleDistance;

        private bool flipImage;

        private Camera main;

        private Camera current;

        [SerializeField]
        private Shader shaderBilinear;

        [SerializeField]
        private Shader shaderBicubic;

        [SerializeField]
        private Shader shaderNeighbor;

        [SerializeField]
        private Shader shaderDefault;

        private Material materialBilinear;

        private Material materialBicubic;

        private Material materialNearest;

        private Material materialDefault;

        private Material materialCurrent;

        private Material materialOld;

        private CommandBuffer compositionCommand;

        private MadGoatSSAA mainComponent;

        private int postVolumePass;

        private int postVolumePassOld;

        public float Multiplier
        {
            get
            {
                return this.multiplier;
            }
            set
            {
                this.multiplier = value;
            }
        }

        public float Sharpness
        {
            get
            {
                return this.sharpness;
            }
            set
            {
                this.sharpness = value;
            }
        }

        public bool UseShader
        {
            get
            {
                return this.useShader;
            }
            set
            {
                this.useShader = value;
            }
        }

        public float SampleDistance
        {
            get
            {
                return this.sampleDistance;
            }
            set
            {
                this.sampleDistance = value;
            }
        }

        public bool FlipImage
        {
            get
            {
                return this.flipImage;
            }
            set
            {
                this.flipImage = value;
            }
        }

        public Camera Main
        {
            get
            {
                return this.main;
            }
            set
            {
                this.main = value;
            }
        }

        public Camera Current
        {
            get
            {
                return this.current;
            }
            set
            {
                this.current = value;
            }
        }

        public Shader ShaderBilinear
        {
            get
            {
                if (this.shaderBilinear == null)
                {
                    this.shaderBilinear = Shader.Find("Hidden/SSAA_Bilinear");
                }
                return this.shaderBilinear;
            }
        }

        public Shader ShaderBicubic
        {
            get
            {
                if (this.shaderBicubic == null)
                {
                    this.shaderBicubic = Shader.Find("Hidden/SSAA_Bicubic");
                }
                return this.shaderBicubic;
            }
        }

        public Shader ShaderNeighbor
        {
            get
            {
                if (this.shaderNeighbor == null)
                {
                    this.shaderNeighbor = Shader.Find("Hidden/SSAA_Nearest");
                }
                return this.shaderNeighbor;
            }
        }

        public Shader ShaderDefault
        {
            get
            {
                if (this.shaderDefault == null)
                {
                    this.shaderDefault = Shader.Find("Hidden/SSAA_Def");
                }
                return this.shaderDefault;
            }
        }

        public Material MaterialBilinear
        {
            get
            {
                if (this.materialBicubic == null)
                {
                    this.materialBicubic = new Material(this.ShaderBicubic);
                }
                return this.materialBicubic;
            }
        }

        public Material MaterialBicubic
        {
            get
            {
                if (this.materialBilinear == null)
                {
                    this.materialBilinear = new Material(this.ShaderBilinear);
                }
                return this.materialBilinear;
            }
        }

        public Material MaterialNearest
        {
            get
            {
                if (this.materialNearest == null)
                {
                    this.materialNearest = new Material(this.ShaderNeighbor);
                }
                return this.materialNearest;
            }
        }

        public Material MaterialDefault
        {
            get
            {
                if (this.materialDefault == null)
                {
                    this.materialDefault = new Material(this.ShaderDefault);
                }
                return this.materialDefault;
            }
        }

        public Material MaterialCurrent
        {
            get
            {
                return this.materialCurrent;
            }
            set
            {
                this.materialCurrent = value;
            }
        }

        public Material MaterialOld
        {
            get
            {
                return this.materialOld;
            }
            set
            {
                this.materialOld = value;
            }
        }

        public CommandBuffer CompositionCommand
        {
            get
            {
                if (this.compositionCommand == null)
                {
                    this.compositionCommand = new CommandBuffer();
                }
                return this.compositionCommand;
            }
            set
            {
                this.compositionCommand = value;
            }
        }

        public MadGoatSSAA MainComponent
        {
            get
            {
                return this.mainComponent;
            }
            set
            {
                this.mainComponent = value;
            }
        }

        public string GetScreenshotName => (this.MainComponent.useProductName ? Application.productName : this.MainComponent.namePrefix) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssff") + "_" + this.MainComponent.screenshotSettings.outputResolution.y + "p";

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

        private void SetupCommand(CommandBuffer cb, CameraEvent evt)
        {
            cb.Clear();
            cb.name = "SSAA_COMPOSITION";
            RenderTargetIdentifier source = new RenderTargetIdentifier(this.Main.targetTexture);
            cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
            cb.Blit(source, BuiltinRenderTextureType.CameraTarget, this.MaterialCurrent, 0);
            if (new List<CommandBuffer>(this.Current.GetCommandBuffers(evt)).Find((CommandBuffer x) => x.name == "SSAA_COMPOSITION") == null && !MadGoatSSAA_Utils.DetectSRP())
            {
                this.Current.AddCommandBuffer(evt, cb);
            }
        }

        private void ClearCommand(CommandBuffer cb, CameraEvent evt)
        {
            cb.Clear();
            if (new List<CommandBuffer>(this.Current.GetCommandBuffers(evt)).Find((CommandBuffer x) => x.name == "SSAA_COMPOSITION") != null && !MadGoatSSAA_Utils.DetectSRP())
            {
                this.Current.RemoveCommandBuffer(evt, cb);
            }
        }

        private void UpdateCommand()
        {
            this.MaterialCurrent.SetFloat("_ResizeWidth", Screen.width);
            this.MaterialCurrent.SetFloat("_ResizeHeight", Screen.height);
            this.MaterialCurrent.SetFloat("_Sharpness", this.Sharpness);
            this.MaterialCurrent.SetFloat("_SampleDistance", this.SampleDistance);
            if (this.Current.clearFlags == CameraClearFlags.Color || this.Current.clearFlags == CameraClearFlags.Skybox)
            {
                this.MaterialCurrent.SetOverrideTag("RenderType", "Opaque");
                this.MaterialCurrent.SetInt("_SrcBlend", 1);
                this.MaterialCurrent.SetInt("_DstBlend", 0);
                this.MaterialCurrent.SetInt("_ZWrite", 1);
                this.MaterialCurrent.renderQueue = -1;
            }
            else
            {
                this.MaterialCurrent.SetOverrideTag("RenderType", "Transparent");
                this.MaterialCurrent.SetInt("_SrcBlend", 5);
                this.MaterialCurrent.SetInt("_DstBlend", 10);
                this.MaterialCurrent.SetInt("_ZWrite", 0);
                this.MaterialCurrent.renderQueue = 3000;
            }
        }

        public void OnMainEnable()
        {
            this.mainComponent = this.Main.GetComponent<MadGoatSSAA>();
            this.MaterialCurrent = this.materialDefault;
            if (MadGoatSSAA_Utils.DetectSRP())
            {
                this.SetupCBSRP();
            }
            else
            {
                this.SetupCommand(this.CompositionCommand, CameraEvent.AfterImageEffects);
            }
        }

        public void OnMainDisable()
        {
            if (!MadGoatSSAA_Utils.DetectSRP())
            {
                this.ClearCommand(this.CompositionCommand, CameraEvent.AfterImageEffects);
            }
        }

        public void OnMainRender()
        {
            if (MadGoatSSAA_Utils.DetectSRP())
            {
                this.UpdateCBSRP();
            }
            else
            {
                this.UpdateCommand();
            }
        }

        public void OnMainRenderEnded()
        {
            this.HandleScreenshot();
        }

        public void OnMainFilterChanged(Filter Type)
        {
            this.MaterialOld = this.MaterialCurrent;
            this.PostVolumePassOld = this.PostVolumePass;
            switch (Type)
            {
            case Filter.NEAREST_NEIGHBOR:
                this.MaterialCurrent = this.MaterialNearest;
                this.PostVolumePass = 1;
                break;
            case Filter.BILINEAR:
                this.MaterialCurrent = this.MaterialBilinear;
                this.PostVolumePass = 2;
                break;
            case Filter.BICUBIC:
                this.MaterialCurrent = this.MaterialBicubic;
                this.PostVolumePass = 3;
                break;
            }
            if ((!this.useShader || this.multiplier == 1f) && this.MaterialCurrent != this.MaterialDefault)
            {
                this.MaterialCurrent = this.MaterialDefault;
                this.PostVolumePass = 0;
            }
            if (this.MaterialCurrent != this.MaterialOld)
            {
                this.MaterialOld = this.MaterialCurrent;
                this.PostVolumePassOld = this.PostVolumePass;
                this.ClearCommand(this.CompositionCommand, CameraEvent.AfterImageEffects);
                this.SetupCommand(this.CompositionCommand, CameraEvent.AfterImageEffects);
            }
        }

        private void HandleScreenshot()
        {
            if (this.MainComponent.screenshotSettings.takeScreenshot)
            {
                Material material = new Material(this.ShaderBicubic);
                RenderTexture renderTexture = new RenderTexture((int)this.MainComponent.screenshotSettings.outputResolution.x, (int)this.MainComponent.screenshotSettings.outputResolution.y, 24, RenderTextureFormat.ARGB32);
                bool sRGBWrite = GL.sRGBWrite;
                GL.sRGBWrite = true;
                if (this.MainComponent.screenshotSettings.useFilter)
                {
                    material.SetFloat("_ResizeWidth", (int)this.MainComponent.screenshotSettings.outputResolution.x);
                    material.SetFloat("_ResizeHeight", (int)this.MainComponent.screenshotSettings.outputResolution.y);
                    material.SetFloat("_Sharpness", 0.85f);
                    Graphics.Blit(this.Main.targetTexture, renderTexture, material, 0);
                }
                else
                {
                    Graphics.Blit(this.Main.targetTexture, renderTexture);
                }
                global::UnityEngine.Object.DestroyImmediate(material);
                RenderTexture.active = renderTexture;
                Texture2D texture2D = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGB24, mipChain: false);
                texture2D.ReadPixels(new Rect(0f, 0f, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
                new FileInfo(this.MainComponent.screenshotPath).Directory.Create();
                if (this.MainComponent.imageFormat == ImageFormat.PNG)
                {
                    File.WriteAllBytes(this.MainComponent.screenshotPath + this.GetScreenshotName + ".png", texture2D.EncodeToPNG());
                }
                else if (this.MainComponent.imageFormat == ImageFormat.JPG)
                {
                    File.WriteAllBytes(this.MainComponent.screenshotPath + this.GetScreenshotName + ".jpg", texture2D.EncodeToJPG(this.MainComponent.JPGQuality));
                }
                else
                {
                    File.WriteAllBytes(this.MainComponent.screenshotPath + this.GetScreenshotName + ".exr", texture2D.EncodeToEXR(this.MainComponent.EXR32 ? Texture2D.EXRFlags.OutputAsFloat : Texture2D.EXRFlags.None));
                }
                RenderTexture.active = null;
                renderTexture.Release();
                GL.sRGBWrite = sRGBWrite;
                global::UnityEngine.Object.DestroyImmediate(texture2D);
                this.MainComponent.screenshotSettings.takeScreenshot = false;
            }
        }

        private void SetupCBSRP()
        {
        }

        private void UpdateCBSRP()
        {
        }
    }
}
