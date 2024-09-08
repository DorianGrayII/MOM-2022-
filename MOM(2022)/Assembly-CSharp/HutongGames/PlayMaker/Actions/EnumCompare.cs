namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Compares 2 Enum values and sends Events based on the result.")]
    public class EnumCompare : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmEnum enumVariable;

        [MatchFieldType("enumVariable")]
        public FsmEnum compareTo;

        public FsmEvent equalEvent;

        public FsmEvent notEqualEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the true/false result in a bool variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame. Useful if the enum is changing over time.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.enumVariable = null;
            this.compareTo = null;
            this.equalEvent = null;
            this.notEqualEvent = null;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoEnumCompare();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoEnumCompare();
        }

        private void DoEnumCompare()
        {
            if (this.enumVariable != null && this.compareTo != null)
            {
                bool flag = object.Equals(this.enumVariable.Value, this.compareTo.Value);
                if (this.storeResult != null)
                {
                    this.storeResult.Value = flag;
                }
                if (flag && this.equalEvent != null)
                {
                    base.Fsm.Event(this.equalEvent);
                }
                else if (!flag && this.notEqualEvent != null)
                {
                    base.Fsm.Event(this.notEqualEvent);
                }
            }
        }
    }
}
