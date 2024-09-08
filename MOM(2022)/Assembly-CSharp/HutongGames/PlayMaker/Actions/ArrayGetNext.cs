namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Array)]
    [Tooltip("Each time this action is called it gets the next item from a Array. \nThis lets you quickly loop through all the items of an array to perform actions on them.")]
    public class ArrayGetNext : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Array Variable to use.")]
        public FsmArray array;

        [Tooltip("From where to start iteration, leave as 0 to start from the beginning")]
        public FsmInt startIndex;

        [Tooltip("When to end iteration, leave as 0 to iterate until the end")]
        public FsmInt endIndex;

        [Tooltip("Event to send to get the next item.")]
        public FsmEvent loopEvent;

        [Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
        [UIHint(UIHint.Variable)]
        public FsmBool resetFlag;

        [Tooltip("Event to send when there are no more items.")]
        public FsmEvent finishedEvent;

        [ActionSection("Result")]
        [MatchElementType("array")]
        [UIHint(UIHint.Variable)]
        public FsmVar result;

        [UIHint(UIHint.Variable)]
        public FsmInt currentIndex;

        private int nextItemIndex;

        public override void Reset()
        {
            this.array = null;
            this.startIndex = null;
            this.endIndex = null;
            this.currentIndex = null;
            this.loopEvent = null;
            this.finishedEvent = null;
            this.resetFlag = null;
            this.result = null;
        }

        public override void OnEnter()
        {
            if (this.nextItemIndex == 0 && this.startIndex.Value > 0)
            {
                this.nextItemIndex = this.startIndex.Value;
            }
            if (this.resetFlag.Value)
            {
                this.nextItemIndex = this.startIndex.Value;
                this.resetFlag.Value = false;
            }
            this.DoGetNextItem();
            base.Finish();
        }

        private void DoGetNextItem()
        {
            if (this.nextItemIndex >= this.array.Length)
            {
                this.nextItemIndex = 0;
                this.currentIndex.Value = this.array.Length - 1;
                base.Fsm.Event(this.finishedEvent);
                return;
            }
            this.result.SetValue(this.array.Get(this.nextItemIndex));
            if (this.nextItemIndex >= this.array.Length)
            {
                this.nextItemIndex = 0;
                this.currentIndex.Value = this.array.Length - 1;
                base.Fsm.Event(this.finishedEvent);
                return;
            }
            if (this.endIndex.Value > 0 && this.nextItemIndex >= this.endIndex.Value)
            {
                this.nextItemIndex = 0;
                this.currentIndex.Value = this.endIndex.Value;
                base.Fsm.Event(this.finishedEvent);
                return;
            }
            this.nextItemIndex++;
            this.currentIndex.Value = this.nextItemIndex - 1;
            if (this.loopEvent != null)
            {
                base.Fsm.Event(this.loopEvent);
            }
        }
    }
}
