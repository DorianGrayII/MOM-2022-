using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("RESOURCE_CHANCE", "")]
    public class ResourceChance : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Resource", true)]
        public Resource resource;

        [Prototype("Chance", true)]
        public float chance;

        public static explicit operator ResourceChance(Enum e)
        {
            return DataBase.Get<ResourceChance>(e);
        }

        public static explicit operator ResourceChance(string e)
        {
            return DataBase.Get<ResourceChance>(e, reportMissing: true);
        }
    }
}
