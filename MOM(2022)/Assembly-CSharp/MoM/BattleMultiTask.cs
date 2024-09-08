using System.Collections.Generic;
using System.Text;
using DBDef;
using DBEnum;
using MHUtils;
using UnityEngine;

namespace MOM
{
    public class BattleMultiTask : ITask
    {
        public List<BattleUnit> attackers;

        public List<BattleUnit> defenders;

        public BattlePlayer aPlayer;

        public BattlePlayer dPlayer;

        public Battle forReadOnly_Battle;

        public MHRandom random;

        public Dictionary<int, bool> canMeleeAttack;

        private List<BattleAttackStack> battleStacks = new List<BattleAttackStack>();

        public int maxRound = 50;

        public int initialDistance = 8;

        public bool aUseMana;

        public bool dUseMana;

        private int GetComboID(BattleUnit a, BattleUnit b)
        {
            return a.ID | (b.ID << 16);
        }

        public object Execute()
        {
            this.InitializeMelee();
            int num = this.BattleResultAchieved();
            if (num != 0)
            {
                return num;
            }
            for (int i = 0; i < this.maxRound; i++)
            {
                bool rangedOnly = i < 2;
                this.TriggerEventForSide(this.defenders, EEnchantmentType.BattleTurnStartEffect, ESkillType.BattleTurnStartEffect);
                this.TriggerEventForSide(this.attackers, EEnchantmentType.BattleTurnStartEffect, ESkillType.BattleTurnStartEffect);
                num = this.BattleResultAchieved();
                if (num != 0)
                {
                    return num;
                }
                this.AttemptUsingWizardTower();
                this.AttemptCasting(attacker: false);
                num = this.BattleResultAchieved();
                if (num != 0)
                {
                    return num;
                }
                foreach (BattleUnit defender in this.defenders)
                {
                    if (defender.IsAlive())
                    {
                        this.UnitTurn(defender, this.attackers, rangedOnly);
                    }
                }
                for (int num2 = this.defenders.Count - 1; num2 >= 0; num2--)
                {
                    if (this.defenders[num2].IsAlive())
                    {
                        this.defenders[num2].BattleCountdownUpdate(isAttackerTurn: false);
                    }
                }
                num = this.BattleResultAchieved();
                if (num != 0)
                {
                    return num;
                }
                this.AttemptCasting(attacker: true);
                num = this.BattleResultAchieved();
                if (num != 0)
                {
                    return num;
                }
                foreach (BattleUnit attacker in this.attackers)
                {
                    if (attacker.IsAlive())
                    {
                        this.UnitTurn(attacker, this.defenders, rangedOnly);
                    }
                }
                for (int num3 = this.attackers.Count - 1; num3 >= 0; num3--)
                {
                    if (this.attackers[num3].IsAlive())
                    {
                        this.attackers[num3].BattleCountdownUpdate(isAttackerTurn: true);
                    }
                }
                num = this.BattleResultAchieved();
                if (num != 0)
                {
                    return num;
                }
                this.TriggerEventForSide(this.defenders, EEnchantmentType.BattleTurnEndEffect, ESkillType.BattleTurnEndEffect);
                this.TriggerEventForSide(this.attackers, EEnchantmentType.BattleTurnEndEffect, ESkillType.BattleTurnEndEffect);
            }
            return 0;
        }

        private void TriggerEventForSide(List<BattleUnit> units, EEnchantmentType eType, ESkillType sType)
        {
            foreach (BattleUnit unit in units)
            {
                unit.TriggerScripts(eType);
                unit.TriggerSkillScripts(sType);
            }
        }

