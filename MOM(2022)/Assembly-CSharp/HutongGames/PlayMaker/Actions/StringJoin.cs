namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.String)]
    [Tooltip("Join an array of strings into a single string.")]
    public class StringJoin : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.String, "", 0, 0, 65536)]
        [Tooltip("Array of string to join into a single string.")]
        public FsmArray stringArray;

        [Tooltip("Separator to add between each string.")]
        public FsmString separator;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the joined string in string variable.")]
        public FsmString storeResult;

        public override void OnEnter()
        {
            if (!this.stringArray.IsNone && !this.storeResult.IsNone)
            {
                this.storeResult.Value = string.Join(this.separator.Value, this.stringArray.stringValues);
            }
            base.Finish();
        }
    }
}
