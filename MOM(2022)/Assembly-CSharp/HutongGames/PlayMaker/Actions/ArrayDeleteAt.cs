namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;

    [ActionCategory(ActionCategory.Array), Tooltip("Delete the item at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
    public class ArrayDeleteAt : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array Variable to use.")]
        public FsmArray array;
        [Tooltip("The index into the array.")]
        public FsmInt index;
        [ActionSection("Result"), Tooltip("The event to trigger if the index is out of range")]
        public FsmEvent indexOutOfRangeEvent;

        private void DoDeleteAt()
        {
            if ((this.index.Value < 0) || (this.index.Value >= this.array.Length))
            {
                base.Fsm.Event(this.indexOutOfRangeEvent);
            }
            else
            {
                List<object> list = new List<object>(this.array.Values);
                list.RemoveAt(this.index.Value);
                this.array.Values = list.ToArray();
            }
        }

        public override void OnEnter()
        {
            this.DoDeleteAt();
            base.Finish();
        }

        public override void Reset()
        {
            this.array = null;
            this.index = null;
            this.indexOutOfRangeEvent = null;
        }
    }
}

