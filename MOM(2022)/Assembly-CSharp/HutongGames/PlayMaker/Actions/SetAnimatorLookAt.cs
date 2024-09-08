using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Sets look at position and weights. A GameObject can be set to control the look at position, or it can be manually expressed.")]
    public class SetAnimatorLookAt : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The target. An Animator component is required.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The gameObject to look at")]
        public FsmGameObject target;

        [Tooltip("The look-at position. If Target GameObject set, targetPosition is used as an offset from Target")]
        public FsmVector3 targetPosition;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("The global weight of the LookAt, multiplier for other parameters. Range from 0 to 1")]
        public FsmFloat weight;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("determines how much the body is involved in the LookAt. Range from 0 to 1")]
        public FsmFloat bodyWeight;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("determines how much the head is involved in the LookAt. Range from 0 to 1")]
        public FsmFloat headWeight;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("determines how much the eyes are involved in the LookAt. Range from 0 to 1")]
        public FsmFloat eyesWeight;

        [HasFloatSlider(0f, 1f)]
        [Tooltip("0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
        public FsmFloat clampWeight;

        [Tooltip("Repeat every frame during OnAnimatorIK(). Useful for changing over time.")]
        public bool everyFrame;

        private Animator _animator;

        private Transform _transform;

        public override void Reset()
        {
            this.gameObject = null;
            this.target = null;
            this.targetPosition = new FsmVector3
            {
                UseVariable = true
            };
            this.weight = 1f;
            this.bodyWeight = 0.3f;
            this.headWeight = 0.6f;
            this.eyesWeight = 1f;
            this.clampWeight = 0.5f;
            this.everyFrame = false;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleAnimatorIK = true;
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
            GameObject value = this.target.Value;
            if (value != null)
            {
                this._transform = value.transform;
            }
        }

        public override void DoAnimatorIK(int layerIndex)
        {
            this.DoSetLookAt();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetLookAt()
        {
            if (this._animator == null)
            {
                return;
            }
            if (this._transform != null)
            {
                if (this.targetPosition.IsNone)
                {
                    this._animator.SetLookAtPosition(this._transform.position);
                }
                else
                {
                    this._animator.SetLookAtPosition(this._transform.position + this.targetPosition.Value);
                }
            }
            else if (!this.targetPosition.IsNone)
            {
                this._animator.SetLookAtPosition(this.targetPosition.Value);
            }
            if (!this.clampWeight.IsNone)
            {
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value, this.clampWeight.Value);
            }
            else if (!this.eyesWeight.IsNone)
            {
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value);
            }
            else if (!this.headWeight.IsNone)
            {
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value);
            }
            else if (!this.bodyWeight.IsNone)
            {
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value);
            }
            else if (!this.weight.IsNone)
            {
                this._animator.SetLookAtWeight(this.weight.Value);
            }
        }
    }
}
