using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns the feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
    public class GetAnimatorFeetPivotActive : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The feet pivot Blending. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
        public FsmFloat feetPivotActive;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.feetPivotActive = null;
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
            this.DoGetFeetPivotActive();
            base.Finish();
        }

        private void DoGetFeetPivotActive()
        {
            if (!(this._animator == null))
            {
                this.feetPivotActive.Value = this._animator.feetPivotActive;
            }
        }
    }
}
