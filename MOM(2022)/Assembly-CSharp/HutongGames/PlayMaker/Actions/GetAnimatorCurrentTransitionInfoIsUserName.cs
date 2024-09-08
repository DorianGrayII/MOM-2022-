using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Check the active Transition user-specified name on a specified layer.")]
    public class GetAnimatorCurrentTransitionInfoIsUserName : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [Tooltip("The user-specified name to check the transition against.")]
        public FsmString userName;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("True if name matches")]
        public FsmBool nameMatch;

        [Tooltip("Event send if name matches")]
        public FsmEvent nameMatchEvent;

        [Tooltip("Event send if name doesn't match")]
        public FsmEvent nameDoNotMatchEvent;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.layerIndex = null;
            this.userName = null;
            this.nameMatch = null;
            this.nameMatchEvent = null;
            this.nameDoNotMatchEvent = null;
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
            this.IsName();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.IsName();
        }

        private void IsName()
        {
            if (this._animator != null)
            {
                bool flag = this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsUserName(this.userName.Value);
                if (!this.nameMatch.IsNone)
                {
                    this.nameMatch.Value = flag;
                }
                if (flag)
                {
                    base.Fsm.Event(this.nameMatchEvent);
                }
                else
                {
                    base.Fsm.Event(this.nameDoNotMatchEvent);
                }
            }
        }
    }
}
