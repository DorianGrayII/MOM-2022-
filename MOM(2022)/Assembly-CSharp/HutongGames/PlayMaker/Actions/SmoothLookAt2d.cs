using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Smoothly Rotates a 2d Game Object so its right vector points at a Target. The target can be defined as a 2d Game Object or a 2d/3d world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
    public class SmoothLookAt2d : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to rotate to face a target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("A target GameObject.")]
        public FsmGameObject targetObject;

        [Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector2 targetPosition2d;

        [Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector3 targetPosition;

        [Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;

        [HasFloatSlider(0.5f, 15f)]
        [Tooltip("How fast the look at moves.")]
        public FsmFloat speed;

        [Tooltip("Draw a line in the Scene View to the look at position.")]
        public FsmBool debug;

        [Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
        public FsmFloat finishTolerance;

        [Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
        public FsmEvent finishEvent;

        private GameObject previousGo;

        private Quaternion lastRotation;

        private Quaternion desiredRotation;

        public override void Reset()
        {
            this.gameObject = null;
            this.targetObject = null;
            this.targetPosition2d = new FsmVector2
            {
                UseVariable = true
            };
            this.targetPosition = new FsmVector3
            {
                UseVariable = true
            };
            this.rotationOffset = 0f;
            this.debug = false;
            this.speed = 5f;
            this.finishTolerance = 1f;
            this.finishEvent = null;
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
        }

        public override void OnEnter()
        {
            this.previousGo = null;
        }

        public override void OnLateUpdate()
        {
            this.DoSmoothLookAt();
        }

        private void DoSmoothLookAt()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                return;
            }
            GameObject value = this.targetObject.Value;
            if (this.previousGo != ownerDefaultTarget)
            {
                this.lastRotation = ownerDefaultTarget.transform.rotation;
                this.desiredRotation = this.lastRotation;
                this.previousGo = ownerDefaultTarget;
            }
            Vector3 vector = new Vector3(this.targetPosition2d.Value.x, this.targetPosition2d.Value.y, 0f);
            if (!this.targetPosition.IsNone)
            {
                vector += this.targetPosition.Value;
            }
            if (value != null)
            {
                vector = value.transform.position;
                Vector3 zero = Vector3.zero;
                if (!this.targetPosition.IsNone)
                {
                    zero += this.targetPosition.Value;
                }
                if (!this.targetPosition2d.IsNone)
                {
                    zero.x += this.targetPosition2d.Value.x;
                    zero.y += this.targetPosition2d.Value.y;
                }
                if (!this.targetPosition2d.IsNone || !this.targetPosition.IsNone)
                {
                    vector += value.transform.TransformPoint(this.targetPosition2d.Value);
                }
            }
            Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
            vector2.Normalize();
            float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
            this.desiredRotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
            this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
            ownerDefaultTarget.transform.rotation = this.lastRotation;
            if (this.debug.Value)
            {
                Debug.DrawLine(ownerDefaultTarget.transform.position, vector, Color.grey);
            }
            if (this.finishEvent != null && Mathf.Abs(Vector3.Angle(this.desiredRotation.eulerAngles, this.lastRotation.eulerAngles)) <= this.finishTolerance.Value)
            {
                base.Fsm.Event(this.finishEvent);
            }
        }
    }
}
