namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Array), Tooltip("Add an item to the end of an Array.")]
    public class ArrayAdd : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array Variable to use.")]
        public FsmArray array;
        [RequiredField, MatchElementType("array"), Tooltip("Item to add.")]
        public FsmVar value;

        private void DoAddValue()
        {
            this.array.Resize(this.array.Length + 1);
            this.value.UpdateValue();
            this.array.Set(this.array.Length - 1, this.value.GetValue());
        }

        public override void OnEnter()
        {
            this.DoAddValue();
            base.Finish();
        }

        public override void Reset()
        {
            this.array = null;
            this.value = null;
        }
    }
}

