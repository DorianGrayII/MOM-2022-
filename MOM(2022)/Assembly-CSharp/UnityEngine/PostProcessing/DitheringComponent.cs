namespace UnityEngine.PostProcessing
{
    using System;
    using UnityEngine;

    public sealed class DitheringComponent : PostProcessingComponentRenderTexture<DitheringModel>
    {
        private Texture2D[] noiseTextures;
        private int textureIndex;
        private const int k_TextureCount = 0x40;

        private void LoadNoiseTextures()
        {
            this.noiseTextures = new Texture2D[0x40];
            for (int i = 0; i < 0x40; i++)
            {
                this.noiseTextures[i] = Resources.Load<Texture2D>("Bluenoise64/LDR_LLL1_" + i.ToString());
            }
        }

        public override void OnDisable()
        {
            this.noiseTextures = null;
        }

        public override void Prepare(Material uberMaterial)
        {
            int num3 = this.textureIndex + 1;
            this.textureIndex = num3;
            if (num3 >= 0x40)
            {
                this.textureIndex = 0;
            }
            float z = UnityEngine.Random.value;
            float w = UnityEngine.Random.value;
            if (this.noiseTextures == null)
            {
                this.LoadNoiseTextures();
            }
            Texture2D textured = this.noiseTextures[this.textureIndex];
            uberMaterial.EnableKeyword("DITHERING");
            uberMaterial.SetTexture(Uniforms._DitheringTex, textured);
            uberMaterial.SetVector(Uniforms._DitheringCoords, new Vector4(((float) base.context.width) / ((float) textured.width), ((float) base.context.height) / ((float) textured.height), z, w));
        }

        public override bool active
        {
            get
            {
                return (base.model.enabled && !base.context.interrupted);
            }
        }

        private static class Uniforms
        {
            internal static readonly int _DitheringTex = Shader.PropertyToID("_DitheringTex");
            internal static readonly int _DitheringCoords = Shader.PropertyToID("_DitheringCoords");
        }
    }
}

