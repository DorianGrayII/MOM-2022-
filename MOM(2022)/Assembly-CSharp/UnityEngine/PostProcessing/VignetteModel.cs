﻿namespace UnityEngine.PostProcessing
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    public class VignetteModel : PostProcessingModel
    {
        [SerializeField]
        private Settings m_Settings = Settings.defaultSettings;

        public override void Reset()
        {
            this.m_Settings = Settings.defaultSettings;
        }

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

        public enum Mode
        {
            Classic,
            Masked
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Settings
        {
            [Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
            public VignetteModel.Mode mode;
            [ColorUsage(false), Tooltip("Vignette color. Use the alpha channel for transparency.")]
            public Color color;
            [Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
            public Vector2 center;
            [Range(0f, 1f), Tooltip("Amount of vignetting on screen.")]
            public float intensity;
            [Range(0.01f, 1f), Tooltip("Smoothness of the vignette borders.")]
            public float smoothness;
            [Range(0f, 1f), Tooltip("Lower values will make a square-ish vignette.")]
            public float roundness;
            [Tooltip("A black and white mask to use as a vignette.")]
            public Texture mask;
            [Range(0f, 1f), Tooltip("Mask opacity.")]
            public float opacity;
            [Tooltip("Should the vignette be perfectly round or be dependent on the current aspect ratio?")]
            public bool rounded;
            public static VignetteModel.Settings defaultSettings
            {
                get
                {
                    return new VignetteModel.Settings { 
                        mode = VignetteModel.Mode.Classic,
                        color = new Color(0f, 0f, 0f, 1f),
                        center = new Vector2(0.5f, 0.5f),
                        intensity = 0.45f,
                        smoothness = 0.2f,
                        roundness = 1f,
                        mask = null,
                        opacity = 1f,
                        rounded = false
                    };
                }
            }
        }
    }
}

