namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns true if automatic matching is active. Can also send events")]
    public class GetAnimatorIsMatchingTarget : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if automatic matching is active")]
        public FsmBool isMatchingActive;
        [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is active")]
        public FsmEvent matchingActivatedEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if automatic matching is not active")]
        public FsmEvent matchingDeactivedEvent;
        private Animator _animator;

        private void DoCheckIsMatchingActive()
        {
            if (this._animator != null)
            {
                bool isMatchingTarget = this._animator.isMatchingTarget;
                this.isMatchingActive.Value = isMatchingTarget;
                if (isMatchingTarget)
                {
                    base.Fsm.Event(this.matchingActivatedEvent);
                }
                else
                {
                    base.Fsm.Event(this.matchingDeactivedEvent);
                }
            }
        }

        public override void OnActionUpdate()
        {
            this.DoCheckIsMatchingActive();
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
                    this.DoCheckIsMatchingActive();
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
            this.isMatchingActive = null;
            this.matchingActivatedEvent = null;
            this.matchingDeactivedEvent = null;
        }
    }
}

