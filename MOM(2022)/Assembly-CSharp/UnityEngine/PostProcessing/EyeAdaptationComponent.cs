namespace UnityEngine.PostProcessing
{
    public sealed class EyeAdaptationComponent : PostProcessingComponentRenderTexture<EyeAdaptationModel>
    {
        private static class Uniforms
        {
            internal static readonly int _Params = Shader.PropertyToID("_Params");

            internal static readonly int _Speed = Shader.PropertyToID("_Speed");

            internal static readonly int _ScaleOffsetRes = Shader.PropertyToID("_ScaleOffsetRes");

            internal static readonly int _ExposureCompensation = Shader.PropertyToID("_ExposureCompensation");

            internal static readonly int _AutoExposure = Shader.PropertyToID("_AutoExposure");

            internal static readonly int _DebugWidth = Shader.PropertyToID("_DebugWidth");
        }

        private ComputeShader m_EyeCompute;

        private ComputeBuffer m_HistogramBuffer;

        private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];

        private int m_AutoExposurePingPing;

        private RenderTexture m_CurrentAutoExposure;

        private RenderTexture m_DebugHistogram;

        private static uint[] s_EmptyHistogramBuffer;

        private bool m_FirstFrame = true;

        private const int k_HistogramBins = 64;

        private const int k_HistogramThreadX = 16;

        private const int k_HistogramThreadY = 16;

        public override bool active
        {
            get
            {
                if (base.model.enabled && SystemInfo.supportsComputeShaders)
                {
                    return !base.context.interrupted;
                }
                return false;
            }
        }

        public void ResetHistory()
        {
            this.m_FirstFrame = true;
        }

        public override void OnEnable()
        {
            this.m_FirstFrame = true;
        }

        public override void OnDisable()
        {
            RenderTexture[] autoExposurePool = this.m_AutoExposurePool;
            for (int i = 0; i < autoExposurePool.Length; i++)
            {
                GraphicsUtils.Destroy(autoExposurePool[i]);
            }
            if (this.m_HistogramBuffer != null)
            {
                this.m_HistogramBuffer.Release();
            }
            this.m_HistogramBuffer = null;
            if (this.m_DebugHistogram != null)
            {
                this.m_DebugHistogram.Release();
            }
            this.m_DebugHistogram = null;
        }

        private Vector4 GetHistogramScaleOffsetRes()
        {
            EyeAdaptationModel.Settings settings = base.model.settings;
            float num = settings.logMax - settings.logMin;
            float num2 = 1f / num;
            float y = (float)(-settings.logMin) * num2;
            return new Vector4(num2, y, Mathf.Floor((float)base.context.width / 2f), Mathf.Floor((float)base.context.height / 2f));
        }

        public Texture Prepare(RenderTexture source, Material uberMaterial)
        {
            EyeAdaptationModel.Settings settings = base.model.settings;
            if (this.m_EyeCompute == null)
            {
                this.m_EyeCompute = Resources.Load<ComputeShader>("Shaders/EyeHistogram");
            }
            Material material = base.context.materialFactory.Get("Hidden/Post FX/Eye Adaptation");
            material.shaderKeywords = null;
            if (this.m_HistogramBuffer == null)
            {
                this.m_HistogramBuffer = new ComputeBuffer(64, 4);
            }
            if (EyeAdaptationComponent.s_EmptyHistogramBuffer == null)
            {
                EyeAdaptationComponent.s_EmptyHistogramBuffer = new uint[64];
            }
            Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes();
            RenderTexture renderTexture = base.context.renderTextureFactory.Get((int)histogramScaleOffsetRes.z, (int)histogramScaleOffsetRes.w, 0, source.format);
            Graphics.Blit(source, renderTexture);
            if (this.m_AutoExposurePool[0] == null || !this.m_AutoExposurePool[0].IsCreated())
            {
                this.m_AutoExposurePool[0] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
            }
            if (this.m_AutoExposurePool[1] == null || !this.m_AutoExposurePool[1].IsCreated())
            {
                this.m_AutoExposurePool[1] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
            }
            this.m_HistogramBuffer.SetData(EyeAdaptationComponent.s_EmptyHistogramBuffer);
            int kernelIndex = this.m_EyeCompute.FindKernel("KEyeHistogram");
            this.m_EyeCompute.SetBuffer(kernelIndex, "_Histogram", this.m_HistogramBuffer);
            this.m_EyeCompute.SetTexture(kernelIndex, "_Source", renderTexture);
            this.m_EyeCompute.SetVector("_ScaleOffsetRes", histogramScaleOffsetRes);
            this.m_EyeCompute.Dispatch(kernelIndex, Mathf.CeilToInt((float)renderTexture.width / 16f), Mathf.CeilToInt((float)renderTexture.height / 16f), 1);
            base.context.renderTextureFactory.Release(renderTexture);
            settings.highPercent = Mathf.Clamp(settings.highPercent, 1.01f, 99f);
            settings.lowPercent = Mathf.Clamp(settings.lowPercent, 1f, settings.highPercent - 0.01f);
            material.SetBuffer("_Histogram", this.m_HistogramBuffer);
            material.SetVector(Uniforms._Params, new Vector4(settings.lowPercent * 0.01f, settings.highPercent * 0.01f, Mathf.Exp(settings.minLuminance * 0.6931472f), Mathf.Exp(settings.maxLuminance * 0.6931472f)));
            material.SetVector(Uniforms._Speed, new Vector2(settings.speedDown, settings.speedUp));
            material.SetVector(Uniforms._ScaleOffsetRes, histogramScaleOffsetRes);
            material.SetFloat(Uniforms._ExposureCompensation, settings.keyValue);
            if (settings.dynamicKeyValue)
            {
                material.EnableKeyword("AUTO_KEY_VALUE");
            }
            if (this.m_FirstFrame || !Application.isPlaying)
            {
                this.m_CurrentAutoExposure = this.m_AutoExposurePool[0];
                Graphics.Blit(null, this.m_CurrentAutoExposure, material, 1);
                Graphics.Blit(this.m_AutoExposurePool[0], this.m_AutoExposurePool[1]);
            }
            else
            {
                int autoExposurePingPing = this.m_AutoExposurePingPing;
                RenderTexture source2 = this.m_AutoExposurePool[++autoExposurePingPing % 2];
                RenderTexture renderTexture2 = this.m_AutoExposurePool[++autoExposurePingPing % 2];
                Graphics.Blit(source2, renderTexture2, material, (int)settings.adaptationType);
                this.m_AutoExposurePingPing = ++autoExposurePingPing % 2;
                this.m_CurrentAutoExposure = renderTexture2;
            }
            if (base.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
            {
                if (this.m_DebugHistogram == null || !this.m_DebugHistogram.IsCreated())
                {
                    this.m_DebugHistogram = new RenderTexture(256, 128, 0, RenderTextureFormat.ARGB32)
                    {
                        filterMode = FilterMode.Point,
                        wrapMode = TextureWrapMode.Clamp
                    };
                }
                material.SetFloat(Uniforms._DebugWidth, this.m_DebugHistogram.width);
                Graphics.Blit(null, this.m_DebugHistogram, material, 2);
            }
            this.m_FirstFrame = false;
            return this.m_CurrentAutoExposure;
        }

        public void OnGUI()
        {
            if (!(this.m_DebugHistogram == null) && this.m_DebugHistogram.IsCreated())
            {
                GUI.DrawTexture(new Rect(base.context.viewport.x * (float)Screen.width + 8f, 8f, this.m_DebugHistogram.width, this.m_DebugHistogram.height), this.m_DebugHistogram);
            }
        }
    }
}
