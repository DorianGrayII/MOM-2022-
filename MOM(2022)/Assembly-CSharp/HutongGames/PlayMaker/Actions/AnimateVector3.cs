using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Animates the value of a Vector3 Variable using an Animation Curve.")]
    public class AnimateVector3 : AnimateFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmVector3 vectorVariable;

        [RequiredField]
        public FsmAnimationCurve curveX;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.x.")]
        public Calculation calculationX;

        [RequiredField]
        public FsmAnimationCurve curveY;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.y.")]
        public Calculation calculationY;

        [RequiredField]
        public FsmAnimationCurve curveZ;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.z.")]
        public Calculation calculationZ;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.vectorVariable = new FsmVector3
            {
                UseVariable = true
            };
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[3];
            base.fromFloats = new float[3];
            base.fromFloats[0] = (this.vectorVariable.IsNone ? 0f : this.vectorVariable.Value.x);
            base.fromFloats[1] = (this.vectorVariable.IsNone ? 0f : this.vectorVariable.Value.y);
            base.fromFloats[2] = (this.vectorVariable.IsNone ? 0f : this.vectorVariable.Value.z);
            base.curves = new AnimationCurve[3];
            base.curves[0] = this.curveX.curve;
            base.curves[1] = this.curveY.curve;
            base.curves[2] = this.curveZ.curve;
            base.calculations = new Calculation[3];
            base.calculations[0] = this.calculationX;
            base.calculations[1] = this.calculationY;
            base.calculations[2] = this.calculationZ;
            base.Init();
            if (Math.Abs(base.delay.Value) < 0.01f)
            {
                this.UpdateVariableValue();
            }
        }

        private void UpdateVariableValue()
        {
            if (!this.vectorVariable.IsNone)
            {
                this.vectorVariable.Value = new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]);
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
