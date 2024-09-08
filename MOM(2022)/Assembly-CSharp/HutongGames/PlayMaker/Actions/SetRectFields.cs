namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Rect), HutongGames.PlayMaker.Tooltip("Sets the individual fields of a Rect Variable. To leave any field unchanged, set variable to 'None'.")]
    public class SetRectFields : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmRect rectVariable;
        public FsmFloat x;
        public FsmFloat y;
        public FsmFloat width;
        public FsmFloat height;
        public bool everyFrame;

        private void DoSetRectFields()
        {
            if (!this.rectVariable.IsNone)
            {
                Rect rect = this.rectVariable.get_Value();
                if (!this.x.IsNone)
                {
                    rect.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    rect.y = this.y.Value;
                }
                if (!this.width.IsNone)
                {
                    rect.width = this.width.Value;
                }
                if (!this.height.IsNone)
                {
                    rect.height = this.height.Value;
                }
                this.rectVariable.set_Value(rect);
            }
        }

        public override void OnEnter()
        {
            this.DoSetRectFields();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetRectFields();
        }

        public override void Reset()
        {
            this.rectVariable = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.width = num3;
            FsmFloat num4 = new FsmFloat();
            num4.UseVariable = true;
            this.height = num4;
            this.everyFrame = false;
        }
    }
}

