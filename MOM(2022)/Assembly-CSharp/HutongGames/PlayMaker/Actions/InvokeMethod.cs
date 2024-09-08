using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Invokes a Method in a Behaviour attached to a Game Object. See Unity InvokeMethod docs.")]
    public class InvokeMethod : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The game object that owns the behaviour.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Script)]
        [Tooltip("The behaviour that contains the method.")]
        public FsmString behaviour;

        [RequiredField]
        [UIHint(UIHint.Method)]
        [Tooltip("The name of the method to invoke.")]
        public FsmString methodName;

        [HasFloatSlider(0f, 10f)]
        [Tooltip("Optional time delay in seconds.")]
        public FsmFloat delay;

        [Tooltip("Call the method repeatedly.")]
        public FsmBool repeating;

        [HasFloatSlider(0f, 10f)]
        [Tooltip("Delay between repeated calls in seconds.")]
        public FsmFloat repeatDelay;

        [Tooltip("Stop calling the method when the state is exited.")]
        public FsmBool cancelOnExit;

        private MonoBehaviour component;

        public override void Reset()
        {
            this.gameObject = null;
            this.behaviour = null;
            this.methodName = "";
            this.delay = null;
            this.repeating = false;
            this.repeatDelay = 1f;
            this.cancelOnExit = false;
        }

        public override void OnEnter()
        {
            this.DoInvokeMethod(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        private void DoInvokeMethod(GameObject go)
        {
            if (!(go == null))
            {
                this.component = go.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as MonoBehaviour;
                if (this.component == null)
                {
                    base.LogWarning("InvokeMethod: " + go.name + " missing behaviour: " + this.behaviour.Value);
                }
                else if (this.repeating.Value)
                {
                    this.component.InvokeRepeating(this.methodName.Value, this.delay.Value, this.repeatDelay.Value);
                }
                else
                {
                    this.component.Invoke(this.methodName.Value, this.delay.Value);
                }
            }
        }

        public override void OnExit()
        {
            if (!(this.component == null) && this.cancelOnExit.Value)
            {
                this.component.CancelInvoke(this.methodName.Value);
            }
        }
    }
}
