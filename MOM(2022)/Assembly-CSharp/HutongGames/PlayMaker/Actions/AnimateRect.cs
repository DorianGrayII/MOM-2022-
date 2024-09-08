using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("AnimateVariables")]
    [Tooltip("Animates the value of a Rect Variable using an Animation Curve.")]
    public class AnimateRect : AnimateFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmRect rectVariable;

        [RequiredField]
        public FsmAnimationCurve curveX;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.x.")]
        public Calculation calculationX;

        [RequiredField]
        public FsmAnimationCurve curveY;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.y.")]
        public Calculation calculationY;

        [RequiredField]
        public FsmAnimationCurve curveW;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.width.")]
        public Calculation calculationW;

        [RequiredField]
        public FsmAnimationCurve curveH;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.height.")]
        public Calculation calculationH;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.rectVariable = new FsmRect
            {
                UseVariable = true
            };
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[4];
            base.fromFloats = new float[4];
            base.fromFloats[0] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.x);
            base.fromFloats[1] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.y);
            base.fromFloats[2] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.width);
            base.fromFloats[3] = (this.rectVariable.IsNone ? 0f : this.rectVariable.Value.height);
            base.curves = new AnimationCurve[4];
            base.curves[0] = this.curveX.curve;
            base.curves[1] = this.curveY.curve;
            base.curves[2] = this.curveW.curve;
            base.curves[3] = this.curveH.curve;
            base.calculations = new Calculation[4];
            base.calculations[0] = this.calculationX;
            base.calculations[1] = this.calculationY;
            base.calculations[2] = this.calculationW;
            base.calculations[3] = this.calculationH;
            base.Init();
            if (Math.Abs(base.delay.Value) < 0.01f)
            {
                this.UpdateVariableValue();
            }
        }

        private void UpdateVariableValue()
        {
            if (!this.rectVariable.IsNone)
            {
                this.rectVariable.Value = new Rect(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (base.isRunning)
            {
                this.UpdateVariableValue();
            }
            if (this.finishInNextStep && !base.looping)
            {
                base.Finish();
                base.Fsm.Event(base.finishEvent);
            }
            if (base.finishAction && !this.finishInNextStep)
            {
                this.UpdateVariableValue();
                this.finishInNextStep = true;
            }
        }
    }
}
