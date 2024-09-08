using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Controls culling of this Animator component.\nIf true, set to 'AlwaysAnimate': always animate the entire character. Object is animated even when offscreen.\nIf False, set to 'BasedOnRenderes' or CullUpdateTransforms ( On Unity 5) animation is disabled when renderers are not visible.")]
    public class SetAnimatorCullingMode : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
        public FsmBool alwaysAnimate;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.alwaysAnimate = null;
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
            this.SetCullingMode();
            base.Finish();
        }

        private void SetCullingMode()
        {
            if (!(this._animator == null))
            {
                this._animator.cullingMode = ((!this.alwaysAnimate.Value) ? AnimatorCullingMode.CullUpdateTransforms : AnimatorCullingMode.AlwaysAnimate);
            }
        }
    }
}
