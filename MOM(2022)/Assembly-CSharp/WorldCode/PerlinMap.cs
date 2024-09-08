namespace WorldCode
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class PerlinMap
    {
        private Dictionary<Layer, Vector3> perlinSettings = new Dictionary<Layer, Vector3>();

        public Vector3 GetLayerData(Layer layer)
        {
            return this.perlinSettings[layer];
        }

        public void Initialize(MHRandom random, float scale)
        {
            if (scale <= 0f)
            {
                scale = 1f;
            }
            this.perlinSettings[Layer.Height1] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.015f, 0.023f));
            this.perlinSettings[Layer.Height2] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.045f, 0.075f));
            this.perlinSettings[Layer.Height3] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.1f, 0.15f));
            this.perlinSettings[Layer.Height4] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.3f, 0.35f));
            this.perlinSettings[Layer.Humidity1] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.04f, 0.07f));
            this.perlinSettings[Layer.Humidity2] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.08f, 0.1f));
            this.perlinSettings[Layer.Humidity3] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.12f, 0.18f));
            this.perlinSettings[Layer.HillAr1] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.02f, 0.04f));
            this.perlinSettings[Layer.HillAr2] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.15f, 0.25f));
            this.perlinSettings[Layer.HillMini] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.2f, 0.4f));
            this.perlinSettings[Layer.RivDiff] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.03f, 0.05f));
            this.perlinSettings[Layer.Forest1] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.12f, 0.15f));
            this.perlinSettings[Layer.Forest2] = new Vector3(random.GetFloat(0f, 1f), random.GetFloat(0f, 1f), scale * random.GetFloat(0.22f, 0.3f));
        }

        public void InitializeFactionNoise()
        {
            this.perlinSettings[Layer.FactionRegions] = new Vector3(UnityEngine.Random.Range((float) 0f, (float) 1f), UnityEngine.Random.Range((float) 0f, (float) 1f), UnityEngine.Random.Range((float) 0.03f, (float) 0.04f));
        }

        public float ProduceValueAtLayer(Layer layer, Vector2 pos)
        {
            Vector3 layerData = this.GetLayerData(layer);
            return Mathf.PerlinNoise(layerData.x + (pos.x * layerData.z), layerData.y + (pos.y * layerData.z));
        }

        public enum Layer
        {
            Height1,
            Height2,
            Height3,
            Height4,
            Humidity1,
            Humidity2,
            Humidity3,
            RivDiff,
            HillAr1,
            HillAr2,
            HillMini,
            Forest1,
            Forest2,
            FactionRegions,
            MAX
        }
    }
}

