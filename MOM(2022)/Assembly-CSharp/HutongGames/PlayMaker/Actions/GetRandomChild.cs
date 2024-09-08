using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Gets a Random Child of a Game Object.")]
    public class GetRandomChild : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeResult;

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
        }

        public override void OnEnter()
        {
            this.DoGetRandomChild();
            base.Finish();
        }

        private void DoGetRandomChild()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                int childCount = ownerDefaultTarget.transform.childCount;
                if (childCount != 0)
                {
                    this.storeResult.Value = ownerDefaultTarget.transform.GetChild(Random.Range(0, childCount)).gameObject;
                }
            }
        }
    }
}
