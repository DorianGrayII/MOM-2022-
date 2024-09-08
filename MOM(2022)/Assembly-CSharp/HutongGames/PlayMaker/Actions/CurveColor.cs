namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Color Variable FROM-TO with assistance of Deformation Curves.")]
    public class CurveColor : CurveFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmColor colorVariable;
        [RequiredField]
        public FsmColor fromValue;
        [RequiredField]
        public FsmColor toValue;
        [RequiredField]
        public FsmAnimationCurve curveR;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Red and toValue.Rec.")]
        public CurveFsmAction.Calculation calculationR;
        [RequiredField]
        public FsmAnimationCurve curveG;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Green and toValue.Green.")]
        public CurveFsmAction.Calculation calculationG;
        [RequiredField]
        public FsmAnimationCurve curveB;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Blue and toValue.Blue.")]
        public CurveFsmAction.Calculation calculationB;
        [RequiredField]
        public FsmAnimationCurve curveA;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Alpha and toValue.Alpha.")]
        public CurveFsmAction.Calculation calculationA;
        private Color clr;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[4];
            base.fromFloats = new float[] { this.fromValue.IsNone ? 0f : this.fromValue.get_Value().r, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().g, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().b, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().a };
            base.toFloats = new float[] { this.toValue.IsNone ? 0f : this.toValue.get_Value().r, this.toValue.IsNone ? 0f : this.toValue.get_Value().g, this.toValue.IsNone ? 0f : this.toValue.get_Value().b, this.toValue.IsNone ? 0f : this.toValue.get_Value().a };
            base.curves = new AnimationCurve[] { this.curveR.curve, this.curveG.curve, this.curveB.curve, this.curveA.curve };
            base.calculations = new CurveFsmAction.Calculation[] { this.calculationR, this.calculationG, this.calculationB, this.calculationA };
            base.Init();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.colorVariable.IsNone && base.isRunning)
            {
                this.clr = new Color(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
                this.colorVariable.set_Value(this.clr);
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
                if (!this.colorVariable.IsNone)
                {
                    this.clr = new Color(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
                    this.colorVariable.set_Value(this.clr);
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            FsmColor color1 = new FsmColor();
            color1.UseVariable = true;
            this.colorVariable = color1;
            FsmColor color2 = new FsmColor();
            color2.UseVariable = true;
            this.toValue = color2;
            FsmColor color3 = new FsmColor();
            color3.UseVariable = true;
            this.fromValue = color3;
        }
    }
}

