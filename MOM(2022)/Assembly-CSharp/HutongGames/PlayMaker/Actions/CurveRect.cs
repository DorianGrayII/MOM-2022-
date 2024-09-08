using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("AnimateVariables")]
    [Tooltip("Animates the value of a Rect Variable FROM-TO with assistance of Deformation Curves.")]
    public class CurveRect : CurveFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmRect rectVariable;

        [RequiredField]
        public FsmRect fromValue;

        [RequiredField]
        public FsmRect toValue;

        [RequiredField]
        public FsmAnimationCurve curveX;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
        public Calculation calculationX;

        [RequiredField]
        public FsmAnimationCurve curveY;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
        public Calculation calculationY;

        [RequiredField]
        public FsmAnimationCurve curveW;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.width and toValue.width.")]
        public Calculation calculationW;

        [RequiredField]
        public FsmAnimationCurve curveH;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.height and toValue.height.")]
        public Calculation calculationH;

        private Rect rct;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.rectVariable = new FsmRect
            {
                UseVariable = true
            };
            this.toValue = new FsmRect
            {
                UseVariable = true
            };
            this.fromValue = new FsmRect
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
            base.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.x);
            base.fromFloats[1] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.y);
            base.fromFloats[2] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.width);
            base.fromFloats[3] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.height);
            base.toFloats = new float[4];
            base.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value.x);
            base.toFloats[1] = (this.toValue.IsNone ? 0f : this.toValue.Value.y);
            base.toFloats[2] = (this.toValue.IsNone ? 0f : this.toValue.Value.width);
            base.toFloats[3] = (this.toValue.IsNone ? 0f : this.toValue.Value.height);
            base.curves = new AnimationCurve[4];
            base.curves[0] = this.curveX.curve;
            base.curves[1] = this.curveY.curve;
            base.curves[2] = this.curveW.curve;
            base.curves[3] = this.curveH.curve;
            base.calculations = new Calculation[4];
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
                this.rectVariable.Value = this.rct;
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
                    this.rectVariable.Value = this.rct;
                }
                this.finishInNextStep = true;
            }
        }
    }
}
