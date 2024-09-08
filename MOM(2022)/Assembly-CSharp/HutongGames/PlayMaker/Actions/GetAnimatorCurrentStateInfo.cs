using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Gets the current State information on a specified layer")]
    public class GetAnimatorCurrentStateInfo : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The layer's index")]
        public FsmInt layerIndex;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The layer's name.")]
        public FsmString name;

        [UIHint(UIHint.Variable)]
        [Tooltip("The layer's name Hash. Obsolete in Unity 5, use fullPathHash or shortPathHash instead, nameHash will be the same as shortNameHash for legacy")]
        public FsmInt nameHash;

        [UIHint(UIHint.Variable)]
        [Tooltip("The full path hash for this state.")]
        public FsmInt fullPathHash;

        [UIHint(UIHint.Variable)]
        [Tooltip("The name Hash. Doest not include the parent layer's name")]
        public FsmInt shortPathHash;

        [UIHint(UIHint.Variable)]
        [Tooltip("The layer's tag hash")]
        public FsmInt tagHash;

        [UIHint(UIHint.Variable)]
        [Tooltip("Is the state looping. All animations in the state must be looping")]
        public FsmBool isStateLooping;

        [UIHint(UIHint.Variable)]
        [Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
        public FsmFloat length;

        [UIHint(UIHint.Variable)]
        [Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
        public FsmFloat normalizedTime;

        [UIHint(UIHint.Variable)]
        [Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
        public FsmInt loopCount;

        [UIHint(UIHint.Variable)]
        [Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
        public FsmFloat currentLoopProgress;

        private Animator _animator;

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
            this.GetLayerInfo();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.GetLayerInfo();
        }

        private void GetLayerInfo()
        {
            if (this._animator != null)
            {
                AnimatorStateInfo currentAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
                if (!this.fullPathHash.IsNone)
                {
                    this.fullPathHash.Value = currentAnimatorStateInfo.fullPathHash;
                }
                if (!this.shortPathHash.IsNone)
                {
                    this.shortPathHash.Value = currentAnimatorStateInfo.shortNameHash;
                }
                if (!this.nameHash.IsNone)
                {
                    this.nameHash.Value = currentAnimatorStateInfo.shortNameHash;
                }
                if (!this.name.IsNone)
                {
                    this.name.Value = this._animator.GetLayerName(this.layerIndex.Value);
                }
                if (!this.tagHash.IsNone)
                {
                    this.tagHash.Value = currentAnimatorStateInfo.tagHash;
                }
                if (!this.length.IsNone)
                {
                    this.length.Value = currentAnimatorStateInfo.length;
                }
                if (!this.isStateLooping.IsNone)
                {
                    this.isStateLooping.Value = currentAnimatorStateInfo.loop;
                }
                if (!this.normalizedTime.IsNone)
                {
                    this.normalizedTime.Value = currentAnimatorStateInfo.normalizedTime;
                }
                if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
                {
                    this.loopCount.Value = (int)Math.Truncate(currentAnimatorStateInfo.normalizedTime);
                    this.currentLoopProgress.Value = currentAnimatorStateInfo.normalizedTime - (float)this.loopCount.Value;
                }
            }
        }
    }
}
