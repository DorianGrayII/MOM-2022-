namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("CHANGEABLE_COST", "")]
    public class ChangeableCost : DBClass
    {
        public static string abbreviation = "";
        [Prototype("MaxMultipier", true)]
        public int maxMultipier;
        [Prototype("CostPerPoint", true)]
        public int costPerPoint;
        [Prototype("DesType", true)]
        public string desType;

        public static explicit operator ChangeableCost(Enum e)
        {
            return DataBase.Get<ChangeableCost>(e, false);
        }

        public static explicit operator ChangeableCost(string e)
        {
            return DataBase.Get<ChangeableCost>(e, true);
        }
    }
}

