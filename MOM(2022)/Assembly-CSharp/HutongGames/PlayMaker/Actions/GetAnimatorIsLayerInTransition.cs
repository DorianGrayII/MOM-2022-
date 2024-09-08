namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns true if the specified layer is in a transition. Can also send events")]
    public class GetAnimatorIsLayerInTransition : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if automatic matching is active")]
        public FsmBool isInTransition;
        [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is active")]
        public FsmEvent isInTransitionEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is not active")]
        public FsmEvent isNotInTransitionEvent;
        private Animator _animator;

        private void DoCheckIsInTransition()
        {
            if (this._animator != null)
            {
                bool flag = this._animator.IsInTransition(this.layerIndex.Value);
                if (!this.isInTransition.IsNone)
                {
                    this.isInTransition.Value = flag;
                }
                if (flag)
                {
                    base.Fsm.Event(this.isInTransitionEvent);
                }
                else
                {
                    base.Fsm.Event(this.isNotInTransitionEvent);
                }
            }
        }

        public override void OnActionUpdate()
        {
            this.DoCheckIsInTransition();
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
                    this.DoCheckIsInTransition();
                    if (!base.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.isInTransition = null;
            this.isInTransitionEvent = null;
            this.isNotInTransitionEvent = null;
        }
    }
}

