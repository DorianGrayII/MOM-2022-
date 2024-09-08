namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Physics), Tooltip("Gets info on the last joint break event.")]
    public class GetJointBreakInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Get the force that broke the joint.")]
        public FsmFloat breakForce;

        public override void OnEnter()
        {
            this.breakForce.Value = base.Fsm.JointBreakForce;
            base.Finish();
        }

        public override void Reset()
        {
            this.breakForce = null;
        }
    }
}

