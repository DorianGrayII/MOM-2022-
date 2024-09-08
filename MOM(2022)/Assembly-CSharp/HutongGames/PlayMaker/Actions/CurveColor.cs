using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Animates the value of a Color Variable FROM-TO with assistance of Deformation Curves.")]
    public class CurveColor : CurveFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmColor colorVariable;

        [RequiredField]
        public FsmColor fromValue;

        [RequiredField]
        public FsmColor toValue;

        [RequiredField]
        public FsmAnimationCurve curveR;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Red and toValue.Rec.")]
        public Calculation calculationR;

        [RequiredField]
        public FsmAnimationCurve curveG;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Green and toValue.Green.")]
        public Calculation calculationG;

        [RequiredField]
        public FsmAnimationCurve curveB;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Blue and toValue.Blue.")]
        public Calculation calculationB;

        [RequiredField]
        public FsmAnimationCurve curveA;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Alpha and toValue.Alpha.")]
        public Calculation calculationA;

        private Color clr;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.colorVariable = new FsmColor
            {
                UseVariable = true
            };
            this.toValue = new FsmColor
            {
                UseVariable = true
            };
            this.fromValue = new FsmColor
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
            base.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.r);
            base.fromFloats[1] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.g);
            base.fromFloats[2] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.b);
            base.fromFloats[3] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.a);
            base.toFloats = new float[4];
            base.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value.r);
            base.toFloats[1] = (this.toValue.IsNone ? 0f : this.toValue.Value.g);
            base.toFloats[2] = (this.toValue.IsNone ? 0f : this.toValue.Value.b);
            base.toFloats[3] = (this.toValue.IsNone ? 0f : this.toValue.Value.a);
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
                this.colorVariable.Value = this.clr;
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
                    this.colorVariable.Value = this.clr;
                }
                this.finishInNextStep = true;
            }
        }
    }
}
