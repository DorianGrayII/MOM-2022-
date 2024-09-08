using System.Collections.Generic;
using System.IO;
using System.Text;
using MHUtils;
using ProtoBuf;

namespace MOM
{
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
                if (base.defender == base.attacker)
                {
                    num2++;
                    continue;
                }
                base.attacker.attackingSide = true;
                base.defender.attackingSide = false;
                base.InitializeMelee();
                float num4 = this.UnitWar();
                if (num4 <= 0.4f)
                {
                    num3++;
                }
                else if (num4 <= 0.6f)
                {
                    num2++;
                }
                else
                {
                    num++;
                }
            }
            return num * 3 + num2;
        }

        public float UnitWar()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            float num4 = -1f;
            EnchantmentManager enchantmentManager = base.attacker.GetEnchantmentManager();
            int iteration = enchantmentManager.iteration;
            MemoryStream memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, enchantmentManager);
            enchantmentManager = base.defender.GetEnchantmentManager();
            int iteration2 = enchantmentManager.iteration;
            MemoryStream memoryStream2 = new MemoryStream();
            Serializer.Serialize(memoryStream2, enchantmentManager);
            List<BattleAttackStack> battleStacks = new List<BattleAttackStack>();
            for (int i = 0; i < this.iterations; i++)
            {
                int num5 = this.Singlebattle(battleStacks);
                enchantmentManager = base.attacker.GetEnchantmentManager();
                if (enchantmentManager.iteration != iteration)
                {
                    base.attacker.DeserializeEnchantmentManager(memoryStream);
                }
                enchantmentManager = base.defender.GetEnchantmentManager();
                if (enchantmentManager.iteration != iteration2)
                {
                    base.defender.DeserializeEnchantmentManager(memoryStream2);
                }
                switch (num5)
                {
                case -1:
                    num2++;
                    break;
                case 1:
                    num++;
                    break;
                default:
                    num3++;
                    break;
                }
                if (num > (num2 + num3 + 1) * 10)
                {
                    num4 = 1f;
                    break;
                }
                if (num2 > (num + num3 + 1) * 10)
                {
                    num4 = 0f;
                    break;
                }
                if (num3 > (num + num2 + 1) * 10)
                {
                    num4 = 0.5f;
                    break;
                }
            }
            memoryStream.Dispose();
            memoryStream2.Dispose();
            if (num4 > -1f)
            {
                return num4;
            }
            if (num + num2 == 0)
            {
                return 0.5f;
            }
            return (float)num / (float)(num + num2 + num3);
        }

        private int Singlebattle(List<BattleAttackStack> battleStacks)
        {
            base.attacker.ResetUnit();
            base.defender.ResetUnit();
            base.SetDistance(this.initialDistance);
            if (this.maxRound <= 0)
            {
                this.maxRound = int.MaxValue;
            }
            for (int num = this.maxRound; num >= 0; num--)
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

        private void ResolveTurn(BattleUnit active, BattleUnit oponent, List<BattleAttackStack> battleStacks)
        {
            int movementSpeed = active.GetCurentFigure().movementSpeed;
            int movementSpeed2 = oponent.GetCurentFigure().movementSpeed;
            int num = movementSpeed;
            while (num > 0)
            {
                Battle.AttackForm attackForm = base.AttackPossible(active, oponent);
                if (attackForm == Battle.AttackForm.eMelee && base.MeleeAttackPreference(active))
                {
                    int num2 = (movementSpeed + 1) / 2;
                    num -= num2;
                    BattleAttackStack orCreate = this.GetOrCreate(null, active, oponent, base.random, battleStacks);
                    base.Execute(orCreate);
                }
                else if (attackForm == Battle.AttackForm.eRanged)
                {
                    base.ChangeDistance(num - 1, movementSpeed2 + 1);
                    num = 0;
                    active.GetCurentFigure().rangedAmmo--;
                    BattleAttackStack orCreate2 = this.GetOrCreate(null, active, oponent, base.random, battleStacks);
                    base.Execute(orCreate2);
                }
                else if (HexCoordinates.HexDistance(active.GetPosition(), oponent.GetPosition()) == 1)
                {
                    if (base.attacker.GetCurentFigure().rangedAmmo == 0)
                    {
                        break;
                    }
                    if (num <= 1)
                    {
                        base.ChangeDistance(num, movementSpeed2 + 1);
                        break;
                    }
                    base.ChangeDistance(num - 1, movementSpeed2 + 1);
                    attackForm = base.AttackPossible(active, oponent);
                    if (attackForm == Battle.AttackForm.eRanged)
                    {
                        num = 0;
                        active.GetCurentFigure().rangedAmmo--;
                        BattleAttackStack orCreate3 = this.GetOrCreate(null, active, oponent, base.random, battleStacks);
                        base.Execute(orCreate3);
                    }
                }
                else
                {
                    num = base.ChangeDistance(-num, 1);
                }
            }
        }

        private BattleAttackStack GetOrCreate(Battle battle, BattleUnit active, BattleUnit oponent, MHRandom random, List<BattleAttackStack> battleStacks = null)
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
                    sb.Append(" F" + i + ":-;");
                    continue;
                }
                sb.Append(" F" + i + ":[<color=red>" + source[i] + "</color>];");
            }
            sb.AppendLine("---");
        }
    }
}
