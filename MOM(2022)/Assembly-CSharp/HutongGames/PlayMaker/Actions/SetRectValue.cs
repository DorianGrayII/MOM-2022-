namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Rect), Tooltip("Sets the value of a Rect Variable.")]
    public class SetRectValue : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmRect rectVariable;
        [RequiredField]
        public FsmRect rectValue;
        public bool everyFrame;

        public override void OnEnter()
        {
            this.rectVariable.set_Value(this.rectValue.get_Value());
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.rectVariable.set_Value(this.rectValue.get_Value());
        }

        public override void Reset()
        {
            this.rectVariable = null;
            this.rectValue = null;
            this.everyFrame = false;
        }
    }
}

