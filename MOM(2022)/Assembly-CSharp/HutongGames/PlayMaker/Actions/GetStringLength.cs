namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.String), Tooltip("Gets the Length of a String.")]
    public class GetStringLength : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmString stringVariable;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmInt storeResult;
        public bool everyFrame;

        private void DoGetStringLength()
        {
            if ((this.stringVariable != null) && (this.storeResult != null))
            {
                this.storeResult.Value = this.stringVariable.Value.Length;
            }
        }

        public override void OnEnter()
        {
            this.DoGetStringLength();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetStringLength();
        }

        public override void Reset()
        {
            this.stringVariable = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

