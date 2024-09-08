﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Smoothly Rotates a Game Object so its forward vector points at a Target. The target can be defined as a Game Object or a world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
    public class SmoothLookAt : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate to face a target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("A target GameObject.")]
        public FsmGameObject targetObject;
        [HutongGames.PlayMaker.Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector3 targetPosition;
        [HutongGames.PlayMaker.Tooltip("Used to keep the game object generally upright. If left undefined the world y axis is used.")]
        public FsmVector3 upVector;
        [HutongGames.PlayMaker.Tooltip("Force the game object to remain vertical. Useful for characters.")]
        public FsmBool keepVertical;
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
                if ((obj3 != null) || !this.targetPosition.IsNone)
                {
                    if (this.previousGo != ownerDefaultTarget)
                    {
                        this.lastRotation = ownerDefaultTarget.transform.rotation;
                        this.desiredRotation = this.lastRotation;
                        this.previousGo = ownerDefaultTarget;
                    }
                    Vector3 end = (obj3 == null) ? this.targetPosition.get_Value() : (!this.targetPosition.IsNone ? obj3.transform.TransformPoint(this.targetPosition.get_Value()) : obj3.transform.position);
                    if (this.keepVertical.Value)
                    {
                        end.y = ownerDefaultTarget.transform.position.y;
                    }
                    Vector3 forward = end - ownerDefaultTarget.transform.position;
                    if ((forward != Vector3.zero) && (forward.sqrMagnitude > 0f))
                    {
                        this.desiredRotation = Quaternion.LookRotation(forward, this.upVector.IsNone ? Vector3.up : this.upVector.get_Value());
                    }
                    this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
                    ownerDefaultTarget.transform.rotation = this.lastRotation;
                    if (this.debug.Value)
                    {
                        Debug.DrawLine(ownerDefaultTarget.transform.position, end, Color.grey);
                    }
                    if ((this.finishEvent != null) && (Mathf.Abs(Vector3.Angle(end - ownerDefaultTarget.transform.position, ownerDefaultTarget.transform.forward)) <= this.finishTolerance.Value))
                    {
                        base.Fsm.Event(this.finishEvent);
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
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.targetPosition = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.upVector = vector2;
            this.keepVertical = true;
            this.debug = false;
            this.speed = 5f;
            this.finishTolerance = 1f;
            this.finishEvent = null;
        }
    }
}

