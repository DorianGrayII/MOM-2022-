namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ActionCategory(ActionCategory.Array), HutongGames.PlayMaker.Tooltip("Shuffle values in an array. Optionally set a start index and range to shuffle only part of the array.")]
    public class ArrayShuffle : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Array to shuffle.")]
        public FsmArray array;
        [HutongGames.PlayMaker.Tooltip("Optional start Index for the shuffling. Leave it to none or 0 for no effect")]
        public FsmInt startIndex;
        [HutongGames.PlayMaker.Tooltip("Optional range for the shuffling, starting at the start index if greater than 0. Leave it to none or 0 for no effect, it will shuffle the whole array")]
        public FsmInt shufflingRange;

        public override void OnEnter()
        {
            List<object> list = new List<object>(this.array.Values);
            int minInclusive = 0;
            int b = list.Count - 1;
            if (this.startIndex.Value > 0)
            {
                minInclusive = Mathf.Min(this.startIndex.Value, b);
            }
            if (this.shufflingRange.Value > 0)
            {
                b = Mathf.Min((int) (list.Count - 1), (int) (minInclusive + this.shufflingRange.Value));
            }
            for (int i = b; i > minInclusive; i--)
            {
                int num4 = UnityEngine.Random.Range(minInclusive, i + 1);
                object obj2 = list[i];
                list[i] = list[num4];
                list[num4] = obj2;
            }
            this.array.Values = list.ToArray();
            base.Finish();
        }

        public override void Reset()
        {
            this.array = null;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.startIndex = num1;
            FsmInt num2 = new FsmInt();
            num2.UseVariable = true;
            this.shufflingRange = num2;
        }
    }
}

