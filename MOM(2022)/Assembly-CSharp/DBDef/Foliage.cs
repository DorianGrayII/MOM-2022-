using System;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("FOLIAGE", "")]
    public class Foliage : DBClass
    {
        public static string abbreviation = "";

        [Prototype("BattleOnly", false)]
        public bool battleOnly;

        [Prototype("WorldOnly", false)]
        public bool worldOnly;

        [Prototype("AllowRotation", false)]
        public bool allowRotation;

        [Prototype("TreeName", true)]
        public string treeName;

        [Prototype("Forest", false)]
        public bool forest;

        [Prototype("Chance", false)]
        public FInt chance;

        [Prototype("Count", true)]
        public int count;

        [Prototype("Color1", true)]
        public Color color1;

        [Prototype("Color2", true)]
        public Color color2;

        [Prototype("Color3", true)]
        public Color color3;

        public static explicit operator Foliage(Enum e)
        {
            return DataBase.Get<Foliage>(e);
        }

        public static explicit operator Foliage(string e)
        {
            return DataBase.Get<Foliage>(e, reportMissing: true);
        }
    }
}
