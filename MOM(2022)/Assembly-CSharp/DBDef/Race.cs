namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("RACE", "")]
    public class Race : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";
        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;
        [Prototype("ProductionDescription", false)]
        public string productionDescription;
        [Prototype("BaseRace", false)]
        public bool baseRace;
        [Prototype("ArcanusRace", false)]
        public bool arcanusRace;
        [Prototype("RepresentativeUnit", false)]
        public Unit representativeUnit;
        [Prototype("VisualGroup", false)]
        public string visualGroup;
        [Prototype("RaceTension", false)]
        public RaceTension[] raceTension;
        [Prototype("TensionFallback", false)]
        public Race tensionFallback;
        [Prototype("TensionFallback2", false)]
        public FInt tensionFallback2;
        [Prototype("Dlc", false)]
        public string dlc;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Race(Enum e)
        {
            return DataBase.Get<Race>(e, false);
        }

        public static explicit operator Race(string e)
        {
            return DataBase.Get<Race>(e, true);
        }

        public void Set_raceTension(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.raceTension = new RaceTension[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is RaceTension))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("raceTension of type RaceTension received invalid type from array! " + text1);
                    }
                    this.raceTension[i] = list[i] as RaceTension;
                }
            }
        }
    }
}

