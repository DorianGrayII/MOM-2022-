using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns the name of a layer from its index")]
    public class GetAnimatorLayerName : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer index")]
        public FsmInt layerIndex;

        [ActionSection("Results")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The layer name")]
        public FsmString layerName;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.layerIndex = null;
            this.layerName = null;
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
            this.DoGetLayerName();
            base.Finish();
        }

        private void DoGetLayerName()
        {
            if (!(this._animator == null))
            {
                this.layerName.Value = this._animator.GetLayerName(this.layerIndex.Value);
            }
        }
    }
}
