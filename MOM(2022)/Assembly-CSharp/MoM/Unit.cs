using System;
using System.Collections.Generic;
using System.Text;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class Unit : BaseUnit
    {
        public class UnitEvent
        {
            public enum Reason
            {
                Damage = 0
            }

            public Reason reason;

            public int damage;

            public UnitEvent(int[] values)
            {
                this.damage = 0;
                foreach (int num in values)
                {
                    this.damage += num;
                }
                this.reason = Reason.Damage;
            }

            public UnitEvent(int value)
            {
                this.damage = value;
                this.reason = Reason.Damage;
            }
        }

        [ProtoMember(1)]
        public Reference<Group> group;

        [ProtoMember(2)]
        public bool simulationUnit;

        [ProtoMember(3)]
        public bool everAssignedToGroup;

        [ProtoMember(4)]
        public int heroEverAssignedToWizard;

        public int MaxCount()
        {
            if (base.dbSource.Get() is global::DBDef.Unit)
            {
                return (base.dbSource.Get() as global::DBDef.Unit).figures;
            }
            return 1;
        }

        public Formation CreateFormation(IPlanePosition owner)
        {
            Vector3i position = owner.GetPosition();
            object owner2;
            if (owner == null)
            {
                owner2 = this;
            }
            else
            {
                owner2 = owner;
            }
            return Formation.CreateFormation(this, position, (IPlanePosition)owner2, Vector3.zero);
        }

        public static Unit CreateFrom(Subrace source, bool simulation = false)
        {
            Unit unit = new Unit();
            unit.dbSource = source;
            unit.race = source.race;
            unit.canNaturalHeal = source.naturalHealing;
            unit.canGainXP = source.gainsXP;
            unit.RegisterEntity();
            unit.attributes = new Attributes(unit, source.tags);
            if (source is global::DBDef.Unit unit2)
            {
                unit.figureCount = unit2.figures;
            }
            else
            {
                unit.figureCount = 1;
            }
            if (source.skills != null)
            {
                Skill[] skills = source.skills;
                foreach (Skill skill2 in skills)
                {
                    unit.AddSkill(skill2);
                }
            }
            unit.currentFigureHP = unit.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
            if (source is Hero)
            {
                unit.artefactManager = new ArtefactManager(unit);
                if (!simulation && (source as Hero).skillPacks != null)
                {
                    SkillPack[] skillPacks = (source as Hero).skillPacks;
                    foreach (SkillPack skillPack in skillPacks)
                    {
                        if (skillPack.skills == null || skillPack.skills.Length == 0)
                        {
                            continue;
                        }
                        List<DBReference<Skill>> skills2 = unit.GetSkills();
                        Skill[] array = Array.FindAll(skillPack.skills, (Skill o) => DataBase.GetType<Skill>().Contains(o));
                        for (int j = 0; j < 10; j++)
                        {
                            int @int = new MHRandom().GetInt(0, array.Length);
                            Skill skill = array[@int];
                            if (skill.applicationScript != null && skill.applicationScript.triggerType == ESkillType.Caster)
                            {
                                if (skills2.Find((DBReference<Skill> o) => o.Get().applicationScript != null && o.Get().applicationScript.triggerType == ESkillType.Caster) == null)
                                {
                                    unit.AddSkill(skill);
                                }
                                else if (skill.applicationScript.fIntParam != 0)
                                {
                                    unit.GetAttributes().AddToBase(TAG.MANA_POINTS, skill.applicationScript.fIntParam);
                                }
                                else
                                {
                                    unit.GetAttributes().AddToBase(TAG.MANA_POINTS, (FInt)2.5f);
                                }
                                break;
                            }
                            if (skill.versionSuper == null || !(skills2.Find((DBReference<Skill> o) => o == skill.versionSuper) != null))
                            {
                                if (skills2.Find((DBReference<Skill> o) => o == skill) == null)
                                {
                                    unit.AddSkill(skill);
                                }
                                else if (skill.versionSuper != null)
                                {
                                    unit.AddSkill(skill.versionSuper);
                                    unit.RemoveSkill(skill);
                                }
                                break;
                            }
                        }
                    }
                }
                int num = 0;
                GameManager gameManager = GameManager.Get();
                foreach (Group registeredGroup in gameManager.registeredGroups)
                {
                    foreach (Reference<Unit> unit3 in registeredGroup.GetUnits())
                    {
                        if (unit3.Get().dbSource == source)
                        {
                            num++;
                        }
                    }
                }
                if (gameManager.wizards != null)
                {
                    foreach (PlayerWizard wizard in gameManager.wizards)
                    {
                        foreach (DeadHero deadHero in wizard.GetDeadHeroes())
                        {
                            if (deadHero.dbSource == source)
                            {
                                num++;
                            }
                        }
                    }
                }
                if (num > 0 && num <= (source as Hero).alterName.Length)
                {
                    string id = (source as Hero).alterName[num - 1];
                    unit.customName = global::DBUtils.Localization.Get(id, true);
                }
            }
            if (source.spellPack != null)
            {
                SpellPack[] spellPack = source.spellPack;
                foreach (SpellPack spellPack2 in spellPack)
                {
                    if (spellPack2.spells != null)
                    {
                        Spell[] spells = spellPack2.spells;
                        foreach (Spell spell in spells)
                        {
                            unit.GetSpellManager().Add(spell);
                        }
                    }
                }
            }
            return unit;
        }

        public void UpdateFrom(BattleUnit source)
        {
            base.figureCount = source.FigureCount();
            base.currentFigureHP = Mathf.Clamp(source.currentFigureHP, 1, this.GetAttFinal(TAG.HIT_POINTS).ToInt());
        }

        public override DescriptionInfo GetDescriptionInfo()
        {
            return base.dbSource.Get().GetDescriptionInfo();
        }

        public override Vector3 GetPhysicalPosition()
        {
            if (this.group != null)
            {
                return this.group.Get().GetPhysicalPosition();
            }
            return Vector3.zero;
        }

        public override Vector3i GetPosition()
        {
            if (this.group == null)
            {
                Debug.LogError("Unit without group asked for position! " + this.GetID());
                return Vector3i.invalid;
            }
            return this.group.Get().GetPosition();
        }

        public override global::WorldCode.Plane GetPlane()
        {
            if (this.group == null)
            {
                Debug.LogError("Unit without group asked for plane! " + this.GetID());
                return null;
            }
            return this.group.Get().GetPlane();
        }

        public override string GetDBName()
        {
            return base.dbSource.dbName;
        }

        public override int FigureCount()
        {
            return base.figureCount;
        }

        public void UpdateMP()
        {
            base.Mp = this.GetMaxMP();
            if (!base.canMove)
            {
                base.Mp = FInt.ZERO;
            }
        }

        public FInt GetMaxMP()
        {
            if (this.group == null)
            {
                return FInt.ZERO;
            }
            return base.attributes.GetFinal(TAG.MOVEMENT_POINTS);
        }

        public bool RegainTo1MP()
        {
            if (this.group == null)
            {
                return false;
            }
            if (base.canMove)
            {
                base.Mp = FInt.Max(base.Mp, FInt.ONE);
            }
            return false;
        }

        public bool NewTurn()
        {
            if (this.group == null)
            {
                return false;
            }
            bool result = false;
            this.UpdateMP();
            if (base.canNaturalHeal && this.GetTotalHpPercent() < 1f)
            {
                FInt fInt = FInt.ZERO;
                foreach (Reference<Unit> unit in this.group.Get().GetUnits())
                {
                    FInt fInt2 = unit.Get().attributes.GetFinal(TAG.HEALER);
                    if (this.GetAttFinal(TAG.MECHANICAL_UNIT) > 0)
                    {
                        fInt2 = FInt.Max(fInt2, unit.Get().GetAttFinal(TAG.MECHANICIAN));
                    }
                    if (fInt2 > fInt)
                    {
                        fInt = fInt2;
                    }
                }
                if (this.group.Get().GetLocationHost() is TownLocation)
                {
                    fInt += 1f;
                }
                result = this.Heal((fInt + 0.5f).ToFloat());
            }
            if (base.canGainXP && this.GetWizardOwner() != null)
            {
                base.xp++;
            }
            if (base.attributes.Contains(TAG.METEOR_STORM_AFFECTED) && !(this.group.Get().GetLocationHost() is TownLocation))
            {
                MHRandom random = new MHRandom();
                int[] array = new int[9] { 4, 0, 0, 0, 0, 0, 0, 0, 0 };
                int num = base.attributes.GetFinal((Tag)TAG.METEOR_STORM_AFFECTED).ToInt();
                for (int i = 0; i < num; i++)
                {
                    array = (int[])ScriptLibrary.Call("ProduceAreaSpellDamage", array[0], array, base.figureCount);
                    if (base.attributes.Contains(TAG.MAGIC_IMMUNITY) || base.attributes.Contains(TAG.FIRE_IMMUNITY) || base.attributes.Contains(TAG.RIGHTEOUSNESS))
                    {
                        this.ApplyImmolationDamage(array, random, 50, null, replaceDefenceWithModifier: true);
                    }
                    else if (base.attributes.Contains(TAG.ELEMENTAL_ARMOR))
                    {
                        this.ApplyImmolationDamage(array, random, 10);
                    }
                    else if (base.attributes.Contains(TAG.RESIST_ELEMENTS) || base.attributes.Contains(TAG.BLESS))
                    {
                        this.ApplyImmolationDamage(array, random, 3);
                    }
                    else if (base.attributes.Contains(TAG.LARGE_SHIELD))
                    {
                        this.ApplyImmolationDamage(array, random, 2);
                    }
                    else
                    {
                        this.ApplyImmolationDamage(array, random, 0);
                    }
                }
            }
            return result;
        }

        public bool Heal(float percent, bool ignoreCanNaturalHeal = false)
        {
            if (!base.canNaturalHeal && !ignoreCanNaturalHeal)
            {
                return false;
            }
            if (this.GetTotalHpPercent() >= 1f)
            {
                return false;
            }
            int num = base.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
            Subrace subrace = base.dbSource.Get();
            if (subrace is global::DBDef.Unit)
            {
                global::DBDef.Unit unit = subrace as global::DBDef.Unit;
                int num2 = (base.figureCount - 1) * num + base.currentFigureHP;
                int num3 = num * unit.figures;
                int num4 = Mathf.CeilToInt((float)num3 * percent * 0.1f);
                num2 = Mathf.Clamp(num2 + num4, 0, num3);
                if (num2 % num > 0)
                {
                    base.figureCount = num2 / num + 1;
                    base.currentFigureHP = num2 % num;
                }
                else
                {
                    base.figureCount = num2 / num;
                    base.currentFigureHP = num;
                }
            }
            else
            {
                int num5 = Mathf.CeilToInt((float)num * percent * 0.1f);
                base.currentFigureHP = Mathf.Clamp(base.currentFigureHP + num5, 0, num);
            }
            return true;
        }

        public override float GetTotalHpPercent()
        {
            float num = (float)base.currentFigureHP / base.GetAttributes().GetFinal(TAG.HIT_POINTS).ToFloat();
            Subrace subrace = base.dbSource.Get();
            if (subrace is global::DBDef.Unit)
            {
                global::DBDef.Unit unit = subrace as global::DBDef.Unit;
                float num2 = 1f / (float)unit.figures;
                return ((float)(base.figureCount - 1) + num) * num2;
            }
            return num;
        }

        public override SkillManager GetSkillManager()
        {
            if (base.skillManager == null)
            {
                base.skillManager = new SkillManager(this);
            }
            return base.skillManager;
        }

        public void Destroy()
        {
            if (this.group != null)
            {
                if (base.dbSource.Get() is Hero && this.GetWizardOwner() != null && this.GetWizardOwner().ID != 0)
                {
                    base.artefactManager.ReturnEquipment(this.GetWizardOwner());
                    this.GetWizardOwner().heroes.Remove(this);
                    this.GetWizardOwner().AddToDeadHeroesList(this);
                }
                this.group.Get().RemoveUnit(this);
                List<EnchantmentInstance> enchantments = this.GetEnchantments();
                if (enchantments != null && enchantments.Count > 0)
                {
                    for (int num = enchantments.Count - 1; num >= 0; num--)
                    {
                        this.RemoveEnchantment(enchantments[num]);
                    }
                }
                MHEventSystem.TriggerEvent<Unit>(this, "Destroy");
            }
            else if (this.everAssignedToGroup)
            {
                Debug.LogError("Destroying a unit after detaching it from group may lead to data corruption " + this.GetDBName());
            }
            base.enchantmentManager?.Destroy();
            this.group = null;
            this.UnregisterEntity();
        }

        public void DestroyNoGroup()
        {
            if (this.heroEverAssignedToWizard > 0)
            {
                PlayerWizard wizard = GameManager.GetWizard(this.heroEverAssignedToWizard);
                if (wizard != null)
                {
                    base.artefactManager.ReturnEquipment(wizard);
                    wizard.heroes.Remove(this);
                    wizard.AddToDeadHeroesList(this);
                }
            }
            if (this.group != null)
            {
                this.group.Get().RemoveUnit(this);
            }
            List<EnchantmentInstance> enchantments = this.GetEnchantments();
            if (enchantments != null && enchantments.Count > 0)
            {
                for (int num = enchantments.Count - 1; num >= 0; num--)
                {
                    this.RemoveEnchantment(enchantments[num]);
                }
            }
            base.enchantmentManager?.Destroy();
            this.group = null;
            this.UnregisterEntity();
        }

        public void Destroy(bool dismissed = false)
        {
            if (this.group != null)
            {
                if (base.dbSource.Get() is Hero && this.GetWizardOwner() != null && this.GetWizardOwner().ID != 0)
                {
                    base.artefactManager.ReturnEquipment(this.GetWizardOwner());
                    this.GetWizardOwner().heroes.Remove(this);
                    if (!dismissed)
                    {
                        this.GetWizardOwner().AddToDeadHeroesList(this);
                    }
                }
                this.group.Get().RemoveUnit(this);
                List<EnchantmentInstance> enchantments = this.GetEnchantments();
                if (enchantments != null && enchantments.Count > 0)
                {
                    for (int num = enchantments.Count - 1; num >= 0; num--)
                    {
                        this.RemoveEnchantment(enchantments[num]);
                    }
                }
                MHEventSystem.TriggerEvent<Unit>(this, "Destroy");
            }
            else if (this.everAssignedToGroup)
            {
                Debug.LogError("Destroying a unit after detaching it from group may lead to data corruption");
            }
            base.enchantmentManager?.Destroy();
            this.group = null;
            this.UnregisterEntity();
        }

        public override EnchantmentManager GetEnchantmentManager()
        {
            if (base.enchantmentManager == null)
            {
                base.enchantmentManager = new EnchantmentManager(this);
            }
            return base.enchantmentManager;
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
            if (this.group == null)
            {
                return null;
            }
            return GameManager.GetWizard(this.group.Get().GetOwnerID());
        }

        public override int GetTotalCastingSkill()
        {
            return -1;
        }

        public override int GetMana()
        {
            return base.GetAttributes().GetFinal(TAG.MANA_POINTS).ToInt();
        }

        public override void SetMana(int m)
        {
        }

        public override void AttributesChanged()
        {
            if (this.group != null)
            {
                this.group.Get().maxMPCache = -1;
            }
        }

        protected override void FigureCountChanged(int prevCount, int newCount)
        {
            if (!(this.group != null))
            {
                return;
            }
            this.group.Get().valueCache = 0;
            if (this.group.Get().GetPlane() != null)
            {
                Formation mapFormation = this.group.Get().GetMapFormation(createIfMissing: false);
                if (!(mapFormation == null) && this.group.Get().IsModelVisible() && this.FigureCount() != mapFormation.GetCharacterActors().Count)
                {
                    mapFormation.UpdateFigureCount();
                }
            }
        }

        protected override void FigureHealthChanged()
        {
            if (this.group != null)
            {
                this.group.Get().valueCache = 0;
            }
        }

        public void ApplyImmolationDamage(int[] damage, MHRandom random, int defenceModifier, Spell spell = null, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null)
        {
            if (base.figureCount < 1 || damage == null || damage.Length < 1)
            {
                return;
            }
            int num = 0;
            int num2 = damage[num];
            MHEventSystem.TriggerEvent<Unit>(this, new UnitEvent(damage));
            BattleFigure battleFigure = new BattleFigure(this, base.attributes);
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
                    int defence = battleFigure.defence;
                    defence = ((!replaceDefenceWithModifier) ? (defence + defenceModifier) : defenceModifier);
                    int num3 = random.GetSuccesses(battleFigure.defenceChance, defence);
                    if (base.invulnerabilityProtection > 0)
                    {
                        num3 += base.invulnerabilityProtection;
                    }
                    sb?.AppendLine(header + " defense " + num3 + " for damage " + num2 + " results in " + (num2 - num3));
                    num2 -= num3;
                    if (num2 > 0)
                    {
                        base.currentFigureHP -= num2;
                        num2 = 0;
                        sb?.AppendLine(header + " figure " + base.figureCount + " lost hp, " + base.currentFigureHP + " hp left");
                    }
                }
                base.figureCount--;
                base.currentFigureHP = battleFigure.maxHitPoints;
                sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
            }
            if (base.figureCount == 0)
            {
                this.Destroy();
            }
        }

        public void ApplyResistFigureDeath(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null)
        {
            int num = this.FigureCount();
            if (num < 1)
            {
                return;
            }
            BattleFigure battleFigure = new BattleFigure(this, base.attributes);
            float num2 = (float)(battleFigure.resist + resistIncrease - resistReduction) * 0.1f;
            if ((double)num2 >= 1.0)
            {
                sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
                return;
            }
            for (int i = 0; i < num; i++)
            {
                if (random.GetSuccesses(num2, 1) == 0)
                {
                    base.figureCount--;
                }
            }
            if (num != base.figureCount && base.figureCount != 0)
            {
                base.currentFigureHP = battleFigure.maxHitPoints;
            }
            if (base.figureCount == 0)
            {
                this.Destroy();
            }
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
        }

        public void ApplyResistUnitDeath(MHRandom random, int resistReduction, int resistIncrease = 0, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null)
        {
            if (this.FigureCount() < 1)
            {
                return;
            }
            float num = (float)(new BattleFigure(this, base.attributes).resist + resistIncrease - resistReduction) * 0.1f;
            if ((double)num >= 1.0)
            {
                sb?.AppendLine(header + " do not lost any figure.");
                return;
            }
            if (random.GetSuccesses(num, 1) == 0)
            {
                this.Destroy();
            }
            sb?.AppendLine(header + " killed.");
        }

        public void ApplyDoomDmg(int dam, BattleAttack battleAttack, StringBuilder sb = null, string header = null)
        {
            if (base.figureCount < 1 || dam < 0)
            {
                return;
            }
            MHEventSystem.TriggerEvent<Unit>(this, new UnitEvent(dam));
            BattleFigure battleFigure = new BattleFigure(this, base.attributes);
            while (base.currentFigureHP >= 0)
            {
                int num = base.currentFigureHP;
                base.currentFigureHP -= dam;
                dam -= num;
                if (base.currentFigureHP <= 0)
                {
                    base.figureCount--;
                    base.currentFigureHP = battleFigure.maxHitPoints;
                }
                if (base.figureCount == 0)
                {
                    this.Destroy();
                    break;
                }
                if (dam <= 0)
                {
                    break;
                }
            }
            sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
        }

        public void ApplyDamage(int[] damage, MHRandom random, BattleAttack battleAttack, int defenceModifier, bool replaceDefenceWithModifier = false, StringBuilder sb = null, string header = null, DBClass data = null)
        {
            if (base.figureCount < 1 || damage == null || damage.Length < 1)
            {
                return;
            }
            int num = 0;
            int num2 = damage[num];
            MHEventSystem.TriggerEvent<Unit>(this, new UnitEvent(damage));
            BattleFigure battleFigure = new BattleFigure(this, base.attributes);
            float chance = battleFigure.defenceChance;
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
                    int defence = battleFigure.defence;
                    defence = ((!replaceDefenceWithModifier) ? (defence + defenceModifier) : defenceModifier);
                    if (battleAttack != null)
                    {
                        BattleUnit source = battleAttack.source;
                        if (source.targetDefMod != 0f)
                        {
                            chance = battleFigure.defenceChance;
                            chance = Mathf.Clamp01(chance - source.targetDefMod);
                        }
                    }
                    this.UpdateDefenceByAttackType(ref defence, battleAttack, data);
                    int num3 = random.GetSuccesses(chance, defence);
                    if (base.invulnerabilityProtection > 0)
                    {
                        num3 += base.invulnerabilityProtection;
                    }
                    sb?.AppendLine(header + " defense " + num3 + " for damage " + num2 + " results in " + (num2 - num3));
                    num2 -= num3;
                    if (num2 > 0)
                    {
                        int num4 = base.currentFigureHP;
                        base.currentFigureHP -= num2;
                        num2 -= num4;
                        sb?.AppendLine(header + " figure " + base.figureCount + " lost hp, " + base.currentFigureHP + " hp left");
                    }
                }
                base.figureCount--;
                base.currentFigureHP = battleFigure.maxHitPoints;
                sb?.AppendLine(header + " lost figure, " + base.figureCount + "left");
            }
            if (base.figureCount == 0)
            {
                this.Destroy();
            }
        }

        private void UpdateDefenceByAttackType(ref int defTryCount, BattleAttack battleAttack, DBClass data)
        {
            if (battleAttack != null)
            {
                if (battleAttack.isPiercing)
                {
                    defTryCount /= 2;
                }
                if (battleAttack.isIllusion && !base.GetAttributes().Contains(TAG.ILLUSIONS_IMMUNITY))
                {
                    defTryCount = 0;
                }
            }
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
        }

        public bool IsSettler()
        {
            TAG t = TAG.SETTLER_UNIT;
            return base.GetAttributes().GetFinal(t) > 0;
        }

        public bool IsEngineer()
        {
            TAG t = TAG.ENGINEER_UNIT;
            return base.GetAttributes().GetFinal(t) > 0;
        }

        public bool IsPurifier()
        {
            TAG t = TAG.PURIFER_UNIT;
            return base.GetAttributes().GetFinal(t) > 0;
        }

        public bool IsMelder()
        {
            TAG t = TAG.MELDER_UNIT;
            return base.GetAttributes().GetFinal(t) > 0;
        }

        public bool IsAdvMelder()
        {
            TAG t = TAG.MELDER_UNIT;
            if (base.GetAttributes().GetFinal(t) > 0)
            {
                return this.GetSkillManager().GetSkills().Contains((Skill)SKILL.MELD_ADVANCED);
            }
            return false;
        }

        public int GetModifiedWorldUnitValue(TAG tag, FInt value)
        {
            return (int)ScriptLibrary.Call("GetModifiedWorldUnitValue", this, (Tag)tag, value);
        }

        public int GetWorldUnitValue()
        {
            return (int)ScriptLibrary.Call("GetWorldUnitValue", this);
        }

        public static bool HeroInUse(Subrace s)
        {
            if (s == null)
            {
                return false;
            }
            foreach (PlayerWizard wizard in GameManager.GetWizards())
            {
                if (wizard.heroes != null)
                {
                    foreach (Reference<Unit> hero in wizard.heroes)
                    {
                        if (hero.Get().dbSource == s)
                        {
                            return true;
                        }
                    }
                }
                if (wizard.deadHeroes == null)
                {
                    continue;
                }
                foreach (DeadHero deadHero in wizard.GetDeadHeroes())
                {
                    if (deadHero.dbSource == s)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HeroInUseByWizard(Subrace s, int wizardId)
        {
            if (s == null)
            {
                return false;
            }
            if (GameManager.GetWizard(wizardId).heroes != null)
            {
                foreach (Reference<Unit> hero in GameManager.GetWizard(wizardId).heroes)
                {
                    if (hero != null && hero.Get().dbSource == s)
                    {
                        return true;
                    }
                }
            }
            List<DeadHero> deadHeroes = GameManager.GetWizard(wizardId).GetDeadHeroes();
            if (deadHeroes != null)
            {
                foreach (DeadHero item in deadHeroes)
                {
                    if (item != null && item.dbSource == s)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public FInt GetUpkeepChannelerManaDiscount()
        {
            if (base.GetAttributes().GetFinal(TAG.FANTASTIC_CLASS) == 0)
            {
                return FInt.ZERO;
            }
            if (this.group == null)
            {
                return FInt.ZERO;
            }
            if (this.group.Get().GetOwnerID() < PlayerWizard.HumanID())
            {
                return FInt.ZERO;
            }
            FInt channelerFantasticUnitsUpkeepDiscount = GameManager.GetWizard(this.group.Get().GetOwnerID()).channelerFantasticUnitsUpkeepDiscount;
            return base.GetManaUpkeep() * channelerFantasticUnitsUpkeepDiscount;
        }

        public FInt GetUpkeepConjuerManaDiscount()
        {
            if (base.GetAttributes().GetFinal(TAG.FANTASTIC_CLASS) == 0)
            {
                return FInt.ZERO;
            }
            if (this.group == null)
            {
                return FInt.ZERO;
            }
            if (this.group.Get().GetOwnerID() < PlayerWizard.HumanID())
            {
                return FInt.ZERO;
            }
            FInt conjuerFantasticUnitsUpkeepDiscount = GameManager.GetWizard(this.group.Get().GetOwnerID()).conjuerFantasticUnitsUpkeepDiscount;
            return base.GetManaUpkeep() * conjuerFantasticUnitsUpkeepDiscount;
        }

        public FInt GetUpkeepNatureSummonerManaDiscount()
        {
            if (base.GetAttributes().GetFinal(TAG.FANTASTIC_CLASS) == 0)
            {
                return FInt.ZERO;
            }
            if (base.race.Get() != (Race)RACE.REALM_NATURE)
            {
                return FInt.ZERO;
            }
            if (this.group == null)
            {
                return FInt.ZERO;
            }
            if (this.group.Get().GetOwnerID() < PlayerWizard.HumanID())
            {
                return FInt.ZERO;
            }
            FInt fantasticNatureUnitsUpkeepDiscount = GameManager.GetWizard(this.group.Get().GetOwnerID()).fantasticNatureUnitsUpkeepDiscount;
            return base.GetManaUpkeep() * fantasticNatureUnitsUpkeepDiscount;
        }

        public bool IsHero()
        {
            return base.dbSource.Get() is Hero;
        }

        public bool CanTravelOverLand()
        {
            if (!(base.GetAttributes().GetFinal(TAG.CAN_WALK) > 0))
            {
                return this.CanFly();
            }
            return true;
        }

        public bool CanTravelOverWater()
        {
            if (!(base.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0))
            {
                return this.CanFly();
            }
            return true;
        }

        public bool CanFly()
        {
            if (base.GetAttributes().GetFinal(TAG.NON_CORPOREAL) > 0 || base.GetAttributes().GetFinal(TAG.CAN_FLY) > 0 || base.GetAttributes().GetFinal(TAG.WIND_WALKING) > 0)
            {
                return true;
            }
            return false;
        }

        public override int GetWizardOwnerID()
        {
            if (this.group == null)
            {
                return 0;
            }
            return this.group.Get().GetOwnerID();
        }

        public int GetTotalHealth()
        {
            return base._totalHP;
        }

        protected override void CalculateTotalHealth()
        {
            int num = base.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
            int totalHP = (base.figureCount - 1) * num + base.currentFigureHP;
            base._totalHP = totalHP;
        }
    }
}
