using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
    public sealed class MotionBlurComponent : PostProcessingComponentCommandBuffer<MotionBlurModel>
    {
        private static class Uniforms
        {
            internal static readonly int _VelocityScale = Shader.PropertyToID("_VelocityScale");

            internal static readonly int _MaxBlurRadius = Shader.PropertyToID("_MaxBlurRadius");

            internal static readonly int _RcpMaxBlurRadius = Shader.PropertyToID("_RcpMaxBlurRadius");

            internal static readonly int _VelocityTex = Shader.PropertyToID("_VelocityTex");

            internal static readonly int _MainTex = Shader.PropertyToID("_MainTex");

            internal static readonly int _Tile2RT = Shader.PropertyToID("_Tile2RT");

            internal static readonly int _Tile4RT = Shader.PropertyToID("_Tile4RT");

            internal static readonly int _Tile8RT = Shader.PropertyToID("_Tile8RT");

            internal static readonly int _TileMaxOffs = Shader.PropertyToID("_TileMaxOffs");

            internal static readonly int _TileMaxLoop = Shader.PropertyToID("_TileMaxLoop");

            internal static readonly int _TileVRT = Shader.PropertyToID("_TileVRT");

            internal static readonly int _NeighborMaxTex = Shader.PropertyToID("_NeighborMaxTex");

            internal static readonly int _LoopCount = Shader.PropertyToID("_LoopCount");

            internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");

            internal static readonly int _History1LumaTex = Shader.PropertyToID("_History1LumaTex");

            internal static readonly int _History2LumaTex = Shader.PropertyToID("_History2LumaTex");

            internal static readonly int _History3LumaTex = Shader.PropertyToID("_History3LumaTex");

            internal static readonly int _History4LumaTex = Shader.PropertyToID("_History4LumaTex");

            internal static readonly int _History1ChromaTex = Shader.PropertyToID("_History1ChromaTex");

            internal static readonly int _History2ChromaTex = Shader.PropertyToID("_History2ChromaTex");

            internal static readonly int _History3ChromaTex = Shader.PropertyToID("_History3ChromaTex");

            internal static readonly int _History4ChromaTex = Shader.PropertyToID("_History4ChromaTex");

            internal static readonly int _History1Weight = Shader.PropertyToID("_History1Weight");

            internal static readonly int _History2Weight = Shader.PropertyToID("_History2Weight");

            internal static readonly int _History3Weight = Shader.PropertyToID("_History3Weight");

            internal static readonly int _History4Weight = Shader.PropertyToID("_History4Weight");
        }

        private enum Pass
        {
            VelocitySetup = 0,
            TileMax1 = 1,
            TileMax2 = 2,
            TileMaxV = 3,
            NeighborMax = 4,
            Reconstruction = 5,
            FrameCompression = 6,
            FrameBlendingChroma = 7,
            FrameBlendingRaw = 8
        }

        public class ReconstructionFilter
        {
            private RenderTextureFormat m_VectorRTFormat = RenderTextureFormat.RGHalf;

            private RenderTextureFormat m_PackedRTFormat = RenderTextureFormat.ARGB2101010;

            public ReconstructionFilter()
            {
                this.CheckTextureFormatSupport();
            }

            private void CheckTextureFormatSupport()
            {
                if (!SystemInfo.SupportsRenderTextureFormat(this.m_PackedRTFormat))
                {
                    this.m_PackedRTFormat = RenderTextureFormat.ARGB32;
                }
            }

            public bool IsSupported()
            {
                return SystemInfo.supportsMotionVectors;
            }

            public void ProcessImage(PostProcessingContext context, CommandBuffer cb, ref MotionBlurModel.Settings settings, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material)
            {
                int num = (int)(5f * (float)context.height / 100f);
                int num2 = ((num - 1) / 8 + 1) * 8;
                float value = settings.shutterAngle / 360f;
                cb.SetGlobalFloat(Uniforms._VelocityScale, value);
                cb.SetGlobalFloat(Uniforms._MaxBlurRadius, num);
                cb.SetGlobalFloat(Uniforms._RcpMaxBlurRadius, 1f / (float)num);
                int velocityTex = Uniforms._VelocityTex;
                cb.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, this.m_PackedRTFormat, RenderTextureReadWrite.Linear);
                cb.Blit(null, velocityTex, material, 0);
                int tile2RT = Uniforms._Tile2RT;
                cb.GetTemporaryRT(tile2RT, context.width / 2, context.height / 2, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, velocityTex);
                cb.Blit(velocityTex, tile2RT, material, 1);
                int tile4RT = Uniforms._Tile4RT;
                cb.GetTemporaryRT(tile4RT, context.width / 4, context.height / 4, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile2RT);
                cb.Blit(tile2RT, tile4RT, material, 2);
                cb.ReleaseTemporaryRT(tile2RT);
                int tile8RT = Uniforms._Tile8RT;
                cb.GetTemporaryRT(tile8RT, context.width / 8, context.height / 8, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile4RT);
                cb.Blit(tile4RT, tile8RT, material, 2);
                cb.ReleaseTemporaryRT(tile4RT);
                Vector2 vector = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
                cb.SetGlobalVector(Uniforms._TileMaxOffs, vector);
                cb.SetGlobalFloat(Uniforms._TileMaxLoop, (int)((float)num2 / 8f));
                int tileVRT = Uniforms._TileVRT;
                cb.GetTemporaryRT(tileVRT, context.width / num2, context.height / num2, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile8RT);
                cb.Blit(tile8RT, tileVRT, material, 3);
                cb.ReleaseTemporaryRT(tile8RT);
                int neighborMaxTex = Uniforms._NeighborMaxTex;
                int width = context.width / num2;
                int height = context.height / num2;
                cb.GetTemporaryRT(neighborMaxTex, width, height, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tileVRT);
                cb.Blit(tileVRT, neighborMaxTex, material, 4);
                cb.ReleaseTemporaryRT(tileVRT);
                cb.SetGlobalFloat(Uniforms._LoopCount, Mathf.Clamp(settings.sampleCount / 2, 1, 64));
                cb.SetGlobalTexture(Uniforms._MainTex, source);
                cb.Blit(source, destination, material, 5);
                cb.ReleaseTemporaryRT(velocityTex);
                cb.ReleaseTemporaryRT(neighborMaxTex);
            }
        }

        public class FrameBlendingFilter
        {
            private struct Frame
            {
                public RenderTexture lumaTexture;

                public RenderTexture chromaTexture;

                private float m_Time;

                private RenderTargetIdentifier[] m_MRT;

                public float CalculateWeight(float strength, float currentTime)
                {
                    if (Mathf.Approximately(this.m_Time, 0f))
                    {
                        return 0f;
                    }
                    float num = Mathf.Lerp(80f, 16f, strength);
                    return Mathf.Exp((this.m_Time - currentTime) * num);
                }

                public void Release()
                {
                    if (this.lumaTexture != null)
                    {
                        RenderTexture.ReleaseTemporary(this.lumaTexture);
                    }
                    if (this.chromaTexture != null)
                    {
                        RenderTexture.ReleaseTemporary(this.chromaTexture);
                    }
                    this.lumaTexture = null;
                    this.chromaTexture = null;
                }

                public void MakeRecord(CommandBuffer cb, RenderTargetIdentifier source, int width, int height, Material material)
                {
                    this.Release();
                    this.lumaTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
                    this.chromaTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
                    this.lumaTexture.filterMode = FilterMode.Point;
                    this.chromaTexture.filterMode = FilterMode.Point;
                    if (this.m_MRT == null)
                    {
                        this.m_MRT = new RenderTargetIdentifier[2];
                    }
                    this.m_MRT[0] = this.lumaTexture;
                    this.m_MRT[1] = this.chromaTexture;
                    cb.SetGlobalTexture(Uniforms._MainTex, source);
                    cb.SetRenderTarget(this.m_MRT, this.lumaTexture);
                    cb.DrawMesh(GraphicsUtils.quad, Matrix4x4.identity, material, 0, 6);
                    this.m_Time = Time.time;
                }

                public void MakeRecordRaw(CommandBuffer cb, RenderTargetIdentifier source, int width, int height, RenderTextureFormat format)
                {
                    this.Release();
                    this.lumaTexture = RenderTexture.GetTemporary(width, height, 0, format);
                    this.lumaTexture.filterMode = FilterMode.Point;
                    cb.SetGlobalTexture(Uniforms._MainTex, source);
                    cb.Blit(source, this.lumaTexture);
                    this.m_Time = Time.time;
                }
            }

            private bool m_UseCompression;

            private RenderTextureFormat m_RawTextureFormat;

            private Frame[] m_FrameList;

            private int m_LastFrameCount;

            public FrameBlendingFilter()
            {
                this.m_UseCompression = FrameBlendingFilter.CheckSupportCompression();
                this.m_RawTextureFormat = FrameBlendingFilter.GetPreferredRenderTextureFormat();
                this.m_FrameList = new Frame[4];
            }

            public void Dispose()
            {
                Frame[] frameList = this.m_FrameList;
                foreach (Frame frame in frameList)
                {
                    frame.Release();
                }
            }

            public void PushFrame(CommandBuffer cb, RenderTargetIdentifier source, int width, int height, Material material)
            {
                int frameCount = Time.frameCount;
                if (frameCount != this.m_LastFrameCount)
                {
                    int num = frameCount % this.m_FrameList.Length;
                    if (this.m_UseCompression)
                    {
                        this.m_FrameList[num].MakeRecord(cb, source, width, height, material);
                    }
                    else
                    {
                        this.m_FrameList[num].MakeRecordRaw(cb, source, width, height, this.m_RawTextureFormat);
                    }
                    this.m_LastFrameCount = frameCount;
                }
            }

            public void BlendFrames(CommandBuffer cb, float strength, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material)
            {
                float time = Time.time;
                Frame frameRelative = this.GetFrameRelative(-1);
                Frame frameRelative2 = this.GetFrameRelative(-2);
                Frame frameRelative3 = this.GetFrameRelative(-3);
                Frame frameRelative4 = this.GetFrameRelative(-4);
                cb.SetGlobalTexture(Uniforms._History1LumaTex, frameRelative.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History2LumaTex, frameRelative2.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History3LumaTex, frameRelative3.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History4LumaTex, frameRelative4.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History1ChromaTex, frameRelative.chromaTexture);
                cb.SetGlobalTexture(Uniforms._History2ChromaTex, frameRelative2.chromaTexture);
                cb.SetGlobalTexture(Uniforms._History3ChromaTex, frameRelative3.chromaTexture);
                cb.SetGlobalTexture(Uniforms._History4ChromaTex, frameRelative4.chromaTexture);
                cb.SetGlobalFloat(Uniforms._History1Weight, frameRelative.CalculateWeight(strength, time));
                cb.SetGlobalFloat(Uniforms._History2Weight, frameRelative2.CalculateWeight(strength, time));
                cb.SetGlobalFloat(Uniforms._History3Weight, frameRelative3.CalculateWeight(strength, time));
                cb.SetGlobalFloat(Uniforms._History4Weight, frameRelative4.CalculateWeight(strength, time));
                cb.SetGlobalTexture(Uniforms._MainTex, source);
                cb.Blit(source, destination, material, this.m_UseCompression ? 7 : 8);
            }

            private static bool CheckSupportCompression()
            {
                if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8))
                {
                    return SystemInfo.supportedRenderTargetCount > 1;
                }
                return false;
            }

            private static RenderTextureFormat GetPreferredRenderTextureFormat()
            {
                RenderTextureFormat[] array = new RenderTextureFormat[3]
                {
                    RenderTextureFormat.RGB565,
                    RenderTextureFormat.ARGB1555,
                    RenderTextureFormat.ARGB4444
                };
                foreach (RenderTextureFormat renderTextureFormat in array)
                {
                    if (SystemInfo.SupportsRenderTextureFormat(renderTextureFormat))
                    {
                        return renderTextureFormat;
                    }
                }
                return RenderTextureFormat.Default;
            }

            private Frame GetFrameRelative(int offset)
            {
                int num = (Time.frameCount + this.m_FrameList.Length + offset) % this.m_FrameList.Length;
                return this.m_FrameList[num];
            }
        }

        private ReconstructionFilter m_ReconstructionFilter;

        private FrameBlendingFilter m_FrameBlendingFilter;

        private bool m_FirstFrame = true;

        public ReconstructionFilter reconstructionFilter
        {
            get
            {
                if (this.m_ReconstructionFilter == null)
                {
                    this.m_ReconstructionFilter = new ReconstructionFilter();
                }
                return this.m_ReconstructionFilter;
            }
        }

        public FrameBlendingFilter frameBlendingFilter
        {
            get
            {
                if (this.m_FrameBlendingFilter == null)
                {
                    this.m_FrameBlendingFilter = new FrameBlendingFilter();
                }
                return this.m_FrameBlendingFilter;
            }
        }

        public override bool active
        {
            get
            {
                MotionBlurModel.Settings settings = base.model.settings;
                if (base.model.enabled && ((settings.shutterAngle > 0f && this.reconstructionFilter.IsSupported()) || settings.frameBlending > 0f) && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2)
                {
                    return !base.context.interrupted;
                }
                return false;
            }
        }

        public override string GetName()
        {
            return "Motion Blur";
        }

        public void ResetHistory()
        {
            if (this.m_FrameBlendingFilter != null)
            {
                this.m_FrameBlendingFilter.Dispose();
            }
            this.m_FrameBlendingFilter = null;
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
        }

        public override CameraEvent GetCameraEvent()
        {
            return CameraEvent.BeforeImageEffects;
        }

        public override void OnEnable()
        {
            this.m_FirstFrame = true;
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
            if (this.m_FirstFrame)
            {
                this.m_FirstFrame = false;
                return;
            }
            Material material = base.context.materialFactory.Get("Hidden/Post FX/Motion Blur");
            Material mat = base.context.materialFactory.Get("Hidden/Post FX/Blit");
            MotionBlurModel.Settings settings = base.model.settings;
            RenderTextureFormat format = (base.context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
            int tempRT = Uniforms._TempRT;
            cb.GetTemporaryRT(tempRT, base.context.width, base.context.height, 0, FilterMode.Point, format);
            if (settings.shutterAngle > 0f && settings.frameBlending > 0f)
            {
                this.reconstructionFilter.ProcessImage(base.context, cb, ref settings, BuiltinRenderTextureType.CameraTarget, tempRT, material);
                this.frameBlendingFilter.BlendFrames(cb, settings.frameBlending, tempRT, BuiltinRenderTextureType.CameraTarget, material);
                this.frameBlendingFilter.PushFrame(cb, tempRT, base.context.width, base.context.height, material);
            }
            else if (settings.shutterAngle > 0f)
            {
                cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
                cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRT, mat, 0);
                this.reconstructionFilter.ProcessImage(base.context, cb, ref settings, tempRT, BuiltinRenderTextureType.CameraTarget, material);
            }
            else if (settings.frameBlending > 0f)
            {
                cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
                cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRT, mat, 0);
                this.frameBlendingFilter.BlendFrames(cb, settings.frameBlending, tempRT, BuiltinRenderTextureType.CameraTarget, material);
                this.frameBlendingFilter.PushFrame(cb, tempRT, base.context.width, base.context.height, material);
            }
            cb.ReleaseTemporaryRT(tempRT);
        }

        public override void OnDisable()
        {
            if (this.m_FrameBlendingFilter != null)
            {
                this.m_FrameBlendingFilter.Dispose();
            }
        }
    }
}
