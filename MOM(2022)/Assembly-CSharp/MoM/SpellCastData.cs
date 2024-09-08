namespace MOM
{
    using DBDef;
    using DBEnum;
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpellCastData
    {
        public ISpellCaster caster;
        public List<BattleUnit> casterFriendly;
        public List<BattleUnit> casterEnemy;
        public Battle battle;
        public int friendlyValue;
        public int enemyValue;

        public SpellCastData(ISpellCaster caster, Battle source)
        {
            this.caster = caster;
            this.battle = source;
        }

        public SpellCastData(ISpellCaster caster, List<BattleUnit> own, List<BattleUnit> opposing)
        {
            this.caster = caster;
            this.casterFriendly = own;
            this.casterEnemy = opposing;
        }

        public BattleUnit CreateSummon(int owner, DBDef.Unit unit)
        {
            bool attackingSide = !(this.caster is BattleUnit) ? (((this.casterFriendly == null) || (this.casterFriendly.Count <= 0)) || this.casterFriendly[0].attackingSide) : (this.caster as BattleUnit).attackingSide;
            MOM.Unit source = MOM.Unit.CreateFrom(unit, false);
            BattleUnit unit2 = BattleUnit.Create(source, false, owner, attackingSide);
            unit2.battlePosition = Vector3i.zero;
            source.simulationUnit = true;
            unit2.simulated = true;
            if (unit2.GetWizardOwner() != null)
            {
                unit2.GetWizardOwner().ModifyUnitSkillsByTraits(unit2);
            }
            this.casterFriendly.Add(unit2);
            return unit2;
        }

        public void DebugUnitRefresh()
        {
            foreach (BattleUnit local1 in this.GetFriendlyUnits())
            {
                local1.figureCount = local1.maxCount;
                FInt attFinal = IAttributeableExtension.GetAttFinal(local1, TAG.HIT_POINTS);
                local1.currentFigureHP = attFinal.ToInt();
            }
            foreach (BattleUnit local2 in this.GetEnemyUnits())
            {
                local2.figureCount = local2.maxCount;
                local2.currentFigureHP = IAttributeableExtension.GetAttFinal(local2, TAG.HIT_POINTS).ToInt();
            }
        }

        private void EnsureFriendlyFoeInitialization()
        {
            if (this.casterFriendly == null)
            {
                if (this.caster is PlayerWizard)
                {
                    if (ReferenceEquals(this.battle.attacker.GetWizardOwner(), this.caster))
                    {
                        this.casterFriendly = this.battle.GetUnits(true);
                        this.casterEnemy = this.battle.GetUnits(false);
                    }
                    else
                    {
                        this.casterFriendly = this.battle.GetUnits(false);
                        this.casterEnemy = this.battle.GetUnits(true);
                    }
                }
                else if (!(this.caster is BattleUnit))
                {
                    Debug.LogError("Unsupported! battle cast data");
                }
                else
                {
                    BattleUnit caster = this.caster as BattleUnit;
                    if (ReferenceEquals(this.battle.attacker.GetWizardOwner(), caster.GetWizardOwner()))
                    {
                        this.casterFriendly = this.battle.GetUnits(true);
                        this.casterEnemy = this.battle.GetUnits(false);
                    }
                    else
                    {
                        this.casterFriendly = this.battle.GetUnits(false);
                        this.casterEnemy = this.battle.GetUnits(true);
                    }
                }
                this.friendlyValue = 0;
                this.enemyValue = 0;
            }
        }

        public BattleUnit GetCasterAsBattleUnit()
        {
            return (this.caster as BattleUnit);
        }

        public List<BattleUnit> GetEnemyUnits()
        {
            this.EnsureFriendlyFoeInitialization();
            return this.casterEnemy;
        }

        public int GetEnemyValue()
        {
            if (this.enemyValue == 0)
            {
                foreach (BattleUnit unit in this.GetEnemyUnits())
                {
                    this.enemyValue += unit.GetBattleUnitValue();
                }
            }
            return this.enemyValue;
        }

        public List<BattleUnit> GetFriendlyUnits()
        {
            this.EnsureFriendlyFoeInitialization();
            return this.casterFriendly;
        }

        public int GetFriendlyValue()
        {
            if (this.friendlyValue == 0)
            {
                foreach (BattleUnit unit in this.GetFriendlyUnits())
                {
                    this.friendlyValue += unit.GetBattleUnitValue();
                }
            }
            return this.friendlyValue;
        }

        public PlayerWizard GetPlayerWizard()
        {
            int wizardID = this.GetWizardID();
            return ((wizardID != 0) ? GameManager.GetWizard(wizardID) : null);
        }

        public int GetWizardID()
        {
            PlayerWizard wizardOwner = this.caster.GetWizardOwner();
            if (wizardOwner != null)
            {
                return wizardOwner.ID;
            }
            PlayerWizard local1 = wizardOwner;
            return 0;
        }

        public bool IsCasterAttackingSide()
        {
            return (this.battle.attacker.GetID() == this.GetWizardID());
        }
    }
}

