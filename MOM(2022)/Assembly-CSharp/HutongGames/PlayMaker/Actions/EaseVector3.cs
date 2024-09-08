using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.AnimateVariables)]
    [Tooltip("Easing Animation - Vector3")]
    public class EaseVector3 : EaseFsmAction
    {
        [RequiredField]
        public FsmVector3 fromValue;

        [RequiredField]
        public FsmVector3 toValue;

        [UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;

        private bool finishInNextStep;

        public override void Reset()
        {
            base.Reset();
            this.vector3Variable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[3];
            base.fromFloats[0] = this.fromValue.Value.x;
            base.fromFloats[1] = this.fromValue.Value.y;
            base.fromFloats[2] = this.fromValue.Value.z;
            base.toFloats = new float[3];
            base.toFloats[0] = this.toValue.Value.x;
            base.toFloats[1] = this.toValue.Value.y;
            base.toFloats[2] = this.toValue.Value.z;
            base.resultFloats = new float[3];
            this.finishInNextStep = false;
            this.vector3Variable.Value = this.fromValue.Value;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!this.vector3Variable.IsNone && base.isRunning)
            {
                this.vector3Variable.Value = new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]);
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
                if (!this.vector3Variable.IsNone)
                {
                    this.vector3Variable.Value = new Vector3(base.reverse.IsNone ? this.toValue.Value.x : (base.reverse.Value ? this.fromValue.Value.x : this.toValue.Value.x), base.reverse.IsNone ? this.toValue.Value.y : (base.reverse.Value ? this.fromValue.Value.y : this.toValue.Value.y), base.reverse.IsNone ? this.toValue.Value.z : (base.reverse.Value ? this.fromValue.Value.z : this.toValue.Value.z));
                }
                this.finishInNextStep = true;
            }
        }
    }
}
