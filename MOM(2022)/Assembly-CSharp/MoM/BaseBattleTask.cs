using MHUtils;
using UnityEngine;

namespace MOM
{
    public abstract class BaseBattleTask : ITask
    {
        public BattleUnit attacker;

        public BattleUnit defender;

        public MHRandom random;

        private bool aCanAttackMelee;

        private bool dCanAttackMelee;

        private int[] aDamageBuffers;

        private int[] dDamageBuffers;

        public abstract object Execute();

        protected Battle.AttackForm AttackPossible(BattleUnit active, BattleUnit oponent)
        {
            if (HexCoordinates.HexDistance(active.GetPosition(), oponent.GetPosition()) == 1)
            {
                return this.CanAttackMelee(active) ? Battle.AttackForm.eMelee : Battle.AttackForm.eNone;
            }
            return (active.GetCurentFigure().rangedAmmo > 0) ? Battle.AttackForm.eRanged : Battle.AttackForm.eNone;
        }

        protected void Execute(BattleAttackStack stack)
        {
            stack.ExecuteSkillStack();
        }

        protected bool CanAttackMelee(BattleUnit source)
        {
            if (this.attacker == source)
            {
                return this.aCanAttackMelee;
            }
            return this.dCanAttackMelee;
        }

        protected void SetDistance(int dist)
        {
            this.attacker.battlePosition = Vector3i.BuildHexCoord(dist, 0);
            this.defender.battlePosition = Vector3i.zero;
        }

        protected int ChangeDistance(int mp, int target)
        {
            this.defender.battlePosition = Vector3i.zero;
            short x = this.attacker.battlePosition.x;
            if (x + mp < 1)
            {
                this.attacker.battlePosition = Vector3i.BuildHexCoord(1, 0);
                return Mathf.Abs(x + mp) + 1;
            }
            if (x != target && mp > 0)
            {
                int num = target - x;
                int num2 = Mathf.Abs(num);
                if (mp >= num2)
                {
                    this.attacker.battlePosition = Vector3i.BuildHexCoord(target, 0);
                    return mp - num2;
                }
                int num3 = ((num > 0) ? 1 : (-1));
                this.attacker.battlePosition = Vector3i.BuildHexCoord(num3 * mp + x, 0);
                return 0;
            }
            this.attacker.battlePosition = Vector3i.BuildHexCoord(x + mp, 0);
            return 0;
        }

        protected void InitializeMelee()
        {
            this.SetDistance(1);
            Battle.AttackForm attackForm = Battle.AttackFormPossible(this.attacker, this.defender);
            this.aCanAttackMelee = this.attacker.GetBaseFigure().attack > 0 && attackForm == Battle.AttackForm.eMelee;
            attackForm = Battle.AttackFormPossible(this.defender, this.attacker);
            this.dCanAttackMelee = this.defender.GetBaseFigure().attack > 0 && attackForm == Battle.AttackForm.eMelee;
        }

        protected bool MeleeAttackPreference(BattleUnit bu)
        {
            if (bu.GetCurentFigure().rangedAmmo != 0)
            {
                return bu.GetCurentFigure().attack > bu.GetCurentFigure().rangedAttack;
            }
            return true;
        }
    }
}
