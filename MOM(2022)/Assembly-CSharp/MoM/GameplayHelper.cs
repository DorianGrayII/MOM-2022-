using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;

namespace MOM
{
    public class GameplayHelper
    {
        private static GameplayHelper singleton;

        private Dictionary<Town, List<Building>> militaryBuilding;

        private Dictionary<Town, List<Building>> moneyBuilding;

        private Dictionary<Town, List<Building>> foodBuilding;

        private Dictionary<Town, List<Building>> powerBuilding;

        private Dictionary<Town, List<Building>> populationBuilding;

        private Dictionary<Town, List<Building>> generalEconomyBuilding;

        private Dictionary<Town, HashSet<Building>> nonMilitaryEconomyBuilding;

        private Dictionary<Town, global::DBDef.Unit> settler;

        private Dictionary<Town, global::DBDef.Unit> engineer;

        private Dictionary<Town, List<global::DBDef.Unit>> army;

        public static GameplayHelper Get()
        {
            if (GameplayHelper.singleton == null)
            {
                GameplayHelper.singleton = new GameplayHelper();
            }
            return GameplayHelper.singleton;
        }

        public List<Building> GetMilitaryBuildings(Town t)
        {
            if (this.militaryBuilding == null)
            {
                this.militaryBuilding = new Dictionary<Town, List<Building>>();
            }
            if (!this.militaryBuilding.ContainsKey(t))
            {
                List<Building> list = new List<Building>();
                if (t.possibleBuildings != null)
                {
                    List<global::DBDef.Unit> type = DataBase.GetType<global::DBDef.Unit>();
                    Building[] possibleBuildings = t.possibleBuildings;
                    foreach (Building v in possibleBuildings)
                    {
                        if (v.tags != null && Array.Find(v.tags, (Tag o) => o == (Tag)TAG.MILITARY) != null)
                        {
                            list.Add(v);
                        }
                        else if (type.Find((global::DBDef.Unit o) => o.race == t.race && o.requiredBuildings != null && Array.Find(o.requiredBuildings, (Building k) => k == v) != null) != null)
                        {
                            list.Add(v);
                        }
                    }
                    for (int l = 0; l < list.Count; l++)
                    {
                        Building building = list[l];
                        if (building.parentBuildingRequired == null || building.parentBuildingRequired.Length == 0)
                        {
                            continue;
                        }
                        possibleBuildings = building.parentBuildingRequired;
                        foreach (Building i in possibleBuildings)
                        {
                            if (!list.Contains(i) && Array.Find(t.possibleBuildings, (Building o) => o == i) != null)
                            {
                                list.Add(i);
                            }
                        }
                    }
                }
                this.militaryBuilding[t] = list;
            }
            return this.militaryBuilding[t];
        }

        public List<Building> GetMoneyBuildings(Town t)
        {
            if (this.moneyBuilding == null)
            {
                this.moneyBuilding = new Dictionary<Town, List<Building>>();
            }
            if (!this.moneyBuilding.ContainsKey(t))
            {
                this.moneyBuilding[t] = this.GetBuildinsOfType(t, EEnchantmentType.GoldModifier);
            }
            return this.moneyBuilding[t];
        }

        public List<Building> GetFoodBuildings(Town t)
        {
            if (this.foodBuilding == null)
            {
                this.foodBuilding = new Dictionary<Town, List<Building>>();
            }
            if (!this.foodBuilding.ContainsKey(t))
            {
                this.foodBuilding[t] = this.GetBuildinsOfType(t, EEnchantmentType.FoodModifier);
            }
            return this.foodBuilding[t];
        }

        public List<Building> GetPopulationBuildings(Town t)
        {
            if (this.populationBuilding == null)
            {
                this.populationBuilding = new Dictionary<Town, List<Building>>();
            }
            if (!this.populationBuilding.ContainsKey(t))
            {
                this.populationBuilding[t] = this.GetBuildinsOfType(t, EEnchantmentType.PopulationGrowModifier);
            }
            return this.populationBuilding[t];
        }

        public List<Building> GetPowerBuildings(Town t)
        {
            if (this.powerBuilding == null)
            {
                this.powerBuilding = new Dictionary<Town, List<Building>>();
            }
            if (!this.powerBuilding.ContainsKey(t))
            {
                this.powerBuilding[t] = this.GetBuildinsOfType(t, EEnchantmentType.PowerModifier);
                this.powerBuilding[t].AddRange(this.GetBuildinsOfType(t, EEnchantmentType.PowerModifierReligious));
            }
            return this.powerBuilding[t];
        }

