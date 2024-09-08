namespace MHUtils
{
    using System;
    using System.Diagnostics;
    using UnityEngine;

    public class MHTimer
    {
        private Stopwatch time;
        private float unityTimer;

        private MHTimer()
        {
        }

        public float GetTime()
        {
            return ((this.time == null) ? ((Time.realtimeSinceStartup - this.unityTimer) * 1000f) : ((float) this.time.ElapsedMilliseconds));
        }

        public static MHTimer StartNew()
        {
            MHTimer timer1 = new MHTimer();
            timer1.time = Stopwatch.StartNew();
            return timer1;
        }

        public static MHTimer StartUnityTimer()
        {
            MHTimer timer1 = new MHTimer();
            timer1.unityTimer = Time.realtimeSinceStartup;
            return timer1;
        }
    }
}

