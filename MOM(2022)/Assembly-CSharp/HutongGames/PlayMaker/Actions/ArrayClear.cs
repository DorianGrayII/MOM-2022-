namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Array), Tooltip("Sets all items in an Array to their default value: 0, empty string, false, or null depending on their type. Optionally defines a reset value to use.")]
    public class ArrayClear : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("The Array Variable to clear.")]
        public FsmArray array;
        [MatchElementType("array"), Tooltip("Optional reset value. Leave as None for default value.")]
        public FsmVar resetValue;

        public override void OnEnter()
        {
            int length = this.array.Length;
            this.array.Reset();
            this.array.Resize(length);
            if (!this.resetValue.IsNone)
            {
                this.resetValue.UpdateValue();
                object obj2 = this.resetValue.GetValue();
                for (int i = 0; i < length; i++)
                {
                    this.array.Set(i, obj2);
                }
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.array = null;
            FsmVar var1 = new FsmVar();
            var1.useVariable = true;
            this.resetValue = var1;
        }
    }
}

