using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the gravity vector, or individual axis.")]
    public class SetGravity2d : FsmStateAction
    {
        [Tooltip("Gravity as Vector2.")]
        public FsmVector2 vector;

        [Tooltip("Override the x value of the gravity")]
        public FsmFloat x;

        [Tooltip("Override the y value of the gravity")]
        public FsmFloat y;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector = null;
            this.x = new FsmFloat
            {
                UseVariable = true
            };
            this.y = new FsmFloat
            {
                UseVariable = true
            };
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetGravity();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetGravity();
        }

        private void DoSetGravity()
        {
            Vector2 value = this.vector.Value;
            if (!this.x.IsNone)
            {
                value.x = this.x.Value;
            }
            if (!this.y.IsNone)
            {
                value.y = this.y.Value;
            }
            Physics2D.gravity = value;
        }
    }
}
