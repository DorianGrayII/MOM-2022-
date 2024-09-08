namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Array), Tooltip("Add values to an array.")]
    public class ArrayAddRange : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array Variable to use.")]
        public FsmArray array;
        [RequiredField, MatchElementType("array"), Tooltip("The variables to add.")]
        public FsmVar[] variables;

        private void DoAddRange()
        {
            int length = this.variables.Length;
            if (length > 0)
            {
                this.array.Resize(this.array.Length + length);
                foreach (FsmVar var in this.variables)
                {
                    var.UpdateValue();
                    this.array.Set(this.array.Length - length, var.GetValue());
                    length--;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoAddRange();
            base.Finish();
        }

        public override void Reset()
        {
            this.array = null;
            this.variables = new FsmVar[2];
        }
    }
}

