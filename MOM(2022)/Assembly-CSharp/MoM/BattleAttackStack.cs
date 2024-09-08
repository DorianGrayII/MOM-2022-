using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using UnityEngine;

namespace MOM
{
    public class BattleAttackStack
    {
        public BattleUnit attacker;

        public BattleUnit defender;

        public Battle battle;

        public List<BattleAttack> attackQueue = new List<BattleAttack>();

        public MHRandom random;

        public bool ranged;

        private bool sorted;

        public static List<int[]> rollCache = new List<int[]>();

        private static int cacheIndex = 0;

        public static bool logBattle;

        public BattleAttackStack(Battle battle, BattleUnit attacker, BattleUnit defender, MHRandom random)
        {
            this.battle = battle;
            this.random = random;
            this.attacker = attacker;
            this.defender = defender;
            BattleAttackStack.cacheIndex = 0;
            this.ranged = HexCoordinates.HexDistance(attacker.GetPosition(), defender.GetPosition()) > 1;
            if (this.ranged)
            {
                if (attacker.canAttack)
                {
                    this.Trigger(attacker, ESkillType.BattleRangedAttack, defender);
                    this.Trigger(attacker, ESkillType.BattleAttackAddon, defender);
                    this.Trigger(attacker, ESkillType.BattleAttackAddon2, defender);
                }
            }
            else
            {
                if (attacker.canAttack)
                {
                    this.Trigger(attacker, ESkillType.BattleAttack, defender);
                }
                if (defender.canContrAttack)
                {
                    this.Trigger(defender, ESkillType.BattleAttack, attacker);
                }
                if (attacker.canAttack)
                {
                    this.Trigger(attacker, ESkillType.BattleAttackAddon, defender);
                }
                if (defender.canContrAttack)
                {
                    this.Trigger(defender, ESkillType.BattleAttackAddon, attacker);
                }
                if (attacker.canAttack)
                {
                    this.Trigger(attacker, ESkillType.BattleAttackAddon2, defender);
                }
                if (defender.canContrAttack)
                {
                    this.Trigger(defender, ESkillType.BattleAttackAddon2, attacker);
                }
                if (battle != null && battle.fireWall && !attacker.nonCorporealMovement && battle.StepThroughFirewall(attacker.GetPosition(), defender.GetPosition()))
                {
                    Skill skill = (Skill)SKILL.IMMOLATION_5_ADDON2;
                    if (skill.script == null || skill.script.Length == 0)
                    {
                        Debug.LogError(skill.dbName + " Immolation is missing script to apply effect from firewall!");
                    }
                    else if (battle.defenderUnits.Contains(defender))
                    {
                        this.CreateBattleAttack(skill.script[0], defender, attacker, skill);
                    }
                    else
                    {
                        this.CreateBattleAttack(skill.script[0], attacker, defender, skill);
                    }
                }
            }
            if (attacker.canAttack)
            {
                this.Trigger(attacker, ESkillType.BattleStackModifier, defender);
            }
            if (defender.canContrAttack)
            {
                this.Trigger(defender, ESkillType.BattleStackModifier, attacker);
            }
        }

        public void ExecuteSkillStack(bool populateHUD = false)
        {
            if (this.attackQueue == null)
            {
                return;
            }
            if (!this.sorted)
            {
                this.attackQueue.Sort((BattleAttack a, BattleAttack b) => a.initiative.CompareTo(b.initiative));
                this.sorted = true;
            }
            int num = -1;
            if (populateHUD)
            {
                BattleHUD.CombatLogStartStack(this);
            }
            for (int i = 0; i < this.attackQueue.Count; i++)
            {
                BattleAttack battleAttack = this.attackQueue[i];
                FInt initiative = battleAttack.initiative;
                if (num < i)
                {
                    for (int j = i; j < this.attackQueue.Count; j++)
                    {
                        BattleAttack battleAttack2 = this.attackQueue[j];
                        if (battleAttack2.initiative != initiative)
                        {
                            break;
                        }
                        battleAttack2.ProduceDamages(this.random);
                        num = j;
                    }
                }
                if (populateHUD)
                {
                    BattleHUD.CombatLogPreApplyDamages(battleAttack);
                }
                battleAttack.ApplyDamages(this.random);
                if (populateHUD)
                {
                    BattleHUD.CombatLogPostApplyDamages(battleAttack);
                }
            }
            if (populateHUD && Settings.GetData().GetBattleCameraFollow())
            {
                Vector3 vector = HexCoordinates.HexToWorld3D(this.attacker.GetPosition());
                Vector3 vector2 = HexCoordinates.HexToWorld3D(this.defender.GetPosition());
                CameraController.Target((vector + vector2) / 2f);
            }
        }

        private void CreateBattleAttack(SkillScript s, BattleUnit from, BattleUnit otherUnit, DBReference<Skill> v)
        {
            if (!string.IsNullOrEmpty(s.trigger))
            {
                ScriptLibrary.Call(s.trigger, from, otherUnit, v.Get(), s, this.battle, this);
            }
            else
            {
                BattleAttack battleAttack = new BattleAttack();
                battleAttack.dmg = this.GetRollCache();
                battleAttack.source = from;
                battleAttack.destination = otherUnit;
                battleAttack.initiative = FInt.ONE + from.initiativeModifier;
                battleAttack.attackStack = this;
                battleAttack.skill = v.Get();
                battleAttack.skillScript = s;
                battleAttack.ConsiderWalls(this.battle);
                this.attackQueue.Add(battleAttack);
            }
        }

        public void Trigger(BattleUnit from, ESkillType type, BattleUnit otherUnit)
        {
            if (from.GetSkills() == null)
            {
                return;
            }
            foreach (DBReference<Skill> skill in from.GetSkills())
            {
                if (skill.Get().script == null)
                {
                    continue;
                }
                SkillScript[] script = skill.Get().script;
                foreach (SkillScript skillScript in script)
                {
                    if (skillScript.triggerType == type)
                    {
                        this.CreateBattleAttack(skillScript, from, otherUnit, skill);
                        if (from.doubleShot && type == ESkillType.BattleRangedAttack)
                        {
                            this.CreateBattleAttack(skillScript, from, otherUnit, skill);
                        }
                        if (from.haste)
                        {
                            this.CreateBattleAttack(skillScript, from, otherUnit, skill);
                        }
                    }
                }
            }
        }

        public int[] GetRollCache()
        {
            if (BattleAttackStack.rollCache.Count <= BattleAttackStack.cacheIndex)
            {
                BattleAttackStack.rollCache.Add(new int[16]);
            }
            int[] array = BattleAttackStack.rollCache[BattleAttackStack.cacheIndex];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
            BattleAttackStack.cacheIndex++;
            return array;
        }

        public BattleUnit GetOtherUnit(ISkillable source)
        {
            if (this.attacker == source)
            {
                return this.defender;
            }
            return this.attacker;
        }

        public void Clear()
        {
            BattleAttackStack.cacheIndex = 0;
            if (this.attackQueue == null)
            {
                return;
            }
            foreach (BattleAttack item in this.attackQueue)
            {
                item.dmg = this.GetRollCache();
            }
        }
    }
}
