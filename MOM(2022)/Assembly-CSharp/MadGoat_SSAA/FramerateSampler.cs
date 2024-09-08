using UnityEngine;

namespace MadGoat_SSAA
{
    public class FramerateSampler
    {
        private float updateInterval = 1f;

        private float newPeriod;

        private int intervalTotalFrames;

        private int intervalFrameSum;

        public int CurrentFps;

        public void Update()
        {
            this.intervalTotalFrames++;
            this.intervalFrameSum += (int)(1f / Time.deltaTime);
            if (Time.time > this.newPeriod)
            {
                this.CurrentFps = this.intervalFrameSum / this.intervalTotalFrames;
                this.intervalTotalFrames = 0;
                this.intervalFrameSum = 0;
                this.newPeriod += this.updateInterval;
            }
        }
    }
}
