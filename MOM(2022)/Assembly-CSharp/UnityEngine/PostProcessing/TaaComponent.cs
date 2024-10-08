using System;

namespace UnityEngine.PostProcessing
{
    public sealed class TaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
    {
        private static class Uniforms
        {
            internal static int _Jitter = Shader.PropertyToID("_Jitter");

            internal static int _SharpenParameters = Shader.PropertyToID("_SharpenParameters");

            internal static int _FinalBlendParameters = Shader.PropertyToID("_FinalBlendParameters");

            internal static int _HistoryTex = Shader.PropertyToID("_HistoryTex");

            internal static int _MainTex = Shader.PropertyToID("_MainTex");
        }

        private const string k_ShaderString = "Hidden/Post FX/Temporal Anti-aliasing";

        private const int k_SampleCount = 8;

        private readonly RenderBuffer[] m_MRT = new RenderBuffer[2];

        private int m_SampleIndex;

        private bool m_ResetHistory = true;

        private RenderTexture m_HistoryTexture;

        public override bool active
        {
            get
            {
                if (base.model.enabled && base.model.settings.method == AntialiasingModel.Method.Taa && SystemInfo.supportsMotionVectors && SystemInfo.supportedRenderTargetCount >= 2)
                {
                    return !base.context.interrupted;
                }
                return false;
            }
        }

        public Vector2 jitterVector { get; private set; }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
        }

        public void ResetHistory()
        {
            this.m_ResetHistory = true;
        }

        public void SetProjectionMatrix(Func<Vector2, Matrix4x4> jitteredFunc)
        {
            AntialiasingModel.TaaSettings taaSettings = base.model.settings.taaSettings;
            Vector2 vector = this.GenerateRandomOffset();
            vector *= taaSettings.jitterSpread;
            base.context.camera.nonJitteredProjectionMatrix = base.context.camera.projectionMatrix;
            if (jitteredFunc != null)
            {
                base.context.camera.projectionMatrix = jitteredFunc(vector);
            }
            else
            {
                base.context.camera.projectionMatrix = (base.context.camera.orthographic ? this.GetOrthographicProjectionMatrix(vector) : this.GetPerspectiveProjectionMatrix(vector));
            }
            base.context.camera.useJitteredProjectionMatrixForTransparentRendering = false;
            vector.x /= base.context.width;
            vector.y /= base.context.height;
            base.context.materialFactory.Get("Hidden/Post FX/Temporal Anti-aliasing").SetVector(Uniforms._Jitter, vector);
            this.jitterVector = vector;
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            Material material = base.context.materialFactory.Get("Hidden/Post FX/Temporal Anti-aliasing");
            material.shaderKeywords = null;
            AntialiasingModel.TaaSettings taaSettings = base.model.settings.taaSettings;
            if (this.m_ResetHistory || this.m_HistoryTexture == null || this.m_HistoryTexture.width != source.width || this.m_HistoryTexture.height != source.height)
            {
                if ((bool)this.m_HistoryTexture)
                {
                    RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
                }
                this.m_HistoryTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
                this.m_HistoryTexture.name = "TAA History";
                Graphics.Blit(source, this.m_HistoryTexture, material, 2);
            }
            material.SetVector(Uniforms._SharpenParameters, new Vector4(taaSettings.sharpen, 0f, 0f, 0f));
            material.SetVector(Uniforms._FinalBlendParameters, new Vector4(taaSettings.stationaryBlending, taaSettings.motionBlending, 6000f, 0f));
            material.SetTexture(Uniforms._MainTex, source);
            material.SetTexture(Uniforms._HistoryTex, this.m_HistoryTexture);
            RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            temporary.name = "TAA History";
            this.m_MRT[0] = destination.colorBuffer;
            this.m_MRT[1] = temporary.colorBuffer;
            Graphics.SetRenderTarget(this.m_MRT, source.depthBuffer);
            GraphicsUtils.Blit(material, base.context.camera.orthographic ? 1 : 0);
            RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
            this.m_HistoryTexture = temporary;
            this.m_ResetHistory = false;
        }

        private float GetHaltonValue(int index, int radix)
        {
            float num = 0f;
            float num2 = 1f / (float)radix;
            while (index > 0)
            {
                num += (float)(index % radix) * num2;
                index /= radix;
                num2 /= (float)radix;
            }
            return num;
        }

        private Vector2 GenerateRandomOffset()
        {
            Vector2 result = new Vector2(this.GetHaltonValue(this.m_SampleIndex & 0x3FF, 2), this.GetHaltonValue(this.m_SampleIndex & 0x3FF, 3));
            if (++this.m_SampleIndex >= 8)
            {
                this.m_SampleIndex = 0;
            }
            return result;
        }

        private Matrix4x4 GetPerspectiveProjectionMatrix(Vector2 offset)
        {
            float num = Mathf.Tan((float)Math.PI / 360f * base.context.camera.fieldOfView);
            float num2 = num * base.context.camera.aspect;
            offset.x *= num2 / (0.5f * (float)base.context.width);
            offset.y *= num / (0.5f * (float)base.context.height);
            float num3 = (offset.x - num2) * base.context.camera.nearClipPlane;
            float num4 = (offset.x + num2) * base.context.camera.nearClipPlane;
            float num5 = (offset.y + num) * base.context.camera.nearClipPlane;
            float num6 = (offset.y - num) * base.context.camera.nearClipPlane;
            Matrix4x4 result = default(Matrix4x4);
            result[0, 0] = 2f * base.context.camera.nearClipPlane / (num4 - num3);
            result[0, 1] = 0f;
            result[0, 2] = (num4 + num3) / (num4 - num3);
            result[0, 3] = 0f;
            result[1, 0] = 0f;
            result[1, 1] = 2f * base.context.camera.nearClipPlane / (num5 - num6);
            result[1, 2] = (num5 + num6) / (num5 - num6);
            result[1, 3] = 0f;
            result[2, 0] = 0f;
            result[2, 1] = 0f;
            result[2, 2] = (0f - (base.context.camera.farClipPlane + base.context.camera.nearClipPlane)) / (base.context.camera.farClipPlane - base.context.camera.nearClipPlane);
            result[2, 3] = (0f - 2f * base.context.camera.farClipPlane * base.context.camera.nearClipPlane) / (base.context.camera.farClipPlane - base.context.camera.nearClipPlane);
            result[3, 0] = 0f;
            result[3, 1] = 0f;
            result[3, 2] = -1f;
            result[3, 3] = 0f;
            return result;
        }

        private Matrix4x4 GetOrthographicProjectionMatrix(Vector2 offset)
        {
            float orthographicSize = base.context.camera.orthographicSize;
            float num = orthographicSize * base.context.camera.aspect;
            offset.x *= num / (0.5f * (float)base.context.width);
            offset.y *= orthographicSize / (0.5f * (float)base.context.height);
            float left = offset.x - num;
            float right = offset.x + num;
            float top = offset.y + orthographicSize;
            float bottom = offset.y - orthographicSize;
            return Matrix4x4.Ortho(left, right, bottom, top, base.context.camera.nearClipPlane, base.context.camera.farClipPlane);
        }

        public override void OnDisable()
        {
            if (this.m_HistoryTexture != null)
            {
                RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
            }
            this.m_HistoryTexture = null;
            this.m_SampleIndex = 0;
            this.ResetHistory();
        }
    }
}
