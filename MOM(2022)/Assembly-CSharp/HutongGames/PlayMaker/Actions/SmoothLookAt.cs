using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Smoothly Rotates a Game Object so its forward vector points at a Target. The target can be defined as a Game Object or a world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
    public class SmoothLookAt : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to rotate to face a target.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("A target GameObject.")]
        public FsmGameObject targetObject;

        [Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector3 targetPosition;

        [Tooltip("Used to keep the game object generally upright. If left undefined the world y axis is used.")]
        public FsmVector3 upVector;

        [Tooltip("Force the game object to remain vertical. Useful for characters.")]
        public FsmBool keepVertical;

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
            this.targetPosition = new FsmVector3
            {
                UseVariable = true
            };
            this.upVector = new FsmVector3
            {
                UseVariable = true
            };
            this.keepVertical = true;
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
            if (!(value == null) || !this.targetPosition.IsNone)
            {
                if (this.previousGo != ownerDefaultTarget)
                {
                    this.lastRotation = ownerDefaultTarget.transform.rotation;
                    this.desiredRotation = this.lastRotation;
                    this.previousGo = ownerDefaultTarget;
                }
                Vector3 vector = ((!(value != null)) ? this.targetPosition.Value : ((!this.targetPosition.IsNone) ? value.transform.TransformPoint(this.targetPosition.Value) : value.transform.position));
                if (this.keepVertical.Value)
                {
                    vector.y = ownerDefaultTarget.transform.position.y;
                }
                Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
                if (vector2 != Vector3.zero && vector2.sqrMagnitude > 0f)
                {
                    this.desiredRotation = Quaternion.LookRotation(vector2, this.upVector.IsNone ? Vector3.up : this.upVector.Value);
                }
                this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
                ownerDefaultTarget.transform.rotation = this.lastRotation;
                if (this.debug.Value)
                {
                    Debug.DrawLine(ownerDefaultTarget.transform.position, vector, Color.grey);
                }
                if (this.finishEvent != null && Mathf.Abs(Vector3.Angle(vector - ownerDefaultTarget.transform.position, ownerDefaultTarget.transform.forward)) <= this.finishTolerance.Value)
                {
                    base.Fsm.Event(this.finishEvent);
                }
            }
        }
    }
}
