namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction. Lets you fire an event when minmagnitude is reached")]
    public class SmoothLookAtDirection : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The direction to smoothly rotate towards.")]
        public FsmVector3 targetDirection;
        [HutongGames.PlayMaker.Tooltip("Only rotate if Target Direction Vector length is greater than this threshold.")]
        public FsmFloat minMagnitude;
        [HutongGames.PlayMaker.Tooltip("Keep this vector pointing up as the GameObject rotates.")]
        public FsmVector3 upVector;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Eliminate any tilt up/down as the GameObject rotates.")]
        public FsmBool keepVertical;
        [RequiredField, HasFloatSlider(0.5f, 15f), HutongGames.PlayMaker.Tooltip("How quickly to rotate.")]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
        public bool lateUpdate;
        [HutongGames.PlayMaker.Tooltip("Event to send if the direction difference is less than Min Magnitude.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Stop running the action if the direction difference is less than Min Magnitude.")]
        public FsmBool finish;
        private GameObject previousGo;
        private Quaternion lastRotation;
        private Quaternion desiredRotation;

        private void DoSmoothLookAtDirection()
        {
            if (!this.targetDirection.IsNone)
            {
                GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
                if (ownerDefaultTarget != null)
                {
                    if (this.previousGo != ownerDefaultTarget)
                    {
                        this.lastRotation = ownerDefaultTarget.transform.rotation;
                        this.desiredRotation = this.lastRotation;
                        this.previousGo = ownerDefaultTarget;
                    }
                    Vector3 forward = this.targetDirection.get_Value();
                    if (this.keepVertical.Value)
                    {
                        forward.y = 0f;
                    }
                    bool flag = false;
                    if (forward.sqrMagnitude > this.minMagnitude.Value)
                    {
                        this.desiredRotation = Quaternion.LookRotation(forward, this.upVector.IsNone ? Vector3.up : this.upVector.get_Value());
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

        public override void OnEnter()
        {
            this.previousGo = null;
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoSmoothLookAtDirection();
            }
        }

        public override void OnPreprocess()
        {
            base.Fsm.HandleLateUpdate = true;
        }

        public override void OnUpdate()
        {
            if (!this.lateUpdate)
            {
                this.DoSmoothLookAtDirection();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.targetDirection = vector1;
            this.minMagnitude = 0.1f;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.upVector = vector2;
            this.keepVertical = true;
            this.speed = 5f;
            this.lateUpdate = true;
            this.finishEvent = null;
        }
    }
}

