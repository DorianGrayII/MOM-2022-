using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Animates the value of a Float Variable using an Animation Curve.")]
    public class AnimateFloatV2 : AnimateFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat floatVariable;

        [RequiredField]
        public FsmAnimationCurve animCurve;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to floatVariable")]
        public Calculation calculation;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.floatVariable = new FsmFloat
            {
                UseVariable = true
            };
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[1];
            base.fromFloats = new float[1];
            base.fromFloats[0] = (this.floatVariable.IsNone ? 0f : this.floatVariable.Value);
            base.calculations = new Calculation[1];
            base.calculations[0] = this.calculation;
            base.curves = new AnimationCurve[1];
            base.curves[0] = this.animCurve.curve;
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
    }
}
