// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.BattlePlayer
using DBDef;
using DBUtils;
using MOM;
using ProtoBuf;

[ProtoContract]
public class BattlePlayer : IEnchantable
{
    [ProtoMember(1)]
    public bool playerOwner;

    [ProtoMember(2)]
    public bool autoPlayByAI;

    [ProtoMember(3)]
    public int mana;

    [ProtoMember(4)]
    public int castingSkill;

    [ProtoMember(5)]
    public float towerDistanceCost;

    public PlayerWizard wizard;

    protected EnchantmentManager enchantmentManager;

    [ProtoMember(6)]
    public int isNatureProtected;

    [ProtoMember(7)]
    public int isSorceryProtected;

    [ProtoMember(8)]
    public int isChaosProtected;

    [ProtoMember(9)]
    public int isLifeProtected;

    [ProtoMember(10)]
    public int isDeathProtected;

    [ProtoMember(11)]
    public Reference<PlayerWizard> wizardReference;

    [ProtoMember(12)]
    public bool spellCasted;

    [ProtoMember(13)]
    public bool surrendered;

    [ProtoMember(14)]
    public int castingSkillDevelopment;

    [ProtoMember(15)]
    public bool orcomancer;

    [ProtoMember(16)]
    public bool castingBlock;

    [ProtoAfterDeserialization]
    public void AfterDeserialize()
    {
        this.wizard = this.wizardReference;
    }

    [ProtoBeforeSerialization]
    public void BeforeSerialize()
    {
        this.wizardReference = this.wizard;
    }

    public EnchantmentManager GetEnchantmentManager()
    {
        if (this.enchantmentManager == null)
        {
            if (this.wizard != null)
            {
                this.enchantmentManager = this.wizard.GetEnchantmentManager().CopyEnchantmentManager(this);
            }
            else
            {
                this.enchantmentManager = new EnchantmentManager(this);
            }
        }
        return this.enchantmentManager;
    }

    public PlayerWizard GetWizardOwner()
    {
        return this.wizard;
    }

    public int GetID()
    {
        return this.GetWizardOwner()?.ID ?? 0;
    }

    public string GetName()
    {
        if (this.wizard != null)
        {
            return this.wizard.name;
        }
        return global::DBUtils.Localization.Get("UI_NEUTRAL_UNITS", true);
    }

    public void UseResourcesFor(Spell spell)
    {
        int battleCastingCostByDistance = spell.GetBattleCastingCostByDistance(this.wizard, includeExtraManaCost: true);
        int battleCastingCost = spell.GetBattleCastingCost(this.wizard, includeExtraManaCost: true);
        this.mana -= battleCastingCostByDistance;
        this.castingSkill -= battleCastingCost;
        if (this.wizard.IsHuman && BattleHUD.Get() != null)
        {
            BattleHUD.Get().UpdateGeneralInfo();
        }
    }

    public void FinishedIteratingEnchantments()
    {
    }
}
