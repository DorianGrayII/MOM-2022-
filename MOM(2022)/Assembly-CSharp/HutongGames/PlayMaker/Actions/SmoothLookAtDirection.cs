using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction. Lets you fire an event when minmagnitude is reached")]
    public class SmoothLookAtDirection : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The direction to smoothly rotate towards.")]
        public FsmVector3 targetDirection;

        [Tooltip("Only rotate if Target Direction Vector length is greater than this threshold.")]
        public FsmFloat minMagnitude;

        [Tooltip("Keep this vector pointing up as the GameObject rotates.")]
        public FsmVector3 upVector;

        [RequiredField]
        [Tooltip("Eliminate any tilt up/down as the GameObject rotates.")]
        public FsmBool keepVertical;

        [RequiredField]
        [HasFloatSlider(0.5f, 15f)]
        [Tooltip("How quickly to rotate.")]
        public FsmFloat speed;

        [Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
        public bool lateUpdate;

        [Tooltip("Event to send if the direction difference is less than Min Magnitude.")]
        public FsmEvent finishEvent;

        [Tooltip("Stop running the action if the direction difference is less than Min Magnitude.")]
        public FsmBool finish;

        private GameObject previousGo;

        private Quaternion lastRotation;

        private Quaternion desiredRotation;

        public override void Reset()
        {
            this.gameObject = null;
            this.targetDirection = new FsmVector3
            {
                UseVariable = true
            };
            this.minMagnitude = 0.1f;
            this.upVector = new FsmVector3
            {
                UseVariable = true
            };
            this.keepVertical = true;
            this.speed = 5f;
            this.lateUpdate = true;
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

        public override void OnUpdate()
        {
            if (!this.lateUpdate)
            {
                this.DoSmoothLookAtDirection();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoSmoothLookAtDirection();
            }
        }

        private void DoSmoothLookAtDirection()
        {
            if (this.targetDirection.IsNone)
            {
                return;
            }
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget == null)
            {
                return;
            }
            if (this.previousGo != ownerDefaultTarget)
            {
                this.lastRotation = ownerDefaultTarget.transform.rotation;
                this.desiredRotation = this.lastRotation;
                this.previousGo = ownerDefaultTarget;
            }
            Vector3 value = this.targetDirection.Value;
            if (this.keepVertical.Value)
            {
                value.y = 0f;
            }
            bool flag = false;
            if (value.sqrMagnitude > this.minMagnitude.Value)
            {
                this.desiredRotation = Quaternion.LookRotation(value, this.upVector.IsNone ? Vector3.up : this.upVector.Value);
            }
            else
            {
                flag = true;
            }
            this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
            ownerDefaultTarget.transform.rotation = this.lastRotation;
            if (flag)
            {
                base.Fsm.Event(this.finishEvent);
                if (this.finish.Value)
                {
                    base.Finish();
                }
            }
        }
    }
}
