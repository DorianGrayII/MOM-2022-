namespace UnityEngine.PostProcessing
{
    public sealed class DitheringComponent : PostProcessingComponentRenderTexture<DitheringModel>
    {
        private static class Uniforms
        {
            internal static readonly int _DitheringTex = Shader.PropertyToID("_DitheringTex");

            internal static readonly int _DitheringCoords = Shader.PropertyToID("_DitheringCoords");
        }

        private Texture2D[] noiseTextures;

        private int textureIndex;

        private const int k_TextureCount = 64;

        public override bool active
        {
            get
            {
                if (base.model.enabled)
                {
                    return !base.context.interrupted;
                }
                return false;
            }
        }

        public override void OnDisable()
        {
            this.noiseTextures = null;
        }

        private void LoadNoiseTextures()
        {
            this.noiseTextures = new Texture2D[64];
            for (int i = 0; i < 64; i++)
            {
                this.noiseTextures[i] = Resources.Load<Texture2D>("Bluenoise64/LDR_LLL1_" + i);
            }
        }

        public override void Prepare(Material uberMaterial)
        {
            if (++this.textureIndex >= 64)
            {
                this.textureIndex = 0;
            }
            float value = Random.value;
            float value2 = Random.value;
            if (this.noiseTextures == null)
            {
                this.LoadNoiseTextures();
            }
            Texture2D texture2D = this.noiseTextures[this.textureIndex];
            uberMaterial.EnableKeyword("DITHERING");
            uberMaterial.SetTexture(Uniforms._DitheringTex, texture2D);
            uberMaterial.SetVector(Uniforms._DitheringCoords, new Vector4((float)base.context.width / (float)texture2D.width, (float)base.context.height / (float)texture2D.height, value, value2));
        }
    }
}
