namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("AnimateVariables"), HutongGames.PlayMaker.Tooltip("Animates the value of a Rect Variable using an Animation Curve.")]
    public class AnimateRect : AnimateFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmRect rectVariable;
        [RequiredField]
        public FsmAnimationCurve curveX;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.x.")]
        public AnimateFsmAction.Calculation calculationX;
        [RequiredField]
        public FsmAnimationCurve curveY;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.y.")]
        public AnimateFsmAction.Calculation calculationY;
        [RequiredField]
        public FsmAnimationCurve curveW;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.width.")]
        public AnimateFsmAction.Calculation calculationW;
        [RequiredField]
        public FsmAnimationCurve curveH;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to rectVariable.height.")]
        public AnimateFsmAction.Calculation calculationH;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[4];
            base.fromFloats = new float[] { this.rectVariable.IsNone ? 0f : this.rectVariable.get_Value().x, this.rectVariable.IsNone ? 0f : this.rectVariable.get_Value().y, this.rectVariable.IsNone ? 0f : this.rectVariable.get_Value().width, this.rectVariable.IsNone ? 0f : this.rectVariable.get_Value().height };
            base.curves = new AnimationCurve[] { this.curveX.curve, this.curveY.curve, this.curveW.curve, this.curveH.curve };
            base.calculations = new AnimateFsmAction.Calculation[] { this.calculationX, this.calculationY, this.calculationW, this.calculationH };
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
            FsmRect rect1 = new FsmRect();
            rect1.UseVariable = true;
            this.rectVariable = rect1;
        }

        private void UpdateVariableValue()
        {
            if (!this.rectVariable.IsNone)
            {
                this.rectVariable.set_Value(new Rect(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]));
            }
        }
    }
}

