using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Animates the value of a Color Variable using an Animation Curve.")]
    public class AnimateColor : AnimateFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmColor colorVariable;

        [RequiredField]
        public FsmAnimationCurve curveR;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.r.")]
        public Calculation calculationR;

        [RequiredField]
        public FsmAnimationCurve curveG;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.g.")]
        public Calculation calculationG;

        [RequiredField]
        public FsmAnimationCurve curveB;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.b.")]
        public Calculation calculationB;

        [RequiredField]
        public FsmAnimationCurve curveA;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.a.")]
        public Calculation calculationA;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.colorVariable = new FsmColor
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
            base.fromFloats[0] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.r);
            base.fromFloats[1] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.g);
            base.fromFloats[2] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.b);
            base.fromFloats[3] = (this.colorVariable.IsNone ? 0f : this.colorVariable.Value.a);
            base.curves = new AnimationCurve[4];
            base.curves[0] = this.curveR.curve;
            base.curves[1] = this.curveG.curve;
            base.curves[2] = this.curveB.curve;
            base.curves[3] = this.curveA.curve;
            base.calculations = new Calculation[4];
            base.calculations[0] = this.calculationR;
            base.calculations[1] = this.calculationG;
            base.calculations[2] = this.calculationB;
            base.calculations[3] = this.calculationA;
            base.Init();
            if (Math.Abs(base.delay.Value) < 0.01f)
            {
                this.UpdateVariableValue();
            }
        }

        private void UpdateVariableValue()
        {
            if (!this.colorVariable.IsNone)
            {
                this.colorVariable.Value = new Color(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
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
