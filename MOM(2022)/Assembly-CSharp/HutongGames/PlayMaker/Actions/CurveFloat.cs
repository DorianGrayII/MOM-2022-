namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.AnimateVariables), Tooltip("Animates the value of a Float Variable FROM-TO with assistance of Deformation Curve.")]
    public class CurveFloat : CurveFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmFloat floatVariable;
        [RequiredField]
        public FsmFloat fromValue;
        [RequiredField]
        public FsmFloat toValue;
        [RequiredField]
        public FsmAnimationCurve animCurve;
        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue and toValue.")]
        public CurveFsmAction.Calculation calculation;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[1];
            base.fromFloats = new float[] { this.fromValue.IsNone ? 0f : this.fromValue.Value };
            base.toFloats = new float[] { this.toValue.IsNone ? 0f : this.toValue.Value };
            base.calculations = new CurveFsmAction.Calculation[] { this.calculation };
            base.curves = new AnimationCurve[] { this.animCurve.curve };
            base.Init();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.floatVariable.IsNone && base.isRunning)
            {
                this.floatVariable.Value = base.resultFloats[0];
            }
            if (this.finishInNextStep && !base.looping)
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
                    this.floatVariable.Value = base.resultFloats[0];
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.floatVariable = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.toValue = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.fromValue = num3;
        }
    }
}

