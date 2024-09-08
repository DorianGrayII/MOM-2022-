namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Adds a 2d torque (rotational force) to a Game Object.")]
    public class AddTorque2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject to add torque to.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Option for applying the force")]
        public ForceMode2D forceMode;
        [HutongGames.PlayMaker.Tooltip("Torque")]
        public FsmFloat torque;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private void DoAddTorque()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.AddTorque(this.torque.Value, this.forceMode);
            }
        }

        public override void OnEnter()
        {
            this.DoAddTorque();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoAddTorque();
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.torque = null;
            this.everyFrame = false;
        }
    }
}