        private void AttemptUsingWizardTower()
        {
            EnchantmentManager enchantmentManager = this.forReadOnly_Battle.enchantmentManager;
            if (enchantmentManager == null || this.dPlayer.wizard == null)
            {
                return;
            }
            List<EnchantmentInstance> enchantmentsOfType = enchantmentManager.GetEnchantmentsOfType(EEnchantmentType.BattleWizardTowerEffect);
            if (enchantmentsOfType == null)
            {
                return;
            }
            foreach (EnchantmentInstance item in enchantmentsOfType)
            {
                List<BattleUnit> list = this.attackers.FindAll((BattleUnit o) => o.IsAlive());
                if (list.Count == 0)
                {
                    break;
                }
                BattleUnit battleUnit = list[this.random.GetInt(0, list.Count)];
                if (item.source.Get().scripts != null)
                {
                    EnchantmentScript[] scripts = item.source.Get().scripts;
                    foreach (EnchantmentScript enchantmentScript in scripts)
                    {
                        ScriptLibrary.Call(enchantmentScript.script, this.dPlayer, enchantmentScript, item, battleUnit);
                    }
                }
            }
        }

        private int BattleResultAchieved()
        {
            bool flag = false;
            foreach (BattleUnit attacker in this.attackers)
            {
                if (attacker.IsAlive())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return -1;
            }
            bool flag2 = false;
            foreach (BattleUnit defender in this.defenders)
            {
                if (defender.IsAlive())
                {
                    flag2 = true;
                    break;
                }
            }
            if (!flag2)
            {
                return 1;
            }
            return 0;
        }

        private int ManaToUse(bool attacker)
        {
            int num = 0;
            List<BattleUnit> list = null;
            List<BattleUnit> list2 = null;
            if (attacker)
            {
                num = ((this.aPlayer != null) ? this.aPlayer.mana : 0);
                list = this.attackers;
                list2 = this.defenders;
            }
            else
            {
                num = ((this.dPlayer != null) ? this.dPlayer.mana : 0);
                list = this.defenders;
                list2 = this.attackers;
            }
            if (num > 0)
            {
                int ownValue = 0;
                int otherValue = 0;
                list.ForEach(delegate(BattleUnit o)
                {
                    ownValue += o.GetBattleUnitValue();
                });
                list2.ForEach(delegate(BattleUnit o)
                {
                    otherValue += o.GetBattleUnitValue();
                });
                if (ownValue > otherValue * 10)
                {
                    return (int)((float)num * 0.05f);
                }
                if (ownValue > otherValue * 5)
                {
                    return (int)((float)num * 0.25f);
                }
                if ((float)ownValue > (float)otherValue * 1.5f)
                {
                    return (int)((float)num * 0.5f);
                }
                return num;
            }
            return num;
        }

        private int SkillToUse(bool attacker)
        {
            if (attacker)
            {
                if (this.aPlayer?.wizard != null && this.aPlayer.wizard.banishedTurn == 0)
                {
                    return this.aPlayer.castingSkill;
                }
            }
            else if (this.dPlayer?.wizard != null && this.dPlayer.wizard.banishedTurn == 0)
            {
                return this.dPlayer.castingSkill;
            }
            return 0;
        }

