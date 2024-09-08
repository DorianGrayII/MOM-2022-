using System;
using System.Runtime.InteropServices;

namespace UnityEngine.PostProcessing
{
    [Serializable]
    public class DitheringModel : PostProcessingModel
    {
        [Serializable]
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct Settings
        {
            public static Settings defaultSettings => default(Settings);
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
