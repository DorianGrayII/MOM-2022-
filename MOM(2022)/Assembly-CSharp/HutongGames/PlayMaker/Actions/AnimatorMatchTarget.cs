using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [Tooltip("Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress")]
    public class AnimatorMatchTarget : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Animator))]
        [Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The body part that is involved in the match")]
        public AvatarTarget bodyPart;

        [Tooltip("The gameObject target to match")]
        public FsmGameObject target;

        [Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
        public FsmVector3 targetPosition;

        [Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
        public FsmQuaternion targetRotation;

        [Tooltip("The MatchTargetWeightMask Position XYZ weight")]
        public FsmVector3 positionWeight;

        [Tooltip("The MatchTargetWeightMask Rotation weight")]
        public FsmFloat rotationWeight;

        [Tooltip("Start time within the animation clip (0 - beginning of clip, 1 - end of clip)")]
        public FsmFloat startNormalizedTime;

        [Tooltip("End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop")]
        public FsmFloat targetNormalizedTime;

        [Tooltip("Should always be true")]
        public bool everyFrame;

        private Animator _animator;

        private Transform _transform;

        public override void Reset()
        {
            this.gameObject = null;
            this.bodyPart = AvatarTarget.Root;
            this.target = null;
            this.targetPosition = new FsmVector3
            {
                UseVariable = true
            };
            this.targetRotation = new FsmQuaternion
            {
                UseVariable = true
            };
            this.positionWeight = Vector3.one;
            this.rotationWeight = 0f;
            this.startNormalizedTime = null;
            this.targetNormalizedTime = null;
            this.everyFrame = true;
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
            this.DoMatchTarget();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoMatchTarget();
        }

        private void DoMatchTarget()
        {
            if (!(this._animator == null))
            {
                Vector3 matchPosition = Vector3.zero;
                Quaternion matchRotation = Quaternion.identity;
                if (this._transform != null)
                {
                    matchPosition = this._transform.position;
                    matchRotation = this._transform.rotation;
                }
                if (!this.targetPosition.IsNone)
                {
                    matchPosition += this.targetPosition.Value;
                }
                if (!this.targetRotation.IsNone)
                {
                    matchRotation *= this.targetRotation.Value;
                }
                MatchTargetWeightMask weightMask = new MatchTargetWeightMask(this.positionWeight.Value, this.rotationWeight.Value);
                this._animator.MatchTarget(matchPosition, matchRotation, this.bodyPart, weightMask, this.startNormalizedTime.Value, this.targetNormalizedTime.Value);
            }
        }
    }
}
