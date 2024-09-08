namespace MOM
{
    using MHUtils;
    using System;
    using UnityEngine;

    public abstract class BaseBattleTask : ITask
    {
        public BattleUnit attacker;
        public BattleUnit defender;
        public MHRandom random;
        private bool aCanAttackMelee;
        private bool dCanAttackMelee;
        private int[] aDamageBuffers;
        private int[] dDamageBuffers;

        protected BaseBattleTask()
        {
        }

        protected Battle.AttackForm AttackPossible(BattleUnit active, BattleUnit oponent)
        {
            return ((HexCoordinates.HexDistance(active.GetPosition(), oponent.GetPosition()) != 1) ? ((active.GetCurentFigure().rangedAmmo > 0) ? Battle.AttackForm.eRanged : Battle.AttackForm.eNone) : (this.CanAttackMelee(active) ? Battle.AttackForm.eMelee : Battle.AttackForm.eNone));
        }

        protected bool CanAttackMelee(BattleUnit source)
        {
            return (!ReferenceEquals(this.attacker, source) ? this.dCanAttackMelee : this.aCanAttackMelee);
        }

        protected int ChangeDistance(int mp, int target)
        {
            this.defender.battlePosition = Vector3i.zero;
            short x = this.attacker.battlePosition.x;
            if ((x + mp) < 1)
            {
                this.attacker.battlePosition = Vector3i.BuildHexCoord(1, 0);
                return (Mathf.Abs((int) (x + mp)) + 1);
            }
            if ((x == target) || (mp <= 0))
            {
                this.attacker.battlePosition = Vector3i.BuildHexCoord(x + mp, 0);
                return 0;
            }
            int num2 = target - x;
            int num3 = Mathf.Abs(num2);
            if (mp >= num3)
            {
                this.attacker.battlePosition = Vector3i.BuildHexCoord(target, 0);
                return (mp - num3);
            }
            int num4 = (num2 > 0) ? 1 : -1;
            this.attacker.battlePosition = Vector3i.BuildHexCoord((num4 * mp) + x, 0);
            return 0;
        }

        public abstract object Execute();
        protected void Execute(BattleAttackStack stack)
        {
            stack.ExecuteSkillStack(false);
        }

        protected void InitializeMelee()
        {
            this.SetDistance(1);
            Battle.AttackForm form = Battle.AttackFormPossible(this.attacker, this.defender, -1);
            this.aCanAttackMelee = (this.attacker.GetBaseFigure().attack > 0) && (form == Battle.AttackForm.eMelee);
            form = Battle.AttackFormPossible(this.defender, this.attacker, -1);
            this.dCanAttackMelee = (this.defender.GetBaseFigure().attack > 0) && (form == Battle.AttackForm.eMelee);
        }

        protected bool MeleeAttackPreference(BattleUnit bu)
        {
            return ((bu.GetCurentFigure().rangedAmmo == 0) || (bu.GetCurentFigure().attack > bu.GetCurentFigure().rangedAttack));
        }

        protected void SetDistance(int dist)
        {
            this.attacker.battlePosition = Vector3i.BuildHexCoord(dist, 0);
            this.defender.battlePosition = Vector3i.zero;
        }
    }
}

