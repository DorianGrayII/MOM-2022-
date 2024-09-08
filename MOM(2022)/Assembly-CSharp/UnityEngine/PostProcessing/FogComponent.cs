namespace UnityEngine.PostProcessing
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    public sealed class FogComponent : PostProcessingComponentCommandBuffer<FogModel>
    {
        private const string k_ShaderString = "Hidden/Post FX/Fog";

        public override CameraEvent GetCameraEvent()
        {
            return CameraEvent.AfterImageEffectsOpaque;
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        public override string GetName()
        {
            return "Fog";
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
            FogModel.Settings settings = base.model.settings;
            Material mat = base.context.materialFactory.Get("Hidden/Post FX/Fog");
            mat.shaderKeywords = null;
            Color color = GraphicsUtils.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor;
            mat.SetColor(Uniforms._FogColor, color);
            mat.SetFloat(Uniforms._Density, RenderSettings.fogDensity);
            mat.SetFloat(Uniforms._Start, RenderSettings.fogStartDistance);
            mat.SetFloat(Uniforms._End, RenderSettings.fogEndDistance);
            switch (RenderSettings.fogMode)
            {
                case FogMode.Linear:
                    mat.EnableKeyword("FOG_LINEAR");
                    break;

                case FogMode.Exponential:
                    mat.EnableKeyword("FOG_EXP");
                    break;

                case FogMode.ExponentialSquared:
                    mat.EnableKeyword("FOG_EXP2");
                    break;

                default:
                    break;
            }
            RenderTextureFormat format = base.context.isHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
            cb.GetTemporaryRT(Uniforms._TempRT, base.context.width, base.context.height, 0x18, FilterMode.Bilinear, format);
            cb.Blit(2, Uniforms._TempRT);
            cb.Blit(Uniforms._TempRT, 2, mat, settings.excludeSkybox ? 1 : 0);
            cb.ReleaseTemporaryRT(Uniforms._TempRT);
        }

        public override bool active
        {
            get
            {
                return (base.model.enabled && (base.context.isGBufferAvailable && (RenderSettings.fog && !base.context.interrupted)));
            }
        }

        private static class Uniforms
        {
            internal static readonly int _FogColor = Shader.PropertyToID("_FogColor");
            internal static readonly int _Density = Shader.PropertyToID("_Density");
            internal static readonly int _Start = Shader.PropertyToID("_Start");
            internal static readonly int _End = Shader.PropertyToID("_End");
            internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");
        }
    }
}

