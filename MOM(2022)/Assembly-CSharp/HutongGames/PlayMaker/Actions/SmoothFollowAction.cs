namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Action version of Unity's Smooth Follow script.")]
    public class SmoothFollowAction : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The game object to control. E.g. The camera.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The GameObject to follow.")]
        public FsmGameObject targetObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The distance in the x-z plane to the target.")]
        public FsmFloat distance;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The height we want the camera to be above the target")]
        public FsmFloat height;
        [RequiredField, HutongGames.PlayMaker.Tooltip("How much to dampen height movement.")]
        public FsmFloat heightDamping;
        [RequiredField, HutongGames.PlayMaker.Tooltip("How much to dampen rotation changes.")]
        public FsmFloat rotationDamping;
        private GameObject cachedObject;
        private Transform myTransform;
        private GameObject cachedTarget;
        private Transform targetTransform;

        public override void OnLateUpdate()
        {
            if (this.targetObject.get_Value() != null)
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (ownerDefaultTarget != null)
                {
                    if (this.cachedObject != ownerDefaultTarget)
                    {
                        this.cachedObject = ownerDefaultTarget;
                        this.myTransform = ownerDefaultTarget.transform;
                    }
                    if (this.cachedTarget != this.targetObject.get_Value())
                    {
                        this.cachedTarget = this.targetObject.get_Value();
                        this.targetTransform = this.cachedTarget.transform;
                    }
                    float y = Mathf.Lerp(this.myTransform.position.y, this.targetTransform.position.y + this.height.Value, this.heightDamping.Value * Time.deltaTime);
                    Quaternion quaternion = Quaternion.Euler(0f, Mathf.LerpAngle(this.myTransform.eulerAngles.y, this.targetTransform.eulerAngles.y, this.rotationDamping.Value * Time.deltaTime), 0f);
                    this.myTransform.position = this.targetTransform.position;
                    this.myTransform.position -= (quaternion * Vector3.forward) * this.distance.Value;
                    this.myTransform.position = new Vector3(this.myTransform.position.x, y, this.myTransform.position.z);
                    this.myTransform.LookAt(this.targetTransform);
                }
            }
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.targetObject = null;
            this.distance = 10f;
            this.height = 5f;
            this.heightDamping = 2f;
            this.rotationDamping = 3f;
        }
    }
}