        private void AttemptCasting(bool attacker)
        {
            if ((attacker && !this.aUseMana) || (!attacker && !this.dUseMana))
            {
                return;
            }
            int num = this.SkillToUse(attacker);
            if (num == 0)
            {
                return;
            }
            int num2 = this.ManaToUse(attacker);
            if (num2 < 1)
            {
                return;
            }
            BattlePlayer battlePlayer = (attacker ? this.aPlayer : this.dPlayer);
            if (battlePlayer == null || battlePlayer.GetWizardOwner() == null)
            {
                return;
            }
            PlayerWizard wizardOwner = battlePlayer.GetWizardOwner();
            List<DBReference<Spell>> spells = wizardOwner.GetSpellManager().GetSpells();
            int num3 = 0;
            Spell spell = null;
            object obj = null;
            List<BattleUnit> list = (attacker ? this.attackers : this.defenders);
            List<BattleUnit> list2 = (attacker ? this.defenders : this.attackers);
            StringBuilder stringBuilder = new StringBuilder();
            SpellCastData spellCastData = new SpellCastData(wizardOwner, list, list2);
            foreach (DBReference<Spell> item in spells)
            {
                Spell spell2 = item.Get();
                int battleCastingCost = spell2.GetBattleCastingCost(wizardOwner);
                int battleCastingCostByDistance = spell2.GetBattleCastingCostByDistance(wizardOwner, includeExtraManaCost: true);
                if (battleCastingCostByDistance <= 0 || num2 < battleCastingCostByDistance || num < battleCastingCost || !(bool)ScriptLibrary.Call("IsSpellValidForAutoresolves", spell2))
                {
                    continue;
                }
                if (spell2.targetType.enumType == ETargetType.TargetUnit)
                {
                    List<BattleUnit> list3 = ((spell2.targetType != (TargetType)TARGET_TYPE.UNIT_ENEMY) ? list : list2);
                    foreach (BattleUnit item2 in list3)
                    {
                        int num4 = 0;
                        if (string.IsNullOrEmpty(spell2.targetingScript) || (bool)ScriptLibrary.Call(spell2.targetingScript, spellCastData, item2, spell2))
                        {
                            num4 = (string.IsNullOrEmpty(spell2.aiBattleEvaluationScript) ? 10 : ((int)ScriptLibrary.Call(spell2.aiBattleEvaluationScript, spellCastData, item2, spell2)));
                            num4 = spell2.GetSpellTacticalValue(battleCastingCostByDistance, num4);
                            if (num4 > num3)
                            {
                                num3 = num4;
                                spell = spell2;
                                obj = item2;
                            }
                        }
                    }
                }
                else if (spell2.summonFantasticUnitSpell)
                {
                    int num5 = 0;
                    if (!string.IsNullOrEmpty(spell2.targetingScript) && !(bool)ScriptLibrary.Call(spell2.targetingScript, spellCastData, Vector3i.invalid, spell2))
                    {
                        continue;
                    }
                    num5 = (string.IsNullOrEmpty(spell2.aiBattleEvaluationScript) ? 10 : ((int)ScriptLibrary.Call(spell2.aiBattleEvaluationScript, spellCastData, Vector3i.invalid, spell2)));
                    num5 = spell2.GetSpellTacticalValue(battleCastingCostByDistance, num5);
                    if (num5 <= num3)
                    {
                        continue;
                    }
                    num3 = num5;
                    spell = spell2;
                    obj = Vector3i.invalid;
                }
                if (spell == item.Get())
                {
                    stringBuilder.AppendLine(item.Get().dbName + ": gain " + num3);
                }
            }
            if (num3 > 0 && (bool)ScriptLibrary.Call(spell.battleScript, spellCastData, obj, spell))
            {
                battlePlayer.mana -= spell.GetBattleCastingCostByDistance(wizardOwner);
                battlePlayer.castingSkill -= spell.GetBattleCastingCost(wizardOwner);
            }
        }

