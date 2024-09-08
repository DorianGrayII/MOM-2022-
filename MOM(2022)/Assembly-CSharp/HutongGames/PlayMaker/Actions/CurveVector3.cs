namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Animates the value of a Vector3 Variable FROM-TO with assistance of Deformation Curves.")]
    public class CurveVector3 : CurveFsmAction
    {
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 vectorVariable;
        [RequiredField]
        public FsmVector3 fromValue;
        [RequiredField]
        public FsmVector3 toValue;
        [RequiredField]
        public FsmAnimationCurve curveX;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
        public CurveFsmAction.Calculation calculationX;
        [RequiredField]
        public FsmAnimationCurve curveY;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
        public CurveFsmAction.Calculation calculationY;
        [RequiredField]
        public FsmAnimationCurve curveZ;
        [HutongGames.PlayMaker.Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.z and toValue.z.")]
        public CurveFsmAction.Calculation calculationZ;
        private Vector3 vct;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.finishInNextStep = false;
            base.resultFloats = new float[3];
            base.fromFloats = new float[] { this.fromValue.IsNone ? 0f : this.fromValue.get_Value().x, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().y, this.fromValue.IsNone ? 0f : this.fromValue.get_Value().z };
            base.toFloats = new float[] { this.toValue.IsNone ? 0f : this.toValue.get_Value().x, this.toValue.IsNone ? 0f : this.toValue.get_Value().y, this.toValue.IsNone ? 0f : this.toValue.get_Value().z };
            base.curves = new AnimationCurve[] { this.curveX.curve, this.curveY.curve, this.curveZ.curve };
            base.calculations = new CurveFsmAction.Calculation[] { this.calculationX, this.calculationY, this.calculationZ };
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
                this.vectorVariable.set_Value(this.vct);
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
                    this.vectorVariable.set_Value(this.vct);
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.vectorVariable = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.toValue = vector2;
            FsmVector3 vector3 = new FsmVector3();
            vector3.UseVariable = true;
            this.fromValue = vector3;
        }
    }
}

