using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Clamps the Magnitude of Vector2 Variable.")]
    public class Vector2ClampMagnitude : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Vector2")]
        public FsmVector2 vector2Variable;

        [RequiredField]
        [Tooltip("The maximum Magnitude")]
        public FsmFloat maxLength;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector2Variable = null;
            this.maxLength = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoVector2ClampMagnitude();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoVector2ClampMagnitude();
        }

        private void DoVector2ClampMagnitude()
        {
            this.vector2Variable.Value = Vector2.ClampMagnitude(this.vector2Variable.Value, this.maxLength.Value);
        }
    }
}
