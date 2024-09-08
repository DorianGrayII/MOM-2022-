namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Set the value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
    public class ArraySet : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array Variable to use.")]
        public FsmArray array;

        [Tooltip("The index into the array.")]
        public FsmInt index;

        [RequiredField]
        [MatchElementType("array")]
        [Tooltip("Set the value of the array at the specified index.")]
        public FsmVar value;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        [ActionSection("Events")]
        [Tooltip("The event to trigger if the index is out of range")]
        public FsmEvent indexOutOfRange;

        public override void Reset()
        {
            this.array = null;
            this.index = null;
            this.value = null;
            this.everyFrame = false;
            this.indexOutOfRange = null;
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

        private void DoGetValue()
        {
            if (!this.array.IsNone)
            {
                if (this.index.Value >= 0 && this.index.Value < this.array.Length)
                {
                    this.value.UpdateValue();
                    this.array.Set(this.index.Value, this.value.GetValue());
                }
                else
                {
                    base.Fsm.Event(this.indexOutOfRange);
                }
            }
        }
    }
}
