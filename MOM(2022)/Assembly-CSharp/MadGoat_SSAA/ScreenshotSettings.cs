namespace MadGoat_SSAA
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ScreenshotSettings
    {
        [HideInInspector]
        public bool takeScreenshot;
        [Range(1f, 4f)]
        public int screenshotMultiplier = 1;
        public Vector2 outputResolution = new Vector2(1920f, 1080f);
        public bool useFilter = true;
        [Range(0f, 1f)]
        public float sharpness = 0.85f;
    }
}

