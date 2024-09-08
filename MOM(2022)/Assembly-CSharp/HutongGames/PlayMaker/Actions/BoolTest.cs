namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Sends Events based on the value of a Boolean Variable.")]
    public class BoolTest : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Readonly]
        [Tooltip("The Bool variable to test.")]
        public FsmBool boolVariable;

        [Tooltip("Event to send if the Bool variable is True.")]
        public FsmEvent isTrue;

        [Tooltip("Event to send if the Bool variable is False.")]
        public FsmEvent isFalse;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.boolVariable = null;
            this.isTrue = null;
            this.isFalse = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            base.Fsm.Event(this.boolVariable.Value ? this.isTrue : this.isFalse);
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            base.Fsm.Event(this.boolVariable.Value ? this.isTrue : this.isFalse);
        }
    }
}
