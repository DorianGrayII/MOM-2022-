namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Splits a string into substrings using separator characters.")]
    public class StringSplit : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("String to split.")]
        public FsmString stringToSplit;

        [Tooltip("Characters used to split the string.\nUse '\\n' for newline\nUse '\\t' for tab")]
        public FsmString separators;

        [Tooltip("Remove all leading and trailing white-space characters from each separated string.")]
        public FsmBool trimStrings;

        [Tooltip("Optional characters used to trim each separated string.")]
        public FsmString trimChars;

        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.String, "", 0, 0, 65536)]
        [Tooltip("Store the split strings in a String Array.")]
        public FsmArray stringArray;

        public override void Reset()
        {
            this.stringToSplit = null;
            this.separators = null;
            this.trimStrings = false;
            this.trimChars = null;
            this.stringArray = null;
        }

        public override void OnEnter()
        {
            char[] array = this.trimChars.Value.ToCharArray();
            if (!this.stringToSplit.IsNone && !this.stringArray.IsNone)
            {
                FsmArray fsmArray = this.stringArray;
                object[] values = this.stringToSplit.Value.Split(this.separators.Value.ToCharArray());
                fsmArray.Values = values;
                if (this.trimStrings.Value)
                {
                    for (int i = 0; i < this.stringArray.Values.Length; i++)
                    {
                        if (this.stringArray.Values[i] is string text)
                        {
                            if (!this.trimChars.IsNone && array.Length != 0)
                            {
                                this.stringArray.Set(i, text.Trim(array));
                            }
                            else
                            {
                                this.stringArray.Set(i, text.Trim());
                            }
                        }
                    }
                }
                this.stringArray.SaveChanges();
            }
            base.Finish();
        }
    }
}
