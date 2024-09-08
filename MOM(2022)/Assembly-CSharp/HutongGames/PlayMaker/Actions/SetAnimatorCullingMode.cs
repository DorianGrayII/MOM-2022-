namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Controls culling of this Animator component.\nIf true, set to 'AlwaysAnimate': always animate the entire character. Object is animated even when offscreen.\nIf False, set to 'BasedOnRenderes' or CullUpdateTransforms ( On Unity 5) animation is disabled when renderers are not visible.")]
    public class SetAnimatorCullingMode : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
        public FsmBool alwaysAnimate;
        private Animator _animator;

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
                    this.SetCullingMode();
                    base.Finish();
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.alwaysAnimate = null;
        }

        private void SetCullingMode()
        {
            if (this._animator != null)
            {
                this._animator.cullingMode = this.alwaysAnimate.Value ? AnimatorCullingMode.AlwaysAnimate : AnimatorCullingMode.CullUpdateTransforms;
            }
        }
    }
}

