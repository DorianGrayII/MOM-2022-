using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Animates the value of a Vector3 Variable FROM-TO with assistance of Deformation Curves.")]
    public class CurveVector3 : CurveFsmAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmVector3 vectorVariable;

        [RequiredField]
        public FsmVector3 fromValue;

        [RequiredField]
        public FsmVector3 toValue;

        [RequiredField]
        public FsmAnimationCurve curveX;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
        public Calculation calculationX;

        [RequiredField]
        public FsmAnimationCurve curveY;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
        public Calculation calculationY;

        [RequiredField]
        public FsmAnimationCurve curveZ;

        [Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.z and toValue.z.")]
        public Calculation calculationZ;

        private Vector3 vct;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.vectorVariable = new FsmVector3
            {
                UseVariable = true
            };
            this.toValue = new FsmVector3
            {
                UseVariable = true
            };
            this.fromValue = new FsmVector3
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
            base.fromFloats[0] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.x);
            base.fromFloats[1] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.y);
            base.fromFloats[2] = (this.fromValue.IsNone ? 0f : this.fromValue.Value.z);
            base.toFloats = new float[3];
            base.toFloats[0] = (this.toValue.IsNone ? 0f : this.toValue.Value.x);
            base.toFloats[1] = (this.toValue.IsNone ? 0f : this.toValue.Value.y);
            base.toFloats[2] = (this.toValue.IsNone ? 0f : this.toValue.Value.z);
            base.curves = new AnimationCurve[3];
            base.curves[0] = this.curveX.curve;
            base.curves[1] = this.curveY.curve;
            base.curves[2] = this.curveZ.curve;
            base.calculations = new Calculation[3];
            base.calculations[0] = this.calculationX;
            base.calculations[1] = this.calculationY;
            base.calculations[2] = this.calculationZ;
            base.Init();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.vectorVariable.IsNone && base.isRunning)
            {
                this.vct = new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]);
                this.vectorVariable.Value = this.vct;
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
                if (!this.vectorVariable.IsNone)
                {
                    this.vct = new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]);
                    this.vectorVariable.Value = this.vct;
                }
                this.finishInNextStep = true;
            }
        }
    }
}
