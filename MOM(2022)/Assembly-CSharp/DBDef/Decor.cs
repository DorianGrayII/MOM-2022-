namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("DECOR", "")]
    public class Decor : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Name", true)]
        public string name;
        [Prototype("Chance", true)]
        public FInt chance;
        [Prototype("BillboardingRotation", false)]
        public bool billboardingRotation;

        public static explicit operator Decor(Enum e)
        {
            return DataBase.Get<Decor>(e, false);
        }

        public static explicit operator Decor(string e)
        {
            return DataBase.Get<Decor>(e, true);
        }
    }
}

