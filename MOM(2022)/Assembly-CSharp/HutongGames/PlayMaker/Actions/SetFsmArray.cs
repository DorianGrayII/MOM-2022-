using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
    [Tooltip("Copy an Array Variable in another FSM.")]
    public class SetFsmArray : BaseFsmVariableAction
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

        [RequiredField]
        [Tooltip("Set the content of the array variable.")]
        [UIHint(UIHint.Variable)]
        public FsmArray setValue;

        [Tooltip("If true, makes copies. if false, values share the same reference and editing one array item value will affect the source and vice versa. Warning, this only affect the current items of the source array. Adding or removing items doesn't affect other FsmArrays.")]
        public bool copyValues;

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.variableName = null;
            this.setValue = null;
            this.copyValues = true;
        }

        public override void OnEnter()
        {
            this.DoSetFsmArrayCopy();
            base.Finish();
        }

        private void DoSetFsmArrayCopy()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!base.UpdateCache(ownerDefaultTarget, this.fsmName.Value))
            {
                return;
            }
            FsmArray fsmArray = base.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
            if (fsmArray != null)
            {
                if (fsmArray.ElementType != this.setValue.ElementType)
                {
                    base.LogError("Can only copy arrays with the same elements type. Found <" + fsmArray.ElementType.ToString() + "> and <" + this.setValue.ElementType.ToString() + ">");
                }
                else
                {
                    fsmArray.Resize(0);
                    if (this.copyValues)
                    {
                        fsmArray.Values = this.setValue.Values.Clone() as object[];
                    }
                    else
                    {
                        fsmArray.Values = this.setValue.Values;
                    }
                    fsmArray.SaveChanges();
                }
            }
            else
            {
                base.DoVariableNotFound(this.variableName.Value);
            }
        }
    }
}
