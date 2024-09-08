namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
    public class SetVelocity2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("A Vector2 value for the velocity")]
        public FsmVector2 vector;
        [HutongGames.PlayMaker.Tooltip("The y value of the velocity. Overrides 'Vector' x value if set")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Awake()
        {
            base.Fsm.HandleFixedUpdate = true;
        }

        private void DoSetVelocity()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                Vector2 vector = !this.vector.IsNone ? this.vector.get_Value() : base.rigidbody2d.velocity;
                if (!this.x.IsNone)
                {
                    vector.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    vector.y = this.y.Value;
                }
                base.rigidbody2d.velocity = vector;
            }
        }

        public override void OnEnter()
        {
            this.DoSetVelocity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            this.DoSetVelocity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.vector = null;
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

