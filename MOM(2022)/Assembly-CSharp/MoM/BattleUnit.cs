// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.BattleUnit
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class BattleUnit : BaseUnit
{
    [ProtoIgnore]
    public Formation battleFormation;

    [ProtoIgnore]
    private double[] nnRepresentation;

    [ProtoIgnore]
    public bool simulated;

    [ProtoMember(4)]
    public int maxCount;

    [ProtoMember(6)]
    public Vector3i battlePosition;

    [ProtoMember(8)]
    public FInt initiativeModifier;

    [ProtoMember(11)]
    private BattleFigure currentFigure;

    [ProtoMember(12)]
    private BattleFigure baseFigure;

    [ProtoMember(14)]
    public bool attackingSide;

    [ProtoMember(15)]
    public int ownerID;

    [ProtoMember(16)]
    public int normalDamages;

    [ProtoMember(17)]
    public int undeadDamages;

    [ProtoMember(18)]
    public int irreversibleDamages;

    [ProtoMember(24)]
    public int battleIndex;

    [ProtoMember(25)]
    public int strategicValue;

    [ProtoMember(27)]
    public bool canAttack = true;

    [ProtoMember(28)]
    public bool canContrAttack = true;

    [ProtoMember(29)]
    public bool canCastSpells = true;

    [ProtoMember(30)]
    public bool canDefend = true;

    [ProtoMember(31)]
    public bool haste;

    [ProtoMember(32)]
    public bool teleporting;

    [ProtoMember(33)]
    public int mana;

    [ProtoMember(34)]
    public int maxMana;

    [ProtoMember(40)]
    public bool landMovement;

    [ProtoMember(41)]
    public bool waterMovement;

    [ProtoMember(42)]
    public bool mountainMovement;

    [ProtoMember(43)]
    public bool forestMovement;

    [ProtoMember(44)]
    public bool nonCorporealMovement;

    [ProtoMember(45)]
    private bool _currentlyVisible = true;

    [ProtoMember(46)]
    public bool spellCasted;

    [ProtoMember(47)]
    public bool summon;

    [ProtoMember(48)]
    public bool isHero;

    [ProtoMember(49)]
    public bool earthWalkerMovement;

    [ProtoMember(50)]
    private int lostHP;

    [ProtoIgnore]
    public bool isAnimatedInSimulation;

    [ProtoIgnore]
    public bool isHopingToJoin;

    public bool currentlyVisible
    {
        get
        {
            return this._currentlyVisible;
        }
        set
        {
            if (this._currentlyVisible == value)
            {
                return;
            }
            this._currentlyVisible = value;
            if (this.ownerID != PlayerWizard.HumanID())
            {
                if (value)
                {
                    VerticalMarkerManager.Get().Addmarker(this);
                }
                else
                {
                    VerticalMarkerManager.Get().DestroyMarker(this);
                }
                this.GetOrCreateFormation();
            }
            this.GetPlane()?.GetSearcherData()?.UpdateUnitPosition(this);
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
        }
    }

    public static BattleUnit Create(global::MOM.Unit source, bool abstractMode = false, int ownerOverrideForSummon = -1, bool attackingSide = false)
    {
        BattleUnit battleUnit = new BattleUnit();
        Battle battle = Battle.GetBattle();
        if (battle != null && battle.plane != null)
        {
            battle.plane.ClearSearcherData();
        }
        battleUnit.simulated = battle == null;
        battleUnit.currentFigure = new BattleFigure();
        battleUnit.maxCount = source.MaxCount();
        battleUnit.attackingSide = attackingSide;
        battleUnit.ID = source.ID;
        battleUnit.dbSource = source.dbSource;
        battleUnit.attributes = source.GetAttributes().Clone(battleUnit);
        battleUnit.skillManager = source.CopySkillManager(battleUnit);
        battleUnit.enchantmentManager = source.CopyEnchantmentManager(battleUnit);
        battleUnit.spellManager = source.CopySpellManager(battleUnit);
        battleUnit.race = source.race;
        battleUnit.levelOverride = source.levelOverride;
        battleUnit.canNaturalHeal = source.canNaturalHeal;
        battleUnit.canGainXP = source.canGainXP;
        battleUnit.xp = source.xp;
        battleUnit.isHero = source.dbSource.Get() is Hero;
        battleUnit.doubleShot = source.doubleShot;
        battleUnit.maxMana = source.GetAttFinal(TAG.MANA_POINTS).ToInt();
        battleUnit.SetMana(battleUnit.maxMana);
        if (source.customName != null)
        {
            battleUnit.customName = source.customName;
        }
        if (ownerOverrideForSummon == -1 && abstractMode)
        {
            battleUnit.ownerID = source.ID;
        }
        else if (ownerOverrideForSummon == -1)
        {
            battleUnit.ownerID = source.group.Get().GetOwnerID();
        }
        else
        {
            battleUnit.ownerID = ownerOverrideForSummon;
            battleUnit.summon = true;
        }
        battleUnit.attributes.SetDirty();
        battleUnit.EnsureFinal();
        BattleFigure battleFigure = new BattleFigure(source, battleUnit.attributes);
        battleUnit.baseFigure = battleFigure;
        battleUnit.FillMovementCost();
        battleUnit.ResetUnit();
        battleUnit.figureCount = source.figureCount;
        if (!battleUnit.summon)
        {
            battleUnit.currentFigureHP = source.currentFigureHP;
        }
        else
        {
            battleUnit.currentFigureHP = battleUnit.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
        }
        return battleUnit;
    }

    public void ResetUnit()
    {
        if (this.baseFigure == null)
        {
            Debug.LogError("missing base figure!");
        }
        BattleFigure.Copy(this.baseFigure, this.currentFigure);
        base.currentFigureHP = this.GetCurentFigure().maxHitPoints;
        base.figureCount = this.maxCount;
    }

    public int[] ProduceDamage(int[] damageBuffer, MHRandom random, BattleUnit target)
    {
        if (damageBuffer == null || this.maxCount >= damageBuffer.Length)
        {
            damageBuffer = new int[this.maxCount];
        }
        for (int i = 0; i < damageBuffer.Length; i++)
        {
            if (i >= base.figureCount)
            {
                damageBuffer[i] = 0;
                continue;
            }
            if (!target.canDefend)
            {
                damageBuffer[i] = this.currentFigure.attack;
                continue;
            }
            float num = Mathf.Clamp(1f - target.blurProtection, 0f, 1f);
            float num2 = target.invisibilityProtection;
            if (base.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) > 0)
            {
                num2 = 0f;
            }
            float chance = Mathf.Clamp01(this.currentFigure.attackChance * num - num2);
            damageBuffer[i] = random.GetSuccesses(chance, this.currentFigure.attack);
        }
        return damageBuffer;
    }

    public int[] ProduceDamage(int[] damageBuffer, MHRandom random, float attackChance, int attack, BattleUnit target)
    {
        if (damageBuffer == null || this.maxCount >= damageBuffer.Length)
        {
            damageBuffer = new int[this.maxCount];
        }
        for (int i = 0; i < damageBuffer.Length; i++)
        {
            if (i >= base.figureCount)
            {
                damageBuffer[i] = 0;
                continue;
            }
            if (!target.canDefend)
            {
                damageBuffer[i] = this.currentFigure.attack;
                continue;
            }
            float num = Mathf.Clamp(1f - target.blurProtection, 0f, 1f);
            damageBuffer[i] = random.GetSuccesses(attackChance * num, attack);
        }
        return damageBuffer;
    }

    public int[] ProduceFeardDamage(int[] damageBuffer, MHRandom random, BattleUnit target, int resMod = 0, StringBuilder sb = null, string header = null)
    {
        int num = 0;
        if (damageBuffer == null || this.maxCount >= damageBuffer.Length)
        {
            damageBuffer = new int[this.maxCount];
        }
        for (int i = 0; i < base.figureCount; i++)
        {
            if (random.GetSuccesses((float)(this.currentFigure.resist + resMod) * 0.1f, 1) == 0)
            {
                num++;
            }
        }
        sb?.AppendLine(header + " number of figures scared by Fear " + num);
        int num2 = base.figureCount - num;
        for (int j = 0; j < this.maxCount; j++)
        {
            if (j >= num2)
            {
                damageBuffer[j] = 0;
                continue;
            }
            float num3 = Mathf.Clamp(1f - target.blurProtection, 0f, 1f);
            float num4 = target.invisibilityProtection;
            if (base.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) > 0)
            {
                num4 = 0f;
            }
            float chance = Mathf.Clamp01(this.currentFigure.attackChance * num3 - num4);
            damageBuffer[j] = random.GetSuccesses(chance, this.currentFigure.attack);
        }
        return damageBuffer;
    }

    public int[] ProduceRangedDamage(int[] damageBuffer, MHRandom random, BattleUnit target)
    {
        if (damageBuffer == null || this.maxCount >= damageBuffer.Length)
        {
            damageBuffer = new int[this.maxCount];
        }
        int distance = HexCoordinates.HexDistance(this.battlePosition, target.battlePosition);
        float rangedPenalty = this.RangedPenalty(distance, this.GetRangedTag()).ToFloat();
        for (int i = 0; i < damageBuffer.Length; i++)
        {
            if (i >= base.figureCount)
            {
                damageBuffer[i] = 0;
                continue;
            }
            if (!target.canDefend)
            {
                damageBuffer[i] = this.currentFigure.attack;
                continue;
            }
            float chance = this.HitChance(this, target, rangedPenalty);
            damageBuffer[i] = random.GetSuccesses(chance, this.currentFigure.rangedAttack);
        }
        return damageBuffer;
    }

    public int[] ProduceRangedAreaDamage(int[] damageBuffer, int targetFigureCount, MHRandom random, BattleUnit target)
    {
        if (damageBuffer == null || this.maxCount >= damageBuffer.Length)
        {
            damageBuffer = new int[this.maxCount];
        }
        int num = damageBuffer.Length;
        List<int> list = new List<int>();
        int distance = HexCoordinates.HexDistance(this.battlePosition, target.battlePosition);
        float num2 = this.RangedPenalty(distance, this.GetRangedTag()).ToFloat();
        for (int i = 0; i < num * targetFigureCount; i++)
        {
            if (i < base.figureCount + num * (i / num))
            {
                if (!target.canDefend)
                {
                    list.Add(this.currentFigure.rangedAttack);
                    continue;
                }
                float num3 = Mathf.Clamp01(1f - target.blurProtection);
                float num4 = target.invisibilityProtection;
                if (base.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) > 0)
                {
                    num4 = 0f;
                }
                float chance = Mathf.Clamp01(this.currentFigure.rangedAttackChance * num3 - num2 - num4);
                list.Add(random.GetSuccesses(chance, this.currentFigure.rangedAttack));
            }
            else
            {
                list.Add(0);
            }
        }
        return list.ToArray();
    }

    private void UpdateDefenceByAttackType(ref int defTryCount, BattleAttack battleAttack, DBClass data)
    {
        if (battleAttack != null)
        {
            if (!battleAttack.isPiercing)
            {
                SkillScript skillScript = battleAttack.skillScript;
                if (skillScript == null || skillScript.battleAttackEffect != ESkillBattleAttackEffect.Piercing)
                {
                    goto IL_0028;
                }
            }
            defTryCount /= 2;
            goto IL_0028;
        }
        goto IL_0059;
        IL_0059:
        if (data == null)
        {
            return;
        }
        if (data is Spell)
        {
            switch ((data as Spell).battleAttackEffect)
            {
            case ESkillBattleAttackEffect.Piercing:
                defTryCount /= 2;
                break;
            case ESkillBattleAttackEffect.Illusion:
                if (!base.GetAttributes().Contains(TAG.ILLUSIONS_IMMUNITY))
                {
                    defTryCount = 0;
                }
                break;
            }
        }
        if (!(data is EnchantmentScript))
        {
            return;
        }
        switch ((data as EnchantmentScript).battleAttackEffect)
        {
        case ESkillBattleAttackEffect.Piercing:
            defTryCount /= 2;
            break;
        case ESkillBattleAttackEffect.Illusion:
            if (!base.GetAttributes().Contains(TAG.ILLUSIONS_IMMUNITY))
            {
                defTryCount = 0;
            }
            break;
        }
        return;
        IL_0028:
        if (!battleAttack.isIllusion)
        {
            SkillScript skillScript2 = battleAttack.skillScript;
            if (skillScript2 == null || skillScript2.battleAttackEffect != ESkillBattleAttackEffect.Illusion)
            {
                goto IL_0059;
            }
        }
        if (!base.GetAttributes().Contains(TAG.ILLUSIONS_IMMUNITY))
        {
            defTryCount = 0;
        }
        goto IL_0059;
    }

    public void ApplyDamage(int[] damage, MHRandom random, BattleAttack battleAttack, int defenceModifier, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        if (base.figureCount < 1 || damage == null || damage.Length < 1)
        {
            return;
        }
        int num = 0;
        int num2 = damage[num];
        float chance = this.currentFigure.defenceChance;
        if (battleAttack != null)
        {
            defenceModifier += battleAttack.GetWallDefenceModifier();
        }
        while (base.figureCount > 0)
        {
            while (base.currentFigureHP > 0)
            {
                if (num2 <= 0)
                {
                    num++;
                    if (num >= damage.Length)
                    {
                        return;
                    }
                    num2 = damage[num];
                    if (num2 <= 0)
                    {
                        continue;
                    }
                }
                int num3 = this.currentFigure.defence;
                if (!this.canDefend)
                {
                    num3 = 0;
                }
                num3 = ((!replaceDefenceWithModifier) ? (num3 + defenceModifier) : defenceModifier);
                if (battleAttack != null)
                {
                    BattleUnit source = battleAttack.source;
                    if (source.targetDefMod != 0f)
                    {
                        chance = this.currentFigure.defenceChance;
                        chance = Mathf.Clamp01(chance - source.targetDefMod);
                    }
                }
                this.UpdateDefenceByAttackType(ref num3, battleAttack, data);
                int num4 = random.GetSuccesses(chance, num3);
                if (base.invulnerabilityProtection > 0)
                {
                    num4 += base.invulnerabilityProtection;
                }
                sb?.AppendLine(header + " defense " + num4 + " for damage " + num2 + " results in " + (num2 - num4));
                num2 -= num4;
                if (num2 <= 0)
                {
                    continue;
                }
                if (battleAttack != null)
                {
                    this.UpdateDamagePool(battleAttack.skillScript.damagePool, num2);
                }
                if (data != null)
                {
                    if (data is Spell)
                    {
                        this.UpdateDamagePool((data as Spell).damagePool, num2);
                    }
                    if (data is EnchantmentScript)
                    {
                        this.UpdateDamagePool((data as EnchantmentScript).damagePool, num2);
                    }
                }
                int num5 = base.currentFigureHP;
                base.currentFigureHP -= num2;
                num2 -= num5;
                sb?.AppendLine(header + " figure " + base.figureCount + " lost hp, " + base.currentFigureHP + " hp left");
            }
            base.figureCount--;
            base.currentFigureHP = this.currentFigure.maxHitPoints;
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
        }
    }

    public void ApplyAreaDamage(int[] damage, MHRandom random, BattleAttack battleAttack, int defenceModifier, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        if (base.figureCount < 1 || damage == null || damage.Length < 1)
        {
            return;
        }
        int num = 0;
        int num2 = damage[num];
        float chance = this.currentFigure.defenceChance;
        if (battleAttack != null)
        {
            defenceModifier += battleAttack.GetWallDefenceModifier();
        }
        while (base.figureCount > 0)
        {
            while (base.currentFigureHP > 0)
            {
                if (num2 <= 0)
                {
                    num++;
                    if (num >= damage.Length)
                    {
                        return;
                    }
                    num2 = damage[num];
                    if (num2 <= 0)
                    {
                        continue;
                    }
                }
                int num3 = this.currentFigure.defence;
                if (!this.canDefend)
                {
                    num3 = 0;
                }
                num3 = ((!replaceDefenceWithModifier) ? (num3 + defenceModifier) : defenceModifier);
                if (battleAttack != null)
                {
                    BattleUnit source = battleAttack.source;
                    if (source.targetDefMod != 0f)
                    {
                        chance = this.currentFigure.defenceChance;
                        chance = Mathf.Clamp01(chance - source.targetDefMod);
                    }
                }
                this.UpdateDefenceByAttackType(ref num3, battleAttack, data);
                int num4 = random.GetSuccesses(chance, num3);
                if (base.invulnerabilityProtection > 0)
                {
                    num4 += base.invulnerabilityProtection;
                }
                sb?.AppendLine(header + " defense " + num4 + " for damage " + num2 + " results in " + (num2 - num4));
                num2 -= num4;
                if (num2 <= 0)
                {
                    continue;
                }
                if (battleAttack != null)
                {
                    this.UpdateDamagePool(battleAttack.skillScript.damagePool, num2);
                }
                if (data != null)
                {
                    if (data is Spell)
                    {
                        this.UpdateDamagePool((data as Spell).damagePool, num2);
                    }
                    if (data is EnchantmentScript)
                    {
                        this.UpdateDamagePool((data as EnchantmentScript).damagePool, num2);
                    }
                }
                base.currentFigureHP -= num2;
                num2 = 0;
                sb?.AppendLine(header + " figure " + base.figureCount + " lost hp, " + base.currentFigureHP + " hp left");
            }
            base.figureCount--;
            base.currentFigureHP = this.currentFigure.maxHitPoints;
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
        }
    }

    public void ApplyImmolationDamage(int[] damage, MHRandom random, BattleAttack battleAttack, int defenceModifier, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        if (base.figureCount < 1 || damage == null || damage.Length < 1)
        {
            return;
        }
        int num = 0;
        int num2 = damage[num];
        float chance = this.currentFigure.defenceChance;
        if (battleAttack != null)
        {
            defenceModifier += battleAttack.GetWallDefenceModifier();
        }
        while (base.figureCount > 0)
        {
            while (base.currentFigureHP > 0)
            {
                if (num2 <= 0)
                {
                    num++;
                    if (num >= damage.Length)
                    {
                        return;
                    }
                    num2 = damage[num];
                    if (num2 <= 0)
                    {
                        continue;
                    }
                }
                int num3 = this.currentFigure.defence;
                if (!this.canDefend)
                {
                    num3 = 0;
                }
                num3 = ((!replaceDefenceWithModifier) ? (num3 + defenceModifier) : defenceModifier);
                if (battleAttack != null)
                {
                    BattleUnit source = battleAttack.source;
                    if (source.targetDefMod != 0f)
                    {
                        chance = this.currentFigure.defenceChance;
                        chance = Mathf.Clamp01(chance - source.targetDefMod);
                    }
                }
                int num4 = random.GetSuccesses(chance, num3);
                if (base.invulnerabilityProtection > 0)
                {
                    num4 += base.invulnerabilityProtection;
                }
                sb?.AppendLine(header + " defense " + num4 + " for damage " + num2 + " results in " + (num2 - num4));
                num2 -= num4;
                if (num2 <= 0)
                {
                    continue;
                }
                if (battleAttack != null)
                {
                    this.UpdateDamagePool(battleAttack.skillScript.damagePool, num2);
                }
                if (data != null)
                {
                    if (data is Spell)
                    {
                        this.UpdateDamagePool((data as Spell).damagePool, num2);
                    }
                    if (data is EnchantmentScript)
                    {
                        this.UpdateDamagePool((data as EnchantmentScript).damagePool, num2);
                    }
                }
                base.currentFigureHP -= num2;
                num2 = 0;
                sb?.AppendLine(header + " figure " + base.figureCount + " lost hp, " + base.currentFigureHP + " hp left");
            }
            base.figureCount--;
            base.currentFigureHP = this.currentFigure.maxHitPoints;
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
        }
    }

    public void ApplyResistFigureDeath(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null, BattleAttack battleAttack = null)
    {
        int num = this.FigureCount();
        if (num < 1)
        {
            return;
        }
        float num2 = (float)(this.currentFigure.resist + resistIncrease - resistReduction) * 0.1f;
        if ((double)num2 >= 1.0)
        {
            sb?.AppendLine(header + " do not lost any figure.");
            return;
        }
        for (int i = 0; i < num; i++)
        {
            if (random.GetSuccesses(1f - num2, 1) == 0)
            {
                continue;
            }
            if (battleAttack != null)
            {
                this.UpdateDamagePool(battleAttack.skillScript.damagePool, base.currentFigureHP);
            }
            if (data != null)
            {
                if (data is Spell)
                {
                    this.UpdateDamagePool((data as Spell).damagePool, base.currentFigureHP);
                }
                if (data is EnchantmentScript)
                {
                    this.UpdateDamagePool((data as EnchantmentScript).damagePool, base.currentFigureHP);
                }
            }
            base.figureCount--;
        }
        if (num != base.figureCount && base.figureCount != 0)
        {
            base.currentFigureHP = this.GetCurentFigure().maxHitPoints;
        }
        sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
    }

    public void ApplyResistUnitDeath(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, Spell spell = null, BattleAttack battleAttack = null)
    {
        int num = this.FigureCount();
        if (num < 1)
        {
            return;
        }
        float num2 = (float)(this.currentFigure.resist + resistIncrease + resistReduction) * 0.1f;
        if ((double)num2 >= 1.0)
        {
            sb?.AppendLine(header + " do not lost unit.");
        }
        else
        {
            if (random.GetSuccesses(1f - num2, 1) == 0)
            {
                return;
            }
            for (int i = 0; i < num; i++)
            {
                if (battleAttack != null)
                {
                    this.UpdateDamagePool(battleAttack.skillScript.damagePool, base.currentFigureHP);
                }
                if (spell != null)
                {
                    this.UpdateDamagePool(spell.damagePool, base.currentFigureHP);
                }
                base.currentFigureHP = 0;
                base.figureCount--;
            }
            sb?.AppendLine(header + " killed.");
        }
    }

    public void ApplyResistOneFigureDeath(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null, BattleAttack battleAttack = null)
    {
        int num = this.FigureCount();
        if (num < 1)
        {
            return;
        }
        float num2 = (float)(this.currentFigure.resist + resistIncrease - resistReduction) * 0.1f;
        if ((double)num2 >= 1.0)
        {
            sb?.AppendLine(header + " do not lost any figure.");
            return;
        }
        if (random.GetSuccesses(1f - num2, 1) != 0)
        {
            if (battleAttack != null)
            {
                this.UpdateDamagePool(battleAttack.skillScript.damagePool, base.currentFigureHP);
            }
            if (data != null)
            {
                if (data is Spell)
                {
                    this.UpdateDamagePool((data as Spell).damagePool, base.currentFigureHP);
                }
                if (data is EnchantmentScript)
                {
                    this.UpdateDamagePool((data as EnchantmentScript).damagePool, base.currentFigureHP);
                }
            }
            base.figureCount--;
        }
        if (num != base.figureCount && base.figureCount != 0)
        {
            base.currentFigureHP = this.GetCurentFigure().maxHitPoints;
        }
        if (base.figureCount == 0)
        {
            base.currentFigureHP = 0;
        }
        sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
    }

    public void ApplyResistDisintegrate(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        int num = this.FigureCount();
        if (num < 1)
        {
            return;
        }
        if ((double)((float)(this.currentFigure.resist + resistIncrease - resistReduction) * 0.1f) >= 1.0)
        {
            sb?.AppendLine(header + " resisted disintegrated.");
            return;
        }
        for (int i = 0; i < num; i++)
        {
            if (data != null)
            {
                this.irreversibleDamages += base.currentFigureHP;
            }
            base.currentFigureHP = 0;
            base.figureCount--;
        }
        sb?.AppendLine(header + " disintegrated.");
    }

    public void ApplyPoisonDmg(int[] damage, MHRandom random, BattleAttack battleAttack, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        float num = 0f;
        if (this.FigureCount() < 1)
        {
            return;
        }
        num = ((!replaceDefenceWithModifier) ? ((float)(this.currentFigure.resist + resistIncrease - resistReduction) * 0.1f) : ((float)resistIncrease));
        if ((double)num >= 1.0)
        {
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
            return;
        }
        int num2 = damage[0];
        int successes = random.GetSuccesses(num, num2);
        int num3 = num2 - successes;
        while (base.figureCount > 0 && num3 > 0)
        {
            int num4 = Math.Min(num3, base.currentFigureHP);
            num3 -= num4;
            if (battleAttack != null)
            {
                this.UpdateDamagePool(battleAttack.skillScript.damagePool, num4);
            }
            if (data != null)
            {
                if (data is Spell)
                {
                    this.UpdateDamagePool((data as Spell).damagePool, num4);
                }
                if (data is EnchantmentScript)
                {
                    this.UpdateDamagePool((data as EnchantmentScript).damagePool, num4);
                }
            }
            base.currentFigureHP -= num4;
            if (base.currentFigureHP <= 0)
            {
                base.figureCount--;
                base.currentFigureHP = this.GetCurentFigure().maxHitPoints;
            }
        }
        if (base.figureCount == 0)
        {
            base.currentFigureHP = 0;
        }
        sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
    }

    public void ApplyLifeStealDmg(BattleUnit skillOwner, MHRandom random, BattleAttack battleAttack, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        int num = this.FigureCount();
        int num2 = skillOwner.figureCount;
        int num3 = 0;
        if (num < 1)
        {
            return;
        }
        int num4 = this.currentFigure.resist + resistIncrease - resistReduction;
        if ((double)num4 >= 10.0)
        {
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
            return;
        }
        for (int i = 0; i < num2; i++)
        {
            int num5 = random.GetInt(0, 11) - num4;
            if (num5 <= 0)
            {
                continue;
            }
            while (base.figureCount > 0)
            {
                if (battleAttack != null)
                {
                    this.UpdateDamagePool(battleAttack.skillScript.damagePool, num5);
                }
                if (data != null)
                {
                    if (data is Spell)
                    {
                        this.UpdateDamagePool((data as Spell).damagePool, num5);
                    }
                    if (data is EnchantmentScript)
                    {
                        this.UpdateDamagePool((data as EnchantmentScript).damagePool, num5);
                    }
                }
                int num6 = base.currentFigureHP;
                base.currentFigureHP -= num5;
                num5 -= num6;
                num3 = ((base.currentFigureHP <= 0) ? (num3 + num6) : (num3 + (num6 - base.currentFigureHP)));
                if (base.currentFigureHP <= 0)
                {
                    base.figureCount--;
                    base.currentFigureHP = this.currentFigure.maxHitPoints;
                }
                if (num5 <= 0 || base.figureCount == 0)
                {
                    break;
                }
            }
        }
        skillOwner.HealUnit(num3, ignoreCanNaturalHeal: true);
        if (base.figureCount == 0)
        {
            base.currentFigureHP = 0;
        }
        sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
    }

    public void ApplyPowerDraintDmg(BattleUnit skillOwner, MHRandom random, BattleAttack battleAttack, int powerDrain, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
    {
        int num = this.FigureCount();
        int num2 = skillOwner.figureCount;
        if (num < 1)
        {
            return;
        }
        for (int i = 0; i < num2; i++)
        {
            if (powerDrain > 0 && this.mana > 0 && base.figureCount <= 0)
            {
                continue;
            }
            if (resistIncrease > 0)
            {
                powerDrain /= 2;
            }
            if (this.mana >= powerDrain)
            {
                this.mana -= powerDrain;
            }
            else
            {
                this.mana = 0;
            }
            if (this.baseFigure.rangedAmmo > 0)
            {
                this.baseFigure.rangedAmmo -= powerDrain / 3;
                if (this.baseFigure.rangedAmmo < 0)
                {
                    this.baseFigure.rangedAmmo = 0;
                }
            }
            if (skillOwner.attributes.Contains(TAG.MANA_POINTS) && skillOwner.attributes.Contains(TAG.AMMUNITION))
            {
                skillOwner.mana += powerDrain / 2;
                skillOwner.baseFigure.rangedAmmo += powerDrain / 6;
            }
            else if (skillOwner.attributes.Contains(TAG.MANA_POINTS))
            {
                skillOwner.mana += powerDrain;
            }
        }
        sb?.AppendLine(header + " lost " + powerDrain + " mana, points of" + this.mana + "left");
    }

    public void ApplyBleeding(BattleUnit skillOwner, StringBuilder sb = null, string header = null, BattleAttack ba = null)
    {
        if (this.FigureCount() < 1 || ba == null)
        {
            return;
        }
        _ = ba.skillScript.fIntParam;
        if (ba.skillScript.stringData == null)
        {
            return;
        }
        Skill skill = (Skill)DataBase.Get(ba.skillScript.stringData, reportMissing: false);
        if (skill == null)
        {
            Debug.Log("There is no ench with name " + ba.skillScript.stringData);
            return;
        }
        base.attributes.AddToBase((Tag)TAG.BLEEDING, ba.skillScript.fIntParam);
        if (this.GetSkills().Find((DBReference<Skill> o) => o.Get() == (Skill)SKILL.BLEEDING_UNIT) == null)
        {
            this.AddSkill(skill);
        }
        if (sb != null)
        {
            FInt fIntParam = ba.skillScript.fIntParam;
            sb.AppendLine(header + " apply " + fIntParam.ToString() + " bleeding on target unit.");
        }
    }

    public void ApplyResistStun(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null, BattleAttack battleAttack = null)
    {
        if (this.FigureCount() < 1 || battleAttack == null || battleAttack.skillScript.stringData == null)
        {
            return;
        }
        float num = (float)(this.currentFigure.resist + resistIncrease - resistReduction) * 0.1f;
        if ((double)num >= 1.0)
        {
            sb?.AppendLine(header + " do not lost any figure.");
            return;
        }
        if (random.GetSuccesses(1f - num, 1) != 0)
        {
            Enchantment enchantment = (Enchantment)DataBase.Get(battleAttack.skillScript.stringData, reportMissing: false);
            if (enchantment == null)
            {
                Debug.Log("There is no ench with name " + battleAttack.skillScript.stringData);
                return;
            }
            if (base.enchantmentManager.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.STUN) == null)
            {
                base.enchantmentManager.Add(enchantment, battleAttack.source, enchantment.lifeTime, null, inBattle: true, 10);
                base.attributes.SetDirty();
            }
        }
        sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
    }

    public void ApplyResistMud(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null, BattleAttack battleAttack = null)
    {
        if (this.FigureCount() < 1 || battleAttack == null || battleAttack.skillScript.stringData == null)
        {
            return;
        }
        float num = (float)(this.currentFigure.resist + resistIncrease - resistReduction) * 0.1f;
        if ((double)num >= 1.0)
        {
            sb?.AppendLine(header + " do not lost any figure.");
            return;
        }
        if (random.GetSuccesses(1f - num, 1) != 0)
        {
            Enchantment enchantment = (Enchantment)DataBase.Get(battleAttack.skillScript.stringData, reportMissing: false);
            if (enchantment == null)
            {
                Debug.Log("There is no ench with name " + battleAttack.skillScript.stringData);
                return;
            }
            EnchantmentInstance enchantmentInstance = base.enchantmentManager.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.MUD);
            if (enchantmentInstance == null)
            {
                base.enchantmentManager.Add(enchantment, battleAttack.source, enchantment.lifeTime, null, inBattle: true, 10);
                base.attributes.SetDirty();
            }
            else
            {
                enchantmentInstance.countDown++;
            }
        }
        sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
    }

    public bool IsAlive()
    {
        return base.figureCount > 0;
    }

    public BattleFigure GetBaseFigure()
    {
        return this.baseFigure;
    }

    public BattleFigure GetCurentFigure()
    {
        return this.currentFigure;
    }

    public float GetFakePower()
    {
        return this.GetBaseFigure().GetFakePower() * Mathf.Pow(this.maxCount, 0.7f);
    }

    public int GetMaxFigureCount()
    {
        return this.maxCount;
    }

    public int GetMaxTotalHp()
    {
        return this.GetMaxFigureCount() * this.GetCurentFigure().maxHitPoints;
    }

    public double[] GetDataForNeuralNetwork()
    {
        if (this.nnRepresentation == null)
        {
            this.nnRepresentation = new double[5]
            {
                (double)this.baseFigure.attack * 0.01,
                (double)this.baseFigure.rangedAttack * 0.01,
                (double)this.baseFigure.defence * 0.01,
                (double)this.baseFigure.maxHitPoints * 0.01,
                (double)this.maxCount * 0.01
            };
        }
        return this.nnRepresentation;
    }

    public override DescriptionInfo GetDescriptionInfo()
    {
        return base.dbSource.Get().GetDescriptionInfo();
    }

    public override Vector3 GetPhysicalPosition()
    {
        if (this.battleFormation != null)
        {
            return this.battleFormation.transform.position;
        }
        return Vector3.zero;
    }

    public override Vector3i GetPosition()
    {
        return this.battlePosition;
    }

    public override global::WorldCode.Plane GetPlane()
    {
        if (Battle.GetBattle() == null)
        {
            return null;
        }
        return Battle.GetBattle().plane;
    }

    public Formation GetOrCreateFormation(IPlanePosition owner = null, bool createIfMissing = true)
    {
        IPlanePosition planePosition2;
        if (owner == null)
        {
            IPlanePosition planePosition = this;
            planePosition2 = planePosition;
        }
        else
        {
            planePosition2 = owner;
        }
        if (planePosition2?.GetPlane() == null)
        {
            return null;
        }
        if (this.battleFormation == null && createIfMissing)
        {
            if (Battle.GetBattle() == null)
            {
                Debug.LogError("creating battleunit formation outside battle Is battleunit created from world unit :" + this.simulated);
            }
            else
            {
                bool flag = false;
                foreach (KeyValuePair<BattleUnit, global::MOM.Unit> item in Battle.GetBattle().buToSource)
                {
                    if (item.Key == this)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    Debug.LogError("creating battleunit formation from wrong unit. Is battleunit created from world unit :" + this.simulated);
                }
            }
            Vector3 lookAtDir = ((this.battlePosition.y >= 0) ? HexCoordinates.HexToWorld3D(new Vector3i(1, -1, 0)) : HexCoordinates.HexToWorld3D(new Vector3i(-1, 1, 0)));
            Vector3i position = this.GetPosition();
            IPlanePosition owner2;
            if (owner == null)
            {
                IPlanePosition planePosition = this;
                owner2 = planePosition;
            }
            else
            {
                owner2 = owner;
            }
            this.battleFormation = Formation.CreateFormation(this, position, owner2, lookAtDir);
            VerticalMarkerManager.Get().Addmarker(this);
        }
        if (this.ownerID != PlayerWizard.HumanID() && this.battleFormation != null)
        {
            this.battleFormation.SetVisibility(this.currentlyVisible);
        }
        return this.battleFormation;
    }

    public void MoveTo(Vector3i newPos)
    {
        Battle.UpdateUnitPosition(this, this.battlePosition, newPos);
        this.battlePosition = newPos;
        if (this.GetOrCreateFormation() != null)
        {
            this.GetOrCreateFormation().InstantMove();
        }
    }

    public void MoveViaPath(List<Vector3i> path)
    {
        if (!this.IsAlive() || path == null || path.Count < 2)
        {
            return;
        }
        if (this.GetOrCreateFormation() != null)
        {
            if (path[path.Count - 1] == this.battlePosition)
            {
                path.Invert();
            }
            this.GetOrCreateFormation().Move(path, wastefullLastStep: false);
            MHEventSystem.TriggerEvent<BaseUnit>(this, path);
        }
        Vector3i to = path[path.Count - 1];
        Battle.UpdateUnitPosition(this, this.battlePosition, to);
        this.battlePosition = to;
        Battle.Get().UpdateInvisibility();
        if (path.Count <= 1 || Battle.GetBattle() == null || !Battle.GetBattle().fireWall || !Battle.GetBattle().StepThroughFirewall(path[path.Count - 1], path[path.Count - 2]) || base.figureCount <= 0 || !(base.attributes.GetFinal((Tag)TAG.CAN_FLY) == 0))
        {
            return;
        }
        int[] array = new int[base.figureCount];
        for (int i = 0; i < base.figureCount; i++)
        {
            int num = 0;
            for (int j = 0; j < 5; j++)
            {
                num = ((global::UnityEngine.Random.Range(0f, 1f) < 0.3f) ? 1 : 0);
            }
            array[i] = num;
        }
        this.ApplyDamage(array, new MHRandom(), null, 0);
        VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
        this.GetOrCreateFormation().UpdateFigureCount();
    }

    public void MoveAnimatedTo(Vector3i pos, Battle battle)
    {
        if (battle?.plane?.GetSearcherData() != null)
        {
            if (battle.plane.GetHexAt(pos) == null)
            {
                return;
            }
            HashSet<Vector3i> exclusionPoints = battle.plane.exclusionPoints;
            if ((exclusionPoints != null && exclusionPoints.Contains(pos)) || battle.plane.GetSearcherData().IsUnitAt(pos))
            {
                return;
            }
        }
        if (this.teleporting)
        {
            if (base.Mp > 0)
            {
                base.Mp -= 1;
                List<Vector3i> path = new List<Vector3i>
                {
                    this.GetPosition(),
                    pos
                };
                battle.GainAttention(this.GetOrCreateFormation());
                this.MoveViaPath(path);
            }
            return;
        }
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.GetPlane(), this.GetPosition(), pos, this);
        PathfinderV2.FindPath(requestDataV);
        List<Vector3i> path2 = requestDataV.GetPath();
        if (path2 == null || path2.Count < 2)
        {
            Debug.Log("Path impossible");
            return;
        }
        if (path2[path2.Count - 1] == this.GetPosition())
        {
            path2.Invert();
        }
        if (battle.IsLocationOccupied(path2[path2.Count - 1]))
        {
            path2.RemoveAt(path2.Count - 1);
        }
        path2 = this.CutPathToMP(battle, base.Mp, path2, useMP: true, mpWastingStop: false);
        if (path2 != null && path2.Count >= 2)
        {
            battle.GainAttention(this.GetOrCreateFormation());
            this.MoveViaPath(path2);
        }
    }

    public List<Vector3i> CutPathToMP(Battle battle, FInt maxMP, List<Vector3i> path, bool useMP, bool mpWastingStop)
    {
        int num = 0;
        int num2 = 1;
        bool flag = false;
        if (Battle.GetBattle() != null && Battle.GetBattle().fireWall)
        {
            flag = true;
        }
        int a;
        switch (DifficultySettingsData.GetSetting("UI_BATTLE_MP_TERRAIN_COSTS").value)
        {
        case "1":
            a = 1;
            break;
        case "2":
            a = 2;
            break;
        case "3":
            a = 100;
            break;
        default:
            a = 100;
            break;
        }
        int i;
        for (i = 1; i < path.Count; i++)
        {
            Hex hexAt = battle.plane.GetHexAt(path[i]);
            if (hexAt == null)
            {
                break;
            }
            if (!this.nonCorporealMovement && battle.battleWalls.Count > 0)
            {
                BattleWall battleWall = battle.battleWalls.Find((BattleWall o) => o.position == path[i]);
                if (battleWall != null && battleWall.standing && (!battleWall.gate || this.attackingSide))
                {
                    mpWastingStop = false;
                    break;
                }
            }
            int b = hexAt.MovementCost();
            b = Mathf.Min(a, b);
            if (b > 1)
            {
                if (this.nonCorporealMovement)
                {
                    b = 1;
                }
                else if (this.mountainMovement && (hexAt.HaveFlag(ETerrainType.Mountain) || hexAt.HaveFlag(ETerrainType.Hill)))
                {
                    b = 1;
                }
                else if (this.forestMovement && hexAt.HaveFlag(ETerrainType.Forest))
                {
                    b = 1;
                }
                else if (this.earthWalkerMovement && (hexAt.HaveFlag(ETerrainType.Desert) || hexAt.HaveFlag(ETerrainType.Tundra)))
                {
                    b = 1;
                }
                else if ((this.earthWalkerMovement || this.waterMovement) && hexAt.HaveFlag(ETerrainType.Swamp))
                {
                    b = 1;
                }
            }
            if (hexAt.viaRiver != null && !this.waterMovement && !this.nonCorporealMovement)
            {
                for (int j = 0; j < HexNeighbors.neighbours.Length; j++)
                {
                    if (hexAt.viaRiver[j] && HexNeighbors.neighbours[j] + hexAt.Position == path[i - 1])
                    {
                        b++;
                        break;
                    }
                }
            }
            num += b;
            num2++;
            if (flag && !this.nonCorporealMovement)
            {
                Vector3i a2 = path[i];
                Vector3i b2 = path[i - 1];
                if (Battle.GetBattle().StepThroughFirewall(a2, b2))
                {
                    break;
                }
            }
            if (num >= maxMP)
            {
                break;
            }
        }
        if (useMP)
        {
            base.Mp = FInt.Max(0f, base.Mp - num);
        }
        if (mpWastingStop)
        {
            num2--;
        }
        if (num2 <= 0)
        {
            path.Clear();
            return path;
        }
        return path.GetRange(0, num2);
    }

    public override string GetDBName()
    {
        return base.dbSource.dbName;
    }

    public override int FigureCount()
    {
        return base.figureCount;
    }

    public void UseHalfOfMaxMP()
    {
        int num = this.GetCurentFigure().movementSpeed;
        if (this.haste)
        {
            num *= 2;
        }
        int num2 = (num + 1) / 2;
        if (base.Mp > num2)
        {
            base.Mp -= num2;
        }
        else
        {
            base.Mp = FInt.ZERO;
        }
    }

    public override float GetTotalHpPercent()
    {
        if (base.figureCount == 0)
        {
            return 0f;
        }
        float num = (float)base.figureCount / (float)this.maxCount;
        float num2 = 1f / (float)this.maxCount;
        float num3 = num2 * (float)base.currentFigureHP / (float)this.GetCurentFigure().maxHitPoints;
        return num - num2 + num3;
    }

    public void HealUnit(int heal, bool ignoreCanNaturalHeal = false)
    {
        if (!base.canNaturalHeal && !ignoreCanNaturalHeal)
        {
            return;
        }
        int maxHitPoints = this.GetCurentFigure().maxHitPoints;
        int maxFigureCount = this.GetMaxFigureCount();
        int num = maxHitPoints * maxFigureCount;
        int num2 = (base.figureCount - 1) * maxHitPoints + base.currentFigureHP;
        int b = num - num2 - this.irreversibleDamages;
        heal = Mathf.Min(heal, b);
        while (heal > 0)
        {
            int num3 = maxHitPoints - base.currentFigureHP;
            if (heal >= num3)
            {
                heal -= num3;
                base.currentFigureHP = maxHitPoints;
                this.normalDamages -= num3;
            }
            else
            {
                base.currentFigureHP += heal;
                this.normalDamages -= heal;
                heal = 0;
            }
            if (heal > 0 && base.figureCount < maxFigureCount)
            {
                base.figureCount++;
                base.currentFigureHP = 1;
                heal--;
                this.normalDamages--;
            }
            if (base.currentFigureHP == maxHitPoints && base.figureCount == maxFigureCount)
            {
                break;
            }
        }
    }

    public void UpdateDamagePool(ESkillDamagePool damagePool, int dmg)
    {
        int num = ((dmg > base.currentFigureHP) ? base.currentFigureHP : dmg);
        switch (damagePool)
        {
        case ESkillDamagePool.Undead:
            if (base.attributes.Contains(TAG.MAGIC_IMMUNITY) || base.attributes.Contains(TAG.DEATH_IMMUNITY))
            {
                this.normalDamages += num;
            }
            else
            {
                this.undeadDamages += num;
            }
            break;
        case ESkillDamagePool.Irreversible:
            this.irreversibleDamages += num;
            break;
        default:
            this.normalDamages += num;
            break;
        }
    }

    public override EnchantmentManager GetEnchantmentManager()
    {
        if (base.enchantmentManager == null)
        {
            base.enchantmentManager = new EnchantmentManager(this);
        }
        return base.enchantmentManager;
    }

    public void DeserializeEnchantmentManager(MemoryStream ms)
    {
        ms.Position = 0L;
        base.enchantmentManager = Serializer.Deserialize<EnchantmentManager>(ms);
        base.enchantmentManager.owner = this;
    }

    public override SkillManager GetSkillManager()
    {
        if (base.skillManager == null)
        {
            base.skillManager = new SkillManager(this);
        }
        return base.skillManager;
    }

    public int GetModifiedBattleUnitValue(TAG tag, FInt value)
    {
        return (int)ScriptLibrary.Call("GetModifiedBattleUnitValue", this, (Tag)tag, value);
    }

    public int GetBattleUnitValue()
    {
        if (this.strategicValue == 0)
        {
            this.strategicValue = (int)ScriptLibrary.Call("GetBattleUnitValue", this);
        }
        return this.strategicValue;
    }

    public int GetBattleUnitValueFixedHP(float totalHPShare)
    {
        return (int)ScriptLibrary.Call("GetModifiedValueFixedHP", this, totalHPShare);
    }

    public int GetStrategicValueForSpell(ISpellCaster caster, Battle b, Spell s)
    {
        int num = 20;
        int battleUnitValue = this.GetBattleUnitValue();
        MemoryStream memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, this);
        int num2 = 1;
        int num3 = 0;
        int num4 = battleUnitValue;
        for (int i = 0; i < num; i++)
        {
            memoryStream.Position = 0L;
            BattleUnit battleUnit = Serializer.Deserialize<BattleUnit>(memoryStream);
            battleUnit.simulated = true;
            SpellCastData castData = new SpellCastData(caster, b);
            Battle.CastBattleSpell(s, castData, battleUnit, simulated: true);
            int battleUnitValue2 = battleUnit.GetBattleUnitValue();
            if (num4 == battleUnitValue2)
            {
                num3 += battleUnitValue2;
                num2++;
            }
            else
            {
                num3 += battleUnitValue2;
                num2 = 0;
                num4 = battleUnitValue2;
            }
            if (num2 == 3)
            {
                return num3 / (i + 1);
            }
        }
        memoryStream.Dispose();
        return num3 / num;
    }

    public Multitype<int, int> GetStrategicAttackGain(BattleUnit target, Battle b, bool melee)
    {
        int iterations = 5;
        this.GetBattleUnitValue();
        target.GetBattleUnitValue();
        MemoryStream memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, this);
        MemoryStream memoryStream2 = new MemoryStream();
        Serializer.Serialize(memoryStream2, target);
        MHRandom random = new MHRandom();
        memoryStream.Position = 0L;
        memoryStream2.Position = 0L;
        object obj = MHThread.DirectExecution(new UnitAttackSimulatorTask
        {
            random = random,
            aMStream = memoryStream,
            dMStream = memoryStream2,
            meleeFight = melee,
            battle = b,
            iterations = iterations
        });
        memoryStream.Dispose();
        memoryStream2.Dispose();
        return obj as Multitype<int, int>;
    }

    public override SpellManager GetSpellManager()
    {
        if (base.spellManager == null)
        {
            base.spellManager = new SpellManager(this);
        }
        return base.spellManager;
    }

    public override PlayerWizard GetWizardOwner()
    {
        return GameManager.GetWizard(this.ownerID);
    }

    public override int GetTotalCastingSkill()
    {
        return -1;
    }

    public override int GetMana()
    {
        return this.mana;
    }

    public override void SetMana(int m)
    {
        this.mana = m;
    }

    public void FillMovementCost()
    {
        bool flag = base.GetAttributes().GetFinal(TAG.CAN_FLY) > 0;
        this.nonCorporealMovement = flag || base.GetAttributes().GetFinal(TAG.NON_CORPOREAL) > 0;
        this.landMovement = flag || base.GetAttributes().GetFinal(TAG.CAN_WALK) > 0;
        this.waterMovement = flag || base.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0;
        this.mountainMovement = base.GetAttributes().GetFinal(TAG.MOUNTAINEER) > 0;
        this.forestMovement = base.GetAttributes().GetFinal(TAG.FORESTER) > 0;
        this.earthWalkerMovement = base.GetAttributes().GetFinal(TAG.EARTH_WALKER) > 0;
        this.teleporting = base.GetAttributes().GetFinal(TAG.TELEPORTING) > 0;
    }

    public override void AttributesChanged()
    {
        this.FillMovementCost();
        this.strategicValue = 0;
        this.GetCurentFigure().UpdateFromAttributes(base.GetAttributes());
        this.MenageRaft(Battle.GetBattle());
    }

    protected override void FigureCountChanged(int prevCount, int newCount)
    {
        this.strategicValue = 0;
        if (prevCount > newCount && !this.simulated && this.GetWizardOwner() != null && Battle.Get() != null && this.CanDeathFigureModyfyCastingSkill(this.GetWizardOwner(), this))
        {
            this.DeathFigureCastingSkillModyfication(prevCount - newCount, Battle.Get(), this.GetWizardOwner());
        }
        if (this.FigureCount() < 1)
        {
            if (!this.simulated)
            {
                this.MenageRaft(Battle.GetBattle());
                this.GetPlane()?.ClearUnitPosition(this.battlePosition);
                VerticalMarkerManager.Get().DestroyMarker(this);
            }
        }
        else if (!this.simulated && this.currentlyVisible && this.GetPlane() != null)
        {
            Formation orCreateFormation = this.GetOrCreateFormation(null, createIfMissing: false);
            if (!(orCreateFormation == null) && this.FigureCount() > orCreateFormation.GetCharacterActors().Count)
            {
                orCreateFormation.UpdateFigureCount();
            }
        }
    }

    protected override void FigureHealthChanged()
    {
        this.strategicValue = 0;
        if (this.FigureCount() > 0 && !this.simulated)
        {
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
        }
        BattleHUD.Get()?.SetUnitDirty(this);
    }

    public void Regeneration(bool postBattle = false, int hpReneration = 0)
    {
        if (postBattle)
        {
            base.currentFigureHP = this.currentFigure.maxHitPoints;
            base.figureCount = this.maxCount;
            return;
        }
        for (int i = 0; i < hpReneration; i++)
        {
            if (base.currentFigureHP < this.currentFigure.maxHitPoints)
            {
                base.currentFigureHP++;
            }
            else if (base.figureCount > 0 && base.figureCount < this.maxCount)
            {
                base.figureCount++;
                base.currentFigureHP = 1;
            }
        }
    }

    public int Bleeding()
    {
        FInt @base = base.attributes.GetBase(TAG.BLEEDING);
        int num = 0;
        MHRandom mHRandom = new MHRandom();
        float num2 = base.attributes.GetFinal(TAG.RESIST).ToFloat();
        float chance = Mathf.Clamp01(1f - num2 * 0.1f);
        if (mHRandom.GetSuccesses(chance, 1) > 0 && base.figureCount > 0)
        {
            for (int i = 0; i < @base; i++)
            {
                if (base.currentFigureHP > 1)
                {
                    num++;
                    base.currentFigureHP--;
                    continue;
                }
                num++;
                base.figureCount--;
                if (base.figureCount <= 0)
                {
                    base.currentFigureHP = 0;
                    break;
                }
                base.currentFigureHP = this.currentFigure.maxHitPoints;
            }
        }
        else
        {
            base.attributes.SetBaseTo((Tag)TAG.BLEEDING, 0f);
            _ = base.skillManager.GetSkills().Find((DBReference<Skill> o) => o.Get() == (Skill)SKILL.BLEEDING_UNIT) != null;
            base.skillManager.Remove((Skill)SKILL.BLEEDING_UNIT);
        }
        return num;
    }

    public Tag GetRangedTag()
    {
        if (this.GetAttFinal(TAG.MAGIC_RANGE) > 0)
        {
            return (Tag)TAG.MAGIC_RANGE;
        }
        if (this.GetAttFinal(TAG.LONG_RANGE) > 0)
        {
            return (Tag)TAG.LONG_RANGE;
        }
        return null;
    }

    public FInt RangedPenalty(int distance, Tag rangedTag)
    {
        distance /= 3;
        if (distance < 1)
        {
            return FInt.ZERO;
        }
        if (rangedTag == null)
        {
            return distance * new FInt(0.1f);
        }
        if (rangedTag == (Tag)TAG.LONG_RANGE)
        {
            return new FInt(0.1f);
        }
        return FInt.ZERO;
    }

    public bool CanReanimateAsUndead()
    {
        int num = this.GetCurentFigure().maxHitPoints * this.maxCount;
        return this.undeadDamages >= Math.Max(num / 2, 1);
    }

    public void PlayDeadAnimUI()
    {
        MHEventSystem.TriggerEvent<BattleUnit>(this, null);
    }

    public string GetFastChangeDetector()
    {
        return this.GetEnchantmentManager().iteration.ToString().PadLeft(4, '0') + this.GetCurentFigure().rangedAmmo.ToString().PadLeft(2, '0') + this.mana.ToString().PadLeft(4, '0') + base.figureCount.ToString().PadLeft(2, '0') + base.currentFigureHP.ToString().PadLeft(2, '0') + base.Mp.ToInt().ToString().PadLeft(2, '0');
    }

    public void UseManaFor(Spell spell)
    {
        if (spell.unitBattleCost != 0)
        {
            this.mana -= spell.unitBattleCost;
        }
        else
        {
            this.mana -= spell.battleCost;
        }
        if (this.GetWizardOwner().GetMagicAndResearch() != null)
        {
            this.mana -= this.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraMana;
        }
        this.spellCasted = true;
        base.Mp = FInt.ZERO;
    }

    public void UpdateUnitMP()
    {
        base.Mp = new FInt(this.GetCurentFigure().movementSpeed);
        if (!base.canMove)
        {
            base.Mp = FInt.ZERO;
        }
        if (this.haste)
        {
            base.Mp *= 2;
        }
    }

    public float HitChance(BattleUnit buAttacker, BattleUnit buTarget, float rangedPenalty)
    {
        float num = Mathf.Clamp01(1f - buTarget.blurProtection);
        float num2 = buTarget.invisibilityProtection;
        if (base.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) > 0)
        {
            num2 = 0f;
        }
        return Mathf.Clamp01(buAttacker.currentFigure.rangedAttackChance * num - rangedPenalty - num2);
    }

    public override int GetWizardOwnerID()
    {
        return this.ownerID;
    }

    private bool CanDeathFigureModyfyCastingSkill(PlayerWizard wizard, BattleUnit unit)
    {
        if (wizard.deathFigureCastingSkillFilter != null && (bool)ScriptLibrary.Call(wizard.deathFigureCastingSkillFilter, unit))
        {
            return true;
        }
        return false;
    }

    private void DeathFigureCastingSkillModyfication(int deathFigure, Battle battle, PlayerWizard wizardOwner)
    {
        int num = 5;
        if (battle.attacker.GetID() == wizardOwner.GetID())
        {
            battle.attacker.castingSkill += num * deathFigure;
            if (BattleHUD.Get() != null && battle.attacker.GetWizardOwner().IsHuman)
            {
                BattleHUD.Get().AddingCastingSkillAnimation(this.battlePosition);
            }
        }
        else if (battle.defender.GetID() == wizardOwner.GetID())
        {
            battle.defender.castingSkill += num * deathFigure;
            if (BattleHUD.Get() != null && battle.defender.GetWizardOwner().IsHuman)
            {
                BattleHUD.Get().AddingCastingSkillAnimation(this.battlePosition);
            }
        }
    }

    public int GetTotalHealth()
    {
        return base._totalHP;
    }

    protected override void CalculateTotalHealth()
    {
        int totalHealth = this.GetTotalHealth();
        int num = (base.figureCount - 1) * this.GetCurentFigure().maxHitPoints + base.currentFigureHP;
        if (totalHealth > num)
        {
            this.FairExchange(totalHealth - num);
        }
        base._totalHP = num;
    }

    private void FairExchange(int delta)
    {
        if (this.simulated || !(base.GetAttributes().GetFinal(TAG.FAIR_EXCHANGE) > 0) || Battle.Get() == null || this.GetWizardOwner() == null)
        {
            return;
        }
        int num = (this.lostHP + delta) / 2;
        if (num > 0)
        {
            num *= 5;
            Battle battle = Battle.Get();
            if (this.GetWizardOwner().IsAttackerInBattle(battle))
            {
                battle.attacker.castingSkill += num;
                if (BattleHUD.Get() != null && battle.attacker.GetWizardOwner().IsHuman)
                {
                    BattleHUD.Get().AddingCastingSkillAnimation(this.battlePosition);
                }
            }
            else
            {
                battle.attacker.castingSkill += num;
                if (BattleHUD.Get() != null && battle.defender.GetWizardOwner().IsHuman)
                {
                    BattleHUD.Get().AddingCastingSkillAnimation(this.battlePosition);
                }
            }
        }
        this.lostHP = (this.lostHP + delta) % 2;
    }

    public void CheckIfRaftNeeded()
    {
        if (this.battleFormation.raftModel == null && !this.waterMovement && !this.nonCorporealMovement && this.GetAttFinal(TAG.SHIP) < 1)
        {
            this.battleFormation.raftModel = this.CreateRaft(Battle.GetBattle(), this);
        }
    }

    private void MenageRaft(Battle battle)
    {
        if (!(this.battleFormation != null) || battle == null)
        {
            return;
        }
        this.CheckIfRaftNeeded();
        if (!(this.battleFormation.raftModel == null))
        {
            if ((!this.waterMovement && !this.nonCorporealMovement) || (this.FigureCount() <= 0 && this.GetAttFinal(TAG.SHIP) <= 0))
            {
                this.battleFormation.raftModel.SetActive(value: true);
                battle.SetHexModelPosition(this.battleFormation.raftModel, this.GetPosition());
            }
            else
            {
                this.battleFormation.raftModel.SetActive(value: false);
            }
        }
    }

    private GameObject CreateRaft(Battle battle, BattleUnit bu)
    {
        if (battle != null && !battle.landBattle && bu != null)
        {
            GameObject gameObject = AssetManager.Get<GameObject>("Raft");
            if (gameObject == null)
            {
                Debug.LogError("Model Raft is missing!");
                return null;
            }
            Chunk chunkFor = battle.plane.GetChunkFor(bu.GetPosition());
            GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
            if (gameObject2 == null)
            {
                return null;
            }
            battle.SetHexModelPosition(gameObject2, bu.GetPosition());
            return gameObject2;
        }
        return null;
    }
}
