using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoInclude(300, typeof(PlayerWizardAI))]
    [ProtoContract]
    public class PlayerWizard : Entity, IEnchantable, IAttributable, ISpellCaster, IEventDisplayName
    {
        public enum WizardStatus
        {
            Alive = 0,
            Banished = 1,
            Killed = 2,
            MAX = 3
        }

        public enum Color
        {
            None = 0,
            Green = 1,
            Blue = 2,
            Red = 3,
            Purple = 4,
            Yellow = 5,
            MAX = 6
        }

        public enum Familiar
        {
            Life = 0,
            Death = 1,
            Nature = 2,
            Chaos = 3,
            Sorcery = 4,
            Synergy = 5
        }

        private const int MAX_HERO_COUNT = 6;

        private const int OFFER_HERO_COUNT = 2;

        [ProtoMember(1)]
        public int ID;

        [ProtoMember(2)]
        private DBReference<Wizard> wizard;

        [ProtoMember(3)]
        public DBReference<Race> mainRace;

        [ProtoMember(4)]
        public string name;

        [ProtoMember(5)]
        public int mana;

        [ProtoMember(6)]
        public int money;

        [ProtoMember(7)]
        protected int castingSkill;

        [ProtoMember(8)]
        public int castingSkillDevelopment;

        [ProtoMember(9)]
        private int fame;

        [ProtoMember(10)]
        public FInt percentUnrestModifier;

        [ProtoMember(11)]
        public int unitLevelIncrease;

        [ProtoMember(12)]
        public int banishedTurn;

        [ProtoMember(13)]
        public int turnSkillLeft;

        [ProtoMember(14)]
        public bool showInfoForBanish;

        [ProtoMember(20)]
        private int taxRank;

        [ProtoMember(22)]
        public Color color;

        [ProtoMember(23)]
        public List<Reference<PlayerWizard>> discoveredWizards;

        [ProtoMember(30)]
        public List<Reference<Unit>> heroes = new List<Reference<Unit>>();

        [ProtoMember(31)]
        protected EnchantmentManager enchantmentManager;

        [ProtoMember(32)]
        protected SpellManager spellManager;

        [ProtoMember(33)]
        public MagicAndResearch magicAndResearch;

        [ProtoMember(34)]
        public Reference<TownLocation> summoningCircle;

        [ProtoMember(35)]
        public Reference<TownLocation> wizardTower;

        [ProtoMember(36)]
        private List<SummaryInfo> summaryInfo;

        [ProtoMember(37)]
        public List<SummaryInfo> summaryInfoRegistration;

        [ProtoMember(38)]
        public List<DeadHero> deadHeroes = new List<DeadHero>();

        [ProtoMember(39)]
        public List<Multitype<Vector3i, bool>> raisedVolcanoes = new List<Multitype<Vector3i, bool>>();

        [ProtoMember(40)]
        public Attributes attributes;

        [ProtoMember(41)]
        public List<Artefact> artefacts = new List<Artefact>();

        [ProtoMember(42)]
        protected List<DBReference<Trait>> traits = new List<DBReference<Trait>>();

        [ProtoMember(43)]
        public StatHistory statHistory = new StatHistory();

        [ProtoMember(44)]
        private DiplomacyManager diplomacy;

        [ProtoMember(45)]
        public DBReference<Personality> personality;

        [ProtoMember(52)]
        public int alchemyRatio = 2;

        [ProtoMember(53)]
        public bool isCustom;

        [ProtoMember(54)]
        public FInt globalDispelDificultyIncrease = FInt.ZERO;

        [ProtoMember(55)]
        public NetDictionary<ERealm, FInt> realmDispelDificultyIncrease = new NetDictionary<ERealm, FInt>();

        [ProtoMember(56)]
        public NetDictionary<ERealm, FInt> researchDiscontPercent = new NetDictionary<ERealm, FInt>();

        [ProtoMember(57)]
        public NetDictionary<ERealm, FInt> castCostPercentDiscountRealms = new NetDictionary<ERealm, FInt>();

        [ProtoMember(58)]
        public NetDictionary<DBReference<Spell>, FInt> castCostPercentDiscountSpells = new NetDictionary<DBReference<Spell>, FInt>();

        [ProtoMember(59)]
        public int castingSkillBonus;

        [ProtoMember(60)]
        public int skillIncomBonus;

        [ProtoMember(61)]
        public FInt easierDispelling = FInt.ONE;

        [ProtoMember(62)]
        public bool ignorSpellcastingRange;

        [ProtoMember(63)]
        public FInt lowerEnchantmentPercentUpkeepCost = FInt.ZERO;

        [ProtoMember(64)]
        public FInt channelerFantasticUnitsUpkeepDiscount = FInt.ZERO;

        [ProtoMember(65)]
        public FInt conjuerFantasticUnitsUpkeepDiscount = FInt.ZERO;

        [ProtoMember(66)]
        public FInt lowerFantasticUnitsPercentSummonCost = FInt.ZERO;

        [ProtoMember(67)]
        public FInt lowerResearchFantasticUnitsPercentCost = FInt.ZERO;

        [ProtoMember(68)]
        public FInt buildingsResearchPercentBonus = FInt.ZERO;

        [ProtoMember(69)]
        public FInt townsPowerPercentBonus = FInt.ZERO;

        [ProtoMember(70)]
        public FInt rebelModifier;

        [ProtoMember(71)]
        public float lowerMercenaryAndHeroCost;

        [ProtoMember(72)]
        public FInt manaPercentBonus = FInt.ZERO;

        [ProtoMember(73)]
        public bool myrranRaces;

        [ProtoMember(74)]
        public FInt nodesPowerPercentBonus = FInt.ZERO;

        [ProtoMember(75)]
        public bool famous;

        [ProtoMember(76)]
        private int temporaryFame;

        [ProtoMember(77)]
        public int nextEventDelay;

        [ProtoMember(78)]
        public bool detectMagic;

        [ProtoMember(79)]
        public Familiar familiar;

        [ProtoMember(80)]
        public int banishedBy;

        [ProtoMember(81)]
        public bool banishAnimPlayed;

        [ProtoMember(82)]
        public bool banishTutorialShown;

        [ProtoMember(83)]
        private string quote;

        [ProtoMember(84)]
        public bool isAlive;

        [ProtoMember(85)]
        public bool unitMagicWeapon;

        [ProtoMember(86)]
        public float lowerArtefactCost;

        [ProtoMember(87)]
        public NetDictionary<ERealm, FInt> nodesMasteryPercentBonus;

        [ProtoMember(88)]
        public int startingSpellsResearchCost;

        [ProtoMember(89)]
        public int allCitiesLostTurn;

        [ProtoMember(90)]
        public NetDictionary<string, string> townExtraBuilding;

        [ProtoMember(91)]
        public NetDictionary<string, string> unitModificationSkills;

        [ProtoMember(92)]
        public string startingUnit;

        [ProtoMember(93)]
        public FInt fantasticNatureUnitsUpkeepDiscount = FInt.ZERO;

        [ProtoMember(94)]
        public NetDictionary<string, string> newBuildedTownsModificationEnchs;

        [ProtoMember(95)]
        public string dedicatedRace;

        [ProtoMember(96)]
        public bool traitTechMagic;

        [ProtoMember(97)]
        public bool myrranRefugee;

        [ProtoMember(98)]
        public int heroHireBonus;

        [ProtoMember(99)]
        public NetDictionary<string, string> heroExtraSkill;

        [ProtoMember(101)]
        public string deathFigureCastingSkillFilter;

        [ProtoMember(100)]
        public int traitThePirat;

        [ProtoMember(102)]
        public bool seaMasterTrait;

        public bool addCheatResEachTurn;

        [ProtoIgnore]
        public WizardControlRegion controlRegion;

        [ProtoIgnore]
        private int manaIncomeCache = int.MinValue;

        [ProtoIgnore]
        private bool allowToStoreManaIncomeCache;

        [ProtoIgnore]
        public Unit selectedSummon;

        public string Background => this.wizard.Get().background;

        public string Quote
        {
            get
            {
                return this.quote;
            }
            set
            {
                this.quote = value;
            }
        }

        public Texture2D Graphic => this.wizard.Get().GetDescriptionInfo().GetTexture();

        public bool IsHuman => this.ID == PlayerWizard.HumanID();

        public int TaxRank
        {
            get
            {
                return this.taxRank;
            }
            set
            {
                this.taxRank = value;
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (entity.Value is TownLocation townLocation && townLocation.GetOwnerID() == this.ID)
                    {
                        townLocation.SetUnrestDirty();
                    }
                }
            }
        }

        public int ModifiableCastingSkill
        {
            get
            {
                return this.castingSkill;
            }
            set
            {
                this.castingSkill = Mathf.Max(0, this.castingSkill);
            }
        }

        public PlayerWizard()
        {
        }

        public PlayerWizard(Wizard w, Race r = null, List<Tag> books = null, List<Spell> spells = null, List<Trait> traits = null, Color wizardColor = Color.None, bool custom = false)
        {
            this.RegisterEntity(GameManager.Get().wizards.Count + 1);
            this.Initialize(w, r, books, spells, traits, wizardColor, custom);
        }

        internal void Initialize(Wizard w, Race r = null, List<Tag> books = null, List<Spell> spells = null, List<Trait> customTraits = null, Color wizardColor = Color.None, bool custom = false)
        {
            this.wizard = w;
            this.name = w.GetDescriptionInfo().GetLocalizedName();
            this.mana = 0;
            this.isCustom = custom;
            this.isAlive = true;
            this.allCitiesLostTurn = -1;
            if (wizardColor == Color.None)
            {
                int num = (int)wizardColor;
                Color c;
                num = (int)(c = (Color)(num + 1));
                while (c != Color.MAX && GameManager.GetWizards().Find((PlayerWizard o) => o.color == c) != null)
                {
                    num = (int)(c = (Color)(num + 1));
                }
                if (c == Color.MAX)
                {
                    c = Color.None;
                }
                this.color = c;
            }
            if (r == null)
            {
                List<Race> list = DataBase.GetType<Race>().FindAll((Race o) => o.baseRace);
                int index = global::UnityEngine.Random.Range(0, list.Count);
                this.mainRace = list[index];
            }
            else
            {
                this.mainRace = r;
            }
            Tag tag = (Tag)TAG.MAGIC_BOOK;
            if (books != null)
            {
                this.attributes = new Attributes(this);
                if (!custom)
                {
                    CountedTag[] tags = w.tags;
                    foreach (CountedTag countedTag in tags)
                    {
                        if (countedTag.tag != tag && countedTag.tag.parent != tag)
                        {
                            this.attributes.AddToBase(countedTag, setDirty: false);
                        }
                    }
                }
                foreach (Tag book in books)
                {
                    this.attributes.AddToBase(book, FInt.ONE, setDirty: false);
                }
            }
            else
            {
                this.attributes = new Attributes(this, w.tags);
            }
            FInt fInt = FInt.ZERO;
            Tag tag2 = (Tag)TAG.MAGIC_BOOK;
            foreach (KeyValuePair<DBReference<Tag>, FInt> baseAttribute in this.attributes.baseAttributes)
            {
                Tag tag3 = baseAttribute.Key.Get();
                if (tag3.parent == tag)
                {
                    if (baseAttribute.Value > fInt)
                    {
                        tag2 = tag3;
                        fInt = baseAttribute.Value;
                    }
                    else if (baseAttribute.Value == fInt && fInt > 0)
                    {
                        tag2 = (Tag)TAG.MAGIC_BOOK;
                    }
                }
            }
            if (tag2 == (Tag)TAG.LIFE_MAGIC_BOOK)
            {
                this.familiar = Familiar.Life;
            }
            else if (tag2 == (Tag)TAG.CHAOS_MAGIC_BOOK)
            {
                this.familiar = Familiar.Chaos;
            }
            else if (tag2 == (Tag)TAG.NATURE_MAGIC_BOOK)
            {
                this.familiar = Familiar.Nature;
            }
            else if (tag2 == (Tag)TAG.DEATH_MAGIC_BOOK)
            {
                this.familiar = Familiar.Death;
            }
            else if (tag2 == (Tag)TAG.SORCERY_MAGIC_BOOK)
            {
                this.familiar = Familiar.Sorcery;
            }
            else
            {
                this.familiar = Familiar.Synergy;
            }
            this.TaxRank = 2;
            this.money = DifficultySettingsData.GetSettingAsInt("UI_DIFF_STARTING_GOLD");
            this.mana = 30;
            this.attributes.SetDirty();
            if (spells != null)
            {
                foreach (Spell spell in spells)
                {
                    this.GetSpellManager().Add(spell);
                    this.startingSpellsResearchCost += spell.researchCost;
                }
            }
            this.GetMagicAndResearch();
            this.castingSkill = Mathf.Max(1, this.GetAttributes().GetFinal((Tag)TAG.MAGIC_BOOK).ToInt() * 2);
            if (customTraits != null)
            {
                this.traits.Clear();
                foreach (Trait customTrait in customTraits)
                {
                    this.AddTrait(customTrait);
                }
            }
            this.Quote = (this.isCustom ? global::DBUtils.Localization.Get("DES_CUSTOM_WIZARD_QUOTE", true) : global::DBUtils.Localization.Get(this.wizard.Get().quote, true));
            if (this.addCheatResEachTurn)
            {
                this.castingSkill += 1000;
                this.mana += 1000;
            }
        }

        [ProtoAfterDeserialization]
        public void AfterDeserialize()
        {
            this.GetAttributes().owner = this;
            if (this.magicAndResearch != null)
            {
                this.magicAndResearch.owner = this;
            }
            if (this.enchantmentManager != null)
            {
                this.enchantmentManager.owner = this;
            }
            if (this.spellManager != null)
            {
                this.spellManager.owner = this;
            }
            if (this.artefacts == null)
            {
                this.artefacts = new List<Artefact>();
            }
            if (this.heroes == null)
            {
                this.heroes = new List<Reference<Unit>>();
            }
            if (this.deadHeroes == null)
            {
                this.deadHeroes = new List<DeadHero>();
            }
            if (this.traits == null)
            {
                this.traits = new List<DBReference<Trait>>();
            }
            if (this.unitMagicWeapon)
            {
                ScriptLibrary.Call("TINIT_Alchemy", this);
                this.unitMagicWeapon = false;
            }
        }

        public void Destroy()
        {
            EntityManager.UnregisterEntity(this);
        }

        public DiplomacyManager GetDiplomacy()
        {
            if (this.diplomacy == null)
            {
                this.diplomacy = new DiplomacyManager(this);
            }
            return this.diplomacy;
        }

        public static int HumanID()
        {
            return 1;
        }

        public static int InvalidWizardID()
        {
            return 0;
        }

        public void TownEndTurnUpdate()
        {
            List<Location> registeredLocations = GameManager.Get().registeredLocations;
            if (registeredLocations == null)
            {
                return;
            }
            foreach (Location item in registeredLocations)
            {
                if (item.owner != this.ID)
                {
                    continue;
                }
                if (item is TownLocation)
                {
                    (item as TownLocation).EndTurnUpdate();
                }
                else if (item.locationType == ELocationType.Node && item.GetUnits().Count == 0)
                {
                    if (item.melding == null)
                    {
                        item.SetOwnerID(0);
                    }
                    else if (item.melding.meldOwner != this.ID)
                    {
                        item.SetOwnerID(item.melding.meldOwner);
                    }
                }
            }
        }

        public void EndTurnUpdate()
        {
            this.TownEndTurnUpdate();
            this.UpdateTownMarkers();
            this.statHistory.ProcessTurn(this);
            if (this.ID != PlayerWizard.HumanID())
            {
                return;
            }
            foreach (Group item in GameManager.GetGroupsOfWizard(this.ID))
            {
                if (item.Action == Group.GroupActions.Skip)
                {
                    item.Action = Group.GroupActions.None;
                }
            }
        }

        public void UpdateTownMarkers()
        {
            List<Location> registeredLocations = GameManager.Get().registeredLocations;
            if (registeredLocations == null)
            {
                return;
            }
            foreach (Location item in registeredLocations)
            {
                if (item.owner == this.ID && item is TownLocation)
                {
                    (item as TownLocation).UpdateMarkers();
                }
            }
        }

        public void WizardGainOrLostLocation()
        {
            if (this.controlRegion != null)
            {
                this.controlRegion.regionsDirty = true;
            }
        }

        public int CalculateMoneyIncome(bool includeUpkeep = false, StatDetails details = null)
        {
            if (GameManager.Get()?.timeStopMaster != null)
            {
                return 0;
            }
            int num = 0;
            int income = 0;
            int num2 = 0;
            List<Location> registeredLocations = GameManager.Get().registeredLocations;
            if (registeredLocations != null)
            {
                foreach (Location item in registeredLocations)
                {
                    if (item is TownLocation && item.GetOwnerID() == this.ID)
                    {
                        TownLocation townLocation = item as TownLocation;
                        income += townLocation.CalculateMoneyIncome(includeUpkeep, details);
                    }
                }
                if (this is PlayerWizardAI playerWizardAI)
                {
                    int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
                    income += settingAsInt * 8;
                    income = playerWizardAI.AIIncomeScallar(income);
                }
                num2 = income;
                if (includeUpkeep)
                {
                    int num3 = 0;
                    foreach (Group registeredGroup in GameManager.Get().registeredGroups)
                    {
                        if (registeredGroup.GetOwnerID() != this.ID || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType) || registeredGroup.GetUnits() == null)
                        {
                            continue;
                        }
                        foreach (Reference<Unit> unit in registeredGroup.GetUnits())
                        {
                            int num4 = unit.Get().GetAttributes().GetFinal(TAG.UPKEEP_GOLD)
                                .ToInt();
                            details?.Add("UI_GOLD_DETAILS_UNITS_UPKEEP", -num4);
                            income -= num4;
                            num3 += num4;
                            List<SkillScript> skillsByType = unit.Get().GetSkillsByType(ESkillType.IncomeChange);
                            if (skillsByType == null)
                            {
                                continue;
                            }
                            foreach (SkillScript item2 in skillsByType)
                            {
                                int num5 = (int)ScriptLibrary.Call(item2.activatorMain, unit.Get(), null, null, item2, null, null, null, null);
                                if (num5 > 0)
                                {
                                    details?.Add("UI_GOLD_DETAILS_UNITS_INCOME", num5);
                                }
                                else
                                {
                                    details?.Add("UI_GOLD_DETAILS_UNITS_UPKEEP", num5);
                                }
                                income += num5;
                            }
                            num2 = income;
                            num = 0;
                            unit.Get().ProcessIntigerScripts(EEnchantmentType.GoldModifier, ref income, ref num);
                            unit.Get().ProcessIntigerScripts(EEnchantmentType.GoldModifierMP, ref income, ref num);
                            details?.Add("UI_GOLD_DETAILS_UNITS_INCOME", num2 - income);
                            details?.Add("UI_GOLD_DETAILS_UNITS_UPKEEP", num);
                            income += num;
                        }
                    }
                    int num6 = Mathf.Min(num3, this.GetFame());
                    income += num6;
                    details?.Add("UI_GOLD_DETAILS_UNITS_UPKEEP_FAME", Mathf.Min(num3, this.GetFame()));
                    num2 = income;
                }
                income += Mathf.Max(0, this.CalculateFoodIncome(includeUpkeep: true) / 2);
                details?.Add("UI_GOLD_DETAILS_FOOD", income - num2);
            }
            this.ProcessIntigerScripts(EEnchantmentType.GoldModifier, ref income, details);
            this.ProcessIntigerScripts(EEnchantmentType.GoldModifierMP, ref income, details);
            return income;
        }

        public int CalculatePowerIncome(StatDetails details = null)
        {
            if (this.wizardTower == null)
            {
                return 0;
            }
            int num = 0;
            int income = 0;
            List<Location> registeredLocations = GameManager.Get().registeredLocations;
            if (registeredLocations != null)
            {
                foreach (Location item in registeredLocations)
                {
                    if (item is TownLocation && item.owner == this.ID)
                    {
                        num = income;
                        int num2 = (item as TownLocation).CalculatePowerIncome();
                        income += num2;
                        details?.Add("UI_POWER_DETAILS_TOWN_INCOME", income - num);
                    }
                    else
                    {
                        if (item.locationType != ELocationType.Node || item.owner != this.ID || (item.melding?.meldOwner ?? 0) != this.ID)
                        {
                            continue;
                        }
                        num = income;
                        int value = item.NodePowerIncome();
                        item.ProcessIntigerScripts(EEnchantmentType.PowerModifier, ref value);
                        income += value;
                        int num3 = 0;
                        if (this.nodesPowerPercentBonus != 0)
                        {
                            num3 += (value * this.nodesPowerPercentBonus).ToInt();
                        }
                        if (this.nodesMasteryPercentBonus != null)
                        {
                            if (item.source.Get() == (MagicNode)(Enum)MAGIC_NODE.CHAOS && this.nodesMasteryPercentBonus.ContainsKey(ERealm.Chaos))
                            {
                                num3 += (value * this.nodesMasteryPercentBonus[ERealm.Chaos]).ToInt();
                            }
                            if (item.source.Get() == (MagicNode)(Enum)MAGIC_NODE.NATURE && this.nodesMasteryPercentBonus.ContainsKey(ERealm.Nature))
                            {
                                num3 += (value * this.nodesMasteryPercentBonus[ERealm.Nature]).ToInt();
                            }
                            if (item.source.Get() == (MagicNode)(Enum)MAGIC_NODE.SORCERY && this.nodesMasteryPercentBonus.ContainsKey(ERealm.Sorcery))
                            {
                                num3 += (value * this.nodesMasteryPercentBonus[ERealm.Sorcery]).ToInt();
                            }
                        }
                        if (this.nodesPowerPercentBonus != 0 && this.nodesMasteryPercentBonus != null && ((item.source.Get() == (MagicNode)(Enum)MAGIC_NODE.CHAOS && this.nodesMasteryPercentBonus.ContainsKey(ERealm.Chaos)) || (item.source.Get() == (MagicNode)(Enum)MAGIC_NODE.NATURE && this.nodesMasteryPercentBonus.ContainsKey(ERealm.Nature)) || (item.source.Get() == (MagicNode)(Enum)MAGIC_NODE.SORCERY && this.nodesMasteryPercentBonus.ContainsKey(ERealm.Sorcery))))
                        {
                            num3 += value;
                        }
                        income += num3;
                        details?.Add("UI_POWER_DETAILS_NODE_INCOME", income - num);
                    }
                }
            }
            if (this.GetVolcanoList().Count > 0)
            {
                num = income;
                int num4 = 1;
                GameplayConfiguration gameplayConfiguration = DataBase.Get<GameplayConfiguration>(GAMEPLAY_CONFIGURATION.DEFAULT);
                if (gameplayConfiguration != null && gameplayConfiguration.option != null)
                {
                    Gc gc = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Volcano Spell Power Bonus");
                    if (gc != null)
                    {
                        try
                        {
                            num4 = Convert.ToInt32(gc.setting);
                        }
                        catch
                        {
                            Debug.Log("Volcano Spell Power Bonus configuration contains invalid type, expected int");
                        }
                    }
                }
                income += this.GetVolcanoList().Count * num4;
                details?.Add("UI_POWER_DETAILS_VOLCANO_INCOME", income - num);
            }
            this.ProcessIntigerScripts(EEnchantmentType.PowerModifier, ref income, details);
            this.ProcessIntigerScripts(EEnchantmentType.PowerModifierMP, ref income, details);
            if (this.addCheatResEachTurn)
            {
                num = income;
                income += 1000;
                details?.Add("UI_POWER_DETAILS_CHEAT", income - num);
            }
            return income;
        }

        public int CalculateManaIncome(bool includeUpkeep = false, StatDetails details = null)
        {
            if (this.manaIncomeCache != int.MinValue && this.allowToStoreManaIncomeCache)
            {
                return this.manaIncomeCache;
            }
            if (GameManager.Get()?.timeStopMaster != null)
            {
                int upkeepCost = ((Enchantment)ENCH.TIME_STOP).upkeepCost;
                FInt fInt = this.lowerEnchantmentPercentUpkeepCost;
                return -(upkeepCost - upkeepCost * fInt).ToInt();
            }
            int num = 0;
            int value = this.GetMagicAndResearch().GetManaIncome();
            if (this.wizardTower == null)
            {
                value = 0;
            }
            details?.Add("UI_MANA_DETAILS_POWER", value);
            if (this is PlayerWizardAI playerWizardAI && playerWizardAI.mana < 2000)
            {
                value = playerWizardAI.AIIncomeScallar(value);
            }
            if (this is PlayerWizardAI)
            {
                int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
                int turnNumber = TurnManager.GetTurnNumber();
                int wizardLocationsCount = GameManager.GetWizardLocationsCount(this.ID);
                int num2 = 0;
                num2 += (int)Mathf.Min(50f, (float)(settingAsInt * turnNumber / 15));
                num2 = (int)((float)num2 * Mathf.Clamp((float)wizardLocationsCount * 2f, 0f, 3f));
                value += num2;
            }
            int num3 = value;
            if (this.manaPercentBonus > 0)
            {
                value += (value * this.manaPercentBonus).ToInt();
                details?.Add("UI_MANA_DETAILS_INCOME_BONUS", value - num3);
            }
            if (includeUpkeep)
            {
                foreach (Location registeredLocation in GameManager.Get().registeredLocations)
                {
                    if (!(registeredLocation is TownLocation) || registeredLocation.GetOwnerID() != this.ID)
                    {
                        continue;
                    }
                    TownLocation townLocation = registeredLocation as TownLocation;
                    if (townLocation.buildings != null)
                    {
                        num3 = value;
                        foreach (DBReference<Building> building in townLocation.buildings)
                        {
                            value -= building.Get().upkeepManaCost;
                        }
                        details?.Add("UI_MANA_DETAILS_TOWN_UPKEEP", value - num3);
                    }
                    num3 = value;
                    num = 0;
                    townLocation.ProcessIntigerScripts(EEnchantmentType.ManaModifier, ref value, ref num);
                    townLocation.ProcessIntigerScripts(EEnchantmentType.ManaModifierMP, ref value, ref num);
                    details?.Add("UI_MANA_DETAILS_TOWN_UPKEEP", num);
                    details?.Add("UI_MANA_DETAILS_TOWN_INCOME", value - num3);
                    value += num;
                }
                FInt zERO = FInt.ZERO;
                FInt zERO2 = FInt.ZERO;
                FInt zERO3 = FInt.ZERO;
                FInt zERO4 = FInt.ZERO;
                foreach (Group registeredGroup in GameManager.Get().registeredGroups)
                {
                    if (registeredGroup.GetOwnerID() != this.ID || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType))
                    {
                        continue;
                    }
                    Group group = registeredGroup;
                    if (group.GetUnits() == null)
                    {
                        continue;
                    }
                    foreach (Reference<Unit> unit in group.GetUnits())
                    {
                        zERO += unit.Get().GetManaUpkeep();
                        zERO2 += unit.Get().GetUpkeepChannelerManaDiscount();
                        zERO3 += unit.Get().GetUpkeepConjuerManaDiscount();
                        zERO4 += unit.Get().GetUpkeepNatureSummonerManaDiscount();
                        num3 = value;
                        unit.Get().ProcessIntigerScripts(EEnchantmentType.ManaModifier, ref value);
                        unit.Get().ProcessIntigerScripts(EEnchantmentType.ManaModifierMP, ref value);
                        details?.Add("UI_MANA_DETAILS_UNITS_INCOME", value - num3);
                    }
                }
                int num4 = zERO.ReturnRoundedCeil().ToInt();
                int num5 = zERO2.ReturnRoundedFloor().ToInt();
                int num6 = zERO3.ReturnRoundedFloor().ToInt();
                int num7 = zERO4.ReturnRoundedFloor().ToInt();
                details?.Add("UI_MANA_DETAILS_UNITS_UPKEEP", -num4);
                if (num5 > 0)
                {
                    details?.Add("UI_MANA_CHANNELER_DISCOUNT", num5);
                }
                if (num6 > 0)
                {
                    details?.Add("UI_MANA_CONJUER_DISCOUNT", num6);
                }
                if (num7 > 0)
                {
                    details?.Add("UI_MANA_NATURE_SUMMONER_DISCOUNT", num7);
                }
                int num8 = num4 - num5 - num6 - num7;
                value -= num8;
                int totalManaCost = EnchantmentRegister.GetTotalManaCost(this);
                if (totalManaCost > 0)
                {
                    int num9 = 0;
                    if (this.lowerEnchantmentPercentUpkeepCost > 0)
                    {
                        num9 = (totalManaCost * this.lowerEnchantmentPercentUpkeepCost).ToInt();
                    }
                    value -= totalManaCost - num9;
                    details?.Add("UI_MANA_DETAILS_ENCHANTMENTS", -totalManaCost);
                    if (num9 > 0)
                    {
                        details?.Add("UI_MANA_DETAILS_ENCHANTMENTS_DISCOUNT", num9);
                    }
                }
            }
            if (this.wizardTower != null)
            {
                this.ProcessIntigerScripts(EEnchantmentType.ManaModifier, ref value, details);
                this.ProcessIntigerScripts(EEnchantmentType.ManaModifierMP, ref value, details);
            }
            if (this.addCheatResEachTurn)
            {
                num3 = value;
                value += 1000;
                details?.Add("UI_MANA_DETAILS_CHEAT", value - num3);
            }
            if (this.allowToStoreManaIncomeCache)
            {
                this.manaIncomeCache = value;
            }
            return value;
        }

        public int CalculateResearchIncome(StatDetails details = null)
        {
            if (this.wizardTower == null)
            {
                return 0;
            }
            int income = this.GetMagicAndResearch().GetResearchIncome();
            if (this is PlayerWizardAI playerWizardAI)
            {
                income = playerWizardAI.AIIncomeScallar(income);
            }
            details?.Add("UI_RESEARCH_DETAILS_POWER", income);
            int num = income;
            foreach (Location registeredLocation in GameManager.Get().registeredLocations)
            {
                if (registeredLocation is TownLocation && registeredLocation.GetOwnerID() == this.ID)
                {
                    (registeredLocation as TownLocation).ProcessIntigerScripts(EEnchantmentType.ResearchModifier, ref income);
                }
            }
            details?.Add("UI_RESEARCH_DETAILS_TOWN_INCOME", income - num);
            num = income;
            if (this.buildingsResearchPercentBonus > 0)
            {
                income += (income * this.buildingsResearchPercentBonus).ToInt();
            }
            details?.Add("UI_RESEARCH_DETAILS_SAGE_MASTERY", income - num);
            this.ProcessIntigerScripts(EEnchantmentType.ResearchModifier, ref income, details);
            num = income;
            foreach (Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetOwnerID() != this.ID || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType))
                {
                    continue;
                }
                foreach (Reference<Unit> unit2 in registeredGroup.GetUnits())
                {
                    Unit unit = unit2.Get();
                    income += unit.GetAttributes().GetFinal(TAG.RESEARCH_PRODUCTION).ToInt();
                }
            }
            if (this.buildingsResearchPercentBonus > 0)
            {
                income += ((income - num) * this.buildingsResearchPercentBonus).ToInt();
            }
            details?.Add("UI_RESEARCH_DETAILS_UNITS_INCOME", income - num);
            return income;
        }

        public int CalculateFoodIncome(bool includeUpkeep = false, StatDetails details = null, bool reportStaration = true)
        {
            if (GameManager.Get()?.timeStopMaster != null)
            {
                return 0;
            }
            int num = 0;
            int income = 0;
            List<Location> registeredLocations = GameManager.Get().registeredLocations;
            if (registeredLocations != null)
            {
                foreach (Location item in registeredLocations)
                {
                    if (item is TownLocation && item.owner == this.ID)
                    {
                        TownLocation townLocation = item as TownLocation;
                        income += Mathf.Max(0, townLocation.CalculateFoodFinalIncome(reportStaration));
                    }
                }
            }
            if (this is PlayerWizardAI playerWizardAI)
            {
                income = playerWizardAI.AIIncomeScallar(income, 3f);
            }
            details?.Add("UI_FOOD_DETAILS_TOWN_INCOME", income);
            int num3;
            if (includeUpkeep)
            {
                foreach (Group registeredGroup in GameManager.Get().registeredGroups)
                {
                    if (registeredGroup.GetOwnerID() != this.ID || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType) || registeredGroup.GetUnits() == null)
                    {
                        continue;
                    }
                    foreach (Reference<Unit> unit in registeredGroup.GetUnits())
                    {
                        int num2 = unit.Get().GetAttributes().GetFinal(TAG.UPKEEP_FOOD)
                            .ToInt();
                        income -= num2;
                        details?.Add("UI_FOOD_DETAILS_UNITS_UPKEEP", -num2);
                        num = 0;
                        num3 = income;
                        unit.Get().ProcessIntigerScripts(EEnchantmentType.FoodModifier, ref income, ref num);
                        unit.Get().ProcessIntigerScripts(EEnchantmentType.FoodModifierMP, ref income, ref num);
                        details?.Add("UI_FOOD_DETAILS_UNITS_UPKEEP", num);
                        details?.Add("UI_FOOD_DETAILS_UNITS_INCOME", income - num3);
                        income += num;
                    }
                }
            }
            this.ProcessIntigerScripts(EEnchantmentType.FoodModifier, ref income, details);
            this.ProcessIntigerScripts(EEnchantmentType.FoodModifierMP, ref income, details);
            num3 = income;
            if (this.addCheatResEachTurn)
            {
                income += 1000;
                details?.Add("UI_FOOD_DETAILS_CHEAT", income - num3);
            }
            return income;
        }

        public EnchantmentManager GetEnchantmentManager()
        {
            if (this.enchantmentManager == null)
            {
                this.enchantmentManager = new EnchantmentManager(this);
            }
            return this.enchantmentManager;
        }

        public Attributes GetAttributes()
        {
            return this.attributes;
        }

        public void EnsureFinal()
        {
            this.GetAttributes().GetFinalDictionary();
        }

        public Tax GetTaxRank()
        {
            List<Tax> type = DataBase.GetType<Tax>();
            if (type.Count > this.TaxRank)
            {
                return type[this.TaxRank];
            }
            return null;
        }

        public FInt GetUnrestFromTax()
        {
            List<Tax> type = DataBase.GetType<Tax>();
            if (type.Count > this.TaxRank)
            {
                return type[this.TaxRank].rebelion;
            }
            return FInt.ZERO;
        }

        public FInt GetIncomeFromTax()
        {
            List<Tax> type = DataBase.GetType<Tax>();
            if (type.Count > this.TaxRank)
            {
                return type[this.TaxRank].income;
            }
            return FInt.ZERO;
        }

        public SpellManager GetSpellManager()
        {
            if (this.spellManager == null)
            {
                this.spellManager = new SpellManager(this);
            }
            return this.spellManager;
        }

        public MagicAndResearch GetMagicAndResearch()
        {
            if (this.magicAndResearch == null)
            {
                this.magicAndResearch = new MagicAndResearch(this);
            }
            return this.magicAndResearch;
        }

        public void AdvanceCastingSkill()
        {
            int skillIncome = this.GetMagicAndResearch().GetSkillIncome();
            this.castingSkillDevelopment += skillIncome;
            while (this.castingSkillDevelopment >= this.GetNextLevelCastingSkill())
            {
                this.castingSkillDevelopment -= this.GetNextLevelCastingSkill();
                this.castingSkill++;
            }
        }

        public void PrepareForCast()
        {
            MagicAndResearch magicAndResearch = this.GetMagicAndResearch();
            if (magicAndResearch.curentlyCastSpell == null)
            {
                return;
            }
            Spell curentlyCastSpell = magicAndResearch.curentlyCastSpell;
            this.GetMagicAndResearch().GetCastingProgress(out var curentStatus, out var _, out var _);
            if (!(curentStatus >= 1f))
            {
                return;
            }
            if ((bool)ScriptLibrary.Call("CounterMagicWorld", curentlyCastSpell, this))
            {
                GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
                HUD.Get().SetCasting(null);
                return;
            }
            if (curentlyCastSpell.targetType.enumType == ETargetType.WorldSummon)
            {
                if (this.SummonToWorld(curentlyCastSpell))
                {
                    this.GetMagicAndResearch().ResetCasting();
                }
                else
                {
                    this.GetMagicAndResearch().ResetCasting(returnMana: true);
                }
                return;
            }
            SummaryInfo summaryInfo = new SummaryInfo();
            summaryInfo.spell = curentlyCastSpell;
            summaryInfo.title = global::DBUtils.Localization.Get("UI_CASTINGS_SPELL", true);
            summaryInfo.graphic = curentlyCastSpell.GetDescriptionInfo().graphic;
            summaryInfo.summaryType = SummaryInfo.SummaryType.eCastingSpell;
            if (curentlyCastSpell == (Spell)SPELL.ENCHANT_ITEM || curentlyCastSpell == (Spell)SPELL.CREATE_ARTEFACT)
            {
                summaryInfo.summaryType = SummaryInfo.SummaryType.eCraftingArtefact;
                summaryInfo.graphic = magicAndResearch.craftItemSpell.artefact.graphic;
                summaryInfo.artefact = magicAndResearch.craftItemSpell.artefact;
                ScriptLibrary.Call(curentlyCastSpell.worldScript, this, curentlyCastSpell, null, null);
                FSMMapGame.CastEffect(Vector3i.invalid, curentlyCastSpell, this);
                GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
            }
            if (summaryInfo != null)
            {
                this.AddNotification(summaryInfo);
                GameManager.GetHumanWizard().GetMagicAndResearch().ResetCasting();
            }
        }

        public bool SummonToWorld(Spell spell)
        {
            if (spell.targetType.enumType == ETargetType.WorldSummon && this.summoningCircle != null)
            {
                bool flag = false;
                this.summoningCircle.Get();
                List<Reference<Unit>> list = new List<Reference<Unit>>();
                list.AddRange(this.summoningCircle.Get().GetUnits());
                if ((spell == (Spell)SPELL.SUMMON_HERO || spell == (Spell)SPELL.SUMMON_CHAMPION || spell == (Spell)SPELL.INCARNATION || spell == (Spell)SPELL.TECHCARNATION) && this.heroes.Count >= 6)
                {
                    if (this.ID == PlayerWizard.HumanID())
                    {
                        PopupGeneral.OpenPopup(null, "UI_HERO_SUMMONING_FAILED", "UI_HERO_LIMIT", "UI_OK");
                    }
                    return false;
                }
                if (spell == (Spell)SPELL.SUMMON_HERO || spell == (Spell)SPELL.SUMMON_CHAMPION)
                {
                    return FSMMapGame.Get().PickSumon(this, spell, list);
                }
                flag = (bool)ScriptLibrary.Call(spell.worldScript, this, spell, this.summoningCircle.Get(), null);
                if (flag && this.ID == PlayerWizard.HumanID())
                {
                    List<Reference<Unit>> units = this.summoningCircle.Get().GetUnits();
                    Unit unit = null;
                    foreach (Reference<Unit> item in units)
                    {
                        if (!list.Contains(item))
                        {
                            unit = item;
                            break;
                        }
                    }
                    if (unit != null)
                    {
                        this.AddNotification(new SummaryInfo
                        {
                            summaryType = SummaryInfo.SummaryType.eUnitSummoned,
                            unit = unit,
                            graphic = unit.GetDescriptionInfo().graphic
                        });
                    }
                    return flag;
                }
            }
            return false;
        }

        public PlayerWizard GetWizardOwner()
        {
            return this;
        }

        public int GetTotalCastingSkill()
        {
            TownLocation townLocation = this.wizardTower?.Get();
            int num = 0;
            if (townLocation != null)
            {
                if (townLocation.GetUnits().Count > 0)
                {
                    foreach (Reference<Unit> unit in townLocation.GetUnits())
                    {
                        if (unit.Get().GetSkills().Find(delegate(DBReference<Skill> o)
                        {
                            Skill skill2 = o.Get();
                            return skill2 != null && skill2.applicationScript?.triggerType == ESkillType.Caster;
                        }) != null && unit.Get().GetAttFinal(TAG.HERO_CLASS) > 0)
                        {
                            num += unit.Get().GetAttFinal(TAG.MANA_POINTS).ToInt();
                        }
                    }
                }
                else if (FSMSelectionManager.Get() != null && FSMSelectionManager.Get().GetSelectedGroup() is Group group && group.beforeMovingAway == townLocation)
                {
                    foreach (Reference<Unit> unit2 in group.GetUnits())
                    {
                        if (unit2.Get().GetSkills().Find(delegate(DBReference<Skill> o)
                        {
                            Skill skill = o.Get();
                            return skill != null && skill.applicationScript?.triggerType == ESkillType.Caster;
                        }) != null && unit2.Get().GetAttFinal(TAG.HERO_CLASS) > 0)
                        {
                            num += unit2.Get().GetAttFinal(TAG.MANA_POINTS).ToInt();
                        }
                    }
                }
            }
            return Mathf.Max(1, this.castingSkill + this.castingSkillBonus + num / 2);
        }

        public int GetNextLevelCastingSkill()
        {
            float settingAsFloat = DifficultySettingsData.GetSettingAsFloat("UI_CASTING_SKILL_MULTIPLIER");
            if (settingAsFloat > 0f)
            {
                return (int)((float)(this.castingSkill * 2) * settingAsFloat);
            }
            return this.castingSkill * 2;
        }

        public SummaryInfo GetCastingSummaryInfo()
        {
            if (this.summaryInfo == null)
            {
                this.summaryInfo = new List<SummaryInfo>();
                return null;
            }
            for (int num = this.summaryInfo.Count - 1; num >= 0; num--)
            {
                SummaryInfo summaryInfo = this.summaryInfo[num];
                if (summaryInfo.RequiresAction() && summaryInfo.summaryType == SummaryInfo.SummaryType.eCastingSpell)
                {
                    return summaryInfo;
                }
            }
            return null;
        }

        public SummaryInfo ClearNotifications()
        {
            if (this.summaryInfo == null)
            {
                this.summaryInfo = new List<SummaryInfo>();
                return null;
            }
            SummaryInfo summaryInfo = null;
            int count = this.summaryInfo.Count;
            for (int num = this.summaryInfo.Count - 1; num >= 0; num--)
            {
                SummaryInfo summaryInfo2 = this.summaryInfo[num];
                if (summaryInfo2.RequiresAction())
                {
                    if (summaryInfo == null)
                    {
                        summaryInfo = summaryInfo2;
                    }
                }
                else
                {
                    this.summaryInfo.RemoveAt(num);
                }
            }
            if (count != this.summaryInfo.Count)
            {
                HUD.Get()?.UpdateNotificationGrid();
            }
            return summaryInfo;
        }

        public void RemoveNotificationOfType(SummaryInfo.SummaryType s)
        {
            if (this.summaryInfo == null)
            {
                return;
            }
            while (true)
            {
                int num = this.summaryInfo.FindIndex((SummaryInfo o) => o.summaryType == s);
                if (num == -1)
                {
                    break;
                }
                this.summaryInfo.RemoveAt(num);
            }
            HUD.Get()?.UpdateNotificationGrid();
        }

        public void AddNotification(SummaryInfo s)
        {
            if (this.summaryInfo == null)
            {
                this.summaryInfo = new List<SummaryInfo>();
            }
            if (s.summaryType == SummaryInfo.SummaryType.eCastingSpell || s.summaryType == SummaryInfo.SummaryType.eResearchAvailiable || s.summaryType == SummaryInfo.SummaryType.eCraftingArtefact)
            {
                int num = this.summaryInfo.FindIndex((SummaryInfo o) => o.summaryType == s.summaryType);
                if (num != -1)
                {
                    this.summaryInfo.RemoveAt(num);
                }
            }
            this.summaryInfo.Add(s);
            HUD.Get()?.UpdateNotificationGrid();
        }

        public void RemoveSummaryInfo(SummaryInfo s)
        {
            if (this.summaryInfoRegistration != null && this.summaryInfoRegistration.Contains(s))
            {
                this.summaryInfoRegistration.Remove(s);
                HUD.Get()?.UpdateSummaryInfoGrid();
            }
            else if (this.summaryInfo != null && this.summaryInfo.Contains(s))
            {
                this.summaryInfo.Remove(s);
                HUD.Get()?.UpdateNotificationGrid();
            }
        }

        public void TrackStarvation(TownLocation l, bool starving)
        {
            if (starving)
            {
                if (this.summaryInfo == null)
                {
                    this.summaryInfo = new List<SummaryInfo>();
                }
                if (this.summaryInfo.Find((SummaryInfo o) => o.summaryType == SummaryInfo.SummaryType.eStarvation && o.name == l.GetName()) == null)
                {
                    SummaryInfo s = new SummaryInfo
                    {
                        summaryType = SummaryInfo.SummaryType.eStarvation,
                        location = l,
                        name = l.GetName()
                    };
                    this.AddNotification(s);
                }
            }
            else if (this.summaryInfo != null)
            {
                SummaryInfo summaryInfo = this.summaryInfo.Find((SummaryInfo o) => o.summaryType == SummaryInfo.SummaryType.eStarvation && o.name == l.GetName());
                if (summaryInfo != null)
                {
                    this.RemoveSummaryInfo(summaryInfo);
                }
            }
        }

        public void SanitizeNotifications()
        {
            if (this.summaryInfo == null)
            {
                this.summaryInfo = new List<SummaryInfo>();
            }
            for (int num = this.summaryInfo.Count - 1; num >= 0; num--)
            {
                if ((this.summaryInfo[num]?.unit?.ID ?? (-1)) > 0 && this.summaryInfo[num].unit.Get() == null)
                {
                    this.summaryInfo.RemoveAt(num);
                }
                else if ((this.summaryInfo[num]?.location?.ID ?? (-1)) > 0 && this.summaryInfo[num].location.Get() == null)
                {
                    this.summaryInfo.RemoveAt(num);
                }
            }
        }

        public List<SummaryInfo> GetNotifications()
        {
            this.SanitizeNotifications();
            return this.summaryInfo;
        }

        public List<SummaryInfo> GetSummaryInfo()
        {
            if (this.summaryInfoRegistration == null)
            {
                this.summaryInfoRegistration = new List<SummaryInfo>();
            }
            List<SummaryInfo> toRemove = null;
            foreach (SummaryInfo item in this.summaryInfoRegistration)
            {
                if (item.location == null || item.location.Get().ID < 1)
                {
                    if (toRemove == null)
                    {
                        toRemove = new List<SummaryInfo>();
                    }
                    toRemove.Add(item);
                    continue;
                }
                TownLocation townLocation = item.location.Get() as TownLocation;
                if (townLocation.craftingQueue != null && townLocation.craftingQueue.GetFirst() != null)
                {
                    CraftingItem first = townLocation.craftingQueue.GetFirst();
                    item.title = first.GetDI().GetLocalizedName();
                    item.graphic = first.GetDI().graphic;
                    int num = townLocation.CalculateProductionIncome();
                    int num2 = first.requirementValue - first.progress;
                    if (num < 1)
                    {
                        item.dataInfo = FInt.ZERO;
                    }
                    else
                    {
                        item.dataInfo = new FInt(Mathf.CeilToInt((float)num2 / (float)num));
                    }
                }
            }
            if (toRemove != null)
            {
                this.summaryInfoRegistration = this.summaryInfoRegistration.FindAll((SummaryInfo o) => !toRemove.Contains(o));
            }
            return this.summaryInfoRegistration;
        }

        public TownLocation GetSummoningLocation()
        {
            if (this.summoningCircle == null)
            {
                return null;
            }
            return this.summoningCircle.Get();
        }

        public void SetSummoningLocation(TownLocation l)
        {
            if (this.summoningCircle != null)
            {
                Reference<TownLocation> reference = this.summoningCircle;
                this.summoningCircle = null;
                VerticalMarkerManager.Get().UpdateInfoOnMarker(reference.Get());
            }
            this.summoningCircle = l;
            if (this.summoningCircle != null)
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.summoningCircle.Get());
            }
        }

        public TownLocation GetTowerLocation()
        {
            if (this.wizardTower == null)
            {
                return null;
            }
            return this.wizardTower.Get();
        }

        public float GetDistanceCost(global::WorldCode.Plane p, Vector3i location)
        {
            if (p == null)
            {
                return 3f;
            }
            return this.GetDistanceCost(p.arcanusType, location);
        }

        public float GetDistanceCost(bool arcanus, Vector3i location)
        {
            TownLocation towerLocation = this.GetTowerLocation();
            if (towerLocation != null)
            {
                if (towerLocation.plane.arcanusType != arcanus)
                {
                    if (this.ignorSpellcastingRange)
                    {
                        return 1f;
                    }
                    return 3f;
                }
                int distanceWrapping = World.GetArcanus().GetDistanceWrapping(towerLocation.GetPosition(), location);
                if (this.ignorSpellcastingRange)
                {
                    if (distanceWrapping == 0)
                    {
                        return 0.5f;
                    }
                    if (distanceWrapping > 0)
                    {
                        return 1f;
                    }
                }
                else
                {
                    if (distanceWrapping == 0)
                    {
                        return 0.5f;
                    }
                    if (distanceWrapping < 6)
                    {
                        return 1f;
                    }
                    if (distanceWrapping < 11)
                    {
                        return 1.5f;
                    }
                    if (distanceWrapping < 16)
                    {
                        return 2f;
                    }
                    if (distanceWrapping < 21)
                    {
                        return 2.5f;
                    }
                }
            }
            return 3f;
        }

        public int GetMana()
        {
            return this.GetMana(doInstantAlchemy: false);
        }

        public int GetMana(bool doInstantAlchemy)
        {
            if (doInstantAlchemy)
            {
                int num = this.GetTotalCastingSkill() * 3 - this.mana;
                if (num > 0 && this.alchemyRatio > 0 && this.money > 0)
                {
                    int num2 = Mathf.Min(this.money / this.alchemyRatio, num);
                    this.mana += num2;
                    this.money -= num2 * this.alchemyRatio;
                }
            }
            return this.mana;
        }

        public void SetMana(int m)
        {
            this.mana = m;
        }

        public void AttributesChanged()
        {
        }

        public override int GetID()
        {
            return this.ID;
        }

        public override void SetID(int id)
        {
            this.ID = id;
        }

        public int GetFame()
        {
            int num = 0;
            foreach (Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetOwnerID() != this.ID || (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType))
                {
                    continue;
                }
                Group group = registeredGroup;
                if (group.GetUnits() == null)
                {
                    continue;
                }
                foreach (Reference<Unit> unit in group.GetUnits())
                {
                    List<SkillScript> skillsByType = unit.Get().GetSkillsByType(ESkillType.FameChange);
                    if (skillsByType == null)
                    {
                        continue;
                    }
                    foreach (SkillScript item in skillsByType)
                    {
                        int num2 = (int)ScriptLibrary.Call(item.activatorMain, unit.Get(), null, null, item, null, null, null, null);
                        num += num2;
                    }
                }
            }
            return this.fame + this.temporaryFame + num;
        }

        public void AddFame(int value)
        {
            this.fame += value;
        }

        public void TakeFame(int value)
        {
            if (this.fame - value > 0)
            {
                this.fame -= value;
            }
            else
            {
                this.fame = 0;
            }
        }

        public void AddTemporaryFame(int value)
        {
            this.temporaryFame += value;
        }

        public void TakeTemporaryFame(int value)
        {
            if (this.temporaryFame - value > 0)
            {
                this.temporaryFame -= value;
            }
            else
            {
                this.temporaryFame = 0;
            }
        }

        public List<DeadHero> GetDeadHeroes()
        {
            return this.deadHeroes;
        }

        public void AddToDeadHeroesList(Unit u)
        {
            this.deadHeroes.Add(DeadHero.Create(u));
        }

        public void RemoveFromDeadHeroesList(DeadHero deadHero)
        {
            if (this.deadHeroes.Contains(deadHero))
            {
                this.deadHeroes.Remove(deadHero);
            }
            else
            {
                Debug.LogError("List of death heroes doesn't contain hero " + deadHero.name);
            }
        }

        public IEnumerator FameOffers()
        {
            if (TurnManager.GetTurnNumber() >= 3 && !(this.wizardTower == null))
            {
                yield return this.FameHeroOffer();
                yield return this.FameUnitOffer();
                yield return this.FameArtefactOffer();
            }
        }

        private IEnumerator FameHeroOffer()
        {
            int num = ((this.heroes != null) ? this.heroes.Count : 0);
            int val = (3 + this.fame / 25) / (num + 1);
            val = Math.Min(10, val);
            val = (this.famous ? (val * 2) : val);
            if (num >= 6 || global::UnityEngine.Random.Range(0, 100) >= val)
            {
                yield break;
            }
            List<Hero> list = DataBase.GetType<Hero>().FindAll((Hero o) => o.recruitmentCost <= this.money && o.recruitmentMinFame <= this.fame && (o.recruitmentMinBooks == null || this.GetAttributes().ContainsAll(o.recruitmentMinBooks)) && !Unit.HeroInUseByWizard(o, this.ID) && o.GetTag(TAG.EVENT_ONLY_UNIT) == 0 && !o.unresurrectable);
            if (list.Count <= 0)
            {
                yield break;
            }
            List<Hero> list2 = new List<Hero>();
            int num2 = 0;
            while (num2 < this.GetOfferHeroCount())
            {
                Hero item = list[global::UnityEngine.Random.Range(0, list.Count)];
                if (!list2.Contains(item))
                {
                    list2.Add(item);
                    list.Remove(item);
                    num2++;
                    if (list.Count == 0)
                    {
                        break;
                    }
                }
            }
            List<UnitOffer> offers = new List<UnitOffer>();
            foreach (Hero item2 in list2)
            {
                int num3 = item2.recruitmentCost;
                float @float = MHRandom.Get().GetFloat(0f, 1f);
                int max = ((@float <= 0.7f) ? 1 : ((!((double)@float <= 0.9)) ? 3 : 2));
                int num4 = Mathf.Clamp(this.money / item2.recruitmentCost, 1, max);
                int num5 = 0;
                if (num4 > 1)
                {
                    num5 = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_HERO).expReq[num4 - 1];
                    num3 = num3 * (3 + num4) / 4;
                }
                if (this.lowerMercenaryAndHeroCost > 0f)
                {
                    num3 = (int)((float)num3 * this.lowerMercenaryAndHeroCost);
                }
                Unit unit = Unit.CreateFrom(item2);
                unit.xp = num5;
                offers.Add(new UnitOffer
                {
                    unit = unit,
                    quantity = 1,
                    exp = num5,
                    cost = num3
                });
                this.ModifyUnitSkillsByTraits(unit);
            }
            if (this.ID == PlayerWizard.HumanID())
            {
                UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Standard);
                unitInfo.SetBuyInfo(offers, delegate(object o)
                {
                    if (o is UnitOffer unitOffer2)
                    {
                        this.money -= unitOffer2.cost;
                        this.wizardTower.Get().AddUnit(unitOffer2.unit);
                        unitOffer2.unit.UpdateMP();
                    }
                });
                while (UIManager.IsOpen(UIManager.Layer.Standard, unitInfo))
                {
                    yield return null;
                }
            }
            else
            {
                int index = global::UnityEngine.Random.Range(0, offers.Count);
                UnitOffer unitOffer = offers[index];
                AILocationTactic locationTactic = this.wizardTower.Get().locationTactic;
                if ((locationTactic != null && locationTactic.dangerRank > 0) || (float)this.money * 0.8f > (float)unitOffer.cost)
                {
                    this.money -= unitOffer.cost;
                    Unit unit2 = unitOffer.unit;
                    this.wizardTower.Get().AddUnit(unit2);
                    unit2.UpdateMP();
                }
            }
            foreach (UnitOffer item3 in offers)
            {
                Unit unit3 = item3.unit;
                if (unit3 != null && unit3.group == null)
                {
                    unit3.Destroy();
                }
            }
        }

        private IEnumerator FameUnitOffer()
        {
            int val = ((!this.famous) ? 1 : 2) + this.fame / 20;
            val = Math.Min(10, val);
            if (global::UnityEngine.Random.Range(0, 100) >= val)
            {
                yield break;
            }
            int unitCostMulti = 2;
            if (this.lowerMercenaryAndHeroCost > 0f)
            {
                unitCostMulti = (int)((float)unitCostMulti * this.lowerMercenaryAndHeroCost);
            }
            List<global::DBDef.Unit> list = DataBase.GetType<global::DBDef.Unit>().FindAll((global::DBDef.Unit o) => o.constructionCost * unitCostMulti <= this.money && o.gainsXP && o.GetTag(TAG.SETTLER_UNIT) == 0 && o.GetTag(TAG.DEBUG_TAG) == 0 && o.GetTag(TAG.SHIP) == 0 && o.GetTag(TAG.EVENT_ONLY_UNIT) == 0 && o.GetTag(TAG.UNRECRUITABLE_UNIT) == 0);
            if (!GameManager.Get().registeredLocations.Any((Location o) => !o.GetPlane().arcanusType && o.GetOwnerID() == this.ID))
            {
                list = list.FindAll((global::DBDef.Unit o) => o.GetTag(TAG.MYRRAN_UNIT) == 0);
            }
            if (list.Count <= 0)
            {
                yield break;
            }
            global::DBDef.Unit pick = list[global::UnityEngine.Random.Range(0, list.Count)];
            int num = global::UnityEngine.Random.Range(0, 100) + this.fame;
            int count = 1;
            if (num > 90 && this.money > pick.constructionCost * 3 * unitCostMulti)
            {
                count = 3;
            }
            else if (num > 60 && this.money > pick.constructionCost * 2 * unitCostMulti)
            {
                count = 2;
            }
            num = global::UnityEngine.Random.Range(0, 100) + this.fame;
            int num2 = 1;
            int exp = 0;
            if (pick.gainsXP)
            {
                if (num > 90 && this.money > pick.constructionCost * 3 * count * unitCostMulti)
                {
                    XpToLvl xpToLvl = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT);
                    exp = xpToLvl.expReq[3];
                    num2 = 3;
                }
                else if (num > 60 && (float)this.money > (float)pick.constructionCost * 2.5f * (float)count * (float)unitCostMulti)
                {
                    XpToLvl xpToLvl2 = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT);
                    exp = xpToLvl2.expReq[2];
                    num2 = 2;
                }
                else if (this.money > pick.constructionCost * 2 * count * unitCostMulti)
                {
                    XpToLvl xpToLvl3 = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT);
                    exp = xpToLvl3.expReq[1];
                    num2 = 1;
                }
            }
            int cost = pick.constructionCost * (num2 + 3) / 2 * count;
            if (this.lowerMercenaryAndHeroCost > 0f)
            {
                cost = (int)((float)cost * this.lowerMercenaryAndHeroCost);
            }
            if (this.ID == PlayerWizard.HumanID())
            {
                Unit example = Unit.CreateFrom(pick);
                example.xp = exp;
                this.ModifyUnitSkillsByTraits(example);
                UnitOffer uo = new UnitOffer
                {
                    unit = example,
                    exp = exp,
                    quantity = count,
                    cost = cost
                };
                UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Standard);
                unitInfo.SetBuyInfo(uo, delegate
                {
                    this.money -= cost;
                    for (int j = 0; j < count; j++)
                    {
                        Unit unit2 = Unit.CreateFrom(pick);
                        unit2.xp = exp;
                        this.wizardTower.Get().AddUnit(unit2);
                        unit2.UpdateMP();
                    }
                });
                while (UIManager.IsOpen(UIManager.Layer.Standard, unitInfo))
                {
                    yield return null;
                }
                example.Destroy();
                yield break;
            }
            AILocationTactic locationTactic = this.wizardTower.Get().locationTactic;
            if ((locationTactic != null && locationTactic.dangerRank > 0) || (float)this.money * 0.6f > (float)cost)
            {
                this.money -= cost;
                for (int i = 0; i < count; i++)
                {
                    Unit unit = Unit.CreateFrom(pick);
                    unit.xp = exp;
                    this.wizardTower.Get().AddUnit(unit);
                    unit.UpdateMP();
                }
            }
        }

        private IEnumerator FameArtefactOffer()
        {
            int val = (this.famous ? 4 : 2) + this.fame / 25;
            val = Math.Min(10, val);
            if (global::UnityEngine.Random.Range(0, 100) >= val)
            {
                yield break;
            }
            Artefact artefact = Artefact.CraftRandomByBudget(this.money);
            if (this.ID == PlayerWizard.HumanID() && artefact != null)
            {
                if (this.lowerArtefactCost > 0f)
                {
                    artefact.SetValue((int)((float)artefact.GetValue() * this.lowerArtefactCost));
                }
                bool popupOpen = true;
                PopupPurchaseArtefact.OpenPopup(null, artefact, delegate
                {
                    popupOpen = false;
                });
                while (popupOpen)
                {
                    yield return null;
                }
            }
            else if (artefact != null && global::UnityEngine.Random.Range(0, 100) <= 50)
            {
                this.money -= artefact.GetValue();
                GameManager.GetWizard(this.ID).artefacts.Add(artefact);
            }
        }

        public void AddVolcano(Vector3i pos, bool isArcanum)
        {
            this.raisedVolcanoes.Add(new Multitype<Vector3i, bool>(pos, isArcanum));
        }

        public List<Multitype<Vector3i, bool>> GetVolcanoList()
        {
            return this.raisedVolcanoes;
        }

        public void RemoveVolcanoFromList(Multitype<Vector3i, bool> volcano)
        {
            this.raisedVolcanoes.Remove(volcano);
        }

        public List<Reference<PlayerWizard>> GetDiscoveredWizards()
        {
            if (this.discoveredWizards == null)
            {
                this.discoveredWizards = new List<Reference<PlayerWizard>>();
            }
            return this.discoveredWizards;
        }

        public Personality GetPersonality()
        {
            if (this.personality == null)
            {
                this.PickPersonality();
            }
            return this.personality.Get();
        }

        private void PickPersonality()
        {
            List<Personality> type = DataBase.GetType<Personality>();
            Dictionary<Personality, int> dictionary = new Dictionary<Personality, int>();
            int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
            foreach (Personality item in type)
            {
                FInt zERO = FInt.ZERO;
                if (item.triggerFactorTags == null)
                {
                    dictionary[item] = 1;
                    continue;
                }
                CountedTag[] triggerFactorTags = item.triggerFactorTags;
                foreach (CountedTag countedTag in triggerFactorTags)
                {
                    if (this.GetAttributes().Contains(countedTag.tag, FInt.ONE))
                    {
                        zERO += countedTag.amount;
                    }
                }
                if (zERO > 0)
                {
                    dictionary[item] = zERO.ToInt();
                }
            }
            int aggresionTier = -100;
            switch (settingAsInt)
            {
            case 4:
                aggresionTier = 40;
                break;
            case 3:
                aggresionTier = 0;
                break;
            }
            List<Personality> list = new List<Personality>(dictionary.Keys);
            List<Personality> list2 = list.FindAll((Personality o) => o.hostility >= aggresionTier);
            FInt zERO2 = FInt.ZERO;
            if (list2.Count > 0)
            {
                foreach (Personality item2 in list)
                {
                    if (!list2.Contains(item2))
                    {
                        dictionary.Remove(item2);
                    }
                    else
                    {
                        zERO2 += dictionary[item2];
                    }
                }
            }
            else
            {
                dictionary.Clear();
            }
            if (dictionary.Count == 0)
            {
                foreach (Personality item3 in type)
                {
                    if (item3.hostility >= aggresionTier)
                    {
                        dictionary[item3] = 1;
                        zERO2 += FInt.ONE;
                    }
                }
            }
            if (dictionary.Count < 1)
            {
                Debug.LogWarning("No personality could be chosen!!");
                this.personality = type[0];
                return;
            }
            float num = global::UnityEngine.Random.Range(0f, 1f);
            foreach (KeyValuePair<Personality, int> item4 in dictionary)
            {
                float num2 = (float)item4.Value / zERO2.ToFloat();
                num -= num2;
                if (num <= 0f)
                {
                    this.personality = item4.Key;
                    break;
                }
            }
        }

        public void EnsureWizardIsKnown(int pw)
        {
            this.EnsureWizardIsKnown(GameManager.GetWizard(pw));
        }

        public void EnsureWizardIsKnown(PlayerWizard pw)
        {
            if (this.discoveredWizards == null)
            {
                this.discoveredWizards = new List<Reference<PlayerWizard>>();
            }
            if (!(this.discoveredWizards.Find((Reference<PlayerWizard> o) => o.Get().GetID() == pw.GetID()) == null))
            {
                return;
            }
            if (pw.ID == this.ID)
            {
                Debug.LogError("Diplomacy glitch: self discovering error!");
                return;
            }
            this.discoveredWizards.Add(pw);
            pw.EnsureWizardIsKnown(this);
            if (GameManager.GetHumanWizard() == this)
            {
                pw.GetDiplomacy().InitialGreetings(this);
            }
        }

        public void BanishWizard(int by)
        {
            this.showInfoForBanish = true;
            this.banishedTurn = TurnManager.GetTurnNumber();
            this.banishedBy = by;
            this.banishAnimPlayed = false;
            MagicAndResearch obj = this.GetMagicAndResearch();
            obj.curentlyCastSpell = DataBase.Get<Spell>(SPELL.SPELL_OF_RETURN);
            obj.craftItemSpell = null;
            if (by == PlayerWizard.HumanID() && this.GetWizardStatus() == WizardStatus.Banished)
            {
                UIManager.Get().PopupEvents(this, this);
            }
        }

        public void SetTowerLocation(TownLocation newLocation)
        {
            if (this.wizardTower != null)
            {
                this.wizardTower.Get().RemoveBuilding((Building)BUILDING.FORTRESS, skipFortress: false);
                Reference<TownLocation> reference = this.wizardTower;
                this.wizardTower = null;
                VerticalMarkerManager.Get().UpdateInfoOnMarker(reference.Get());
            }
            this.wizardTower = newLocation;
            if (newLocation != null)
            {
                this.wizardTower.Get().AddBuilding((Building)BUILDING.FORTRESS);
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.wizardTower.Get());
                if (this.summoningCircle == null)
                {
                    this.SetSummoningLocation(this.wizardTower.Get());
                }
            }
        }

        public List<Trait> GetTraits()
        {
            List<Trait> list = new List<Trait>();
            foreach (DBReference<Trait> trait in this.traits)
            {
                list.Add(trait);
            }
            return list;
        }

        public string GetName()
        {
            return this.name;
        }

        public bool HasTrait(Trait t)
        {
            return this.traits.Contains(t);
        }

        public int GetTraitsCount()
        {
            return this.traits.Count;
        }

        public void AddTrait(Trait t)
        {
            this.traits.Add(t);
            ScriptLibrary.Call(t.initialScript, this);
            if (t.startingSpells == null || t.startingSpells.Length == 0)
            {
                return;
            }
            string[] startingSpells = t.startingSpells;
            for (int i = 0; i < startingSpells.Length; i++)
            {
                DBClass dBClass = DataBase.Get(startingSpells[i], reportMissing: false);
                if (!this.GetSpells().Contains((Spell)dBClass))
                {
                    this.AddSpell((Spell)dBClass);
                }
            }
        }

        public bool IsWizard(Wizard w)
        {
            return w == this.wizard?.Get();
        }

        public Wizard GetBaseWizard()
        {
            return this.wizard?.Get();
        }

        public FInt GetDispelDificulty(EnchantmentInstance enchantment)
        {
            ERealm realm = enchantment.source.Get().realm;
            return FInt.ONE + this.globalDispelDificultyIncrease + this.realmDispelDificultyIncrease[realm];
        }

        public FInt GetCastCostPercent(Spell spell)
        {
            return this.castCostPercentDiscountSpells[spell] + this.castCostPercentDiscountRealms[spell.realm];
        }

        public int PlanFoodSurplus()
        {
            return this.CalculateFoodIncome(includeUpkeep: true) / 2;
        }

        public List<object> GetPossibleTradeWares()
        {
            return ScriptLibrary.Call("GetTotalWaresList", this) as List<object>;
        }

        public int AdvantageIfAcquired(object o, bool increasedWill)
        {
            int num = (int)ScriptLibrary.Call("AdvantageIfAcquired", this, o);
            if (num > -2 && num < 1 && increasedWill)
            {
                num++;
            }
            return num;
        }

        public float ValueScale(object o, int advantageIfAcquired)
        {
            return (float)ScriptLibrary.Call("ValueScale", o, advantageIfAcquired);
        }

        public IEnumerator AdvanceWorldWorks()
        {
            List<Group> groupsOfWizard = GameManager.GetGroupsOfWizard(this.ID);
            foreach (Group g in groupsOfWizard)
            {
                if (g.alive && g.CurentMP() > 0)
                {
                    if (g.engineerManager != null)
                    {
                        yield return g.engineerManager.TurnUpdate();
                    }
                    if (g.purificationManager != null)
                    {
                        yield return g.purificationManager.TurnUpdate();
                    }
                }
            }
        }

        public IEnumerator FocusOnGroupWithMPLeft()
        {
            if (!this.IsHuman)
            {
                yield break;
            }
            List<Group> groupsOfWizard = GameManager.GetGroupsOfWizard(this.ID);
            foreach (Group g in groupsOfWizard)
            {
                if (!g.alive || !(g.CurentMP() > 0))
                {
                    continue;
                }
                if (g.destination != Vector3i.invalid)
                {
                    RequestDataV2 requestDataV = RequestDataV2.CreateRequest(g.GetPlane(), g.GetPosition(), g.destination, g);
                    PathfinderV2.FindPath(requestDataV);
                    List<Vector3i> path = requestDataV.GetPath();
                    FSMSelectionManager.Get().Select(g, focus: true);
                    IGroup group = g.MoveViaPath(path, mergeCollidedAlliedGroups: true);
                    Group g2 = group as Group;
                    if (group is Location location)
                    {
                        g2 = location.GetLocalGroup();
                    }
                    if (g2 != null)
                    {
                        Formation mapFormation = g2.GetMapFormation();
                        if (mapFormation != null)
                        {
                            yield return mapFormation.WaitToEndOfMovement();
                            while (!GameManager.Get().IsFocusFree())
                            {
                                yield return null;
                            }
                        }
                    }
                    MHEventSystem.TriggerEvent<FSMMapGame>(g, null);
                    if (g2 != null && g2.CurentMP() > 0)
                    {
                        FSMSelectionManager.Get().Select(g2, focus: true);
                        yield break;
                    }
                }
                else if (g.CurentMP() > 0 && g.Action == Group.GroupActions.None && !g.isActivelyBuilding)
                {
                    FSMSelectionManager.Get().Select(g, focus: true);
                    yield break;
                }
            }
        }

        public bool AnyGroupsWithMPLeft()
        {
            if (this.IsHuman)
            {
                foreach (Group item in GameManager.GetGroupsOfWizard(this.ID))
                {
                    if (item.alive && item.CurentMP() > 0 && item.Action == Group.GroupActions.None && !item.isActivelyBuilding)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public global::UnityEngine.Color GetColor()
        {
            return WizardColors.GetColor(this.color);
        }

        public string GetEventDisplayName()
        {
            return this.name;
        }

        public void ReportForLog()
        {
        }

        public void SetGameScore(bool isVictory, bool masterOfMagicCasted)
        {
            if (!this.IsHuman)
            {
                return;
            }
            if (isVictory)
            {
                if (masterOfMagicCasted)
                {
                    AchievementManager.Progress(AchievementManager.Achievement.SpellmasterOfMagic);
                }
                else
                {
                    AchievementManager.Progress(AchievementManager.Achievement.WarmasterOfMagic);
                }
                if (DifficultySettingsData.GetCurentScoreMultiplier() >= 200)
                {
                    AchievementManager.Progress(AchievementManager.Achievement.WizardSupreme);
                }
                if (GameManager.GetHumanWizard().HasTrait((Trait)TRAIT.MYRRAN))
                {
                    AchievementManager.Progress(AchievementManager.Achievement.LookInTheMyrror);
                }
                AchievementManager.Progress(AchievementManager.Achievement.WhichWizard, this.wizard.Get().dbName);
            }
            HallOfFameBlock hallOfFameBlock = HallOfFameBlock.Load();
            List<Location> list = GameManager.GetLocationsOfWizard(this.ID).FindAll((Location o) => o is TownLocation);
            int count = GameManager.GetWizards().FindAll((PlayerWizard o) => o.banishedBy == this.ID).Count;
            int num = 0;
            foreach (Location item in list)
            {
                TownLocation townLocation = item as TownLocation;
                num += townLocation.GetPopUnits();
            }
            int num2 = 0;
            num2 += this.GetSpellManager().GetSpells().Count;
            num2 += num / 2;
            num2 += count * 50;
            num2 += this.GetFame() * 2;
            if (isVictory)
            {
                int num3 = 2000 - TurnManager.GetTurnNumber() * 2;
                num2 += num3;
            }
            if (masterOfMagicCasted)
            {
                num2 += 250;
            }
            float num4 = (float)DifficultySettingsData.GetCurentScoreMultiplier() * 0.01f;
            num2 = (int)((float)num2 * num4);
            hallOfFameBlock.Add(this.wizard.Get().icon, this.name, this.mainRace.Get().GetDILocalizedName(), num2);
        }

        public void FinishedIteratingEnchantments()
        {
        }

        public int GetWizardOwnerID()
        {
            return this.ID;
        }

        public int GetWizardSpellbooksCount()
        {
            int num = 0;
            NetDictionary<DBReference<Tag>, FInt> finalDictionary = this.attributes.GetFinalDictionary();
            Tag tag = (Tag)TAG.MAGIC_BOOK;
            foreach (KeyValuePair<DBReference<Tag>, FInt> item in finalDictionary)
            {
                if (((Tag)item.Key).parent == tag)
                {
                    num += item.Value.ToInt();
                }
            }
            return num;
        }

        public List<Tag> GetWizardSpellbooks()
        {
            List<Tag> list = new List<Tag>();
            NetDictionary<DBReference<Tag>, FInt> finalDictionary = this.attributes.GetFinalDictionary();
            Tag tag = (Tag)TAG.MAGIC_BOOK;
            foreach (KeyValuePair<DBReference<Tag>, FInt> item in finalDictionary)
            {
                Tag tag2 = item.Key;
                if (tag2.parent == tag)
                {
                    for (int i = 0; i < item.Value; i++)
                    {
                        list.Add(tag2);
                    }
                }
            }
            return list;
        }

        public void AddBook(Tag book, FInt value)
        {
            bool num = this.GetAttributes().GetBase(book) == FInt.ZERO;
            this.GetAttributes().AddToBase(book, value);
            if (this.magicAndResearch != null)
            {
                this.magicAndResearch.UpdateUnlockLimits(book, value);
                this.magicAndResearch.FillResearchOptions();
            }
            if (num)
            {
                Enchantment enchantment = null;
                if (book == (Tag)TAG.NATURE_MAGIC_BOOK)
                {
                    enchantment = (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_NATURE;
                }
                else if (book == (Tag)TAG.SORCERY_MAGIC_BOOK)
                {
                    enchantment = (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_SORCERY;
                }
                else if (book == (Tag)TAG.CHAOS_MAGIC_BOOK)
                {
                    enchantment = (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_CHAOS;
                }
                else if (book == (Tag)TAG.LIFE_MAGIC_BOOK)
                {
                    enchantment = (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_LIFE;
                }
                else if (book == (Tag)TAG.DEATH_MAGIC_BOOK)
                {
                    enchantment = (Enchantment)ENCH.FORTRESS_LIGHTNING_BOLT_DEATH;
                }
                if (enchantment != null && this.wizardTower != null)
                {
                    this.wizardTower.Get().AddEnchantment(enchantment, null).buildingEnchantment = true;
                }
            }
        }

        public WizardStatus GetWizardStatus(bool updateTracker = true)
        {
            if (!this.isAlive)
            {
                return WizardStatus.Killed;
            }
            if (this.banishedTurn > 0)
            {
                if (GameManager.GetWizardTownCount(this.ID) > 0)
                {
                    if (updateTracker)
                    {
                        this.allCitiesLostTurn = -1;
                    }
                    return WizardStatus.Banished;
                }
                if (this.allCitiesLostTurn == -1)
                {
                    if (updateTracker)
                    {
                        this.allCitiesLostTurn = TurnManager.GetTurnNumber();
                    }
                }
                else if (this.allCitiesLostTurn < TurnManager.GetTurnNumber() - 10)
                {
                    return WizardStatus.Killed;
                }
                foreach (Group item in GameManager.GetGroupsOfWizard(this.ID))
                {
                    if (item.GetUnits().Count > 0)
                    {
                        return WizardStatus.Banished;
                    }
                }
                return WizardStatus.Killed;
            }
            if (updateTracker)
            {
                this.allCitiesLostTurn = -1;
            }
            return WizardStatus.Alive;
        }

        public int GetMaxHeroCount()
        {
            return 6;
        }

        public int GetOfferHeroCount()
        {
            return 2 + this.heroHireBonus;
        }

        public void ModifyUnitSkillsByTraits(BaseUnit unit)
        {
            if (this.unitModificationSkills == null || this.unitModificationSkills.Count <= 0)
            {
                return;
            }
            foreach (KeyValuePair<string, string> unitModificationSkill in this.unitModificationSkills)
            {
                Skill skill = (Skill)DataBase.Get(unitModificationSkill.Key, reportMissing: false);
                if (skill != null && (bool)ScriptLibrary.Call(unitModificationSkill.Value, unit, skill, this))
                {
                    unit.AddSkill(skill);
                }
            }
        }

        public void AddTraitBasedUnitSkills(global::DBDef.Unit unit, List<Skill> list)
        {
            if (this.unitModificationSkills == null || this.unitModificationSkills.Count <= 0)
            {
                return;
            }
            foreach (KeyValuePair<string, string> unitModificationSkill in this.unitModificationSkills)
            {
                Skill skill = (Skill)DataBase.Get(unitModificationSkill.Key, reportMissing: false);
                if (skill != null && (bool)ScriptLibrary.Call(unitModificationSkill.Value, unit, skill, this))
                {
                    list.Add(skill);
                }
            }
        }

        public void AddTraitBaseEnchantmentsToNewBuildedTowns(TownLocation l)
        {
            if (l == null)
            {
                return;
            }
            NetDictionary<string, string> netDictionary = this.newBuildedTownsModificationEnchs;
            if (netDictionary == null || netDictionary.Count <= 0)
            {
                return;
            }
            foreach (KeyValuePair<string, string> item in netDictionary)
            {
                Enchantment enchant = (Enchantment)DataBase.Get(item.Key, reportMissing: false);
                if (enchant != null && (bool)ScriptLibrary.Call(item.Value, l, enchant, this) && l.GetEnchantments().Find((EnchantmentInstance o) => o.source == enchant) == null)
                {
                    l.AddEnchantment(enchant, this);
                }
            }
        }

        public void AllowToStoreManaIncomeCache(bool b)
        {
            if (b)
            {
                this.allowToStoreManaIncomeCache = true;
                return;
            }
            this.allowToStoreManaIncomeCache = false;
            this.manaIncomeCache = int.MinValue;
        }
    }
}
