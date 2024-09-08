namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Sets the XY channels of a Vector2 Variable. To leave any channel unchanged, set variable to 'None'.")]
    public class SetVector2XY : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The vector2 target")]
        public FsmVector2 vector2Variable;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The vector2 source")]
        public FsmVector2 vector2Value;
        [HutongGames.PlayMaker.Tooltip("The x component. Override vector2Value if set")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y component.Override vector2Value if set")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoSetVector2XYZ()
        {
            if (this.vector2Variable != null)
            {
                Vector2 vector = this.vector2Variable.get_Value();
                if (!this.vector2Value.IsNone)
                {
                    vector = this.vector2Value.get_Value();
                }
                if (!this.x.IsNone)
                {
                    vector.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    vector.y = this.y.Value;
                }
                this.vector2Variable.set_Value(vector);
            }
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

        public override void Reset()
        {
            this.vector2Variable = null;
            this.vector2Value = null;
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

