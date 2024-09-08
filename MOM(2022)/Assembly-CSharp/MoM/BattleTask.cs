namespace MOM
{
    using MHUtils;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class BattleTask : BaseBattleTask
    {
        public List<BattleUnit> defenderSources;
        public int maxRound = 50;
        public int iterations = 30;
        public int initialDistance = 8;

        public override object Execute()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < this.defenderSources.Count; i++)
            {
                base.defender = this.defenderSources[i];
                if (ReferenceEquals(base.defender, base.attacker))
                {
                    num2++;
                }
                else
                {
                    base.attacker.attackingSide = true;
                    base.defender.attackingSide = false;
                    base.InitializeMelee();
                    float num5 = this.UnitWar();
                    if (num5 <= 0.4f)
                    {
                        num3++;
                    }
                    else if (num5 <= 0.6f)
                    {
                        num2++;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            return ((num * 3) + num2);
        }

        private BattleAttackStack GetOrCreate(Battle battle, BattleUnit active, BattleUnit oponent, MHRandom random, List<BattleAttackStack> battleStacks)
        {
            return new BattleAttackStack(battle, active, oponent, random);
        }

        private void LogAttack(StringBuilder sb, int[] source, string header)
        {
            sb.AppendLine(header + " rolls");
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] < 1)
                {
                    sb.Append(" F" + i.ToString() + ":-;");
                }
                else
                {
                    string[] textArray1 = new string[] { " F", i.ToString(), ":[<color=red>", source[i].ToString(), "</color>];" };
                    sb.Append(string.Concat(textArray1));
                }
            }
            sb.AppendLine("---");
        }

        private void ResolveTurn(BattleUnit active, BattleUnit oponent, List<BattleAttackStack> battleStacks)
        {
            int movementSpeed = active.GetCurentFigure().movementSpeed;
            int num2 = oponent.GetCurentFigure().movementSpeed;
            int mp = movementSpeed;
            while (true)
            {
                if (mp > 0)
                {
                    Battle.AttackForm form = base.AttackPossible(active, oponent);
                    if ((form == Battle.AttackForm.eMelee) && base.MeleeAttackPreference(active))
                    {
                        mp -= (movementSpeed + 1) / 2;
                        BattleAttackStack stack = this.GetOrCreate(null, active, oponent, base.random, battleStacks);
                        base.Execute(stack);
                        continue;
                    }
                    if (form == Battle.AttackForm.eRanged)
                    {
                        base.ChangeDistance(mp - 1, num2 + 1);
                        mp = 0;
                        BattleFigure curentFigure = active.GetCurentFigure();
                        curentFigure.rangedAmmo--;
                        BattleAttackStack stack = this.GetOrCreate(null, active, oponent, base.random, battleStacks);
                        base.Execute(stack);
                        continue;
                    }
                    if (HexCoordinates.HexDistance(active.GetPosition(), oponent.GetPosition()) != 1)
                    {
                        mp = base.ChangeDistance(-mp, 1);
                        continue;
                    }
                    if (base.attacker.GetCurentFigure().rangedAmmo != 0)
                    {
                        if (mp <= 1)
                        {
                            base.ChangeDistance(mp, num2 + 1);
                            return;
                        }
                        base.ChangeDistance(mp - 1, num2 + 1);
                        if (base.AttackPossible(active, oponent) != Battle.AttackForm.eRanged)
                        {
                            continue;
                        }
                        mp = 0;
                        BattleFigure curentFigure = active.GetCurentFigure();
                        curentFigure.rangedAmmo--;
                        BattleAttackStack stack = this.GetOrCreate(null, active, oponent, base.random, battleStacks);
                        base.Execute(stack);
                        continue;
                    }
                }
                return;
            }
        }

        private int Singlebattle(List<BattleAttackStack> battleStacks)
        {
            base.attacker.ResetUnit();
            base.defender.ResetUnit();
            base.SetDistance(this.initialDistance);
            if (this.maxRound <= 0)
            {
                this.maxRound = 0x7fffffff;
            }
            for (int i = this.maxRound; i >= 0; i--)
            {
                this.ResolveTurn(base.attacker, base.defender, battleStacks);
                if (!base.attacker.IsAlive() && !base.defender.IsAlive())
                {
                    return 0;
                }
                if (!base.attacker.IsAlive())
                {
                    return -1;
                }
                if (!base.defender.IsAlive())
                {
                    return 1;
                }
                this.ResolveTurn(base.defender, base.attacker, battleStacks);
                if (!base.attacker.IsAlive() && !base.defender.IsAlive())
                {
                    return 0;
                }
                if (!base.attacker.IsAlive())
                {
                    return -1;
                }
                if (!base.defender.IsAlive())
                {
                    return 1;
                }
            }
            return 0;
        }

        public float UnitWar()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            float num4 = -1f;
            EnchantmentManager enchantmentManager = base.attacker.GetEnchantmentManager();
            int iteration = enchantmentManager.iteration;
            MemoryStream destination = new MemoryStream();
            Serializer.Serialize<EnchantmentManager>(destination, enchantmentManager);
            enchantmentManager = base.defender.GetEnchantmentManager();
            int num6 = enchantmentManager.iteration;
            MemoryStream stream2 = new MemoryStream();
            Serializer.Serialize<EnchantmentManager>(stream2, enchantmentManager);
            List<BattleAttackStack> battleStacks = new List<BattleAttackStack>();
            int num7 = 0;
            while (true)
            {
                if (num7 < this.iterations)
                {
                    int num8 = this.Singlebattle(battleStacks);
                    if (base.attacker.GetEnchantmentManager().iteration != iteration)
                    {
                        base.attacker.DeserializeEnchantmentManager(destination);
                    }
                    enchantmentManager = base.defender.GetEnchantmentManager();
                    if (enchantmentManager.iteration != num6)
                    {
                        base.defender.DeserializeEnchantmentManager(stream2);
                    }
                    if (num8 == -1)
                    {
                        num2++;
                    }
                    else if (num8 == 1)
                    {
                        num++;
                    }
                    else
                    {
                        num3++;
                    }
                    if (num > (((num2 + num3) + 1) * 10))
                    {
                        num4 = 1f;
                    }
                    else if (num2 > (((num + num3) + 1) * 10))
                    {
                        num4 = 0f;
                    }
                    else
                    {
                        if (num3 <= (((num + num2) + 1) * 10))
                        {
                            num7++;
                            continue;
                        }
                        num4 = 0.5f;
                    }
                }
                destination.Dispose();
                stream2.Dispose();
                return ((num4 <= -1f) ? (((num + num2) != 0) ? (((float) num) / ((float) ((num + num2) + num3))) : 0.5f) : num4);
            }
        }
    }
}

