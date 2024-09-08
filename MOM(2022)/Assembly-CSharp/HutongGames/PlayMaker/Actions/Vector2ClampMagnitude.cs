namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Clamps the Magnitude of Vector2 Variable.")]
    public class Vector2ClampMagnitude : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Vector2")]
        public FsmVector2 vector2Variable;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The maximum Magnitude")]
        public FsmFloat maxLength;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoVector2ClampMagnitude()
        {
            this.vector2Variable.set_Value(Vector2.ClampMagnitude(this.vector2Variable.get_Value(), this.maxLength.Value));
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

        public override void Reset()
        {
            this.vector2Variable = null;
            this.maxLength = null;
            this.everyFrame = false;
        }
    }
}

