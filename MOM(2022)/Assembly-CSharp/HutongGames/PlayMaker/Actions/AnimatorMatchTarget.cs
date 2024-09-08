namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress")]
    public class AnimatorMatchTarget : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The body part that is involved in the match")]
        public AvatarTarget bodyPart;
        [HutongGames.PlayMaker.Tooltip("The gameObject target to match")]
        public FsmGameObject target;
        [HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
        public FsmVector3 targetPosition;
        [HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
        public FsmQuaternion targetRotation;
        [HutongGames.PlayMaker.Tooltip("The MatchTargetWeightMask Position XYZ weight")]
        public FsmVector3 positionWeight;
        [HutongGames.PlayMaker.Tooltip("The MatchTargetWeightMask Rotation weight")]
        public FsmFloat rotationWeight;
        [HutongGames.PlayMaker.Tooltip("Start time within the animation clip (0 - beginning of clip, 1 - end of clip)")]
        public FsmFloat startNormalizedTime;
        [HutongGames.PlayMaker.Tooltip("End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop")]
        public FsmFloat targetNormalizedTime;
        [HutongGames.PlayMaker.Tooltip("Should always be true")]
        public bool everyFrame;
        private Animator _animator;
        private Transform _transform;

        private void DoMatchTarget()
        {
            if (this._animator != null)
            {
                Vector3 zero = Vector3.zero;
                Quaternion identity = Quaternion.identity;
                if (this._transform != null)
                {
                    zero = this._transform.position;
                    identity = this._transform.rotation;
                }
                if (!this.targetPosition.IsNone)
                {
                    zero += this.targetPosition.get_Value();
                }
                if (!this.targetRotation.IsNone)
                {
                    identity *= this.targetRotation.get_Value();
                }
                MatchTargetWeightMask weightMask = new MatchTargetWeightMask(this.positionWeight.get_Value(), this.rotationWeight.Value);
                this._animator.MatchTarget(zero, identity, this.bodyPart, weightMask, this.startNormalizedTime.Value, this.targetNormalizedTime.Value);
            }
        }

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
                    GameObject obj3 = this.target.get_Value();
                    if (obj3 != null)
                    {
                        this._transform = obj3.transform;
                    }
                    this.DoMatchTarget();
                    if (!this.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void OnUpdate()
        {
            this.DoMatchTarget();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.bodyPart = AvatarTarget.Root;
            this.target = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.targetPosition = vector1;
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.targetRotation = quaternion1;
            this.positionWeight = (FsmVector3) Vector3.one;
            this.rotationWeight = 0f;
            this.startNormalizedTime = null;
            this.targetNormalizedTime = null;
            this.everyFrame = true;
        }
    }
}

