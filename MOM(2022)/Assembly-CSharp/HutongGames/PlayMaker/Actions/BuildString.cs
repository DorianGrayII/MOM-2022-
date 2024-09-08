namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Builds a String from other Strings.")]
    public class BuildString : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Array of Strings to combine.")]
        public FsmString[] stringParts;

        [Tooltip("Separator to insert between each String. E.g. space character.")]
        public FsmString separator;

        [Tooltip("Add Separator to end of built string.")]
        public FsmBool addToEnd;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the final String in a variable.")]
        public FsmString storeResult;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private string result;

        public override void Reset()
        {
            this.stringParts = new FsmString[3];
            this.separator = null;
            this.addToEnd = true;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoBuildString();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoBuildString();
        }

        private void DoBuildString()
        {
            if (this.storeResult != null)
            {
                this.result = "";
                for (int i = 0; i < this.stringParts.Length - 1; i++)
                {
                    this.result += this.stringParts[i];
                    this.result += this.separator.Value;
                }
                this.result += this.stringParts[this.stringParts.Length - 1];
                if (this.addToEnd.Value)
                {
                    this.result += this.separator.Value;
                }
                this.storeResult.Value = this.result;
            }
        }
    }
}
