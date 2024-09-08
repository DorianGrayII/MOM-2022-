using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Get the left foot bottom height.")]
    public class GetAnimatorLeftFootBottomHeight : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Result")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("the left foot bottom height.")]
        public FsmFloat leftFootHeight;

        [Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
        public bool everyFrame;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.leftFootHeight = null;
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
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
            this._getLeftFootBottonHeight();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            this._getLeftFootBottonHeight();
        }

        private void _getLeftFootBottonHeight()
        {
            if (this._animator != null)
            {
                this.leftFootHeight.Value = this._animator.leftFeetBottomHeight;
            }
        }
    }
}
