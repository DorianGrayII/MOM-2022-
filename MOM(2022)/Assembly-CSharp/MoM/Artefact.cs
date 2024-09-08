using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM.Adventures;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class Artefact : IEventDisplayName
    {
        public const int Misc = 15872;

        public static Dictionary<ArtefactPower, ArtefactPowerSet> artefactPowerToSet;

        private static Dictionary<int, List<ArtefactPower>> spellsByEquipmentType;

        private static Dictionary<int, List<ArtefactPower>> bonusesByEquipmentType;

        private static Dictionary<int, List<ArtefactGraphic>> descriptionsByEquipmentType;

        private static Dictionary<int, List<ArtefactPrefab>> uniqueGraphicsByEquipmentType;

        private static readonly List<ArtefactPower> noPowers = new List<ArtefactPower>();

        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public string localizedDescription;

        [ProtoMember(3)]
        public string graphic;

        [ProtoMember(4)]
        public EEquipmentType equipmentType;

        [ProtoMember(5)]
        public List<DBReference<ArtefactPower>> artefactPowers;

        [ProtoMember(6)]
        public int value;

        [ProtoMember(7)]
        public int hash;

        public const int PREMADE_CHANCE = 50;

        public int GetValue()
        {
            if (this.value == 0)
            {
                ArtefactPrefab artefactPrefab = DataBase.GetType<ArtefactPrefab>().Find((ArtefactPrefab o) => o.eType == this.equipmentType);
                this.value += artefactPrefab.cost;
                if (this.artefactPowers != null)
                {
                    foreach (DBReference<ArtefactPower> artefactPower in this.artefactPowers)
                    {
                        if (artefactPower.Get().cost != -1)
                        {
                            this.value += artefactPower.Get().cost;
                        }
                    }
                }
            }
            return this.value;
        }

        public int SetValue(int itemValue)
        {
            return this.value = itemValue;
        }

        public static Artefact Craft(global::DBDef.Artefact source)
        {
            Artefact artefact = new Artefact();
            artefact.name = source.GetDescriptionInfo().GetLocalizedName();
            artefact.localizedDescription = source.GetDescriptionInfo().GetLocalizedName();
            artefact.graphic = source.GetDescriptionInfo().graphic;
            artefact.equipmentType = source.eType;
            artefact.artefactPowers = new List<DBReference<ArtefactPower>>();
            if (source.power != null)
            {
                ArtefactPower[] power = source.power;
                foreach (ArtefactPower artefactPower in power)
                {
                    artefact.artefactPowers.Add(artefactPower);
                }
            }
            return artefact;
        }

        public static Artefact RandomFactory(int points, EEquipmentType[] factoryByMatch = null, bool reportError = true)
        {
            List<ArtefactPrefab> list = DataBase.GetType<ArtefactPrefab>().FindAll((ArtefactPrefab o) => o.cost < points);
            if (list.Count < 1)
            {
                if (reportError)
                {
                    Debug.LogError("To few points (" + points + ") to build any artefact ");
                }
                return null;
            }
            list.RandomSort();
            if (factoryByMatch != null)
            {
                list = list.FindAll((ArtefactPrefab o) => Array.FindIndex(factoryByMatch, (EEquipmentType k) => o.eType == k) > -1);
            }
            int num = global::UnityEngine.Random.Range(0, list.Count);
            List<ArtefactPower> type = DataBase.GetType<ArtefactPower>();
            for (int i = 0; i < list.Count; i++)
            {
                int index = (i + num) % list.Count;
                ArtefactPrefab b = list[index];
                List<ArtefactPower> list2 = type.FindAll((ArtefactPower o) => o.eTypes != null && o.cost > -1 && o.cost + b.cost <= points && Array.FindIndex(o.eTypes, (EEquipmentType k) => k == b.eType) >= 0);
                if (list2.Count <= 0)
                {
                    continue;
                }
                list2.SortInPlace((ArtefactPower A, ArtefactPower B) => A.cost.CompareTo(B.cost));
                List<ArtefactPower> list3 = new List<ArtefactPower>();
                points -= b.cost;
                for (int j = 0; j < 4; j++)
                {
                    if (j == 3)
                    {
                        ArtefactPower item = list2[list2.Count - 1];
                        list3.Add(item);
                        break;
                    }
                    ArtefactPower power = list2[global::UnityEngine.Random.Range(0, list2.Count)];
                    list3.Add(power);
                    points -= power.cost;
                    list2 = list2.FindAll((ArtefactPower o) => o.cost <= points && Artefact.GetSet(o) != Artefact.GetSet(power));
                    if (list2.Count < 1)
                    {
                        break;
                    }
                }
                Artefact artefact = Artefact.Craft(b, list3);
                Artefact.AddGuarantedPowers(artefact);
                return artefact;
            }
            return null;
        }

        public static Artefact Craft(ArtefactPrefab source, List<ArtefactPower> powers)
        {
            Artefact artefact = new Artefact();
            artefact.name = source.GetDescriptionInfo().GetLocalizedName();
            artefact.localizedDescription = source.GetDescriptionInfo().GetLocalizedDescription();
            artefact.graphic = source.GetDescriptionInfo().graphic;
            artefact.equipmentType = source.eType;
            artefact.artefactPowers = new List<DBReference<ArtefactPower>>();
            if (powers == null || powers.Count == 0)
            {
                return artefact;
            }
            foreach (ArtefactPower power in powers)
            {
                artefact.artefactPowers.Add(power);
            }
            if (source.alternativeGraphic != null)
            {
                Artefact.UTIL_SetAlternativeGraphic(artefact, source);
            }
            return artefact;
        }

        public static Artefact Craft(ArtefactPrefab source, List<ArtefactPower> powers, int minPower, int maxPower)
        {
            Artefact artefact = Artefact.Craft(source, powers);
            int num = artefact.GetValue();
            if (num > maxPower)
            {
                return null;
            }
            if (num >= minPower && artefact.artefactPowers.Count > 0)
            {
                return artefact;
            }
            if (artefact.artefactPowers.Count >= 4)
            {
                return null;
            }
            int points = maxPower - num;
            List<ArtefactPower> list = DataBase.GetType<ArtefactPower>().FindAll((ArtefactPower o) => o.eTypes != null && o.cost <= points && Array.FindIndex(o.eTypes, (EEquipmentType k) => k == artefact.equipmentType) >= 0);
            foreach (DBReference<ArtefactPower> v in artefact.artefactPowers)
            {
                list = list.FindAll((ArtefactPower o) => Artefact.GetSet(o) != Artefact.GetSet(v.Get()));
            }
            if (list.Count > 0)
            {
                list.SortInPlace((ArtefactPower A, ArtefactPower B) => -A.cost.CompareTo(B.cost));
                List<ArtefactPower> list2 = new List<ArtefactPower>();
                int num2 = 3 - artefact.artefactPowers.Count;
                for (int i = 0; i < 4 - artefact.artefactPowers.Count; i++)
                {
                    if (i == num2)
                    {
                        ArtefactPower item = list[0];
                        list2.Add(item);
                        break;
                    }
                    ArtefactPower power = list[global::UnityEngine.Random.Range(0, list.Count)];
                    list2.Add(power);
                    points -= power.cost;
                    list = list.FindAll((ArtefactPower o) => o.cost <= points && Artefact.GetSet(o) != Artefact.GetSet(power));
                    if (list.Count < 1)
                    {
                        break;
                    }
                }
                if (points > maxPower - minPower)
                {
                    return null;
                }
                foreach (ArtefactPower item2 in list2)
                {
                    artefact.artefactPowers.Add(item2);
                }
                artefact.value = 0;
                num = artefact.GetValue();
                if (num < minPower || num > maxPower)
                {
                    Debug.LogError("Created artefact does not fit requirements regardless of the system which should forbid this!");
                }
                Artefact.UTIL_SetAlternativeGraphic(artefact, source);
                Artefact.AddGuarantedPowers(artefact);
                return artefact;
            }
            return null;
        }

        public static Artefact CraftRandomByBudget(int budget, bool preferPremade = true)
        {
            if (preferPremade && global::UnityEngine.Random.Range(0, 100) < 50)
            {
                List<global::DBDef.Artefact> artefactsList = new List<global::DBDef.Artefact>(DataBase.GetType<global::DBDef.Artefact>());
                int i;
                for (i = artefactsList.Count - 1; i >= 0; i--)
                {
                    int num = DataBase.GetType<ArtefactPrefab>().Find((ArtefactPrefab o) => o.eType == artefactsList[i].eType).cost;
                    ArtefactPower[] power = artefactsList[i].power;
                    foreach (ArtefactPower artefactPower in power)
                    {
                        num += artefactPower.cost;
                    }
                    if (num > budget)
                    {
                        artefactsList.RemoveAt(i);
                    }
                }
                if (artefactsList.Count > 0)
                {
                    artefactsList.RandomSortThreadSafe();
                    return Artefact.Craft(artefactsList[0]);
                }
            }
            return Artefact.RandomFactory(budget, null, reportError: false);
        }

        public static void SmashArtefact(object o)
        {
            if (o is Artefact)
            {
                Artefact artefact = o as Artefact;
                PlayerWizard humanWizard = GameManager.GetHumanWizard();
                if (humanWizard.artefacts.Contains(artefact))
                {
                    humanWizard.mana += artefact.GetValue() / 2;
                    humanWizard.artefacts.Remove(artefact);
                    MHEventSystem.TriggerEvent<Artefact>(o, "Smashed");
                    return;
                }
                foreach (Reference<Unit> hero in humanWizard.heroes)
                {
                    foreach (EquipmentSlot equipmentSlot in hero.Get().artefactManager.equipmentSlots)
                    {
                        if (equipmentSlot.item == artefact)
                        {
                            humanWizard.mana += artefact.GetValue() / 2;
                            equipmentSlot.item = null;
                            MHEventSystem.TriggerEvent<Artefact>(o, "Smashed");
                            return;
                        }
                    }
                }
            }
            Debug.LogError("Something went wrong! Couldn't find the artefact");
        }

        public static ArtefactPowerSet GetSet(ArtefactPower power)
        {
            if (Artefact.artefactPowerToSet == null)
            {
                Artefact.artefactPowerToSet = new Dictionary<ArtefactPower, ArtefactPowerSet>();
            }
            if (!Artefact.artefactPowerToSet.ContainsKey(power))
            {
                ArtefactPowerSet artefactPowerSet = DataBase.GetType<ArtefactPowerSet>().Find((ArtefactPowerSet o) => o.power != null && Array.FindIndex(o.power, (ArtefactPower k) => k == power) >= 0);
                if (artefactPowerSet == null)
                {
                    Debug.LogError("Set cannot be found for " + power.dbName);
                }
                Artefact.artefactPowerToSet[power] = artefactPowerSet;
            }
            return Artefact.artefactPowerToSet[power];
        }

        private static void EnsurePowersByPopulated()
        {
            if (Artefact.spellsByEquipmentType != null)
            {
                return;
            }
            Artefact.spellsByEquipmentType = new Dictionary<int, List<ArtefactPower>>();
            Artefact.bonusesByEquipmentType = new Dictionary<int, List<ArtefactPower>>();
            foreach (ArtefactPower item in DataBase.GetType<ArtefactPower>())
            {
                EEquipmentType[] eTypes = item.eTypes;
                foreach (EEquipmentType eEquipmentType in eTypes)
                {
                    ArtefactPowerSet set = Artefact.GetSet(item);
                    if (set.requiredTag != null && set.requiredTag.Length != 0)
                    {
                        Artefact.AddPower(Artefact.spellsByEquipmentType, (int)eEquipmentType, item);
                    }
                    else
                    {
                        Artefact.AddPower(Artefact.bonusesByEquipmentType, (int)eEquipmentType, item);
                    }
                    if ((eEquipmentType & (EEquipmentType)15872) != 0)
                    {
                        if (set.requiredTag != null && set.requiredTag.Length != 0)
                        {
                            Artefact.AddPower(Artefact.spellsByEquipmentType, 15872, item);
                        }
                        else
                        {
                            Artefact.AddPower(Artefact.bonusesByEquipmentType, 15872, item);
                        }
                    }
                }
            }
            foreach (List<ArtefactPower> value in Artefact.bonusesByEquipmentType.Values)
            {
                value.Sort(delegate(ArtefactPower a, ArtefactPower b)
                {
                    int num = Artefact.GetSet(a).dbName.CompareTo(Artefact.GetSet(b).dbName);
                    if (num == 0)
                    {
                        num = a.cost - b.cost;
                    }
                    return num;
                });
            }
        }

        private static void AddPower(Dictionary<int, List<ArtefactPower>> dict, int eType, ArtefactPower power)
        {
            if (!dict.TryGetValue(eType, out var list))
            {
                list = (dict[eType] = new List<ArtefactPower>());
            }
            if (!list.Contains(power))
            {
                list.Add(power);
            }
        }

        public static List<ArtefactPower> GetSpells(int equipmentType)
        {
            Artefact.EnsurePowersByPopulated();
            if (!Artefact.spellsByEquipmentType.TryGetValue(equipmentType, out var result))
            {
                return Artefact.noPowers;
            }
            return result;
        }

        public static List<ArtefactPower> GetBonuses(int equipmentType)
        {
            Artefact.EnsurePowersByPopulated();
            if (!Artefact.bonusesByEquipmentType.TryGetValue(equipmentType, out var result))
            {
                return Artefact.noPowers;
            }
            return result;
        }

        public int GetHash()
        {
            if (this.hash == 0)
            {
                this.hash = this.graphic.GetHashCode() ^ this.equipmentType.ToString().GetHashCode();
                if (this.artefactPowers != null)
                {
                    foreach (DBReference<ArtefactPower> artefactPower in this.artefactPowers)
                    {
                        this.hash ^= artefactPower.dbName.GetHashCode();
                    }
                }
            }
            return this.hash;
        }

        public static Artefact FactoryByRequirements(List<LogicRequirementGroup> requirementGroups, int min, int max)
        {
            List<List<object>> list = new List<List<object>>();
            Artefact.UTIL_ConstructArtefactFactoryOption(requirementGroups, 0, null, list);
            new List<Artefact>();
            list.SortInPlace(delegate(List<object> a, List<object> b)
            {
                int num = -a.Count.CompareTo(b.Count);
                return (num == 0) ? global::UnityEngine.Random.Range(-1, 2) : num;
            });
            foreach (List<object> item in list)
            {
                if (item.Count < 1)
                {
                    continue;
                }
                List<object> list2 = item.FindAll((object o) => o is ArtefactPrefab);
                if (list2.Count > 1 || list2.Count != 1)
                {
                    continue;
                }
                ArtefactPrefab ap = list2[0] as ArtefactPrefab;
                item.Remove(ap);
                List<ArtefactPower> aps = new List<ArtefactPower>();
                item.ForEach(delegate(object o)
                {
                    aps.Add(o as ArtefactPower);
                });
                if (aps.Find((ArtefactPower o) => Array.FindIndex(o.eTypes, (EEquipmentType k) => ap.eType == k) == -1) == null)
                {
                    Artefact artefact = Artefact.Craft(ap, aps, min, max);
                    if (artefact != null)
                    {
                        return artefact;
                    }
                }
            }
            return null;
        }

        private static void UTIL_ConstructArtefactFactoryOption(List<LogicRequirementGroup> requirementGroups, int requrementsIndex, List<object> branchOption, List<List<object>> combinedList)
        {
            if (requirementGroups.Count <= requrementsIndex)
            {
                return;
            }
            LogicRequirementGroup logicRequirementGroup = requirementGroups[requrementsIndex];
            if (logicRequirementGroup.options == null)
            {
                return;
            }
            for (int i = 0; i < logicRequirementGroup.options.Count; i++)
            {
                LogicOptionalGroup ro = logicRequirementGroup.options[i];
                DBClass dBClass = DataBase.Get(ro.typeData, reportMissing: false);
                if (dBClass == null)
                {
                    dBClass = DataBase.GetType<ArtefactPrefab>().Find((ArtefactPrefab o) => o.eType.ToString() == ro.typeData);
                }
                if (dBClass != null)
                {
                    List<object> list = new List<object>();
                    if (branchOption != null)
                    {
                        list.AddRange(branchOption);
                    }
                    list.Add(dBClass);
                    combinedList.Add(list);
                    Artefact.UTIL_ConstructArtefactFactoryOption(requirementGroups, requrementsIndex + 1, list, combinedList);
                }
            }
        }

        private static void UTIL_SetAlternativeGraphic(Artefact a, ArtefactPrefab source)
        {
            ArtefactGraphic[] alternativeGraphic = source.alternativeGraphic;
            foreach (ArtefactGraphic artefactGraphic in alternativeGraphic)
            {
                bool flag = true;
                if (artefactGraphic.requiredPower != null)
                {
                    ArtefactPower[] requiredPower = artefactGraphic.requiredPower;
                    foreach (ArtefactPower artefactPower in requiredPower)
                    {
                        if (!a.artefactPowers.Contains(artefactPower))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (artefactGraphic.requiredPowerSet != null)
                {
                    ArtefactPowerSet[] requiredPowerSet = artefactGraphic.requiredPowerSet;
                    foreach (ArtefactPowerSet obj in requiredPowerSet)
                    {
                        bool flag2 = false;
                        ArtefactPower[] requiredPower = obj.power;
                        foreach (ArtefactPower artefactPower2 in requiredPower)
                        {
                            if (a.artefactPowers.Contains(artefactPower2))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    a.name = artefactGraphic.GetDescriptionInfo().GetLocalizedName();
                    a.localizedDescription = artefactGraphic.GetDescriptionInfo().GetLocalizedDescription();
                    a.graphic = artefactGraphic.GetDescriptionInfo().graphic;
                    break;
                }
            }
        }

        public string GetEventDisplayName()
        {
            return this.name;
        }

        private static void AddGuarantedPowers(Artefact a)
        {
            foreach (ArtefactPower item in DataBase.GetType<ArtefactPower>().FindAll((ArtefactPower o) => o.cost == -1))
            {
                EEquipmentType[] eTypes = item.eTypes;
                for (int i = 0; i < eTypes.Length; i++)
                {
                    if (eTypes[i] == a.equipmentType)
                    {
                        a.artefactPowers.Add(item);
                    }
                }
            }
        }
    }
}
