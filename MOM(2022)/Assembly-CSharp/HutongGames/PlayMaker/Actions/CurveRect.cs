namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("AnimateVariables"), HutongGames.PlayMaker.Tooltip("Animates the value of a Rect Variable FROM-TO with assistance of Deformation Curves.")]
    public class CurveRect : CurveFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmRect rectVariable;
        [RequiredField]
        public FsmRect fromValue;
        [RequiredField]
        public FsmRect toValue;
        [RequiredField]
        public FsmAnimationCurve curveX;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
        public CurveFsmAction.Calculation calculationX;
        [RequiredField]
        public FsmAnimationCurve curveY;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
        public CurveFsmAction.Calculation calculationY;
        [RequiredField]
        public FsmAnimationCurve curveW;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.width and toValue.width.")]
        public CurveFsmAction.Calculation calculationW;
        [RequiredField]
        public FsmAnimationCurve curveH;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.height and toValue.height.")]
        public CurveFsmAction.Calculation calculationH;
        private Rect rct;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[4];
            base.fromFloats = new float[] { this.fromValue.IsNone ? 0f : this.fromValue.get_Value().x, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().y, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().width, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().height };
            base.toFloats = new float[] { this.toValue.IsNone ? 0f : this.toValue.get_Value().x, this.toValue.IsNone ? 0f : this.toValue.get_Value().y, this.toValue.IsNone ? 0f : this.toValue.get_Value().width, this.toValue.IsNone ? 0f : this.toValue.get_Value().height };
            base.curves = new AnimationCurve[] { this.curveX.curve, this.curveY.curve, this.curveW.curve, this.curveH.curve };
            base.calculations = new CurveFsmAction.Calculation[4];
            base.calculations[0] = this.calculationX;
            base.calculations[1] = this.calculationY;
            base.calculations[2] = this.calculationW;
            base.calculations[2] = this.calculationH;
            base.Init();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.rectVariable.IsNone && base.isRunning)
            {
                this.rct = new Rect(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
                this.rectVariable.set_Value(this.rct);
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
                if (!this.rectVariable.IsNone)
                {
                    this.rct = new Rect(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
                    this.rectVariable.set_Value(this.rct);
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            FsmRect rect1 = new FsmRect();
            rect1.UseVariable = true;
            this.rectVariable = rect1;
            FsmRect rect2 = new FsmRect();
            rect2.UseVariable = true;
            this.toValue = rect2;
            FsmRect rect3 = new FsmRect();
            rect3.UseVariable = true;
            this.fromValue = rect3;
        }
    }
}

