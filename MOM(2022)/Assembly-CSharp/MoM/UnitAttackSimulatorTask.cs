namespace MOM
{
    using MHUtils;
    using ProtoBuf;
    using System;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class UnitAttackSimulatorTask : BaseBattleTask
    {
        public MemoryStream aMStream;
        public MemoryStream dMStream;
        public bool meleeFight;
        public int iterations;
        public Battle battle;
        public bool DEBUG_LOG;

        public override object Execute()
        {
            this.aMStream.Position = 0L;
            this.dMStream.Position = 0L;
            base.attacker = Serializer.Deserialize<BattleUnit>(this.aMStream);
            base.defender = Serializer.Deserialize<BattleUnit>(this.dMStream);
            base.attacker.simulated = true;
            base.defender.simulated = true;
            this.SetDistance(this.meleeFight ? 1 : 2);
            Battle.AttackForm form = Battle.AttackFormPossible(base.attacker, base.defender, -1);
            if (this.meleeFight)
            {
                if ((base.attacker.GetCurentFigure().attack <= 0) || (form != Battle.AttackForm.eMelee))
                {
                    return null;
                }
            }
            else if ((base.attacker.GetCurentFigure().rangedAttack <= 0) || (form != Battle.AttackForm.eRanged))
            {
                return null;
            }
            int battleUnitValue = base.attacker.GetBattleUnitValue();
            int num2 = base.defender.GetBattleUnitValue();
            int num3 = 0;
            int num4 = 0;
            StringBuilder message = null;
            for (int i = 0; i < this.iterations; i++)
            {
                BattleAttackStack stack = new BattleAttackStack(this.battle, base.attacker, base.defender, base.random);
                base.Execute(stack);
                int num6 = base.attacker.GetBattleUnitValue();
                int num7 = base.defender.GetBattleUnitValue();
                num3 += num6 - battleUnitValue;
                num4 += num7 - num2;
                if (this.DEBUG_LOG)
                {
                    if (message == null)
                    {
                        message = new StringBuilder();
                        object[] args = new object[] { base.attacker.dbSource.dbName, base.defender.dbSource.dbName, battleUnitValue, num2 };
                        message.AppendFormat("{0}({2}) vs {1}({3}) \n", args);
                    }
                    message.AppendFormat("Result {0} {1} vs {2} \n", i, num6, num7);
                }
                if (i < (this.iterations - 1))
                {
                    this.aMStream.Position = 0L;
                    this.dMStream.Position = 0L;
                    base.attacker = Serializer.Deserialize<BattleUnit>(this.aMStream);
                    base.defender = Serializer.Deserialize<BattleUnit>(this.dMStream);
                    base.attacker.simulated = true;
                    base.defender.simulated = true;
                    this.SetDistance(this.meleeFight ? 1 : 2);
                }
            }
            if (this.DEBUG_LOG)
            {
                Debug.Log(message);
            }
            return new Multitype<int, int>(num3 / this.iterations, num4 / this.iterations);
        }
    }
}

