namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Rotates a Vector3 direction from Current towards Target.")]
    public class Vector3RotateTowards : FsmStateAction
    {
        [RequiredField]
        public FsmVector3 currentDirection;
        [RequiredField]
        public FsmVector3 targetDirection;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Rotation speed in degrees per second")]
        public FsmFloat rotateSpeed;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Max Magnitude per second")]
        public FsmFloat maxMagnitude;

        public override void OnUpdate()
        {
            this.currentDirection.set_Value(Vector3.RotateTowards(this.currentDirection.get_Value(), this.targetDirection.get_Value(), (this.rotateSpeed.Value * 0.01745329f) * Time.deltaTime, this.maxMagnitude.Value));
        }

        public override void Reset()
        {
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.currentDirection = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.targetDirection = vector2;
            this.rotateSpeed = 360f;
            this.maxMagnitude = 1f;
        }
    }
}

