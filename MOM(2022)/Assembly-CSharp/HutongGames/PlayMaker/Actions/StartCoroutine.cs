using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Start a Coroutine in a Behaviour on a Game Object. See Unity StartCoroutine docs.")]
    public class StartCoroutine : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The game object that owns the Behaviour.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Behaviour)]
        [Tooltip("The Behaviour that contains the method to start as a coroutine.")]
        public FsmString behaviour;

        [RequiredField]
        [UIHint(UIHint.Coroutine)]
        [Tooltip("The name of the coroutine method.")]
        public FunctionCall functionCall;

        [Tooltip("Stop the coroutine when the state is exited.")]
        public bool stopOnExit;

        private MonoBehaviour component;

        public override void Reset()
        {
            this.gameObject = null;
            this.behaviour = null;
            this.functionCall = null;
            this.stopOnExit = false;
        }

        public override void OnEnter()
        {
            this.DoStartCoroutine();
            base.Finish();
        }

        private void DoStartCoroutine()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                return;
            }
            this.component = ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as MonoBehaviour;
            if (this.component == null)
            {
                base.LogWarning("StartCoroutine: " + ownerDefaultTarget.name + " missing behaviour: " + this.behaviour.Value);
                return;
            }
            switch (this.functionCall.ParameterType)
            {
            case "None":
                this.component.StartCoroutine(this.functionCall.FunctionName);
                break;
            case "int":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.IntParameter.Value);
                break;
            case "float":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.FloatParameter.Value);
                break;
            case "string":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.StringParameter.Value);
                break;
            case "bool":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.BoolParameter.Value);
                break;
            case "Vector2":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector2Parameter.Value);
                break;
            case "Vector3":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector3Parameter.Value);
                break;
            case "Rect":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.RectParamater.Value);
                break;
            case "GameObject":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.GameObjectParameter.Value);
                break;
            case "Material":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.MaterialParameter.Value);
                break;
            case "Texture":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.TextureParameter.Value);
                break;
            case "Quaternion":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.QuaternionParameter.Value);
                break;
            case "Object":
                this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.ObjectParameter.Value);
                break;
            }
        }

        public override void OnExit()
        {
            if (!(this.component == null) && this.stopOnExit)
            {
                this.component.StopCoroutine(this.functionCall.FunctionName);
            }
        }
    }
}
