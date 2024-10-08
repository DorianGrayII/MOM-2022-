namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Replace a substring with a new String.")]
    public class StringReplace : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString stringVariable;

        public FsmString replace;

        public FsmString with;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString storeResult;

        public bool everyFrame;

        public override void Reset()
        {
            this.stringVariable = null;
            this.replace = "";
            this.with = "";
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoReplace();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoReplace();
        }

        private void DoReplace()
        {
            if (this.stringVariable != null && this.storeResult != null)
            {
                this.storeResult.Value = this.stringVariable.Value.Replace(this.replace.Value, this.with.Value);
            }
        }
    }
}
