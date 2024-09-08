using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Reverse the order of items in an Array.")]
    public class ArrayReverse : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array to reverse.")]
        public FsmArray array;

        public override void Reset()
        {
            this.array = null;
        }

        public override void OnEnter()
        {
            List<object> list = new List<object>(this.array.Values);
            list.Reverse();
            this.array.Values = list.ToArray();
            base.Finish();
        }
    }
}
