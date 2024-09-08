// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.AdventureOutcomeDelta
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;

public class AdventureOutcomeDelta
{
    public class Outcome
    {
        public readonly AdventureOutcome.Types outcomeType;

        public readonly string name;

        public readonly string graphic;

        public readonly int delta;

        public readonly object thing;

        public readonly TownLocation location;

        public readonly int duration;

        public readonly object additionalData;

        public readonly AdventureOutcome.StatType statType;

        public Outcome(AdventureOutcome.Types outcomeType, int delta, object thing, AdventureOutcome.StatType statType = AdventureOutcome.StatType.None, TownLocation location = null, object additionalData = null)
        {
            this.outcomeType = outcomeType;
            this.delta = delta;
            if (thing is IDescriptionInfoType descriptionInfoType)
            {
                DescriptionInfo descriptionInfo = descriptionInfoType.GetDescriptionInfo();
                this.name = descriptionInfo.GetLocalizedName();
                this.graphic = descriptionInfo.graphic;
            }
            this.thing = thing;
            this.statType = statType;
            this.location = location;
            this.additionalData = additionalData;
        }
    }

    private class UnitStats
    {
        public readonly BattleFigure battleFigures;

        public readonly int xp;

        public readonly global::MOM.Unit unit;

        public bool IsHero => this.unit.dbSource.Get() is Hero;

        public UnitStats(global::MOM.Unit unit)
        {
            this.battleFigures = new BattleFigure(unit, unit.attributes);
            this.xp = unit.xp;
            this.unit = unit;
        }
    }

    private class TownStats
    {
        public readonly List<DBReference<Building>> buildings;

        public readonly int population;

        public readonly List<Resource> resources;

        public readonly TownLocation town;

        public TownStats(TownLocation t)
        {
            this.town = t;
            this.buildings = new List<DBReference<Building>>(t.buildings);
            this.population = t.Population;
            this.resources = new List<Resource>(t.GetResources());
        }
    }

    private class BookInfo
    {
        public readonly Dictionary<ERealm, int> books = new Dictionary<ERealm, int>();

