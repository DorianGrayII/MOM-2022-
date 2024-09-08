namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
    public class SetAnimatorBody : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The gameObject target of the ik goal")]
        public FsmGameObject target;
        [HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
        public FsmVector3 position;
        [HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
        public FsmQuaternion rotation;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private Animator _animator;
        private Transform _transform;

        public override void DoAnimatorIK(int layerIndex)
        {
            this.DoSetBody();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        private void DoSetBody()
        {
            if (this._animator != null)
            {
                if (this._transform != null)
                {
                    this._animator.bodyPosition = !this.position.IsNone ? (this._transform.position + this.position.get_Value()) : this._transform.position;
                    if (this.rotation.IsNone)
                    {
                        this._animator.bodyRotation = this._transform.rotation;
                    }
                    else
                    {
                        this._animator.bodyRotation = this._transform.rotation * this.rotation.get_Value();
                    }
                }
                else
                {
                    if (!this.position.IsNone)
                    {
                        this._animator.bodyPosition = this.position.get_Value();
                    }
                    if (!this.rotation.IsNone)
                    {
                        this._animator.bodyRotation = this.rotation.get_Value();
                    }
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
                    GameObject obj3 = this.target.get_Value();
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
            this.target = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.position = vector1;
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.rotation = quaternion1;
            this.everyFrame = false;
        }
    }
}

