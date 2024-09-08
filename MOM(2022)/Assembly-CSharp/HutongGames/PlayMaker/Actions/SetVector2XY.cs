using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Sets the XY channels of a Vector2 Variable. To leave any channel unchanged, set variable to 'None'.")]
    public class SetVector2XY : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;

        [UIHint(UIHint.Variable)]
        [Tooltip("The vector2 source")]
        public FsmVector2 vector2Value;

        [Tooltip("The x component. Override vector2Value if set")]
        public FsmFloat x;

        [Tooltip("The y component.Override vector2Value if set")]
        public FsmFloat y;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.vector2Value = null;
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
            this.DoSetVector2XYZ();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetVector2XYZ();
        }

        private void DoSetVector2XYZ()
        {
            if (this.vector2Variable != null)
            {
                Vector2 value = this.vector2Variable.Value;
                if (!this.vector2Value.IsNone)
                {
                    value = this.vector2Value.Value;
                }
                if (!this.x.IsNone)
                {
                    value.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    value.y = this.y.Value;
                }
                this.vector2Variable.Value = value;
            }
        }
    }
}
