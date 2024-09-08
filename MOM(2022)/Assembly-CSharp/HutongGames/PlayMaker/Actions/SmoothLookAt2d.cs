namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Smoothly Rotates a 2d Game Object so its right vector points at a Target. The target can be defined as a 2d Game Object or a 2d/3d world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
    public class SmoothLookAt2d : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate to face a target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("A target GameObject.")]
        public FsmGameObject targetObject;
        [HutongGames.PlayMaker.Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector2 targetPosition2d;
        [HutongGames.PlayMaker.Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector3 targetPosition;
        [HutongGames.PlayMaker.Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;
        [HasFloatSlider(0.5f, 15f), HutongGames.PlayMaker.Tooltip("How fast the look at moves.")]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("Draw a line in the Scene View to the look at position.")]
        public FsmBool debug;
        [HutongGames.PlayMaker.Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
        public FsmFloat finishTolerance;
        [HutongGames.PlayMaker.Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
        public FsmEvent finishEvent;
        private GameObject previousGo;
        private Quaternion lastRotation;
        private Quaternion desiredRotation;

        private void DoSmoothLookAt()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                GameObject obj3 = this.targetObject.get_Value();
                if (this.previousGo != ownerDefaultTarget)
                {
                    this.lastRotation = ownerDefaultTarget.transform.rotation;
                    this.desiredRotation = this.lastRotation;
                    this.previousGo = ownerDefaultTarget;
                }
                Vector3 end = new Vector3(this.targetPosition2d.get_Value().x, this.targetPosition2d.get_Value().y, 0f);
                if (!this.targetPosition.IsNone)
                {
                    end += this.targetPosition.get_Value();
                }
                if (obj3 != null)
                {
                    end = obj3.transform.position;
                    Vector3 zero = Vector3.zero;
                    if (!this.targetPosition.IsNone)
                    {
                        zero += this.targetPosition.get_Value();
                    }
                    if (!this.targetPosition2d.IsNone)
                    {
                        zero.x += this.targetPosition2d.get_Value().x;
                        zero.y += this.targetPosition2d.get_Value().y;
                    }
                    if (!this.targetPosition2d.IsNone || !this.targetPosition.IsNone)
                    {
                        end += obj3.transform.TransformPoint((Vector3) this.targetPosition2d.get_Value());
                    }
                }
                Vector3 vector2 = end - ownerDefaultTarget.transform.position;
                vector2.Normalize();
                float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
                this.desiredRotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
                this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
                ownerDefaultTarget.transform.rotation = this.lastRotation;
                if (this.debug.Value)
                {
                    Debug.DrawLine(ownerDefaultTarget.transform.position, end, Color.grey);
                }
                if ((this.finishEvent != null) && (Mathf.Abs(Vector3.Angle(this.desiredRotation.eulerAngles, this.lastRotation.eulerAngles)) <= this.finishTolerance.Value))
                {
                    base.Fsm.Event(this.finishEvent);
                }
            }
        }

        public override void OnEnter()
        {
            this.previousGo = null;
        }

        public override void OnLateUpdate()
        {
            this.DoSmoothLookAt();
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.targetObject = null;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.targetPosition2d = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.targetPosition = vector2;
            this.rotationOffset = 0f;
            this.debug = false;
            this.speed = 5f;
            this.finishTolerance = 1f;
            this.finishEvent = null;
        }
    }
}

