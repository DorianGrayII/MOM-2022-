namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the position, rotation and weights of an IK goal. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
    public class SetAnimatorIKGoal : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The IK goal")]
        public AvatarIKGoal iKGoal;
        [HutongGames.PlayMaker.Tooltip("The gameObject target of the ik goal")]
        public FsmGameObject goal;
        [HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
        public FsmVector3 position;
        [HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
        public FsmQuaternion rotation;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
        public FsmFloat positionWeight;
        [HasFloatSlider(0f, 1f), HutongGames.PlayMaker.Tooltip("Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
        public FsmFloat rotationWeight;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when changing over time.")]
        public bool everyFrame;
        private Animator _animator;
        private Transform _transform;

        public override void DoAnimatorIK(int layerIndex)
        {
            this.DoSetIKGoal();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetIKGoal()
        {
            if (this._animator != null)
            {
                if (this._transform == null)
                {
                    if (!this.position.IsNone)
                    {
                        this._animator.SetIKPosition(this.iKGoal, this.position.get_Value());
                    }
                    if (!this.rotation.IsNone)
                    {
                        this._animator.SetIKRotation(this.iKGoal, this.rotation.get_Value());
                    }
                }
                else
                {
                    if (this.position.IsNone)
                    {
                        this._animator.SetIKPosition(this.iKGoal, this._transform.position);
                    }
                    else
                    {
                        this._animator.SetIKPosition(this.iKGoal, this._transform.position + this.position.get_Value());
                    }
                    if (this.rotation.IsNone)
                    {
                        this._animator.SetIKRotation(this.iKGoal, this._transform.rotation);
                    }
                    else
                    {
                        this._animator.SetIKRotation(this.iKGoal, this._transform.rotation * this.rotation.get_Value());
                    }
                }
                if (!this.positionWeight.IsNone)
                {
                    this._animator.SetIKPositionWeight(this.iKGoal, this.positionWeight.Value);
                }
                if (!this.rotationWeight.IsNone)
                {
                    this._animator.SetIKRotationWeight(this.iKGoal, this.rotationWeight.Value);
                }
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
                    GameObject obj3 = this.goal.get_Value();
                    if (obj3 != null)
                    {
                        this._transform = obj3.transform;
                    }
                }
            }
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleAnimatorIK = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.goal = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.position = vector1;
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.rotation = quaternion1;
            this.positionWeight = 1f;
            this.rotationWeight = 1f;
            this.everyFrame = false;
        }
    }
}

