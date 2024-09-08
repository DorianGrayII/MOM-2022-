namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Gets the Vector2 data from the last Event.")]
    public class GetEventVector2Data : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Store the vector2 data in a variable.")]
        public FsmVector2 getVector2Data;

        public override void OnEnter()
        {
            this.getVector2Data.set_Value(Fsm.EventData.Vector2Data);
            base.Finish();
        }

        public override void Reset()
        {
            this.getVector2Data = null;
        }
    }
}

