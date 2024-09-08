namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Vector2), HutongGames.PlayMaker.Tooltip("Rotates a Vector2 direction from Current towards Target.")]
    public class Vector2RotateTowards : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The current direction. This will be the result of the rotation as well.")]
        public FsmVector2 currentDirection;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The direction to reach")]
        public FsmVector2 targetDirection;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Rotation speed in degrees per second")]
        public FsmFloat rotateSpeed;
        private Vector3 current;
        private Vector3 target;

        public override void OnEnter()
        {
            this.current = new Vector3(this.currentDirection.get_Value().x, this.currentDirection.get_Value().y, 0f);
            this.target = new Vector3(this.targetDirection.get_Value().x, this.targetDirection.get_Value().y, 0f);
        }

        public override void OnUpdate()
        {
            this.current.x = this.currentDirection.get_Value().x;
            this.current.y = this.currentDirection.get_Value().y;
            this.current = Vector3.RotateTowards(this.current, this.target, (this.rotateSpeed.Value * 0.01745329f) * Time.deltaTime, 1000f);
            this.currentDirection.set_Value(new Vector2(this.current.x, this.current.y));
        }

        public override void Reset()
        {
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.currentDirection = vector1;
            FsmVector2 vector2 = new FsmVector2();
            vector2.UseVariable = true;
            this.targetDirection = vector2;
            this.rotateSpeed = 360f;
        }
    }
}

