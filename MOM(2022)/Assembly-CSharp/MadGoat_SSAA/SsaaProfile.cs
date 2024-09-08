using System;
using UnityEngine;

namespace MadGoat_SSAA
{
    [Serializable]
    public class SsaaProfile
    {
        [HideInInspector]
        public float multiplier;

        public bool useFilter;

        [Tooltip("Which type of filtering to be used (only applied if useShader is true)")]
        public Filter filterType = Filter.BILINEAR;

        [Tooltip("The sharpness of the filtered image (only applied if useShader is true)")]
        [Range(0f, 1f)]
        public float sharpness;

        [Tooltip("The distance between the samples (only applied if useShader is true)")]
        [Range(0.5f, 2f)]
        public float sampleDistance;

        public SsaaProfile(float mul, bool useDownsampling)
        {
            this.multiplier = mul;
            this.useFilter = useDownsampling;
            this.sharpness = (useDownsampling ? 0.85f : 0f);
            this.sampleDistance = (useDownsampling ? 0.65f : 0f);
        }

        public SsaaProfile(float mul, bool useDownsampling, Filter filterType, float sharp, float sampleDist)
        {
            this.multiplier = mul;
            this.filterType = filterType;
            this.useFilter = useDownsampling;
            this.sharpness = (useDownsampling ? sharp : 0f);
            this.sampleDistance = (useDownsampling ? sampleDist : 0f);
        }
    }
}
