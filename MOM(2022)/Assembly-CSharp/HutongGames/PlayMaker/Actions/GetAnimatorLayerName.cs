namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns the name of a layer from its index")]
    public class GetAnimatorLayerName : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer index")]
        public FsmInt layerIndex;
        [ActionSection("Results"), RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The layer name")]
        public FsmString layerName;
        private Animator _animator;

        private void DoGetLayerName()
        {
            if (this._animator != null)
            {
                this.layerName.Value = this._animator.GetLayerName(this.layerIndex.Value);
            }
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                base.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if (this._animator == null)
                {
                    base.Finish();
                }
                else
                {
                    this.DoGetLayerName();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.layerIndex = null;
            this.layerName = null;
        }
    }
}

