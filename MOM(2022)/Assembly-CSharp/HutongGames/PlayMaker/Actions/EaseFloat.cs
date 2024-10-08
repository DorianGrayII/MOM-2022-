namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Easing Animation - Float")]
    public class EaseFloat : EaseFsmAction
    {
        [RequiredField]
        public FsmFloat fromValue;

        [RequiredField]
        public FsmFloat toValue;

        [UIHint(UIHint.Variable)]
        public FsmFloat floatVariable;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.floatVariable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[1];
            base.fromFloats[0] = this.fromValue.Value;
            base.toFloats = new float[1];
            base.toFloats[0] = this.toValue.Value;
            base.resultFloats = new float[1];
            this.finishInNextStep = false;
            this.floatVariable.Value = this.fromValue.Value;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.floatVariable.IsNone && base.isRunning)
            {
                this.floatVariable.Value = base.resultFloats[0];
            }
            if (this.finishInNextStep)
            {
                base.Finish();
                if (base.finishEvent != null)
                {
                    base.Fsm.Event(base.finishEvent);
                }
            }
            if (base.finishAction && !this.finishInNextStep)
            {
                if (!this.floatVariable.IsNone)
                {
                    this.floatVariable.Value = (base.reverse.IsNone ? this.toValue.Value : (base.reverse.Value ? this.fromValue.Value : this.toValue.Value));
                }
                this.finishInNextStep = true;
            }
        }
    }
}
