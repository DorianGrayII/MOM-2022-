namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Color), Tooltip("Sets the value of a Color Variable.")]
    public class SetColorValue : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmColor colorVariable;
        [RequiredField]
        public FsmColor color;
        public bool everyFrame;

        private void DoSetColorValue()
        {
            if (this.colorVariable != null)
            {
                this.colorVariable.set_Value(this.color.get_Value());
            }
        }

        public override void OnEnter()
        {
            this.DoSetColorValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetColorValue();
        }

        public override void Reset()
        {
            this.colorVariable = null;
            this.color = null;
            this.everyFrame = false;
        }
    }
}

