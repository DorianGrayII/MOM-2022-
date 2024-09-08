namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Check the active Transition name on a specified layer. Format is 'CURRENT_STATE -> NEXT_STATE'.")]
    public class GetAnimatorCurrentTransitionInfoIsName : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [HutongGames.PlayMaker.Tooltip("The name to check the transition against.")]
        public FsmString name;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if name matches")]
        public FsmBool nameMatch;
        [HutongGames.PlayMaker.Tooltip("Event send if name matches")]
        public FsmEvent nameMatchEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if name doesn't match")]
        public FsmEvent nameDoNotMatchEvent;
        private Animator _animator;

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

        public override void OnActionUpdate()
        {
            this.IsName();
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
                    this.IsName();
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
            this.name = null;
            this.nameMatch = null;
            this.nameMatchEvent = null;
            this.nameDoNotMatchEvent = null;
        }
    }
}

