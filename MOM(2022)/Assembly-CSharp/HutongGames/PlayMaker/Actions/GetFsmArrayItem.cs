namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false), HutongGames.PlayMaker.Tooltip("Gets an item in an Array Variable in another FSM.")]
    public class GetFsmArrayItem : BaseFsmVariableIndexAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the FSM.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmName), HutongGames.PlayMaker.Tooltip("Optional name of FSM on Game Object.")]
        public FsmString fsmName;
        [RequiredField, UIHint(UIHint.FsmArray), HutongGames.PlayMaker.Tooltip("The name of the FSM variable.")]
        public FsmString variableName;
        [HutongGames.PlayMaker.Tooltip("The index into the array.")]
        public FsmInt index;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Get the value of the array at the specified index.")]
        public FsmVar storeValue;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is changing.")]
        public bool everyFrame;

        private void DoGetFsmArray()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget, this.fsmName.Value))
            {
                FsmArray fsmArray = base.fsm.FsmVariables.GetFsmArray(this.variableName.Value);
                if (fsmArray == null)
                {
                    base.DoVariableNotFound(this.variableName.Value);
                }
                else if ((this.index.Value < 0) || (this.index.Value >= fsmArray.Length))
                {
                    base.Fsm.Event(base.indexOutOfRange);
                    base.Finish();
                }
                else if (fsmArray.ElementType == this.storeValue.NamedVar.VariableType)
                {
                    this.storeValue.SetValue(fsmArray.Get(this.index.Value));
                }
                else
                {
                    base.LogWarning("Incompatible variable type: " + this.variableName.Value);
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetFsmArray();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetFsmArray();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.fsmName = "";
            this.storeValue = null;
        }
    }
}

