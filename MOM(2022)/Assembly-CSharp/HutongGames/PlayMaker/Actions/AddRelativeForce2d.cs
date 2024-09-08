namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Adds a relative 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
    public class AddRelativeForce2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject to apply the force to.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Option for applying the force")]
        public ForceMode2D forceMode;
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

        private void DoAddRelativeForce()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 relativeForce = this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.get_Value();
                if (!this.vector3.IsNone)
                {
                    relativeForce.x = this.vector3.get_Value().x;
                    relativeForce.y = this.vector3.get_Value().y;
                }
                if (!this.x.IsNone)
                {
                    relativeForce.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    relativeForce.y = this.y.Value;
                }
                base.rigidbody2d.AddRelativeForce(relativeForce, this.forceMode);
            }
        }

        public override void OnEnter()
        {
            this.DoAddRelativeForce();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoAddRelativeForce();
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.forceMode = ForceMode2D.Force;
            this.vector = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.vector3 = vector1;
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

