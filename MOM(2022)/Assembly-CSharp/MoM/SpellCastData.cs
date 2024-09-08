using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using UnityEngine;

namespace MOM
{
    public class SpellCastData
    {
        public ISpellCaster caster;

        public List<BattleUnit> casterFriendly;

        public List<BattleUnit> casterEnemy;

        public Battle battle;

        public int friendlyValue;

        public int enemyValue;

        public SpellCastData(ISpellCaster caster, List<BattleUnit> own, List<BattleUnit> opposing)
        {
            this.caster = caster;
            this.casterFriendly = own;
            this.casterEnemy = opposing;
        }

        public SpellCastData(ISpellCaster caster, Battle source)
        {
            this.caster = caster;
            this.battle = source;
        }

        private void EnsureFriendlyFoeInitialization()
        {
            if (this.casterFriendly != null)
            {
                return;
            }
            if (this.caster is PlayerWizard)
            {
                if (this.battle.attacker.GetWizardOwner() == this.caster)
                {
                    this.casterFriendly = this.battle.GetUnits(attacker: true);
                    this.casterEnemy = this.battle.GetUnits(attacker: false);
                }
                else
                {
                    this.casterFriendly = this.battle.GetUnits(attacker: false);
                    this.casterEnemy = this.battle.GetUnits(attacker: true);
                }
            }
            else if (this.caster is BattleUnit)
            {
                BattleUnit battleUnit = this.caster as BattleUnit;
                if (this.battle.attacker.GetWizardOwner() == battleUnit.GetWizardOwner())
                {
                    this.casterFriendly = this.battle.GetUnits(attacker: true);
                    this.casterEnemy = this.battle.GetUnits(attacker: false);
                }
                else
                {
                    this.casterFriendly = this.battle.GetUnits(attacker: false);
                    this.casterEnemy = this.battle.GetUnits(attacker: true);
                }
            }
            else
            {
                Debug.LogError("Unsupported! battle cast data");
            }
            this.friendlyValue = 0;
            this.enemyValue = 0;
        }

        public List<BattleUnit> GetFriendlyUnits()
        {
            this.EnsureFriendlyFoeInitialization();
            return this.casterFriendly;
        }

        public List<BattleUnit> GetEnemyUnits()
        {
            this.EnsureFriendlyFoeInitialization();
            return this.casterEnemy;
        }

        public int GetWizardID()
        {
            return this.caster.GetWizardOwner()?.ID ?? 0;
        }

        public PlayerWizard GetPlayerWizard()
        {
            int wizardID = this.GetWizardID();
            if (wizardID == 0)
            {
                return null;
            }
            return GameManager.GetWizard(wizardID);
        }

        public bool IsCasterAttackingSide()
        {
            return this.battle.attacker.GetID() == this.GetWizardID();
        }

        public BattleUnit GetCasterAsBattleUnit()
        {
            return this.caster as BattleUnit;
        }

        public void DebugUnitRefresh()
        {
            foreach (BattleUnit friendlyUnit in this.GetFriendlyUnits())
            {
                friendlyUnit.figureCount = friendlyUnit.maxCount;
                friendlyUnit.currentFigureHP = friendlyUnit.GetAttFinal(TAG.HIT_POINTS).ToInt();
            }
            foreach (BattleUnit enemyUnit in this.GetEnemyUnits())
            {
                enemyUnit.figureCount = enemyUnit.maxCount;
                enemyUnit.currentFigureHP = enemyUnit.GetAttFinal(TAG.HIT_POINTS).ToInt();
            }
        }

        public BattleUnit CreateSummon(int owner, global::DBDef.Unit unit)
        {
            bool attackingSide = ((this.caster is BattleUnit) ? (this.caster as BattleUnit).attackingSide : (this.casterFriendly == null || this.casterFriendly.Count <= 0 || this.casterFriendly[0].attackingSide));
            Unit unit2 = Unit.CreateFrom(unit);
            BattleUnit battleUnit = BattleUnit.Create(unit2, abstractMode: false, owner, attackingSide);
            battleUnit.battlePosition = Vector3i.zero;
            unit2.simulationUnit = true;
            battleUnit.simulated = true;
            if (battleUnit.GetWizardOwner() != null)
            {
                battleUnit.GetWizardOwner().ModifyUnitSkillsByTraits(battleUnit);
            }
            this.casterFriendly.Add(battleUnit);
            return battleUnit;
        }

        public int GetFriendlyValue()
        {
            if (this.friendlyValue == 0)
            {
                foreach (BattleUnit friendlyUnit in this.GetFriendlyUnits())
                {
                    this.friendlyValue += friendlyUnit.GetBattleUnitValue();
                }
            }
            return this.friendlyValue;
        }

        public int GetEnemyValue()
        {
            if (this.enemyValue == 0)
            {
                foreach (BattleUnit enemyUnit in this.GetEnemyUnits())
                {
                    this.enemyValue += enemyUnit.GetBattleUnitValue();
                }
            }
            return this.enemyValue;
        }
    }
}
