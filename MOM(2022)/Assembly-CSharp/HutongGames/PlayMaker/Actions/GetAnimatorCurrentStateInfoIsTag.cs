using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Does tag match the tag of the active state in the statemachine")]
    public class GetAnimatorCurrentStateInfoIsTag : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [Tooltip("The tag to check the layer against.")]
        public FsmString tag;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("True if tag matches")]
        public FsmBool tagMatch;

        [Tooltip("Event send if tag matches")]
        public FsmEvent tagMatchEvent;

        [Tooltip("Event send if tag matches")]
        public FsmEvent tagDoNotMatchEvent;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.layerIndex = null;
            this.tag = null;
            this.tagMatch = null;
            this.tagMatchEvent = null;
            this.tagDoNotMatchEvent = null;
            base.everyFrame = false;
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
            this.IsTag();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.IsTag();
        }

        private void IsTag()
        {
            if (this._animator != null)
            {
                if (this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value).IsTag(this.tag.Value))
                {
                    this.tagMatch.Value = true;
                    base.Fsm.Event(this.tagMatchEvent);
                }
                else
                {
                    this.tagMatch.Value = false;
                    base.Fsm.Event(this.tagDoNotMatchEvent);
                }
            }
        }
    }
}
