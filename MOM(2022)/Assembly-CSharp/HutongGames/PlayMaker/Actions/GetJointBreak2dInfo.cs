namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Gets info on the last joint break 2D event.")]
    public class GetJointBreak2dInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable), ObjectType(typeof(Joint2D)), HutongGames.PlayMaker.Tooltip("Get the broken joint.")]
        public FsmObject brokenJoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the reaction force exerted by the broken joint. Unity 5.3+")]
        public FsmVector2 reactionForce;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the magnitude of the reaction force exerted by the broken joint. Unity 5.3+")]
        public FsmFloat reactionForceMagnitude;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the reaction torque exerted by the broken joint. Unity 5.3+")]
        public FsmFloat reactionTorque;

        public override void OnEnter()
        {
            this.StoreInfo();
            base.Finish();
        }

        public override void Reset()
        {
            this.brokenJoint = null;
            this.reactionForce = null;
            this.reactionTorque = null;
        }

        private void StoreInfo()
        {
            if (base.Fsm.get_BrokenJoint2D() != null)
            {
                this.brokenJoint.set_Value(base.Fsm.get_BrokenJoint2D());
                this.reactionForce.set_Value(base.Fsm.get_BrokenJoint2D().reactionForce);
                this.reactionForceMagnitude.Value = base.Fsm.get_BrokenJoint2D().reactionForce.magnitude;
                this.reactionTorque.Value = base.Fsm.get_BrokenJoint2D().reactionTorque;
            }
        }
    }
}

