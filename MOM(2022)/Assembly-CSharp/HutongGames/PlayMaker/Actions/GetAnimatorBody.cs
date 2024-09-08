namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the avatar body mass center position and rotation. Optionally accepts a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
    public class GetAnimatorBody : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
        public FsmVector3 bodyPosition;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
        public FsmQuaternion bodyRotation;
        [HutongGames.PlayMaker.Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
        public FsmGameObject bodyGameObject;
        private Animator _animator;
        private Transform _transform;

        private void DoGetBodyPosition()
        {
            if (this._animator != null)
            {
                this.bodyPosition.set_Value(this._animator.bodyPosition);
                this.bodyRotation.set_Value(this._animator.bodyRotation);
                if (this._transform != null)
                {
                    this._transform.position = this._animator.bodyPosition;
                    this._transform.rotation = this._animator.bodyRotation;
                }
            }
        }

        public override string ErrorCheck()
        {
            return ((base.everyFrameOption == FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK) ? string.Empty : "Getting Body Position should only be done in OnAnimatorIK");
        }

        public override void OnActionUpdate()
        {
            this.DoGetBodyPosition();
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
                    GameObject obj3 = this.bodyGameObject.get_Value();
                    if (obj3 != null)
                    {
                        this._transform = obj3.transform;
                    }
                    if (base.everyFrameOption != FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK)
                    {
                        base.everyFrameOption = FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK;
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            this.gameObject = null;
            this.bodyPosition = null;
            this.bodyRotation = null;
            this.bodyGameObject = null;
            base.everyFrame = false;
            base.everyFrameOption = FsmStateActionAnimatorBase.AnimatorFrameUpdateSelector.OnAnimatorIK;
        }
    }
}

