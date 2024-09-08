using System;

namespace UnityEngine.PostProcessing
{
    [Serializable]
    public class GrainModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            [Tooltip("Enable the use of colored grain.")]
            public bool colored;

            [Range(0f, 1f)]
            [Tooltip("Grain strength. Higher means more visible grain.")]
            public float intensity;

            [Range(0.3f, 3f)]
            [Tooltip("Grain particle size.")]
            public float size;

            [Range(0f, 1f)]
            [Tooltip("Controls the noisiness response curve based on scene luminance. Lower values mean less noise in dark areas.")]
            public float luminanceContribution;

            public static Settings defaultSettings
            {
                get
                {
                    Settings result = default(Settings);
                    result.colored = true;
                    result.intensity = 0.5f;
                    result.size = 1f;
                    result.luminanceContribution = 0.8f;
                    return result;
                }
            }
        }

        [SerializeField]
        private Settings m_Settings = Settings.defaultSettings;

        public Settings settings
        {
            get
            {
                return this.m_Settings;
            }
            set
            {
                this.m_Settings = value;
            }
        }

        public override void Reset()
        {
            this.m_Settings = Settings.defaultSettings;
        }
    }
}
