using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("If true, additional layers affects the mass center")]
    public class SetAnimatorLayersAffectMassCenter : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("If true, additional layers affects the mass center")]
        public FsmBool affectMassCenter;

        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = null;
            this.affectMassCenter = null;
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
            this.SetAffectMassCenter();
            base.Finish();
        }

        private void SetAffectMassCenter()
        {
            if (!(this._animator == null))
            {
                this._animator.layersAffectMassCenter = this.affectMassCenter.Value;
            }
        }
    }
}
