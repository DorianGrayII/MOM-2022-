using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the layer's current weight")]
    public class GetAnimatorLayerWeight : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [ActionSection("Results")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The layer's current weight")]
        public FsmFloat layerWeight;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.layerIndex = null;
            this.layerWeight = null;
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
            this.GetLayerWeight();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.GetLayerWeight();
        }

        private void GetLayerWeight()
        {
            if (this._animator != null)
            {
                this.layerWeight.Value = this._animator.GetLayerWeight(this.layerIndex.Value);
            }
        }
    }
}
