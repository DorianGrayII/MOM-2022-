namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the layer's current weight")]
    public class SetAnimatorLayerWeight : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Sets the layer's current weight")]
        public FsmFloat layerWeight;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
        public bool everyFrame;
        private Animator _animator;

        private void DoLayerWeight()
        {
            if (this._animator != null)
            {
                this._animator.SetLayerWeight(this.layerIndex.Value, this.layerWeight.Value);
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
                    this.DoLayerWeight();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoLayerWeight();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.layerIndex = null;
            this.layerWeight = null;
            this.everyFrame = false;
        }
    }
}

