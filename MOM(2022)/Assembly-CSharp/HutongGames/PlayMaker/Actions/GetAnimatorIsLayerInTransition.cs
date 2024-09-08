using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns true if the specified layer is in a transition. Can also send events")]
    public class GetAnimatorIsLayerInTransition : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("True if automatic matching is active")]
        public FsmBool isInTransition;

        [Tooltip("Event send if automatic matching is active")]
        public FsmEvent isInTransitionEvent;

        [Tooltip("Event send if automatic matching is not active")]
        public FsmEvent isNotInTransitionEvent;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.isInTransition = null;
            this.isInTransitionEvent = null;
            this.isNotInTransitionEvent = null;
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
            this.DoCheckIsInTransition();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoCheckIsInTransition();
        }

        private void DoCheckIsInTransition()
        {
            if (!(this._animator == null))
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
    }
}
