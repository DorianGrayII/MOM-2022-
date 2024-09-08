namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("TOWN", "")]
    public class Town : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";
        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;
        [Prototype("Graphic", false)]
        public TownGraphic graphic;
        [Prototype("Race", true)]
        public Race race;
        [Prototype("Worker", true)]
        public TownPopulation worker;
        [Prototype("Farmer", true)]
        public TownPopulation farmer;
        [Prototype("Rebel", true)]
        public TownPopulation rebel;
        [Prototype("PopulationGrowth", true)]
        public int populationGrowth;
        [Prototype("OutpostGrowth", true)]
        public int outpostGrowth;
        [Prototype("TaxMultiplier", true)]
        public FInt taxMultiplier;
        [Prototype("PossibleBuildings", true)]
        public Building[] possibleBuildings;
        [Prototype("BuildingResourceBonus", false)]
        public BuildingResourceBonus[] buildingResourceBonus;
        [Prototype("Dlc", false)]
        public string dlc;
        [Prototype("EnchantmentData", false)]
        public Enchantment[] enchantmentData;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Town(Enum e)
        {
            return DataBase.Get<Town>(e, false);
        }

        public static explicit operator Town(string e)
        {
            return DataBase.Get<Town>(e, true);
        }

        public void Set_buildingResourceBonus(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.buildingResourceBonus = new BuildingResourceBonus[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is BuildingResourceBonus))
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
                        Debug.LogError("buildingResourceBonus of type BuildingResourceBonus received invalid type from array! " + text1);
                    }
                    this.buildingResourceBonus[i] = list[i] as BuildingResourceBonus;
                }
            }
        }

        public void Set_enchantmentData(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.enchantmentData = new Enchantment[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Enchantment))
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
                        Debug.LogError("enchantmentData of type Enchantment received invalid type from array! " + text1);
                    }
                    this.enchantmentData[i] = list[i] as Enchantment;
                }
            }
        }

        public void Set_possibleBuildings(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.possibleBuildings = new Building[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Building))
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
                        Debug.LogError("possibleBuildings of type Building received invalid type from array! " + text1);
                    }
                    this.possibleBuildings[i] = list[i] as Building;
                }
            }
        }
    }
}

