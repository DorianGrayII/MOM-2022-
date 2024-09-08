namespace MadGoat_SSAA
{
    using System;

    public class DebugData
    {
        public MadGoatSSAA instance;

        public DebugData(MadGoatSSAA instance)
        {
            this.instance = instance;
        }

        public Mode renderMode
        {
            get
            {
                return this.instance.renderMode;
            }
        }

        public float multiplier
        {
            get
            {
                return this.instance.multiplier;
            }
        }

        public bool fssaa
        {
            get
            {
                return this.instance.ssaaUltra;
            }
        }
    }
}

