namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Copy an Array Variable in another FSM.")]
    public class SetFsmArray : BaseFsmVariableAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object.")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmArray), HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        public FsmString variableName;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Set the content of the array variable."), UIHint(UIHint.Variable)]
        public FsmArray setValue;
        [HutongGames.PlayMaker.Tooltip("If true, makes copies. if false, values share the same reference and editing one array item value will affect the source and vice versa. Warning, this only affect the current items of the source array. Adding or removing items doesn't affect other FsmArrays.")]
        public bool copyValues;

        private void DoSetFsmArrayCopy()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget, this.fsmName.Value))
            {
                FsmArray fsmArray = base.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
                if (fsmArray == null)
                {
                    base.DoVariableNotFound(this.variableName.Value);
                }
                else if (fsmArray.ElementType == this.setValue.ElementType)
                {
                    fsmArray.Resize(0);
                    fsmArray.Values = !this.copyValues ? this.setValue.Values : (this.setValue.Values.Clone() as object[]);
                    fsmArray.SaveChanges();
                }
                else
                {
                    string[] textArray1 = new string[] { "Can only copy arrays with the same elements type. Found <", fsmArray.ElementType.ToString(), "> and <", this.setValue.ElementType.ToString(), ">" };
                    base.LogError(string.Concat(textArray1));
                }
            }
        }

        public override void OnEnter()
        {
            this.DoSetFsmArrayCopy();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.variableName = null;
            this.setValue = null;
            this.copyValues = true;
        }
    }
}

