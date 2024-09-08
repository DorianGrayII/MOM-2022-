namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.StateMachine)]
    [Tooltip("Gets the String data from the last Event.")]
    public class GetEventStringData : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the string data in a variable.")]
        public FsmString getStringData;

        public override void Reset()
        {
            this.getStringData = null;
        }

        public override void OnEnter()
        {
            this.getStringData.Value = Fsm.EventData.StringData;
            base.Finish();
        }
    }
}
