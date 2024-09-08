namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the next State information on a specified layer")]
    public class GetAnimatorNextStateInfo : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
        public FsmInt layerIndex;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The layer's name.")]
        public FsmString name;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The layer's name Hash. Obsolete in Unity 5, use fullPathHash or shortPathHash instead, nameHash will be the same as shortNameHash for legacy")]
        public FsmInt nameHash;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The full path hash for this state.")]
        public FsmInt fullPathHash;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The name Hash. Doest not include the parent layer's name")]
        public FsmInt shortPathHash;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The layer's tag hash")]
        public FsmInt tagHash;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Is the state looping. All animations in the state must be looping")]
        public FsmBool isStateLooping;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
        public FsmFloat length;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
        public FsmFloat normalizedTime;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
        public FsmInt loopCount;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
        public FsmFloat currentLoopProgress;
        private Animator _animator;

        private void GetLayerInfo()
        {
            if (this._animator != null)
            {
                AnimatorStateInfo nextAnimatorStateInfo = this._animator.GetNextAnimatorStateInfo(this.layerIndex.Value);
                if (!this.fullPathHash.IsNone)
                {
                    this.fullPathHash.Value = nextAnimatorStateInfo.fullPathHash;
                }
                if (!this.shortPathHash.IsNone)
                {
                    this.shortPathHash.Value = nextAnimatorStateInfo.shortNameHash;
                }
                if (!this.nameHash.IsNone)
                {
                    this.nameHash.Value = nextAnimatorStateInfo.shortNameHash;
                }
                if (!this.name.IsNone)
                {
                    this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
                }
                if (!this.tagHash.IsNone)
                {
                    this.tagHash.Value = nextAnimatorStateInfo.tagHash;
                }
                if (!this.length.IsNone)
                {
                    this.length.Value = nextAnimatorStateInfo.length;
                }
                if (!this.isStateLooping.IsNone)
                {
                    this.isStateLooping.Value = nextAnimatorStateInfo.loop;
                }
                if (!this.normalizedTime.IsNone)
                {
                    this.normalizedTime.Value = nextAnimatorStateInfo.normalizedTime;
                }
                if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
                {
                    this.loopCount.Value = (int) Math.Truncate((double) nextAnimatorStateInfo.normalizedTime);
                    this.currentLoopProgress.Value = nextAnimatorStateInfo.normalizedTime - this.loopCount.Value;
                }
            }
        }

        public override void OnActionUpdate()
        {
            this.GetLayerInfo();
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
                    this.GetLayerInfo();
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
            this.nameHash = null;
            this.fullPathHash = null;
            this.shortPathHash = null;
            this.tagHash = null;
            this.length = null;
            this.normalizedTime = null;
            this.isStateLooping = null;
            this.loopCount = null;
            this.currentLoopProgress = null;
        }
    }
}