        public BookInfo()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            this.books.Clear();
            this.books[ERealm.Arcane] = humanWizard.GetAttFinal(TAG.ARCANE_BOOK).ToInt();
            this.books[ERealm.Nature] = humanWizard.GetAttFinal(TAG.NATURE_MAGIC_BOOK).ToInt();
            this.books[ERealm.Chaos] = humanWizard.GetAttFinal(TAG.CHAOS_MAGIC_BOOK).ToInt();
            this.books[ERealm.Life] = humanWizard.GetAttFinal(TAG.LIFE_MAGIC_BOOK).ToInt();
            this.books[ERealm.Death] = humanWizard.GetAttFinal(TAG.DEATH_MAGIC_BOOK).ToInt();
            this.books[ERealm.Sorcery] = humanWizard.GetAttFinal(TAG.SORCERY_MAGIC_BOOK).ToInt();
            this.books[ERealm.Tech] = humanWizard.GetAttFinal(TAG.TECH_BOOK).ToInt();
        }
    }

    private delegate void ListCallback<T>(T thing, int delta);

    private int money;

    private int mana;

    private int fame;

    private int castingSkillDevelopment;

    private BookInfo books = new BookInfo();

    private List<Trait> traits = new List<Trait>();

    private List<global::MOM.Artefact> artefacts = new List<global::MOM.Artefact>();

    private List<DBReference<Spell>> spells = new List<DBReference<Spell>>();

    private List<EnchantmentInstance> addedEnchantments = new List<EnchantmentInstance>();

    private List<EnchantmentInstance> removedEnchantments = new List<EnchantmentInstance>();

    private Dictionary<int, UnitStats> unitStats = new Dictionary<int, UnitStats>();

    private Dictionary<global::MOM.Unit, List<DBReference<Skill>>> unitSkills = new Dictionary<global::MOM.Unit, List<DBReference<Skill>>>();

    private Dictionary<int, TownStats> townStats = new Dictionary<int, TownStats>();

    private List<Outcome> outcomes;

    public AdventureOutcomeDelta()
    {
        MHEventSystem.RegisterListener<EnchantmentManager>(OnAddEnchantment, this);
        MHEventSystem.RegisterListener("RemoveEnchantment", OnRemoveEnchantment, this);
    }

    public void Destroy()
    {
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
    }

    public void OnAddEnchantment(object oManager, object oEnchantmentInstance)
    {
        if (oEnchantmentInstance is EnchantmentInstance enchantmentInstance && enchantmentInstance.manager.owner.GetWizardOwner() == GameManager.GetHumanWizard() && !(enchantmentInstance.manager.owner is BattleUnit) && !enchantmentInstance.buildingEnchantment)
        {
            this.addedEnchantments.Add(enchantmentInstance);
        }
    }

    public void OnRemoveEnchantment(object oManager, object oEnchantmentInstance)
    {
        if (oEnchantmentInstance is EnchantmentInstance enchantmentInstance && enchantmentInstance.manager.owner.GetWizardOwner() == GameManager.GetHumanWizard() && !(enchantmentInstance.manager.owner is BattleUnit) && !enchantmentInstance.buildingEnchantment)
        {
            this.removedEnchantments.Add(enchantmentInstance);
        }
    }

    public void Store()
    {
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        this.fame = humanWizard.GetFame();
        this.money = humanWizard.money;
        this.mana = humanWizard.mana;
        this.castingSkillDevelopment = humanWizard.castingSkillDevelopment;
        this.artefacts = new List<global::MOM.Artefact>(humanWizard.artefacts);
        foreach (global::MOM.Group item in GameManager.GetGroupsOfWizard(humanWizard.GetID()))
        {
            foreach (Reference<global::MOM.Unit> unit2 in item.GetUnits())
            {
                if (!(unit2.Get().dbSource.Get() is Hero))
                {
                    continue;
                }
                foreach (EquipmentSlot equipmentSlot in unit2.Get().artefactManager.equipmentSlots)
                {
                    if (equipmentSlot.item != null)
                    {
                        this.artefacts.Add(equipmentSlot.item);
                    }
                }
            }
        }
        this.spells = new List<DBReference<Spell>>(humanWizard.GetSpells());
        this.addedEnchantments.Clear();
        this.removedEnchantments.Clear();
        this.townStats.Clear();
        this.unitStats.Clear();
        this.unitSkills.Clear();
        int num = PlayerWizard.HumanID();
        foreach (Entity value in EntityManager.Get().entities.Values)
        {
            if (value is global::MOM.Unit unit)
            {
                PlayerWizard wizardOwner = unit.GetWizardOwner();
                if (wizardOwner != null && wizardOwner.GetID() == num)
                {
                    this.unitStats.Add(unit.GetID(), new UnitStats(unit));
                    this.unitSkills.Add(unit, new List<DBReference<Skill>>(unit.GetSkills()));
                }
            }
            else
            {
                if (!(value is TownLocation townLocation))
                {
                    continue;
                }
                PlayerWizard wizardOwner2 = townLocation.GetWizardOwner();
                if (wizardOwner2 != null && wizardOwner2.GetID() == num)
                {
                    if (this.townStats.ContainsKey(townLocation.GetID()))
                    {
                        Debug.LogError("Towns with the same ID! " + townLocation.GetName() + ", " + this.townStats[townLocation.GetID()].town.GetName());
                    }
                    this.townStats.Add(townLocation.GetID(), new TownStats(townLocation));
                }
            }
        }
        this.books = new BookInfo();
        this.traits = new List<Trait>(humanWizard.GetTraits());
    }

    public List<Outcome> GetOutcomes()
    {
        this.outcomes = new List<Outcome>();
        AdventureOutcomeDelta adventureOutcomeDelta = new AdventureOutcomeDelta();
        adventureOutcomeDelta.Store();
        this.ProcessValue(AdventureOutcome.Types.Fame, this.fame, adventureOutcomeDelta.fame);
        this.ProcessValue(AdventureOutcome.Types.Gold, this.money, adventureOutcomeDelta.money);
        this.ProcessValue(AdventureOutcome.Types.Mana, this.mana, adventureOutcomeDelta.mana);
        this.ProcessValue(AdventureOutcome.Types.CastingSkillDevelopment, this.castingSkillDevelopment, adventureOutcomeDelta.castingSkillDevelopment);
        this.ProcessList(this.artefacts, adventureOutcomeDelta.artefacts, delegate(global::MOM.Artefact a, int delta)
        {
            this.outcomes.Add(new Outcome(AdventureOutcome.Types.Item, delta, a));
        });
        this.ProcessList(this.spells, adventureOutcomeDelta.spells, delegate(DBReference<Spell> sr, int delta)
        {
            Spell thing3 = sr.Get();
            this.outcomes.Add(new Outcome(AdventureOutcome.Types.Spell, delta, thing3));
        });
        List<global::MOM.Unit> ignoreUnits = new List<global::MOM.Unit>();
        foreach (KeyValuePair<int, UnitStats> unitStat in this.unitStats)
        {
            UnitStats value = unitStat.Value;
            if (!adventureOutcomeDelta.unitStats.ContainsKey(unitStat.Key))
            {
                ignoreUnits.Add(value.unit);
            }
        }
        foreach (KeyValuePair<int, UnitStats> unitStat2 in adventureOutcomeDelta.unitStats)
        {
            if (!this.unitStats.ContainsKey(unitStat2.Key))
            {
                UnitStats value2 = unitStat2.Value;
                this.outcomes.Add(new Outcome(value2.IsHero ? AdventureOutcome.Types.Hero : AdventureOutcome.Types.Unit, 1, value2.unit));
                ignoreUnits.Add(value2.unit);
            }
        }
        foreach (KeyValuePair<int, UnitStats> unitStat3 in this.unitStats)
        {
            UnitStats value3 = unitStat3.Value;
            if (adventureOutcomeDelta.unitStats.TryGetValue(unitStat3.Key, out var value4))
            {
                AdventureOutcome.Types outcomeType = (value3.IsHero ? AdventureOutcome.Types.HeroStat : AdventureOutcome.Types.UnitStat);
                if (value3.unit.IsHero() ? (value3.xp < 1000) : (value3.xp < 120))
                {
                    this.ProcessValue(outcomeType, value3.xp, value4.xp, value4.unit, AdventureOutcome.StatType.Experience);
                }
                BattleFigure battleFigures = value3.battleFigures;
                BattleFigure battleFigures2 = value4.battleFigures;
                this.ProcessValue(outcomeType, battleFigures.attack, battleFigures2.attack, value4.unit, AdventureOutcome.StatType.Melee);
                this.ProcessValue(outcomeType, battleFigures.defence, battleFigures2.defence, value4.unit, AdventureOutcome.StatType.Armour);
                this.ProcessValue(outcomeType, battleFigures.resist, battleFigures2.resist, value4.unit, AdventureOutcome.StatType.Resist);
                this.ProcessValue(outcomeType, battleFigures.maxHitPoints, battleFigures2.maxHitPoints, value4.unit, AdventureOutcome.StatType.Hits);
                if (value3.unit.IsRangedUnit())
                {
                    this.ProcessValue(outcomeType, battleFigures.rangedAttack, battleFigures2.rangedAttack, value4.unit, AdventureOutcome.StatType.Ranged);
                    this.ProcessValue(outcomeType, Mathf.RoundToInt(battleFigures.rangedAttackChance * 100f), Mathf.RoundToInt(battleFigures2.rangedAttackChance * 100f), value4.unit, AdventureOutcome.StatType.RangedHitChance);
                }
                this.ProcessValue(outcomeType, Mathf.RoundToInt(battleFigures.attackChance * 100f), Mathf.RoundToInt(battleFigures2.attackChance * 100f), value4.unit, AdventureOutcome.StatType.MeleeHitChance);
                this.ProcessValue(outcomeType, Mathf.RoundToInt(battleFigures.defenceChance * 100f), Mathf.RoundToInt(battleFigures2.defenceChance * 100f), value4.unit, AdventureOutcome.StatType.RangedHitChance);
                this.ProcessValue(outcomeType, battleFigures.movementSpeed, battleFigures2.movementSpeed, value4.unit, AdventureOutcome.StatType.MovementPoints);
            }
            else if (value3.unit.currentFigureHP > 0 && value3.unit.group != null)
            {
                this.outcomes.Add(new Outcome(value3.IsHero ? AdventureOutcome.Types.Hero : AdventureOutcome.Types.Unit, -1, value3.unit));
            }
            else
            {
                this.outcomes.Add(new Outcome(value3.IsHero ? AdventureOutcome.Types.HeroDied : AdventureOutcome.Types.UnitDied, -1, value3.unit));
            }
        }
        foreach (KeyValuePair<global::MOM.Unit, List<DBReference<Skill>>> kvp in this.unitSkills)
        {
            if (ignoreUnits.Contains(kvp.Key))
            {
                continue;
            }
            List<DBReference<Skill>> value5 = kvp.Value;
            if (adventureOutcomeDelta.unitSkills.TryGetValue(kvp.Key, out var value6))
            {
                this.ProcessList(value5, value6, delegate(DBReference<Skill> sr, int delta)
                {
                    Skill thing2 = sr.Get();
                    this.outcomes.Add(new Outcome(AdventureOutcome.Types.Skill, delta, thing2, AdventureOutcome.StatType.None, null, kvp.Key));
                });
            }
        }
        List<TownLocation> ignoreTowns = new List<TownLocation>();
        foreach (KeyValuePair<int, TownStats> townStat in this.townStats)
        {
            if (!adventureOutcomeDelta.townStats.ContainsKey(townStat.Key))
            {
                ignoreTowns.Add(townStat.Value.town);
            }
        }
        foreach (KeyValuePair<int, TownStats> townStat2 in adventureOutcomeDelta.townStats)
        {
            if (!this.townStats.ContainsKey(townStat2.Key))
            {
                this.outcomes.Add(new Outcome(AdventureOutcome.Types.Town, 1, townStat2.Value.town));
                ignoreTowns.Add(townStat2.Value.town);
            }
        }
        foreach (KeyValuePair<int, TownStats> townStat3 in this.townStats)
        {
            TownStats value7 = townStat3.Value;
            if (adventureOutcomeDelta.townStats.TryGetValue(townStat3.Key, out var value8))
            {
                TownLocation town = value8.town;
                if (!ignoreTowns.Contains(town))
                {
                    this.ProcessValue(AdventureOutcome.Types.Population, value7.population, value8.population, null, AdventureOutcome.StatType.None, town);
                    this.ProcessList(value7.buildings, value8.buildings, delegate(DBReference<Building> building, int delta)
                    {
                        Building thing = building.Get();
                        this.outcomes.Add(new Outcome(AdventureOutcome.Types.Building, delta, thing, AdventureOutcome.StatType.None, town));
                    });
                    this.ProcessList(value7.resources, value8.resources, delegate(Resource resource, int delta)
                    {
                        this.outcomes.Add(new Outcome(AdventureOutcome.Types.Resource, delta, resource, AdventureOutcome.StatType.None, town));
                    });
                }
            }
            else
            {
                this.outcomes.Add(new Outcome(AdventureOutcome.Types.Town, -1, townStat3.Value.town));
            }
        }
        foreach (KeyValuePair<ERealm, int> book in adventureOutcomeDelta.books.books)
        {
            int num = book.Value;
            if (this.books.books.TryGetValue(book.Key, out var value9))
            {
                num -= value9;
            }
            if (num > 0)
            {
                if (num > 1)
                {
                    Debug.LogWarning("Adventure outcomes only support reward of one book in a realm at a time.");
                }
                this.outcomes.Add(new Outcome(AdventureOutcome.Types.Spellbook, 1, book.Key));
            }
        }
        foreach (EnchantmentInstance addedEnchantment in this.addedEnchantments)
        {
            if (!IgnoreEnchantment(addedEnchantment))
            {
                this.outcomes.Add(new Outcome(AdventureOutcome.Types.Enchantment, 1, addedEnchantment));
            }
        }
        foreach (EnchantmentInstance removedEnchantment in this.removedEnchantments)
        {
            if (!IgnoreEnchantment(removedEnchantment))
            {
                this.outcomes.Add(new Outcome(AdventureOutcome.Types.Enchantment, -1, removedEnchantment));
            }
        }
        this.ProcessList(this.traits, adventureOutcomeDelta.traits, delegate(Trait t, int delta)
        {
            this.outcomes.Add(new Outcome(AdventureOutcome.Types.Trait, delta, t));
        });
        adventureOutcomeDelta?.Destroy();
        return this.outcomes;
        bool IgnoreEnchantment(EnchantmentInstance e)
        {
            IEnchantable owner = e.manager.owner;
            if (owner is TownLocation item)
            {
                return ignoreTowns.Contains(item);
            }
            if (owner is global::MOM.Unit item2)
            {
                return ignoreUnits.Contains(item2);
            }
            return false;
        }
    }

    private void ProcessValue(AdventureOutcome.Types outcomeType, int original, int now, object thing = null, AdventureOutcome.StatType statType = AdventureOutcome.StatType.None, TownLocation location = null)
    {
        if (original != now)
        {
            this.outcomes.Add(new Outcome(outcomeType, now - original, thing, statType, location));
        }
    }

    private void ProcessList<T>(IEnumerable<T> original, IEnumerable<T> now, ListCallback<T> callback)
    {
        Dictionary<T, int> dictionary = new Dictionary<T, int>();
        foreach (T item in original)
        {
            if (dictionary.ContainsKey(item))
            {
                dictionary[item]++;
            }
            else
            {
                dictionary.Add(item, 1);
            }
        }
        Dictionary<T, int> dictionary2 = new Dictionary<T, int>();
        foreach (T item2 in now)
        {
            if (dictionary2.ContainsKey(item2))
            {
                dictionary2[item2]++;
            }
            else
            {
                dictionary2.Add(item2, 1);
            }
        }
        foreach (KeyValuePair<T, int> item3 in dictionary)
        {
            int value = 0;
            dictionary2.TryGetValue(item3.Key, out value);
            int num = value - item3.Value;
            if (num != 0)
            {
                callback(item3.Key, num);
            }
            dictionary2.Remove(item3.Key);
        }
        foreach (KeyValuePair<T, int> item4 in dictionary2)
        {
            if (item4.Value != 0)
            {
                callback(item4.Key, item4.Value);
            }
        }
    }
}
