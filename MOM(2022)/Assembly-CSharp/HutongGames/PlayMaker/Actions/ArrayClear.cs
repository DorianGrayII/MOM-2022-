namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Sets all items in an Array to their default value: 0, empty string, false, or null depending on their type. Optionally defines a reset value to use.")]
    public class ArrayClear : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array Variable to clear.")]
        public FsmArray array;

        [MatchElementType("array")]
        [Tooltip("Optional reset value. Leave as None for default value.")]
        public FsmVar resetValue;

        public override void Reset()
        {
            this.array = null;
            this.resetValue = new FsmVar
            {
                useVariable = true
            };
        }

        public override void OnEnter()
        {
            int length = this.array.Length;
            this.array.Reset();
            this.array.Resize(length);
            if (!this.resetValue.IsNone)
            {
                this.resetValue.UpdateValue();
                object value = this.resetValue.GetValue();
                for (int i = 0; i < length; i++)
                {
                    this.array.Set(i, value);
                }
            }
            base.Finish();
        }
    }
}
