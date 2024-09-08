namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Adds a force to a Game Object. Use Vector3 variable and/or Float variables for each axis.")]
    public class AddForce : ComponentAction<Rigidbody>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody)), HutongGames.PlayMaker.Tooltip("The GameObject to apply the force to.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollisionInfo actions.")]
        public FsmVector3 atPosition;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("A Vector3 force to add. Optionally override any axis with the X, Y, Z parameters.")]
        public FsmVector3 vector;
        [HutongGames.PlayMaker.Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Force along the Z axis. To leave unchanged, set to 'None'.")]
        public FsmFloat z;
        [HutongGames.PlayMaker.Tooltip("Apply the force in world or local space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("The type of force to apply. See Unity Physics docs.")]
        public ForceMode forceMode;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private void DoAddForce()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector3 vector1;
                if (!this.vector.IsNone)
                {
                    vector1 = this.vector.get_Value();
                }
                else
                {
                    vector1 = new Vector3();
                }
                Vector3 force = vector1;
                if (!this.x.IsNone)
                {
                    force.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    force.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    force.z = this.z.Value;
                }
                if (this.space != Space.World)
                {
                    base.rigidbody.AddRelativeForce(force, this.forceMode);
                }
                else if (!this.atPosition.IsNone)
                {
                    base.rigidbody.AddForceAtPosition(force, this.atPosition.get_Value(), this.forceMode);
                }
                else
                {
                    base.rigidbody.AddForce(force, this.forceMode);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoAddForce();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoAddForce();
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.atPosition = vector1;
            this.vector = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.z = num3;
            this.space = Space.World;
            this.forceMode = ForceMode.Force;
            this.everyFrame = false;
        }
    }
}

