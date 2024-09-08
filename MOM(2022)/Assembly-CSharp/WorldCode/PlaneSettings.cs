namespace WorldCode
{
    using MHUtils;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PlaneSettings
    {
        public float waterLevel;
        public float hillsAboveLevel;
        public float mountainAboveLevel;
        public float mapBorders;
        public Hex sourceHex;
        public float overrideTemperature;
        public void NormalWorld(Vector2i size)
        {
            this.mapBorders = size.x / 8;
            this.waterLevel = 0.55f;
            this.hillsAboveLevel = 0.22f;
            this.mountainAboveLevel = 0.07f;
            this.overrideTemperature = -1f;
        }

        public void LandBattle(Vector2i size, Hex source)
        {
            this.mapBorders = size.x / 8;
            this.waterLevel = 0f;
            this.sourceHex = source;
            this.overrideTemperature = source.temperature;
        }

        public void WaterBattle(Vector2i size, Hex source)
        {
            this.mapBorders = size.x / 8;
            this.waterLevel = 1f;
            this.sourceHex = source;
            this.overrideTemperature = -1f;
        }
    }
}

