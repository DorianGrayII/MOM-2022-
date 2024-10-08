using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Gets the Child of a GameObject by Index.\nE.g., O to get the first child. HINT: Use this with an integer variable to iterate through children.")]
    public class GetChildNum : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to search.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The index of the child to find.")]
        public FsmInt childIndex;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the child in a GameObject variable.")]
        public FsmGameObject store;

        public override void Reset()
        {
            this.gameObject = null;
            this.childIndex = 0;
            this.store = null;
        }

        public override void OnEnter()
        {
            this.store.Value = this.DoGetChildNum(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
            base.Finish();
        }

        private GameObject DoGetChildNum(GameObject go)
        {
            if (go == null || go.transform.childCount == 0 || this.childIndex.Value < 0)
            {
                return null;
            }
            return go.transform.GetChild(this.childIndex.Value % go.transform.childCount).gameObject;
        }
    }
}
