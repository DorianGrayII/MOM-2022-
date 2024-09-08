using System.IO;
using System.Text;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
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
            base.SetDistance(this.meleeFight ? 1 : 2);
            Battle.AttackForm attackForm = Battle.AttackFormPossible(base.attacker, base.defender);
            if (this.meleeFight)
            {
                if (base.attacker.GetCurentFigure().attack <= 0 || attackForm != Battle.AttackForm.eMelee)
                {
                    return null;
                }
            }
            else if (base.attacker.GetCurentFigure().rangedAttack <= 0 || attackForm != Battle.AttackForm.eRanged)
            {
                return null;
            }
            int battleUnitValue = base.attacker.GetBattleUnitValue();
            int battleUnitValue2 = base.defender.GetBattleUnitValue();
            int num = 0;
            int num2 = 0;
            StringBuilder stringBuilder = null;
            for (int i = 0; i < this.iterations; i++)
            {
                BattleAttackStack stack = new BattleAttackStack(this.battle, base.attacker, base.defender, base.random);
                base.Execute(stack);
                int battleUnitValue3 = base.attacker.GetBattleUnitValue();
                int battleUnitValue4 = base.defender.GetBattleUnitValue();
                num += battleUnitValue3 - battleUnitValue;
                num2 += battleUnitValue4 - battleUnitValue2;
                if (this.DEBUG_LOG)
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("{0}({2}) vs {1}({3}) \n", base.attacker.dbSource.dbName, base.defender.dbSource.dbName, battleUnitValue, battleUnitValue2);
                    }
                    stringBuilder.AppendFormat("Result {0} {1} vs {2} \n", i, battleUnitValue3, battleUnitValue4);
                }
                if (i < this.iterations - 1)
                {
                    this.aMStream.Position = 0L;
                    this.dMStream.Position = 0L;
                    base.attacker = Serializer.Deserialize<BattleUnit>(this.aMStream);
                    base.defender = Serializer.Deserialize<BattleUnit>(this.dMStream);
                    base.attacker.simulated = true;
                    base.defender.simulated = true;
                    base.SetDistance(this.meleeFight ? 1 : 2);
                }
            }
            if (this.DEBUG_LOG)
            {
                Debug.Log(stringBuilder);
            }
            return new Multitype<int, int>(num / this.iterations, num2 / this.iterations);
        }
    }
}
