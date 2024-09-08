namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Vector3 Variable using an Animation Curve.")]
    public class AnimateVector3 : AnimateFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vectorVariable;
        [RequiredField]
        public FsmAnimationCurve curveX;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.x.")]
        public AnimateFsmAction.Calculation calculationX;
        [RequiredField]
        public FsmAnimationCurve curveY;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.y.")]
        public AnimateFsmAction.Calculation calculationY;
        [RequiredField]
        public FsmAnimationCurve curveZ;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to vectorVariable.z.")]
        public AnimateFsmAction.Calculation calculationZ;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[3];
            base.fromFloats = new float[] { this.vectorVariable.IsNone ? 0f : this.vectorVariable.get_Value().x, this.vectorVariable.IsNone ? 0f : this.vectorVariable.get_Value().y, this.vectorVariable.IsNone ? 0f : this.vectorVariable.get_Value().z };
            base.curves = new AnimationCurve[] { this.curveX.curve, this.curveY.curve, this.curveZ.curve };
            base.calculations = new AnimateFsmAction.Calculation[] { this.calculationX, this.calculationY, this.calculationZ };
            base.Init();
            if (Math.Abs(base.delay.Value) < 0.01f)
            {
                this.UpdateVariableValue();
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

        public override void Reset()
        {
            base.Reset();
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.vectorVariable = vector1;
        }

        private void UpdateVariableValue()
        {
            if (!this.vectorVariable.IsNone)
            {
                this.vectorVariable.set_Value(new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]));
            }
        }
    }
}

