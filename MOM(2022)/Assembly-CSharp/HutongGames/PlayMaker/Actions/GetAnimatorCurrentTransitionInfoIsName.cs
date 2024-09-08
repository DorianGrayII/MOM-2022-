using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Check the active Transition name on a specified layer. Format is 'CURRENT_STATE -> NEXT_STATE'.")]
    public class GetAnimatorCurrentTransitionInfoIsName : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [Tooltip("The name to check the transition against.")]
        public FsmString name;

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
            this.name = null;
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
                if (this._animator.GetAnimatorTransitionInfo(this.layerIndex.Value).IsName(this.name.Value))
                {
                    this.nameMatch.Value = true;
                    base.Fsm.Event(this.nameMatchEvent);
                }
                else
                {
                    this.nameMatch.Value = false;
                    base.Fsm.Event(this.nameDoNotMatchEvent);
                }
            }
        }
    }
}
