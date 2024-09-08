namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Does tag match the tag of the active state in the statemachine")]
    public class GetAnimatorCurrentStateInfoIsTag : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [HutongGames.PlayMaker.Tooltip("The tag to check the layer against.")]
        public FsmString tag;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if tag matches")]
        public FsmBool tagMatch;
        [HutongGames.PlayMaker.Tooltip("Event send if tag matches")]
        public FsmEvent tagMatchEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if tag matches")]
        public FsmEvent tagDoNotMatchEvent;
        private Animator _animator;

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

        public override void OnActionUpdate()
        {
            this.IsTag();
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
                    this.IsTag();
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
            this.layerIndex = null;
            this.tag = null;
            this.tagMatch = null;
            this.tagMatchEvent = null;
            this.tagDoNotMatchEvent = null;
            base.everyFrame = false;
        }
    }
}

