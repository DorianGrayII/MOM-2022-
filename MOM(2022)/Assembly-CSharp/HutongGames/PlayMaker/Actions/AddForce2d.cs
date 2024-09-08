namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
    public class AddForce2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject to apply the force to.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Option for applying the force")]
        public ForceMode2D forceMode;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
        public FsmVector2 atPosition;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
        public FsmVector2 vector;
        [HutongGames.PlayMaker.Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("A Vector3 force to add. z is ignored")]
        public FsmVector3 vector3;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private void DoAddForce()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 force = this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.get_Value();
                if (!this.vector3.IsNone)
                {
                    force.x = this.vector3.get_Value().x;
                    force.y = this.vector3.get_Value().y;
                }
                if (!this.x.IsNone)
                {
                    force.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    force.y = this.y.Value;
                }
                if (!this.atPosition.IsNone)
                {
                    base.rigidbody2d.AddForceAtPosition(force, this.atPosition.get_Value(), this.forceMode);
                }
                else
                {
                    base.rigidbody2d.AddForce(force, this.forceMode);
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
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.atPosition = vector1;
            this.forceMode = ForceMode2D.Force;
            this.vector = null;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.vector3 = vector2;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            this.everyFrame = false;
        }
    }
}

