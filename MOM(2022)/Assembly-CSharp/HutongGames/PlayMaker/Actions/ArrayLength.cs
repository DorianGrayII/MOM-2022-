namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Array), Tooltip("Gets the number of items in an Array.")]
    public class ArrayLength : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("The Array Variable.")]
        public FsmArray array;
        [UIHint(UIHint.Variable), Tooltip("Store the length in an Int Variable.")]
        public FsmInt length;
        [Tooltip("Repeat every frame. Useful if the array is changing and you're waiting for a particular length.")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.length.Value = this.array.Length;
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.length.Value = this.array.Length;
        }

        public override void Reset()
        {
            this.array = null;
            this.length = null;
            this.everyFrame = false;
        }
    }
}

