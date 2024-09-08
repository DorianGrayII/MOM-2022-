using System.Diagnostics;
using UnityEngine;

namespace MHUtils
{
    public class MHTimer
    {
        private Stopwatch time;

        private float unityTimer;

        private MHTimer()
        {
        }

        public static MHTimer StartNew()
        {
            return new MHTimer
            {
                time = Stopwatch.StartNew()
            };
        }

        public static MHTimer StartUnityTimer()
        {
            return new MHTimer
            {
                unityTimer = Time.realtimeSinceStartup
            };
        }

        public float GetTime()
        {
            if (this.time != null)
            {
                return this.time.ElapsedMilliseconds;
            }
            return (Time.realtimeSinceStartup - this.unityTimer) * 1000f;
        }
    }
}
