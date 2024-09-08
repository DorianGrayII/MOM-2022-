namespace DBDef
{
    using MHUtils;
    using System;

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
            return DataBase.Get<ResourceChance>(e, false);
        }

        public static explicit operator ResourceChance(string e)
        {
            return DataBase.Get<ResourceChance>(e, true);
        }
    }
}

