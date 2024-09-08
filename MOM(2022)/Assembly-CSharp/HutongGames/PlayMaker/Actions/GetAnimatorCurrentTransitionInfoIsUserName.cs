namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Check the active Transition user-specified name on a specified layer.")]
    public class GetAnimatorCurrentTransitionInfoIsUserName : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [HutongGames.PlayMaker.Tooltip("The user-specified name to check the transition against.")]
        public FsmString userName;
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
            this.userName = null;
            this.nameMatch = null;
            this.nameMatchEvent = null;
            this.nameDoNotMatchEvent = null;
        }
    }
}

