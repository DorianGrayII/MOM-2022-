namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Color Variable using an Animation Curve.")]
    public class AnimateColor : AnimateFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmColor colorVariable;
        [RequiredField]
        public FsmAnimationCurve curveR;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.r.")]
        public AnimateFsmAction.Calculation calculationR;
        [RequiredField]
        public FsmAnimationCurve curveG;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.g.")]
        public AnimateFsmAction.Calculation calculationG;
        [RequiredField]
        public FsmAnimationCurve curveB;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.b.")]
        public AnimateFsmAction.Calculation calculationB;
        [RequiredField]
        public FsmAnimationCurve curveA;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to colorVariable.a.")]
        public AnimateFsmAction.Calculation calculationA;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[4];
            base.fromFloats = new float[] { this.colorVariable.IsNone ? 0f : this.colorVariable.get_Value().r, this.colorVariable.IsNone ? 0f : this.colorVariable.get_Value().g, this.colorVariable.IsNone ? 0f : this.colorVariable.get_Value().b, this.colorVariable.IsNone ? 0f : this.colorVariable.get_Value().a };
            base.curves = new AnimationCurve[] { this.curveR.curve, this.curveG.curve, this.curveB.curve, this.curveA.curve };
            base.calculations = new AnimateFsmAction.Calculation[] { this.calculationR, this.calculationG, this.calculationB, this.calculationA };
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
            FsmColor color1 = new FsmColor();
            color1.UseVariable = true;
            this.colorVariable = color1;
        }

        private void UpdateVariableValue()
        {
            if (!this.colorVariable.IsNone)
            {
                this.colorVariable.set_Value(new Color(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]));
            }
        }
    }
}

