namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the position and rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).\nThe position and rotation are only valid when a frame has being evaluated after the SetTarget call")]
    public class GetAnimatorTarget : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The target position")]
        public FsmVector3 targetPosition;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The target rotation")]
        public FsmQuaternion targetRotation;
        [HutongGames.PlayMaker.Tooltip("If set, apply the position and rotation to this gameObject")]
        public FsmGameObject targetGameObject;
        private Animator _animator;
        private Transform _transform;

        private void DoGetTarget()
        {
            if (this._animator != null)
            {
                this.targetPosition.set_Value(this._animator.targetPosition);
                this.targetRotation.set_Value(this._animator.targetRotation);
                if (this._transform != null)
                {
                    this._transform.position = this._animator.targetPosition;
                    this._transform.rotation = this._animator.targetRotation;
                }
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetTarget();
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
                    GameObject obj3 = this.targetGameObject.get_Value();
                    if (obj3 != null)
                    {
                        this._transform = obj3.transform;
                    }
                    this.DoGetTarget();
                    if (!base.everyFrame)
                    {
                        base.Finish();
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.targetPosition = null;
            this.targetRotation = null;
            this.targetGameObject = null;
            base.everyFrame = false;
        }
    }
}

