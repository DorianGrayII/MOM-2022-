namespace MadGoat_SSAA
{
    public class DebugData
    {
        public MadGoatSSAA instance;

        public Mode renderMode => this.instance.renderMode;

        public float multiplier => this.instance.multiplier;

        public bool fssaa => this.instance.ssaaUltra;

        public DebugData(MadGoatSSAA instance)
        {
            this.instance = instance;
        }
    }
}