        private bool UnitTurn(BattleUnit a, List<BattleUnit> opponents, bool rangedOnly)
        {
            if (a.FigureCount() < 1)
            {
                return false;
            }
            int @int = this.random.GetInt(0, opponents.Count);
            if (rangedOnly)
            {
                if (a.GetCurentFigure().rangedAmmo <= 0)
                {
                    return true;
                }
                a.battlePosition = Vector3i.BuildHexCoord(2, 0);
            }
            else if (a.GetCurentFigure().rangedAmmo > 0 && !this.MeleeAttackPreference(a))
            {
                a.battlePosition = Vector3i.BuildHexCoord(2, 0);
            }
            else
            {
                a.battlePosition = Vector3i.BuildHexCoord(1, 0);
            }
            BattleUnit battleUnit = null;
            int count = opponents.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + @int) % count;
                battleUnit = opponents[index];
                if (battleUnit.IsAlive())
                {
                    break;
                }
            }
            battleUnit.battlePosition = Vector3i.zero;
            this.ResolveTurn(a, battleUnit, this.battleStacks);
            return true;
        }

        private void ResolveTurn(BattleUnit active, BattleUnit oponent, List<BattleAttackStack> battleStacks)
        {
            int movementSpeed = active.GetCurentFigure().movementSpeed;
            int num = movementSpeed;
            while (num > 0)
            {
                Battle.AttackForm attackForm = this.AttackPossible(active, oponent);
                if (attackForm == Battle.AttackForm.eMelee && this.MeleeAttackPreference(active))
                {
                    int num2 = (movementSpeed + 1) / 2;
                    num -= num2;
                    BattleAttackStack orCreate = this.GetOrCreate(null, active, oponent, this.random, battleStacks);
                    this.Execute(orCreate);
                    continue;
                }
                if (attackForm == Battle.AttackForm.eRanged)
                {
                    num = 0;
                    active.GetCurentFigure().rangedAmmo--;
                    BattleAttackStack orCreate2 = this.GetOrCreate(null, active, oponent, this.random, battleStacks);
                    this.Execute(orCreate2);
                    continue;
                }
                int num3 = HexCoordinates.HexDistance(active.GetPosition(), oponent.GetPosition());
                if (num3 == 1)
                {
                    if (active.GetCurentFigure().rangedAmmo == 0)
                    {
                        break;
                    }
                    if (num <= 1)
                    {
                        num3 = Mathf.Clamp(num3 + num, 1, this.initialDistance);
                        active.battlePosition = Vector3i.BuildHexCoord(1, 0) * num3;
                        break;
                    }
                    num3 = Mathf.Clamp(num3 + num - 1, 1, this.initialDistance);
                    active.battlePosition = Vector3i.BuildHexCoord(1, 0) * num3;
                    attackForm = this.AttackPossible(active, oponent);
                    if (attackForm == Battle.AttackForm.eRanged)
                    {
                        num = 0;
                        active.GetCurentFigure().rangedAmmo--;
                        BattleAttackStack orCreate3 = this.GetOrCreate(null, active, oponent, this.random, battleStacks);
                        this.Execute(orCreate3);
                    }
                }
                else
                {
                    int num4 = num;
                    num -= num3 - 1;
                    num3 = Mathf.Max(num3 - num4, 1);
                    active.battlePosition = Vector3i.BuildHexCoord(1, 0) * num3;
                }
            }
        }

        private Battle.AttackForm AttackPossible(BattleUnit active, BattleUnit oponent)
        {
            if (HexCoordinates.HexDistance(active.GetPosition(), oponent.GetPosition()) == 1)
            {
                return this.CanMeleeAttack(active, oponent) ? Battle.AttackForm.eMelee : Battle.AttackForm.eNone;
            }
            return (active.GetCurentFigure().rangedAmmo > 0) ? Battle.AttackForm.eRanged : Battle.AttackForm.eNone;
        }

        private BattleAttackStack GetOrCreate(Battle battle, BattleUnit active, BattleUnit oponent, MHRandom random, List<BattleAttackStack> battleStacks = null)
        {
            return new BattleAttackStack(battle, active, oponent, random);
        }

        private void Execute(BattleAttackStack stack)
        {
            stack.ExecuteSkillStack();
        }

        private void InitializeMelee()
        {
            foreach (BattleUnit attacker in this.attackers)
            {
                foreach (BattleUnit defender in this.defenders)
                {
                    this.CanMeleeAttack(attacker, defender);
                    this.CanMeleeAttack(defender, attacker);
                }
            }
        }

        private bool CanMeleeAttack(BattleUnit a, BattleUnit d)
        {
            int comboID = this.GetComboID(a, d);
            if (this.canMeleeAttack == null)
            {
                this.canMeleeAttack = new Dictionary<int, bool>();
            }
            if (!this.canMeleeAttack.ContainsKey(comboID))
            {
                a.battlePosition = Vector3i.BuildHexCoord(1, 0);
                d.battlePosition = Vector3i.zero;
                Battle.AttackForm attackForm = Battle.AttackFormPossible(a, d);
                this.canMeleeAttack[comboID] = a.GetBaseFigure().attack > 0 && attackForm == Battle.AttackForm.eMelee;
            }
            return this.canMeleeAttack[comboID];
        }

        private bool MeleeAttackPreference(BattleUnit bu)
        {
            if (bu.GetCurentFigure().rangedAmmo != 0)
            {
                return bu.GetCurentFigure().attack > bu.GetCurentFigure().rangedAttack;
            }
            return true;
        }
    }
}
