namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;

    [ActionCategory(ActionCategory.Array), Tooltip("Sort items in an Array.")]
    public class ArraySort : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array to sort.")]
        public FsmArray array;

        public override void OnEnter()
        {
            List<object> list = new List<object>(this.array.Values);
            list.Sort();
            this.array.Values = list.ToArray();
            base.Finish();
        }

        public override void Reset()
        {
            this.array = null;
        }
    }
}

