namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the position, rotation and weights of an IK goal. A GameObject can be set to use for the position and rotation")]
    public class GetAnimatorIKGoal : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The IK goal"), ObjectType(typeof(AvatarIKGoal))]
        public FsmEnum iKGoal;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The gameObject to apply ik goal position and rotation to")]
        public FsmGameObject goal;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Gets The position of the ik goal. If Goal GameObject define, position is used as an offset from Goal")]
        public FsmVector3 position;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Gets The rotation of the ik goal.If Goal GameObject define, rotation is used as an offset from Goal")]
        public FsmQuaternion rotation;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Gets The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
        public FsmFloat positionWeight;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
        public FsmFloat rotationWeight;
        private Animator _animator;
        private Transform _transform;
        private AvatarIKGoal _iKGoal;

        private void DoGetIKGoal()
        {
            if (this._animator != null)
            {
                this._iKGoal = (AvatarIKGoal) this.iKGoal.Value;
                if (this._transform != null)
                {
                    this._transform.position = this._animator.GetIKPosition(this._iKGoal);
                    this._transform.rotation = this._animator.GetIKRotation(this._iKGoal);
                }
                if (!this.position.IsNone)
                {
                    this.position.set_Value(this._animator.GetIKPosition(this._iKGoal));
                }
                if (!this.rotation.IsNone)
                {
                    this.rotation.set_Value(this._animator.GetIKRotation(this._iKGoal));
                }
                if (!this.positionWeight.IsNone)
                {
                    this.positionWeight.Value = this._animator.GetIKPositionWeight(this._iKGoal);
                }
                if (!this.rotationWeight.IsNone)
                {
                    this.rotationWeight.Value = this._animator.GetIKRotationWeight(this._iKGoal);
                }
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetIKGoal();
            if (!base.everyFrame)
            {
                base.Finish();
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

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.iKGoal = null;
            this.goal = null;
            this.position = null;
            this.rotation = null;
            this.positionWeight = null;
            this.rotationWeight = null;
        }
    }
}

