using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Set an item in an Array Variable in another FSM.")]
    public class SetFsmArrayItem : BaseFsmVariableIndexAction
    {
        [RequiredField]
        [Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.FsmName)]
        [Tooltip("Optional name of FSM on Game Object.")]
        public FsmString fsmName;

        [RequiredField]
        [UIHint(UIHint.FsmArray)]
        [Tooltip("The name of the FSM variable.")]
        public FsmString variableName;

        [Tooltip("The index into the array.")]
        public FsmInt index;

        [RequiredField]
        [Tooltip("Set the value of the array at the specified index.")]
        public FsmVar value;

        [Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.value = null;
        }

        public override void OnEnter()
        {
            this.DoSetFsmArray();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetFsmArray()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!base.UpdateCache(ownerDefaultTarget, this.fsmName.Value))
            {
                return;
            }
            FsmArray fsmArray = base.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
            if (fsmArray != null)
            {
                if (this.index.Value < 0 || this.index.Value >= fsmArray.Length)
                {
                    base.Fsm.Event(base.indexOutOfRange);
                    base.Finish();
                }
                else if (fsmArray.ElementType == this.value.NamedVar.VariableType)
                {
                    this.value.UpdateValue();
                    fsmArray.Set(this.index.Value, this.value.GetValue());
                }
                else
                {
                    base.LogWarning("Incompatible variable type: " + this.variableName.Value);
                }
            }
            else
            {
                base.DoVariableNotFound(this.variableName.Value);
            }
        }

        public override void OnUpdate()
        {
            this.DoSetFsmArray();
        }
    }
}
