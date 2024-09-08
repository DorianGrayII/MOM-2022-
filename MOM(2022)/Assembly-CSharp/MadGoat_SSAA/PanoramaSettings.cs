namespace MadGoat_SSAA
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PanoramaSettings
    {
        public int panoramaSize;
        [Range(1f, 4f)]
        public int panoramaMultiplier;
        public bool useFilter = true;
        [Range(0f, 1f)]
        public float sharpness = 0.85f;

        public PanoramaSettings(int size, int mul)
        {
            this.panoramaMultiplier = mul;
            this.panoramaSize = size;
        }
    }
}

