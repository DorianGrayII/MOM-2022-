using UnityEngine;

namespace MadGoat_SSAA
{
    public static class MadGoatSSAA_Utils
    {
        public const string ssaa_version = "1.9.2";

        public static void CopyFrom(this Camera current, Camera other, RenderTexture rt)
        {
            current.CopyFrom(other);
            current.targetTexture = rt;
        }

        public static bool DetectSRP()
        {
            return false;
        }
    }
}
