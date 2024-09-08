namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("TERRAIN_GRAPHIC", "TG")]
    public class TerrainGraphic : DBClass
    {
        public static string abbreviation = "TG";
        [Prototype("Diffuse", true)]
        public string diffuse;
        [Prototype("Normal", false)]
        public string normal;
        [Prototype("Specular", false)]
        public string specular;
        [Prototype("Height", false)]
        public string height;
        [Prototype("Mixer", false)]
        public string mixer;
        [Prototype("BlockRotation", false)]
        public bool blockRotation;
        [Prototype("MeshDensity", false)]
        public int meshDensity;

        public static explicit operator TerrainGraphic(Enum e)
        {
            return DataBase.Get<TerrainGraphic>(e, false);
        }

        public static explicit operator TerrainGraphic(string e)
        {
            return DataBase.Get<TerrainGraphic>(e, true);
        }
    }
}

