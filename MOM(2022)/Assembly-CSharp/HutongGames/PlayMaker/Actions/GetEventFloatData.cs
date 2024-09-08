namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Gets the Float data from the last Event.")]
    public class GetEventFloatData : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Store the float data in a variable.")]
        public FsmFloat getFloatData;

        public override void OnEnter()
        {
            this.getFloatData.Value = Fsm.EventData.FloatData;
            base.Finish();
        }

        public override void Reset()
        {
            this.getFloatData = null;
        }
    }
}

