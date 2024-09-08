using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Action version of Unity's Smooth Follow script.")]
    public class SmoothFollowAction : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The game object to control. E.g. The camera.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The GameObject to follow.")]
        public FsmGameObject targetObject;

        [RequiredField]
        [Tooltip("The distance in the x-z plane to the target.")]
        public FsmFloat distance;

        [RequiredField]
        [Tooltip("The height we want the camera to be above the target")]
        public FsmFloat height;

        [RequiredField]
        [Tooltip("How much to dampen height movement.")]
        public FsmFloat heightDamping;

        [RequiredField]
        [Tooltip("How much to dampen rotation changes.")]
        public FsmFloat rotationDamping;

        private GameObject cachedObject;

        private Transform myTransform;

        private GameObject cachedTarget;

        private Transform targetTransform;

        public override void Reset()
        {
            this.gameObject = null;
            this.targetObject = null;
            this.distance = 10f;
            this.height = 5f;
            this.heightDamping = 2f;
            this.rotationDamping = 3f;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
        }

        public override void OnLateUpdate()
        {
            if (this.targetObject.Value == null)
            {
                return;
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                if (this.cachedObject != ownerDefaultTarget)
                {
                    this.cachedObject = ownerDefaultTarget;
                    this.myTransform = ownerDefaultTarget.transform;
                }
                if (this.cachedTarget != this.targetObject.Value)
                {
                    this.cachedTarget = this.targetObject.Value;
                    this.targetTransform = this.cachedTarget.transform;
                }
                float y = this.targetTransform.eulerAngles.y;
                float b = this.targetTransform.position.y + this.height.Value;
                float y2 = this.myTransform.eulerAngles.y;
                float y3 = this.myTransform.position.y;
                y2 = Mathf.LerpAngle(y2, y, this.rotationDamping.Value * Time.deltaTime);
                y3 = Mathf.Lerp(y3, b, this.heightDamping.Value * Time.deltaTime);
                Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
                this.myTransform.position = this.targetTransform.position;
                this.myTransform.position -= quaternion * Vector3.forward * this.distance.Value;
                this.myTransform.position = new Vector3(this.myTransform.position.x, y3, this.myTransform.position.z);
                this.myTransform.LookAt(this.targetTransform);
            }
        }
    }
}
