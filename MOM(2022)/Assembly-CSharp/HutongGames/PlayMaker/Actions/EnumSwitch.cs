namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Sends an Event based on the value of an Enum Variable.")]
    public class EnumSwitch : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmEnum enumVariable;

        [CompoundArray("Enum Switches", "Compare Enum Values", "Send")]
        [MatchFieldType("enumVariable")]
        public FsmEnum[] compareTo;

        public FsmEvent[] sendEvent;

        public bool everyFrame;

        public override void Reset()
        {
            this.enumVariable = null;
            this.compareTo = new FsmEnum[0];
            this.sendEvent = new FsmEvent[0];
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoEnumSwitch();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoEnumSwitch();
        }

        private void DoEnumSwitch()
        {
            if (this.enumVariable.IsNone)
            {
                return;
            }
            for (int i = 0; i < this.compareTo.Length; i++)
            {
                if (object.Equals(this.enumVariable.Value, this.compareTo[i].Value))
                {
                    base.Fsm.Event(this.sendEvent[i]);
                    break;
                }
            }
        }
    }
}
