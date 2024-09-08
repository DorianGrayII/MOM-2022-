using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).\n The scale is relative to Unity's Default Avatar")]
    public class GetAnimatorHumanScale : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Result")]
        [UIHint(UIHint.Variable)]
        [Tooltip("the scale of the current Avatar")]
        public FsmFloat humanScale;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.humanScale = null;
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
            this.DoGetHumanScale();
            base.Finish();
        }

        private void DoGetHumanScale()
        {
            if (!(this._animator == null))
            {
                this.humanScale.Value = this._animator.humanScale;
            }
        }
    }
}
