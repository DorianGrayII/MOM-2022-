namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Gets the Child of a GameObject by Index.\nE.g., O to get the first child. HINT: Use this with an integer variable to iterate through children.")]
    public class GetChildNum : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to search.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The index of the child to find.")]
        public FsmInt childIndex;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the child in a GameObject variable.")]
        public FsmGameObject store;

        private GameObject DoGetChildNum(GameObject go)
        {
            return (((go == null) || ((go.transform.childCount == 0) || (this.childIndex.Value < 0))) ? null : go.transform.GetChild(this.childIndex.Value % go.transform.childCount).gameObject);
        }

        public override void OnEnter()
        {
            this.store.set_Value(this.DoGetChildNum(base.Fsm.GetOwnerDefaultTarget(this.gameObject)));
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.childIndex = 0;
            this.store = null;
        }
    }
}

