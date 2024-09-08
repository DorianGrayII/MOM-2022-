namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Logic), Tooltip("Sends an Event based on the value of an Enum Variable.")]
    public class EnumSwitch : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmEnum enumVariable;
        [CompoundArray("Enum Switches", "Compare Enum Values", "Send"), MatchFieldType("enumVariable")]
        public FsmEnum[] compareTo;
        public FsmEvent[] sendEvent;
        public bool everyFrame;

        private void DoEnumSwitch()
        {
            if (!this.enumVariable.IsNone)
            {
                for (int i = 0; i < this.compareTo.Length; i++)
                {
                    if (Equals(this.enumVariable.Value, this.compareTo[i].Value))
                    {
                        base.Fsm.Event(this.sendEvent[i]);
                        return;
                    }
                }
            }
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

        public override void Reset()
        {
            this.enumVariable = null;
            this.compareTo = new FsmEnum[0];
            this.sendEvent = new FsmEvent[0];
            this.everyFrame = false;
        }
    }
}

