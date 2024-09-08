using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns the Animator controller layer count")]
    public class GetAnimatorLayerCount : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Results")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The Animator controller layer count")]
        public FsmInt layerCount;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.layerCount = null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
                return;
            }
            this._animator = ownerDefaultTarget.GetComponent<Animator>();
            if (this._animator == null)
            {
                base.Finish();
                return;
            }
            this.DoGetLayerCount();
            base.Finish();
        }

        private void DoGetLayerCount()
        {
            if (!(this._animator == null))
            {
                this.layerCount.Value = this._animator.layerCount;
            }
        }
    }
}
