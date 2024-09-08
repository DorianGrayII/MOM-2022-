using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Returns the pivot weight and/or position. The pivot is the most stable point between the avatar's left and right foot.\n For a weight value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
    public class GetAnimatorPivot : FsmStateActionAnimatorBase
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [ActionSection("Results")]
        [UIHint(UIHint.Variable)]
        [Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
        public FsmFloat pivotWeight;

        [UIHint(UIHint.Variable)]
        [Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
        public FsmVector3 pivotPosition;

        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.pivotWeight = null;
            this.pivotPosition = null;
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
            this.DoCheckPivot();
            if (!base.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnActionUpdate()
        {
            this.DoCheckPivot();
        }

        private void DoCheckPivot()
        {
            if (!(this._animator == null))
            {
                if (!this.pivotWeight.IsNone)
                {
                    this.pivotWeight.Value = this._animator.pivotWeight;
                }
                if (!this.pivotPosition.IsNone)
                {
                    this.pivotPosition.Value = this._animator.pivotPosition;
                }
            }
        }
    }
}
