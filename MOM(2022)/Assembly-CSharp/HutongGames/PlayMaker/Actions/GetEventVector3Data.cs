namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Gets the Vector3 data from the last Event.")]
    public class GetEventVector3Data : FsmStateAction
    {
        [UIHint(UIHint.Variable), Tooltip("Store the vector3 data in a variable.")]
        public FsmVector3 getVector3Data;

        public override void OnEnter()
        {
            this.getVector3Data.set_Value(Fsm.EventData.Vector3Data);
            base.Finish();
        }

        public override void Reset()
        {
            this.getVector3Data = null;
        }
    }
}

