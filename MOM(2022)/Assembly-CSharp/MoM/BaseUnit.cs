// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.BaseUnit
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using MoM.Scripts;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoInclude(200, typeof(global::MOM.Unit))]
[ProtoInclude(201, typeof(BattleUnit))]
[ProtoContract]
public abstract class BaseUnit : Entity, IUnit, IPlanePosition, IAttributable, ISkillable, IEnchantable, ISpellCaster, IEventDisplayName
{
    public static Dictionary<Subrace, int> unitBaseStrength;

    public static Dictionary<Subrace, BattleUnit> unitBaseSamples;

    private static Dictionary<int, int> xpToLevelUnit;

    private static Dictionary<int, int> xpToLevelHero;

    [ProtoMember(1)]
    public Attributes attributes;

    [ProtoMember(2)]
    public int ID;

    [ProtoMember(3)]
    private int _figureCount;

    [ProtoMember(4)]
    public int _currentFigureHP;

    [ProtoMember(5)]
    private FInt mp;

    [ProtoMember(7)]
    public float blurProtection;

    [ProtoMember(8)]
    public int invulnerabilityProtection;

    [ProtoMember(9)]
    public float invisibilityProtection;

    [ProtoMember(11)]
    public int levelOverride;

    [ProtoMember(12)]
    private int _xp;

    [ProtoMember(13)]
    protected int _totalHP;

    [ProtoMember(20)]
    public DBReference<Subrace> dbSource;

    [ProtoMember(21)]
    public DBReference<Race> race;

    [ProtoMember(22)]
    protected SkillManager skillManager;

    [ProtoMember(23)]
    protected EnchantmentManager enchantmentManager;

    [ProtoMember(24)]
    protected SpellManager spellManager;

    [ProtoMember(30)]
    public bool isSpellLock;

    [ProtoMember(32)]
    public bool canMove = true;

    [ProtoMember(33)]
    public float targetDefMod;

    [ProtoMember(34)]
    public bool windMasteryNegative;

    [ProtoMember(35)]
    public bool chaosSurgeEffect;

    [ProtoMember(36)]
    public bool darknessEffect;

    [ProtoMember(37)]
    public bool rangeAttack;

    [ProtoMember(40)]
    public bool canNaturalHeal;

    [ProtoMember(41)]
    public bool canGainXP;

    [ProtoMember(42)]
    public string customName;

    [ProtoMember(43)]
    public ArtefactManager artefactManager;

    [ProtoMember(44)]
    public string creationStackRecord;

    [ProtoMember(45)]
    public bool doubleShot;

    public int xp
    {
        get
        {
            return this._xp;
        }
        set
        {
            if ((!this.canGainXP && this is global::MOM.Unit) || this.IsUnitXpMaxed())
            {
                return;
            }
            int num = this._xp;
            int level = this.GetLevel();
            this._xp = value;
            int level2 = this.GetLevel();
            if (level2 > level && this is global::MOM.Unit unit)
            {
                PlayerWizard wizardOwner = this.GetWizardOwner();
                if (wizardOwner != null && wizardOwner.IsHuman)
                {
                    this._xp = num;
                    SummaryInfo summaryInfo = new SummaryInfo();
                    summaryInfo.summaryType = SummaryInfo.SummaryType.eUnitLeveledUp;
                    summaryInfo.unit = unit;
                    summaryInfo.graphic = unit.GetDescriptionInfo().graphic;
                    summaryInfo.attributeDelta = new AttributeDelta(this);
                    this._xp = value;
                    summaryInfo.attributeDelta.CalcDeltas(this);
                    wizardOwner.AddNotification(summaryInfo);
                }
                unit.group?.Get()?.TriggerOnJoinScripts(unit);
            }
            if (level != level2)
            {
                this.GetAttributes().SetDirty();
            }
        }
    }

    public int currentFigureHP
    {
        get
        {
            return this._currentFigureHP;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            this._currentFigureHP = value;
            this.FigureHealthChanged();
            this.CalculateTotalHealth();
        }
    }

