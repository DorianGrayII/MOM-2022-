using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Easing Animation - Color")]
    public class EaseColor : EaseFsmAction
    {
        [RequiredField]
        public FsmColor fromValue;

        [RequiredField]
        public FsmColor toValue;

        [UIHint(UIHint.Variable)]
        public FsmColor colorVariable;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.colorVariable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[4];
            base.fromFloats[0] = this.fromValue.Value.r;
            base.fromFloats[1] = this.fromValue.Value.g;
            base.fromFloats[2] = this.fromValue.Value.b;
            base.fromFloats[3] = this.fromValue.Value.a;
            base.toFloats = new float[4];
            base.toFloats[0] = this.toValue.Value.r;
            base.toFloats[1] = this.toValue.Value.g;
            base.toFloats[2] = this.toValue.Value.b;
            base.toFloats[3] = this.toValue.Value.a;
            base.resultFloats = new float[4];
            this.finishInNextStep = false;
            this.colorVariable.Value = this.fromValue.Value;
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
                this.colorVariable.Value = new Color(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2], base.resultFloats[3]);
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
                    this.colorVariable.Value = new Color(base.reverse.IsNone ? this.toValue.Value.r : (base.reverse.Value ? this.fromValue.Value.r : this.toValue.Value.r), base.reverse.IsNone ? this.toValue.Value.g : (base.reverse.Value ? this.fromValue.Value.g : this.toValue.Value.g), base.reverse.IsNone ? this.toValue.Value.b : (base.reverse.Value ? this.fromValue.Value.b : this.toValue.Value.b), base.reverse.IsNone ? this.toValue.Value.a : (base.reverse.Value ? this.fromValue.Value.a : this.toValue.Value.a));
                }
                this.finishInNextStep = true;
            }
        }
    }
}
