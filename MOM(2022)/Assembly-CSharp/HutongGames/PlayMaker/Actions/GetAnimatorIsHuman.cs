namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Returns true if the current rig is humanoid, false if it is generic. Can also sends events")]
    public class GetAnimatorIsHuman : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if the current rig is humanoid, False if it is generic")]
        public FsmBool isHuman;
        [HutongGames.PlayMaker.Tooltip("Event send if rig is humanoid")]
        public FsmEvent isHumanEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if rig is generic")]
        public FsmEvent isGenericEvent;
        private Animator _animator;

        private void DoCheckIsHuman()
        {
            if (this._animator != null)
            {
                bool isHuman = this._animator.isHuman;
                if (!this.isHuman.IsNone)
                {
                    this.isHuman.Value = isHuman;
                }
                if (isHuman)
                {
                    base.Fsm.Event(this.isHumanEvent);
                }
                else
                {
                    base.Fsm.Event(this.isGenericEvent);
                }
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
                    this.DoCheckIsHuman();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isHuman = null;
            this.isHumanEvent = null;
            this.isGenericEvent = null;
        }
    }
}

