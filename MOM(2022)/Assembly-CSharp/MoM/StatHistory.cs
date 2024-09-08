using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class StatHistory
    {
        public enum Stats
        {
            Magic = 0,
            Research = 1,
            Army = 2,
            Towns = 3,
            Fame = 4,
            GoldIncome = 5,
            FoodIncome = 6,
            ManaIncome = 7,
            MAX = 8
        }

        [ProtoMember(1)]
        public NetDictionary<Stats, List<int>> stats;

        [ProtoAfterDeserialization]
        public void PostLoadSanitazation()
        {
            NetDictionary<Stats, List<int>> netDictionary = this.stats;
            if (netDictionary[Stats.Magic] == null)
            {
                List<int> list2 = (netDictionary[Stats.Magic] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.Research] == null)
            {
                List<int> list2 = (netDictionary[Stats.Research] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.Army] == null)
            {
                List<int> list2 = (netDictionary[Stats.Army] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.Towns] == null)
            {
                List<int> list2 = (netDictionary[Stats.Towns] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.Fame] == null)
            {
                List<int> list2 = (netDictionary[Stats.Fame] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.GoldIncome] == null)
            {
                List<int> list2 = (netDictionary[Stats.GoldIncome] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.FoodIncome] == null)
            {
                List<int> list2 = (netDictionary[Stats.FoodIncome] = new List<int>());
            }
            netDictionary = this.stats;
            if (netDictionary[Stats.ManaIncome] == null)
            {
                List<int> list2 = (netDictionary[Stats.ManaIncome] = new List<int>());
            }
        }

        public StatHistory()
        {
            this.stats = new NetDictionary<Stats, List<int>>();
            this.stats[Stats.Magic] = new List<int>();
            this.stats[Stats.Research] = new List<int>();
            this.stats[Stats.Army] = new List<int>();
            this.stats[Stats.Towns] = new List<int>();
            this.stats[Stats.Fame] = new List<int>();
            this.stats[Stats.GoldIncome] = new List<int>();
            this.stats[Stats.FoodIncome] = new List<int>();
            this.stats[Stats.ManaIncome] = new List<int>();
        }

        public void ProcessTurn(PlayerWizard w)
        {
            this.stats[Stats.Magic].Add(w.CalculatePowerIncome());
            this.stats[Stats.Research].Add(this.CalcResearch(w));
            this.stats[Stats.Army].Add(this.CalcArmy(w));
            this.stats[Stats.Towns].Add(this.CalcTowns(w));
            this.stats[Stats.Fame].Add(this.CalcFame(w));
            this.stats[Stats.GoldIncome].Add(this.CalcGoldIncome(w));
            this.stats[Stats.FoodIncome].Add(this.CalcFoodIncome(w));
            this.stats[Stats.ManaIncome].Add(this.CalcManaIncome(w));
        }

        public int CalcResearch(PlayerWizard w)
        {
            int num = 0;
            foreach (DBReference<Spell> spell2 in w.GetSpellManager().GetSpells())
            {
                Spell spell = spell2.Get();
                num += spell.researchCost;
            }
            return num + w.GetMagicAndResearch().researchProgress;
        }

        public int CalcArmy(PlayerWizard w)
        {
            int num = 0;
            foreach (Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetOwnerID() != w.ID || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType))
                {
                    continue;
                }
                List<Reference<Unit>> units = registeredGroup.GetUnits();
                if (units == null)
                {
                    continue;
                }
                foreach (Reference<Unit> item in units)
                {
                    if (!(item.Get().GetAttFinal(TAG.CONSTRUCTION_UNIT) > 0))
                    {
                        num += item.Get().GetWorldUnitValue();
                    }
                }
            }
            return num;
        }

        public int CalcTowns(PlayerWizard w)
        {
            return GameManager.GetWizardTownCount(w.GetID());
        }

        public int CalcFame(PlayerWizard w)
        {
            return w.GetFame();
        }

        public int CalcGoldIncome(PlayerWizard w)
        {
            return w.CalculateMoneyIncome();
        }

        public int CalcFoodIncome(PlayerWizard w)
        {
            return w.CalculateFoodIncome();
        }

        public int CalcManaIncome(PlayerWizard w)
        {
            return w.CalculateManaIncome();
        }
    }
}
