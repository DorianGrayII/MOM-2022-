namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Animator), HutongGames.PlayMaker.Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
    public class GetAnimatorRoot : FsmStateActionAnimatorBase
    {
        [RequiredField, CheckForComponent(typeof(Animator)), HutongGames.PlayMaker.Tooltip("The target.")]
        public FsmOwnerDefault gameObject;
        [ActionSection("Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
        public FsmVector3 rootPosition;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The avatar body mass center")]
        public FsmQuaternion rootRotation;
        [HutongGames.PlayMaker.Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
        public FsmGameObject bodyGameObject;
        private Animator _animator;
        private Transform _transform;

        private void DoGetBodyPosition()
        {
            if (this._animator != null)
            {
                this.rootPosition.set_Value(this._animator.rootPosition);
                this.rootRotation.set_Value(this._animator.rootRotation);
                if (this._transform != null)
                {
                    this._transform.position = this._animator.rootPosition;
                    this._transform.rotation = this._animator.rootRotation;
                }
            }
        }

        public override void OnActionUpdate()
        {
            this.DoGetBodyPosition();
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
                    this.DoGetBodyPosition();
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
            this.rootPosition = null;
            this.rootRotation = null;
            this.bodyGameObject = null;
        }
    }
}

