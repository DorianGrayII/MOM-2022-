namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Gets the Int data from the last Event.")]
    public class GetEventIntData : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the int data in a variable.")]
        public FsmInt getIntData;

        public override void Reset()
        {
            this.getIntData = null;
        }

        public override void OnEnter()
        {
            this.getIntData.Value = Fsm.EventData.IntData;
            base.Finish();
        }
    }
}
