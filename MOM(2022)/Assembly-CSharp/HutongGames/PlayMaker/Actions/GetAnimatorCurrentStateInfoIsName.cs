namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
    public class GetAnimatorCurrentStateInfoIsName : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [HutongGames.PlayMaker.Tooltip("The name to check the layer against.")]
        public FsmString name;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("True if name matches")]
        public FsmBool isMatching;
        [HutongGames.PlayMaker.Tooltip("Event send if name matches")]
        public FsmEvent nameMatchEvent;
        [HutongGames.PlayMaker.Tooltip("Event send if name doesn't match")]
        public FsmEvent nameDoNotMatchEvent;
        private Animator _animator;

        private void IsName()
        {
            if (this._animator != null)
            {
                AnimatorStateInfo currentAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
                if (!this.isMatching.IsNone)
                {
                    this.isMatching.Value = currentAnimatorStateInfo.IsName(this.name.Value);
                }
                if (currentAnimatorStateInfo.IsName(this.name.Value))
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
            this.name = null;
            this.nameMatchEvent = null;
            this.nameDoNotMatchEvent = null;
            base.everyFrame = false;
        }
    }
}