        public List<Building> GetOtherEconomyBuildings(Town t)
        {
            if (this.generalEconomyBuilding == null)
            {
                this.generalEconomyBuilding = new Dictionary<Town, List<Building>>();
            }
            if (!this.generalEconomyBuilding.ContainsKey(t))
            {
                List<Building> list = new List<Building>(t.possibleBuildings);
                list.RemoveAll((Building o) => this.GetMilitaryBuildings(t).Contains(o) || this.GetMoneyBuildings(t).Contains(o) || this.GetFoodBuildings(t).Contains(o) || this.GetPopulationBuildings(t).Contains(o) || this.GetPowerBuildings(t).Contains(o));
                this.generalEconomyBuilding[t] = list;
            }
            return this.generalEconomyBuilding[t];
        }

        public HashSet<Building> GetEconomyBuildings(Town t)
        {
            if (this.nonMilitaryEconomyBuilding == null)
            {
                this.nonMilitaryEconomyBuilding = new Dictionary<Town, HashSet<Building>>();
            }
            if (!this.nonMilitaryEconomyBuilding.ContainsKey(t))
            {
                HashSet<Building> hashSet = new HashSet<Building>();
                foreach (Building moneyBuilding in this.GetMoneyBuildings(t))
                {
                    hashSet.Add(moneyBuilding);
                }
                foreach (Building foodBuilding in this.GetFoodBuildings(t))
                {
                    hashSet.Add(foodBuilding);
                }
                foreach (Building populationBuilding in this.GetPopulationBuildings(t))
                {
                    hashSet.Add(populationBuilding);
                }
                foreach (Building powerBuilding in this.GetPowerBuildings(t))
                {
                    hashSet.Add(powerBuilding);
                }
                this.nonMilitaryEconomyBuilding[t] = hashSet;
            }
            return this.nonMilitaryEconomyBuilding[t];
        }

        private List<Building> GetBuildinsOfType(Town t, EEnchantmentType eType)
        {
            List<Building> list = new List<Building>();
            if (t.possibleBuildings != null)
            {
                Building[] possibleBuildings = t.possibleBuildings;
                foreach (Building building in possibleBuildings)
                {
                    if (building.enchantments == null)
                    {
                        continue;
                    }
                    Enchantment[] enchantments = building.enchantments;
                    foreach (Enchantment enchantment in enchantments)
                    {
                        if (enchantment.scripts != null && Array.Find(enchantment.scripts, (EnchantmentScript o) => o.triggerType == eType) != null)
                        {
                            list.Add(building);
                            break;
                        }
                    }
                }
            }
            return list;
        }

        public static List<global::DBDef.Unit> GetTownProducedArmy(Town t)
        {
            if (GameplayHelper.Get().army == null)
            {
                GameplayHelper.Get().army = new Dictionary<Town, List<global::DBDef.Unit>>();
            }
            if (!GameplayHelper.Get().army.ContainsKey(t))
            {
                List<global::DBDef.Unit> value = DataBase.GetType<global::DBDef.Unit>().FindAll((global::DBDef.Unit o) => o.race == t.race && o.GetTag(TAG.CONSTRUCTION_UNIT) == 0);
                GameplayHelper.Get().army[t] = value;
            }
            if (GameplayHelper.Get().army.ContainsKey(t))
            {
                return GameplayHelper.Get().army[t];
            }
            return null;
        }

        public static global::DBDef.Unit GetTownProducedSettler(Town t)
        {
            if (GameplayHelper.Get().settler == null)
            {
                GameplayHelper.Get().settler = new Dictionary<Town, global::DBDef.Unit>();
            }
            if (!GameplayHelper.Get().settler.ContainsKey(t))
            {
                foreach (global::DBDef.Unit item in DataBase.GetType<global::DBDef.Unit>())
                {
                    if (item.race == t.race && item.GetTag((Tag)TAG.SETTLER_UNIT) > 0)
                    {
                        GameplayHelper.Get().settler[t] = item;
                        break;
                    }
                }
            }
            if (GameplayHelper.Get().settler.ContainsKey(t))
            {
                return GameplayHelper.Get().settler[t];
            }
            return null;
        }

        public static global::DBDef.Unit GetTownProducedEngineer(Town t)
        {
            if (GameplayHelper.Get().engineer == null)
            {
                GameplayHelper.Get().engineer = new Dictionary<Town, global::DBDef.Unit>();
            }
            if (!GameplayHelper.Get().engineer.ContainsKey(t))
            {
                foreach (global::DBDef.Unit item in DataBase.GetType<global::DBDef.Unit>())
                {
                    if (item.race == t.race && item.GetTag((Tag)TAG.ENGINEER_UNIT) > 0)
                    {
                        GameplayHelper.Get().engineer[t] = item;
                        break;
                    }
                }
            }
            if (GameplayHelper.Get().engineer.ContainsKey(t))
            {
                return GameplayHelper.Get().engineer[t];
            }
            return null;
        }
    }
}
