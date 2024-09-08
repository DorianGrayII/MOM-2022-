using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Gets the number of children that a GameObject has.")]
    public class GetChildCount : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to test.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the number of children in an int variable.")]
        public FsmInt storeResult;

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            this.DoGetChildCount();
            base.Finish();
        }

        private void DoGetChildCount()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                this.storeResult.Value = ownerDefaultTarget.transform.childCount;
            }
        }
    }
}