    public int figureCount
    {
        get
        {
            return this._figureCount;
        }
        set
        {
            if (value < 0)
            {
                Debug.LogError("Setting negative figure count is not valid!");
            }
            int prevCount = this._figureCount;
            this._figureCount = value;
            this.FigureCountChanged(prevCount, value);
        }
    }

    public FInt Mp
    {
        get
        {
            return this.mp;
        }
        set
        {
            FInt fInt = this.mp;
            this.mp = value;
            if ((this.GetWizardOwner() == GameManager.GetHumanWizard() && this.mp == 0 && fInt > 0) || (this.mp > 0 && fInt == 0))
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
            }
        }
    }

    public BaseUnit()
    {
        MHZombieMemoryDetector.Track(this);
    }

    [ProtoAfterDeserialization]
    public void AfterDeserialize()
    {
        if (this.attributes != null)
        {
            this.attributes.owner = this;
        }
        if (this.spellManager != null)
        {
            this.spellManager.owner = this;
        }
        if (this.enchantmentManager != null)
        {
            this.enchantmentManager.owner = this;
        }
        if (this.skillManager != null)
        {
            this.skillManager.owner = this;
        }
    }

    public Attributes GetAttributes()
    {
        return this.attributes;
    }

    public abstract int FigureCount();

    protected abstract void FigureCountChanged(int prevCount, int newCount);

    protected abstract void FigureHealthChanged();

    protected abstract void CalculateTotalHealth();

    public abstract string GetDBName();

    public string GetModel3dName()
    {
        if (!string.IsNullOrEmpty(this.dbSource?.Get().model3d))
        {
            return this.dbSource?.Get().model3d;
        }
        return this.GetDescriptionInfo().graphic;
    }

    public abstract DescriptionInfo GetDescriptionInfo();

    public abstract Vector3 GetPhysicalPosition();

    public abstract global::WorldCode.Plane GetPlane();

    public abstract Vector3i GetPosition();

    public abstract void AttributesChanged();

    public abstract SkillManager GetSkillManager();

    public abstract EnchantmentManager GetEnchantmentManager();

    public abstract PlayerWizard GetWizardOwner();

    public abstract SpellManager GetSpellManager();

    public abstract int GetTotalCastingSkill();

    public abstract int GetMana();

    public abstract void SetMana(int m);

    public abstract float GetTotalHpPercent();

    public static void EnsureUnitStr()
    {
        if (BaseUnit.unitBaseStrength != null)
        {
            return;
        }
        List<Multitype<BattleUnit, int>> list = PowerEstimate.GetList();
        BaseUnit.unitBaseSamples = new Dictionary<Subrace, BattleUnit>();
        BaseUnit.unitBaseStrength = new Dictionary<Subrace, int>();
        foreach (Multitype<BattleUnit, int> item in list)
        {
            BaseUnit.unitBaseSamples[item.t0.dbSource.Get()] = item.t0;
            BaseUnit.unitBaseStrength[item.t0.dbSource.Get()] = item.t1;
        }
    }

    public static BattleUnit UnitBaseRef(BaseUnit b)
    {
        if (b != null)
        {
            if (BaseUnit.unitBaseSamples != null && BaseUnit.unitBaseSamples.ContainsKey(b.dbSource.Get()))
            {
                return BaseUnit.unitBaseSamples[b.dbSource.Get()];
            }
            Debug.LogError("Missing unit sample for " + b.dbSource.dbName);
        }
        return null;
    }

    public int GetSimpleUnitStrength()
    {
        if (this.dbSource.Get() is global::DBDef.Unit)
        {
            float num = (this.dbSource.Get() as global::DBDef.Unit).figures;
            float num2 = (float)this.figureCount / num;
            return Mathf.RoundToInt((float)BaseUnit.GetUnitStrength(this.dbSource.Get()) * num2);
        }
        return BaseUnit.GetUnitStrength(this.dbSource.Get());
    }

    public FInt GetUnitSourceSampleTag(TAG t)
    {
        return BaseUnit.unitBaseSamples[this.dbSource.Get()].GetAttFinal(t);
    }

    public Attributes GetUnitSourceSampleAttributes()
    {
        return BaseUnit.unitBaseSamples[this.dbSource.Get()].attributes;
    }

    public static int GetUnitStrength(Subrace s)
    {
        BaseUnit.EnsureUnitStr();
        if (BaseUnit.unitBaseStrength.ContainsKey(s))
        {
            return BaseUnit.unitBaseStrength[s];
        }
        Debug.LogWarning("Missing strength value for unit in dictionary");
        return 1;
    }

    public override int GetID()
    {
        return this.ID;
    }

    public override void SetID(int id)
    {
        this.ID = id;
    }

    private int GetLevelFromXP(int value, XpToLvl xpToLvl = null)
    {
        Dictionary<int, int> dictionary = null;
        if (this.dbSource.Get() is Hero)
        {
            if (BaseUnit.xpToLevelHero == null)
            {
                BaseUnit.xpToLevelHero = new Dictionary<int, int>();
            }
            dictionary = BaseUnit.xpToLevelHero;
        }
        else
        {
            if (BaseUnit.xpToLevelUnit == null)
            {
                BaseUnit.xpToLevelUnit = new Dictionary<int, int>();
            }
            dictionary = BaseUnit.xpToLevelUnit;
        }
        if (!dictionary.ContainsKey(value))
        {
            int value2 = 0;
            for (int i = 0; i < xpToLvl.expReq.Length && this.xp >= xpToLvl.expReq[i]; i++)
            {
                value2 = i;
            }
            dictionary[value] = value2;
        }
        return dictionary[value];
    }

    public bool IsUnitXpMaxed()
    {
        XpToLvl xpToLvl = null;
        int num = 0;
        if (this.dbSource.Get() is Hero)
        {
            xpToLvl = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_HERO);
            num = xpToLvl.expReq.Length;
        }
        else
        {
            xpToLvl = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT);
            num = xpToLvl.expReq.Length;
        }
        return this.xp >= xpToLvl.expReq[num - 1];
    }

    public int GetLevel()
    {
        XpToLvl xpToLvl = null;
        int num = 0;
        if (this.dbSource.Get() is Hero)
        {
            xpToLvl = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_HERO);
            num = xpToLvl.expReq.Length;
        }
        else
        {
            xpToLvl = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT);
            num = xpToLvl.expReq.Length + 2;
        }
        int num2 = this.GetLevelFromXP(this.xp, xpToLvl) + 1;
        if (!this.canGainXP)
        {
            return num2;
        }
        PlayerWizard wizardOwner = this.GetWizardOwner();
        int num3 = 0;
        if (wizardOwner != null)
        {
            num3 += wizardOwner.unitLevelIncrease;
        }
        return Mathf.Min(Mathf.Max(num2, this.levelOverride) + num3, num);
    }

    public string GetName()
    {
        if (!string.IsNullOrEmpty(this.customName))
        {
            return this.customName;
        }
        return this.dbSource.Get().GetDescriptionInfo().GetLocalizedName();
    }

    public FInt GetManaUpkeep()
    {
        return this.GetAttFinal(TAG.UPKEEP_MANA);
    }

    public bool IsInvisibleUnit()
    {
        return this.GetAttributes().GetFinal(TAG.INVISIBILITY) > 0;
    }

    public string GetEventDisplayName()
    {
        return this.GetName();
    }

    public bool IsRangedUnit()
    {
        if (this.GetAttFinal(TAG.NORMAL_RANGE) > 0 || this.GetAttFinal(TAG.MAGIC_RANGE) > 0 || this.GetAttFinal(TAG.BOULDER_RANGE) > 0)
        {
            return true;
        }
        return false;
    }

    public virtual void FinishedIteratingEnchantments()
    {
    }

    public abstract int GetWizardOwnerID();

    public void UpdateCurentFigureHpWithoutMarkers(int newHP)
    {
        if (newHP < 0)
        {
            this._currentFigureHP = 0;
        }
        else
        {
            this._currentFigureHP = newHP;
        }
    }
}
