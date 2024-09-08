namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns the Animator controller layer count")]
    public class GetAnimatorLayerCount : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Animator controller layer count")]
        public FsmInt layerCount;
        private Animator _animator;

        private void DoGetLayerCount()
        {
            if (this._animator != null)
            {
                this.layerCount.Value = this._animator.layerCount;
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
                    this.DoGetLayerCount();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.layerCount = null;
        }
    }
}

