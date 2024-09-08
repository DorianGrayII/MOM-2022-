namespace UnityEngine.PostProcessing
{
    using System;
    using UnityEngine;

    public sealed class FxaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
    {
        public void Render(RenderTexture source, RenderTexture destination)
        {
            AntialiasingModel.FxaaSettings fxaaSettings = base.model.settings.fxaaSettings;
            Material mat = base.context.materialFactory.Get("Hidden/Post FX/FXAA");
            AntialiasingModel.FxaaQualitySettings settings2 = AntialiasingModel.FxaaQualitySettings.presets[(int) fxaaSettings.preset];
            AntialiasingModel.FxaaConsoleSettings settings3 = AntialiasingModel.FxaaConsoleSettings.presets[(int) fxaaSettings.preset];
            mat.SetVector(Uniforms._QualitySettings, new Vector3(settings2.subpixelAliasingRemovalAmount, settings2.edgeDetectionThreshold, settings2.minimumRequiredLuminance));
            mat.SetVector(Uniforms._ConsoleSettings, new Vector4(settings3.subpixelSpreadAmount, settings3.edgeSharpnessAmount, settings3.edgeDetectionThreshold, settings3.minimumRequiredLuminance));
            Graphics.Blit(source, destination, mat, 0);
        }

        public override bool active
        {
            get
            {
                return (base.model.enabled && ((base.model.settings.method == AntialiasingModel.Method.Fxaa) && !base.context.interrupted));
            }
        }

        private static class Uniforms
        {
            internal static readonly int _QualitySettings = Shader.PropertyToID("_QualitySettings");
            internal static readonly int _ConsoleSettings = Shader.PropertyToID("_ConsoleSettings");
        }
    }
}

