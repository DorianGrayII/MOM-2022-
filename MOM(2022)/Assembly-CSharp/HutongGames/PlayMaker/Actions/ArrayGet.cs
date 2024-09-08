namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Array), Tooltip("Get a value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
    public class ArrayGet : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The Array Variable to use.")]
        public FsmArray array;
        [Tooltip("The index into the array.")]
        public FsmInt index;
        [RequiredField, MatchElementType("array"), UIHint(UIHint.Variable), Tooltip("Store the value in a variable.")]
        public FsmVar storeValue;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        [ActionSection("Events"), Tooltip("The event to trigger if the index is out of range")]
        public FsmEvent indexOutOfRange;

        private void DoGetValue()
        {
            if (!this.array.IsNone && !this.storeValue.IsNone)
            {
                if ((this.index.Value >= 0) && (this.index.Value < this.array.Length))
                {
                    this.storeValue.SetValue(this.array.Get(this.index.Value));
                }
                else
                {
                    base.Fsm.Event(this.indexOutOfRange);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetValue();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetValue();
        }

        public override void Reset()
        {
            this.array = null;
            this.index = null;
            this.everyFrame = false;
            this.storeValue = null;
            this.indexOutOfRange = null;
        }
    }
}

