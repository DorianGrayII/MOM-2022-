using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("AnimateVariables")]
    [Tooltip("Easing Animation - Rect.")]
    public class EaseRect : EaseFsmAction
    {
        [RequiredField]
        public FsmRect fromValue;

        [RequiredField]
        public FsmRect toValue;

        [UIHint(UIHint.Variable)]
        public FsmRect rectVariable;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.rectVariable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[4];
            base.fromFloats[0] = this.fromValue.Value.x;
            base.fromFloats[1] = this.fromValue.Value.y;
            base.fromFloats[2] = this.fromValue.Value.width;
            base.fromFloats[3] = this.fromValue.Value.height;
            base.toFloats = new float[4];
            base.toFloats[0] = this.toValue.Value.x;
            base.toFloats[1] = this.toValue.Value.y;
            base.toFloats[2] = this.toValue.Value.width;
            base.toFloats[3] = this.toValue.Value.height;
            base.resultFloats = new float[4];
            this.finishInNextStep = false;
            this.rectVariable.Value = this.fromValue.Value;
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
                this.rectVariable.Value = new Rect(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
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
                    this.rectVariable.Value = new Rect(base.reverse.IsNone ? this.toValue.Value.x : (base.reverse.Value ? this.fromValue.Value.x : this.toValue.Value.x), base.reverse.IsNone ? this.toValue.Value.y : (base.reverse.Value ? this.fromValue.Value.y : this.toValue.Value.y), base.reverse.IsNone ? this.toValue.Value.width : (base.reverse.Value ? this.fromValue.Value.width : this.toValue.Value.width), base.reverse.IsNone ? this.toValue.Value.height : (base.reverse.Value ? this.fromValue.Value.height : this.toValue.Value.height));
                }
                this.finishInNextStep = true;
            }
        }
    }
}
