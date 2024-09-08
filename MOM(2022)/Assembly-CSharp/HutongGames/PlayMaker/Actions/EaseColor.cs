namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Easing Animation - Color")]
    public class EaseColor : EaseFsmAction
    {
        [RequiredField]
        public FsmColor fromValue;
        [RequiredField]
        public FsmColor toValue;
        [UIHint(UIHint.Variable)]
        public FsmColor colorVariable;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[] { this.fromValue.get_Value().r, this.fromValue.get_Value().g, this.fromValue.get_Value().b, this.fromValue.get_Value().a };
            base.toFloats = new float[] { this.toValue.get_Value().r, this.toValue.get_Value().g, this.toValue.get_Value().b, this.toValue.get_Value().a };
            base.resultFloats = new float[4];
            this.finishInNextStep = false;
            this.colorVariable.set_Value(this.fromValue.get_Value());
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.colorVariable.IsNone && base.isRunning)
            {
                this.colorVariable.set_Value(new Color(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]));
            }
            if (this.finishInNextStep)
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
                    this.colorVariable.set_Value(new Color(base.reverse.IsNone ? this.toValue.get_Value().r : (base.reverse.Value ? this.fromValue.get_Value().r : this.toValue.get_Value().r), base.reverse.IsNone ? this.toValue.get_Value().g : (base.reverse.Value ? this.fromValue.get_Value().g : this.toValue.get_Value().g), base.reverse.IsNone ? this.toValue.get_Value().b : (base.reverse.Value ? this.fromValue.get_Value().b : this.toValue.get_Value().b), base.reverse.IsNone ? this.toValue.get_Value().a : (base.reverse.Value ? this.fromValue.get_Value().a : this.toValue.get_Value().a)));
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.colorVariable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }
    }
}

