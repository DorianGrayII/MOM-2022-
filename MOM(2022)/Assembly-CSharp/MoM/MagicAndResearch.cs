// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.MagicAndResearch
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class MagicAndResearch
{
    public enum SpellStatus
    {
        NotResearched = 0,
        ReadyForResearch = 1,
        ResearchedOrAcquired = 2,
        MAX = 3
    }

    public PlayerWizard owner;

    public static Dictionary<ERealm, Tag> realmToTag;

    [ProtoMember(1)]
    public bool lockMana;

    [ProtoMember(2)]
    public bool lockResearch;

    [ProtoMember(3)]
    public bool lockSkill;

    [ProtoMember(4)]
    public float manaShare;

    [ProtoMember(5)]
    public float researchShare;

    [ProtoMember(6)]
    public float skillShare;

    [ProtoMember(7)]
    public List<DBReference<Spell>> curentResearchOptions = new List<DBReference<Spell>>();

    [ProtoMember(8)]
    public List<MagicUnlocks> unlocksLimits;

    [ProtoMember(11)]
    public DBReference<Spell> curentlyCasted;

    [ProtoMember(12)]
    public int castingProgress;

    [ProtoMember(13)]
    public DBReference<Spell> curentlyResearched;

    [ProtoMember(14)]
    public int researchProgress;

    [ProtoMember(15)]
    public CraftItemSpell craftItemSpell;

    [ProtoMember(16)]
    public ExtensionItemSpell extensionItemSpellWorld = new ExtensionItemSpell();

    [ProtoMember(17)]
    public ExtensionItemSpell extensionItemSpellBattle = new ExtensionItemSpell();

    [ProtoIgnore]
    public Spell curentlyCastSpell
    {
        get
        {
            if (this.curentlyCasted == null)
            {
                return null;
            }
            return this.curentlyCasted.Get();
        }
        set
        {
            if (value == null)
            {
                this.curentlyCasted = null;
            }
            else if (value != (Spell)SPELL.ENCHANT_ITEM && value != (Spell)SPELL.CREATE_ARTEFACT && this.owner != null)
            {
                int turnSkillLeft = this.owner.turnSkillLeft;
                int worldCastingCost = value.GetWorldCastingCost(this.owner);
                if (turnSkillLeft >= worldCastingCost && this.owner.mana + this.castingProgress >= worldCastingCost)
                {
                    int num = Mathf.Max(0, worldCastingCost - this.castingProgress);
                    this.ResetCasting();
                    this.curentlyCasted = value;
                    this.castingProgress = worldCastingCost;
                    this.owner.mana -= num;
                    this.owner.turnSkillLeft -= worldCastingCost;
                    if (this.owner.ID != PlayerWizard.HumanID())
                    {
                        return;
                    }
                    if ((bool)ScriptLibrary.Call("CounterMagicWorld", value, this.owner))
                    {
                        this.ResetCasting();
                        return;
                    }
                    if (value.targetType.enumType == ETargetType.WorldSummon)
                    {
                        if (!string.IsNullOrEmpty(value.targetingScript) && !(bool)ScriptLibrary.Call(value.targetingScript, new SpellCastData(this.owner, null), null, value))
                        {
                            this.ResetCasting(returnMana: true);
                        }
                        else if (this.owner.SummonToWorld(value))
                        {
                            this.ResetCasting();
                        }
                        else
                        {
                            this.ResetCasting(returnMana: true);
                        }
                        return;
                    }
                    SummaryInfo summaryInfo = new SummaryInfo();
                    summaryInfo.spell = value;
                    summaryInfo.title = global::DBUtils.Localization.Get("UI_CASTINGS_SPELL", true);
                    summaryInfo.graphic = value.GetDescriptionInfo().graphic;
                    summaryInfo.summaryType = SummaryInfo.SummaryType.eCastingSpell;
                    if (summaryInfo != null)
                    {
                        this.owner.AddNotification(summaryInfo);
                    }
                    this.ResetCasting();
                }
                else if (value.targetType.enumType == ETargetType.WorldSummon && !string.IsNullOrEmpty(value.targetingScript) && !(bool)ScriptLibrary.Call(value.targetingScript, new SpellCastData(this.owner, null), null, value))
                {
                    this.ResetCasting(returnMana: true);
                }
                else
                {
                    int num2 = Mathf.Min(turnSkillLeft, this.owner.mana);
                    this.ResetCasting(returnMana: true);
                    this.curentlyCasted = value;
                    this.castingProgress = num2;
                    this.owner.mana -= num2;
                    this.owner.turnSkillLeft -= num2;
                }
            }
            else
            {
                this.curentlyCasted = value;
            }
        }
    }

    public MagicAndResearch()
    {
    }

    public MagicAndResearch(PlayerWizard owner)
    {
        this.owner = owner;
        this.Initialize();
    }

    private void Initialize()
    {
        this.owner.GetAttributes();
        this.manaShare = 1f / 3f;
        this.researchShare = 1f / 3f;
        this.skillShare = 1f - this.manaShare - this.researchShare;
        this.ProduceInitialSpellUnlocks();
        this.FillResearchOptions();
    }

    private void AddSpellByRarity(ERarity rarity, ERealm realm, BooksAdvantage bAdv)
    {
        List<Spell> list = null;
        SpellsSection[] startingSpells;
        if (bAdv.startingSpells != null)
        {
            startingSpells = bAdv.startingSpells;
            foreach (SpellsSection spellsSection in startingSpells)
            {
                if (spellsSection.rarity == rarity && spellsSection.count >= 1)
                {
                    list = DataBase.GetType<Spell>().FindAll((Spell o) => o.realm == realm && o.rarity == rarity && !o.researchExclusion);
                    this.ExcludeOwnedSpells(list);
                    this.AddResearchedSpell(list, spellsSection.count);
                }
            }
        }
        if (bAdv.guaranteedSpells == null)
        {
            return;
        }
        startingSpells = bAdv.guaranteedSpells;
        foreach (SpellsSection spellsSection2 in startingSpells)
        {
            if (spellsSection2.rarity != rarity || spellsSection2.count < 1)
            {
                continue;
            }
            if (list == null)
            {
                list = DataBase.GetType<Spell>().FindAll((Spell o) => o.realm == realm && o.rarity == rarity && !o.researchExclusion);
                this.ExcludeOwnedSpells(list);
            }
            this.AddCurentResearchOptions(list, spellsSection2.count);
        }
    }

    private void ProduceInitialSpellUnlocks()
    {
        List<MagicUnlocks> unlockLimits = this.GetUnlockLimits();
        List<Spell> list = DataBase.GetType<Spell>().FindAll((Spell o) => o.researchCost == 0 && o.researchExclusion);
        this.AddResearchedSpell(list, -1);
        if (this.owner.GetSpellManager().GetSpells().Count != list.Count)
        {
            return;
        }
        foreach (MagicUnlocks item in unlockLimits)
        {
            if (item.realm != ERealm.Arcane && item.realm != ERealm.Tech)
            {
                this.AddSpellByRarity(ERarity.Common, item.realm, item.booksAdvantages.Get());
                this.AddSpellByRarity(ERarity.Uncommon, item.realm, item.booksAdvantages.Get());
                this.AddSpellByRarity(ERarity.Rare, item.realm, item.booksAdvantages.Get());
                this.AddSpellByRarity(ERarity.VeryRare, item.realm, item.booksAdvantages.Get());
            }
        }
    }

    private List<Spell> GetSpellsToPickOne()
    {
        List<MagicUnlocks> unlockLimits = this.GetUnlockLimits();
        List<Spell> list = new List<Spell>();
        List<Spell> type = DataBase.GetType<Spell>();
        for (int i = 1; i <= 4; i++)
        {
            ERarity r = (ERarity)i;
            foreach (MagicUnlocks v in unlockLimits)
            {
                List<Spell> list2 = type.FindAll((Spell o) => o.realm == v.realm && o.rarity == r && !o.researchExclusion && o.dbName != "SPELL-SPELL_OF_MASTERY");
                MHRandom random = new MHRandom(v.seed + i);
                list2.SortInPlace((Spell a, Spell b) => random.GetInt(-1, 2));
                this.ExcludeOwnedSpells(list2);
                int researchLimit = v.GetResearchLimit(r);
                int researchedCount = v.GetResearchedCount(r);
                int num = researchLimit - researchedCount;
                if (num > 0)
                {
                    if (list2.Count > num)
                    {
                        list2 = list2.GetRange(0, num);
                    }
                    list.AddRange(list2);
                }
            }
            if (list.Count > 0)
            {
                break;
            }
        }
        Spell mastery = (Spell)SPELL.SPELL_OF_MASTERY;
        if (list.Count == 0 && this.owner.GetSpells().Find((DBReference<Spell> o) => o.Get() == mastery) == null && this.owner.GetMagicAndResearch().GetCurentResearchOptions().Find((DBReference<Spell> o) => o.Get() == mastery) == null)
        {
            list.Add(mastery);
        }
        return list;
    }

    private bool LimitZero(ERarity r, MagicUnlocks u)
    {
        BooksAdvantage booksAdvantage = u.booksAdvantages.Get();
        if (booksAdvantage.researchLimit == null)
        {
            return true;
        }
        SpellsSection[] researchLimit = booksAdvantage.researchLimit;
        foreach (SpellsSection spellsSection in researchLimit)
        {
            if (spellsSection.rarity == r)
            {
                return spellsSection.count == 0;
            }
        }
        return true;
    }

    private bool CanAddToResearch(ERarity r, MagicUnlocks u)
    {
        int researchedCount = u.GetResearchedCount(r);
        BooksAdvantage booksAdvantage = u.booksAdvantages.Get();
        if (booksAdvantage.researchLimit == null)
        {
            return false;
        }
        SpellsSection[] researchLimit = booksAdvantage.researchLimit;
        foreach (SpellsSection spellsSection in researchLimit)
        {
            if (spellsSection.rarity == r)
            {
                return spellsSection.count > researchedCount;
            }
        }
        return false;
    }

    public void FillResearchOptions()
    {
        for (int i = this.GetCurentResearchOptions().Count; i < 8; i++)
        {
            List<Spell> spellsToPickOne = this.GetSpellsToPickOne();
            if (spellsToPickOne.Count < 1)
            {
                break;
            }
            int index = global::UnityEngine.Random.Range(0, spellsToPickOne.Count);
            this.AddCurentResearchOptions(spellsToPickOne[index]);
        }
    }

    public List<MagicUnlocks> GetUnlockLimits()
    {
        if (this.unlocksLimits == null)
        {
            this.unlocksLimits = new List<MagicUnlocks>();
            foreach (Tag item in DataBase.GetType<Tag>().FindAll((Tag o) => o.parent == (Tag)TAG.MAGIC_BOOK))
            {
                FInt final = this.owner.GetAttributes().GetFinal(item);
                if (final > 0)
                {
                    this.unlocksLimits.Add(new MagicUnlocks(item, final));
                }
            }
            this.unlocksLimits.Add(new MagicUnlocks((Tag)TAG.ARCANE_BOOK, FInt.ONE * 11));
        }
        return this.unlocksLimits;
    }

    public List<MagicUnlocks> UpdateUnlockLimits(Tag book, FInt value)
    {
        if (this.unlocksLimits != null)
        {
            bool flag = false;
            ERealm realmFromTag = MagicAndResearch.GetRealmFromTag(book);
            foreach (MagicUnlocks unlocksLimit in this.unlocksLimits)
            {
                if (unlocksLimit.realm == realmFromTag)
                {
                    flag = true;
                    unlocksLimit.UpdateTo(this.owner.GetAttFinal(book).ToInt());
                }
            }
            if (!flag)
            {
                this.unlocksLimits.Add(new MagicUnlocks(book, value));
            }
        }
        return this.unlocksLimits;
    }

    public static Dictionary<ERealm, Tag> RealmTagDictionary()
    {
        if (MagicAndResearch.realmToTag == null)
        {
            MagicAndResearch.realmToTag = new Dictionary<ERealm, Tag>();
            MagicAndResearch.realmToTag[ERealm.Chaos] = (Tag)TAG.CHAOS_MAGIC_BOOK;
            MagicAndResearch.realmToTag[ERealm.Sorcery] = (Tag)TAG.SORCERY_MAGIC_BOOK;
            MagicAndResearch.realmToTag[ERealm.Nature] = (Tag)TAG.NATURE_MAGIC_BOOK;
            MagicAndResearch.realmToTag[ERealm.Life] = (Tag)TAG.LIFE_MAGIC_BOOK;
            MagicAndResearch.realmToTag[ERealm.Death] = (Tag)TAG.DEATH_MAGIC_BOOK;
            MagicAndResearch.realmToTag[ERealm.Arcane] = (Tag)TAG.ARCANE_BOOK;
            MagicAndResearch.realmToTag[ERealm.Tech] = (Tag)TAG.TECH_BOOK;
        }
        return MagicAndResearch.realmToTag;
    }

    public static Tag GetTagForRealm(ERealm realm)
    {
        if (!MagicAndResearch.RealmTagDictionary().ContainsKey(realm))
        {
            Debug.LogError("unknown real for matching with tag on MagicAdnResearch " + realm);
            return null;
        }
        return MagicAndResearch.RealmTagDictionary()[realm];
    }

    public static ERealm GetRealmFromTag(Tag t)
    {
        foreach (KeyValuePair<ERealm, Tag> item in MagicAndResearch.RealmTagDictionary())
        {
            if (item.Value == t)
            {
                return item.Key;
            }
        }
        return ERealm.Arcane;
    }

    public static BooksAdvantage GetBooksAdvantage(ERealm realm, int bookCount)
    {
        if (bookCount > 0)
        {
            BooksAdvantage booksAdvantage = DataBase.Get<BooksAdvantage>("BOOKS_ADVANTAGE-BOOK" + bookCount, reportMissing: false);
            if (booksAdvantage == null)
            {
                List<BooksAdvantage> type = DataBase.GetType<BooksAdvantage>();
                booksAdvantage = type[type.Count - 1];
            }
            return booksAdvantage;
        }
        return null;
    }

    public int GetTotalPower(StatDetails details = null)
    {
        return this.owner.CalculatePowerIncome(details);
    }

    public Multitype<int, int, int> GetPowerSplit()
    {
        int totalPower = this.GetTotalPower();
        int num = Mathf.FloorToInt(this.researchShare * (float)totalPower);
        int num2 = Mathf.FloorToInt(this.manaShare * (float)totalPower);
        int num3 = Mathf.FloorToInt(this.skillShare * (float)totalPower);
        if (totalPower > 0 && totalPower != num + num2 + num3)
        {
            float num4 = 1f / (float)totalPower;
            for (totalPower -= num + num2 + num3; totalPower > 0; totalPower--)
            {
                float num5 = this.researchShare - (float)num * num4;
                float num6 = this.manaShare - (float)num2 * num4;
                float num7 = this.skillShare - (float)num3 * num4;
                if (num5 > num6 && num5 > num7)
                {
                    num++;
                }
                else if (num6 > num7)
                {
                    num2++;
                }
                else
                {
                    num3++;
                }
            }
        }
        return new Multitype<int, int, int>(num, num2, num3);
    }

    public int GetResearchIncome()
    {
        return this.GetPowerSplit().t0;
    }

    public int GetManaIncome(Spell s = null)
    {
        return this.GetPowerSplit().t1;
    }

    public int GetSkillIncome()
    {
        return this.GetPowerSplit().t2 * (100 + this.owner.skillIncomBonus) / 100;
    }

    public void SetManaLock(bool b)
    {
        if (b)
        {
            this.lockResearch = false;
            this.lockSkill = false;
        }
        this.lockMana = b;
    }

    public void SetResearchLock(bool b)
    {
        if (b)
        {
            this.lockMana = false;
            this.lockSkill = false;
        }
        this.lockResearch = b;
    }

    public void SetSkillLock(bool b)
    {
        if (b)
        {
            this.lockResearch = false;
            this.lockMana = false;
        }
        this.lockSkill = b;
    }

    public void SetManaShare(float f)
    {
        this.ShareBalance(f, ref this.manaShare, ref this.researchShare, this.lockResearch, ref this.skillShare, this.lockSkill);
    }

    public void SetResearchShare(float f)
    {
        this.ShareBalance(f, ref this.researchShare, ref this.manaShare, this.lockMana, ref this.skillShare, this.lockSkill);
    }

    public void SetSkillShare(float f)
    {
        this.ShareBalance(f, ref this.skillShare, ref this.researchShare, this.lockResearch, ref this.manaShare, this.lockMana);
    }

    private void ShareBalance(float newVal, ref float a, ref float b, bool lb, ref float c, bool lc)
    {
        a = newVal;
        a = Mathf.Clamp01(a);
        if (lb)
        {
            a = Mathf.Min(a, 1f - b);
            c = 1f - b - a;
        }
        else if (lc)
        {
            a = Mathf.Min(a, 1f - c);
            b = 1f - c - a;
        }
        else
        {
            float num = ((!(b + c < 0.001f)) ? (b / (b + c)) : 0.5f);
            b = (1f - a) * num;
            c = 1f - a - b;
        }
    }

    private List<DBReference<Spell>> GetCurentResearchOptions()
    {
        if (this.curentResearchOptions == null)
        {
            this.curentResearchOptions = new List<DBReference<Spell>>();
        }
        return this.curentResearchOptions;
    }

    private void AddCurentResearchOptions(Spell s)
    {
        this.GetCurentResearchOptions().Add(s);
        this.GetUnlockLimits().Find((MagicUnlocks o) => o.realm == s.realm).SetResearched(s.rarity);
    }

    private void AddCurentResearchOptions(List<Spell> ss, int count)
    {
        if (ss == null || ss.Count == 0)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            if (ss.Count == 0)
            {
                break;
            }
            int index = global::UnityEngine.Random.Range(0, ss.Count);
            Spell s = ss[index];
            this.AddCurentResearchOptions(s);
            ss.RemoveAt(index);
        }
    }

    public void RemoveFromCurrentResearchOptions(Spell s)
    {
        if (this.GetCurentResearchOptions().Contains(s))
        {
            this.GetCurentResearchOptions().Remove(s);
            this.FillResearchOptions();
        }
        if (this.curentlyResearched != null && this.curentlyResearched.Get() == s)
        {
            this.curentlyResearched = null;
            this.researchProgress = 0;
        }
    }

    private void AddResearchedSpell(List<Spell> ss, int count)
    {
        if (ss == null || ss.Count == 0)
        {
            return;
        }
        if (count == -1)
        {
            foreach (Spell v in ss)
            {
                this.GetUnlockLimits().Find((MagicUnlocks o) => o.realm == v.realm)?.SetResearched(v.rarity);
                this.owner.GetSpellManager().Add(v);
            }
            return;
        }
        for (int i = 0; i < count; i++)
        {
            if (ss.Count >= 1)
            {
                int index = global::UnityEngine.Random.Range(0, ss.Count);
                Spell s = ss[index];
                this.GetUnlockLimits().Find((MagicUnlocks o) => o.realm == s.realm)?.SetResearched(s.rarity);
                this.owner.GetSpellManager().Add(s);
                ss.RemoveAt(index);
                continue;
            }
            break;
        }
    }

    private void ExcludeOwnedSpells(List<Spell> ss)
    {
        foreach (DBReference<Spell> curentResearchOption in this.GetCurentResearchOptions())
        {
            Spell item = curentResearchOption.Get();
            if (ss.Contains(item))
            {
                ss.Remove(item);
            }
        }
        foreach (DBReference<Spell> spell in this.owner.GetSpellManager().GetSpells())
        {
            Spell item2 = spell.Get();
            if (ss.Contains(item2))
            {
                ss.Remove(item2);
            }
        }
    }

    public IEnumerator ProgressResearchSpell()
    {
        if (!(this.curentlyResearched != null))
        {
            yield break;
        }
        this.researchProgress += this.owner.CalculateResearchIncome();
        if (this.researchProgress < this.curentlyResearched.Get().GetResearchCost(this.owner))
        {
            yield break;
        }
        this.owner.GetSpellManager().Add(this.curentlyResearched.Get());
        if (this.owner.IsHuman)
        {
            TheRoom theRoom = TheRoom.Open(this.owner, TheRoom.RoomEvents.SpellResearched, this.curentlyResearched);
            if (theRoom != null)
            {
                yield return theRoom.WaitWhileOpen();
            }
            if (this.curentlyResearched.Get().rarity == ERarity.VeryRare)
            {
                AchievementManager.Progress(AchievementManager.Achievement.PowerOfKnowledge);
            }
            AchievementManager.Progress(AchievementManager.Achievement.JustGettingStarted);
        }
        this.RemoveFromCurrentResearchOptions(this.curentlyResearched);
        if (this.owner.IsHuman && GameManager.GetHumanWizard().GetMagicAndResearch().curentResearchOptions.Count > 0)
        {
            SummaryInfo s = new SummaryInfo
            {
                summaryType = SummaryInfo.SummaryType.eResearchAvailiable
            };
            this.owner.AddNotification(s);
        }
    }

    public void ProgressCast()
    {
        if (this.curentlyCastSpell != null)
        {
            int a = Mathf.Min(this.owner.GetMana(), this.owner.turnSkillLeft);
            if (GameManager.GetHumanWizard().addCheatResEachTurn)
            {
                a = this.owner.GetMana();
            }
            int worldCastingCost = this.curentlyCastSpell.GetWorldCastingCost(this.owner, includeExtraManaCost: true);
            int b = Math.Max(0, worldCastingCost - this.castingProgress);
            a = Mathf.Min(a, b);
            this.owner.mana -= a;
            this.owner.turnSkillLeft -= a;
            this.castingProgress += a;
        }
    }

    public void ChangeResearchPointsAndUpdate(int value, bool add)
    {
        if (this.curentlyResearched != null)
        {
            if (add)
            {
                this.researchProgress += value;
            }
            else if (this.researchProgress - value <= 0)
            {
                this.researchProgress = 0;
            }
            else
            {
                this.researchProgress -= value;
            }
            if (this.researchProgress >= this.curentlyResearched.Get().GetResearchCost(this.owner))
            {
                this.owner.GetSpellManager().Add(this.curentlyResearched.Get());
                this.RemoveFromCurrentResearchOptions(this.curentlyResearched);
            }
        }
    }

    public void GetResearchProgress(out float curentStatus, out float nextTurnStatus, out int turnsLeft)
    {
        if (this.curentlyResearched == null)
        {
            curentStatus = 0f;
            nextTurnStatus = 0f;
            turnsLeft = 0;
            return;
        }
        PlayerWizard playerWizard = this.owner;
        Spell s = this.curentlyResearched.Get();
        int num = playerWizard.CalculateResearchIncome();
        int researchCost = s.GetResearchCost(playerWizard);
        if (researchCost > 0)
        {
            curentStatus = Mathf.Clamp01((float)this.researchProgress / (float)researchCost);
            nextTurnStatus = Mathf.Clamp01(curentStatus + (float)num / (float)researchCost);
        }
        else
        {
            curentStatus = 0f;
            nextTurnStatus = 0f;
        }
        float f = 0f;
        if (num > 0)
        {
            f = (float)(s.GetResearchCost(playerWizard) - this.researchProgress) / (float)num;
        }
        turnsLeft = Mathf.CeilToInt(f);
    }

    public int GetCastingTimeLeft()
    {
        this.GetCastingProgress(out var _, out var _, out var turnsLeft);
        return turnsLeft;
    }

    public void GetCastingProgress(out float curentStatus, out float nextTurnStatus, out int turnsLeft)
    {
        if (this.curentlyCastSpell == null)
        {
            curentStatus = 0f;
            nextTurnStatus = 0f;
            turnsLeft = 0;
            return;
        }
        PlayerWizard playerWizard = this.owner;
        Spell spell = this.curentlyCastSpell;
        int a = playerWizard.CalculateManaIncome(includeUpkeep: true) + playerWizard.mana;
        int worldCastingCost = spell.GetWorldCastingCost(playerWizard, includeExtraManaCost: true);
        int totalCastingSkill = playerWizard.GetTotalCastingSkill();
        int num = Mathf.Min(a, totalCastingSkill);
        if (worldCastingCost > 0)
        {
            float value = (float)this.castingProgress / (float)worldCastingCost;
            float value2 = (float)(num + this.castingProgress) / (float)worldCastingCost;
            curentStatus = Mathf.Clamp01(value);
            nextTurnStatus = Mathf.Clamp01(value2);
            int castingTurnsInt = spell.GetCastingTurnsInt(playerWizard);
            if (castingTurnsInt < 0)
            {
                turnsLeft = 0;
            }
            else if (castingTurnsInt > 99)
            {
                turnsLeft = 99;
            }
            else
            {
                turnsLeft = castingTurnsInt;
            }
        }
        else
        {
            curentStatus = 0f;
            nextTurnStatus = 0f;
            turnsLeft = 0;
        }
    }

    public void ResetCasting(bool returnMana = false)
    {
        if (returnMana && this.castingProgress > 0)
        {
            this.owner.mana += this.castingProgress;
        }
        this.castingProgress = 0;
        this.curentlyCastSpell = null;
        this.craftItemSpell = null;
    }

    public int SpellInterestLevel(Spell s)
    {
        if (s.realm != ERealm.Arcane && s.realm != ERealm.Tech)
        {
            Tag tagForRealm = MagicAndResearch.GetTagForRealm(s.realm);
            if (this.owner.GetAttFinal(tagForRealm) == 0)
            {
                return -2;
            }
        }
        TargetType targetType = s.targetType;
        bool flag = !string.IsNullOrEmpty(s.battleScript);
        bool flag2 = !string.IsNullOrEmpty(s.worldScript);
        int num = 0;
        foreach (DBReference<Spell> spell in this.owner.GetSpellManager().GetSpells())
        {
            if (spell.Get() == s)
            {
                return -2;
            }
            if (spell.Get().targetType == targetType)
            {
                bool flag3 = !string.IsNullOrEmpty(spell.Get().battleScript);
                bool flag4 = !string.IsNullOrEmpty(spell.Get().worldScript);
                if ((!flag || flag3) && (!flag2 || flag4))
                {
                    num++;
                }
            }
        }
        if (!flag)
        {
            Personality personality = this.owner.GetPersonality();
            if (personality.hostility <= 20 && targetType.dbName.Contains("ENEMY"))
            {
                num += 1 + (20 - personality.hostility) / 5;
            }
        }
        if (num < 2)
        {
            return 1;
        }
        if (num < 5)
        {
            return 0;
        }
        return -1;
    }

    public void CleanExtrensionItemSpellWorld()
    {
        this.extensionItemSpellWorld.extraMana = 0;
        this.extensionItemSpellWorld.extraPower = 0;
        this.extensionItemSpellWorld.extraSkill = 0;
    }
}
