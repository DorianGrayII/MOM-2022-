namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Logic), Tooltip("Compare 2 Object Variables and send events based on the result.")]
    public class ObjectCompare : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Readonly]
        public FsmObject objectVariable;
        [RequiredField]
        public FsmObject compareTo;
        [Tooltip("Event to send if the 2 object values are equal.")]
        public FsmEvent equalEvent;
        [Tooltip("Event to send if the 2 object values are not equal.")]
        public FsmEvent notEqualEvent;
        [UIHint(UIHint.Variable), Tooltip("Store the result in a variable.")]
        public FsmBool storeResult;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoObjectCompare()
        {
            bool flag = this.objectVariable.get_Value() == this.compareTo.get_Value();
            this.storeResult.Value = flag;
            base.Fsm.Event(flag ? this.equalEvent : this.notEqualEvent);
        }

        public override void OnEnter()
        {
            this.DoObjectCompare();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoObjectCompare();
        }

        public override void Reset()
        {
            this.objectVariable = null;
            this.compareTo = null;
            this.storeResult = null;
            this.equalEvent = null;
            this.notEqualEvent = null;
            this.everyFrame = false;
        }
    }
}

