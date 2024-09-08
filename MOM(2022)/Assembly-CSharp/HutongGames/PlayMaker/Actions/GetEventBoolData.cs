namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Gets the Bool data from the last Event.")]
    public class GetEventBoolData : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Store the bool data in a variable.")]
        public FsmBool getBoolData;

        public override void OnEnter()
        {
            this.getBoolData.Value = Fsm.EventData.BoolData;
            base.Finish();
        }

        public override void Reset()
        {
            this.getBoolData = null;
        }
    }
}

