namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.AnimateVariables), HutongGames.PlayMaker.Tooltip("Easing Animation - Vector3")]
    public class EaseVector3 : EaseFsmAction
    {
        [RequiredField]
        public FsmVector3 fromValue;
        [RequiredField]
        public FsmVector3 toValue;
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector3Variable;
        private bool finishInNextStep;

        public override void OnEnter()
        {
            base.OnEnter();
            base.fromFloats = new float[] { this.fromValue.get_Value().x, this.fromValue.get_Value().y, this.fromValue.get_Value().z };
            base.toFloats = new float[] { this.toValue.get_Value().x, this.toValue.get_Value().y, this.toValue.get_Value().z };
            base.resultFloats = new float[3];
            this.finishInNextStep = false;
            this.vector3Variable.set_Value(this.fromValue.get_Value());
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
                this.vector3Variable.set_Value(new Vector3(base.resultFloats[0], base.resultFloats[1], base.resultFloats[2]));
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
                    this.vector3Variable.set_Value(new Vector3(base.reverse.IsNone ? this.toValue.get_Value().x : (base.reverse.Value ? this.fromValue.get_Value().x : this.toValue.get_Value().x), base.reverse.IsNone ? this.toValue.get_Value().y : (base.reverse.Value ? this.fromValue.get_Value().y : this.toValue.get_Value().y), base.reverse.IsNone ? this.toValue.get_Value().z : (base.reverse.Value ? this.fromValue.get_Value().z : this.toValue.get_Value().z)));
                }
                this.finishInNextStep = true;
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.vector3Variable = null;
            this.fromValue = null;
            this.toValue = null;
            this.finishInNextStep = false;
        }
    }
}

