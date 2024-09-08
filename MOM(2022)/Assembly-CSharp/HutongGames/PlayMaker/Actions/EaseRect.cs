namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("AnimateVariables"), HutongGames.PlayMaker.Tooltip("Easing Animation - Rect.")]
    public class EaseRect : EaseFsmAction
    {
        [RequiredField]
        public FsmRect fromValue;
        [RequiredField]
        public FsmRect toValue;
        [UIHint(UIHint.Variable)]
        public FsmRect rectVariable;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[] { this.fromValue.get_Value().x, this.fromValue.get_Value().y, this.fromValue.get_Value().width, this.fromValue.get_Value().height };
            base.toFloats = new float[] { this.toValue.get_Value().x, this.toValue.get_Value().y, this.toValue.get_Value().width, this.toValue.get_Value().height };
            base.resultFloats = new float[4];
            this.finishInNextStep = false;
            this.rectVariable.set_Value(this.fromValue.get_Value());
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.rectVariable.IsNone && base.isRunning)
            {
                this.rectVariable.set_Value(new Rect(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]));
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
                if (!this.rectVariable.IsNone)
                {
                    this.rectVariable.set_Value(new Rect(base.reverse.IsNone ? this.toValue.get_Value().x : (base.reverse.Value ? this.fromValue.get_Value().x : this.toValue.get_Value().x), base.reverse.IsNone ? this.toValue.get_Value().y : (base.reverse.Value ? this.fromValue.get_Value().y : this.toValue.get_Value().y), base.reverse.IsNone ? this.toValue.get_Value().width : (base.reverse.Value ? this.fromValue.get_Value().width : this.toValue.get_Value().width), base.reverse.IsNone ? this.toValue.get_Value().height : (base.reverse.Value ? this.fromValue.get_Value().height : this.toValue.get_Value().height)));
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.rectVariable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }
    }
}

