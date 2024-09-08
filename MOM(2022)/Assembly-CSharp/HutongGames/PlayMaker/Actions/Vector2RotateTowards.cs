using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Vector2)]
    [Tooltip("Rotates a Vector2 direction from Current towards Target.")]
    public class Vector2RotateTowards : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The current direction. This will be the result of the rotation as well.")]
        public FsmVector2 currentDirection;

        [RequiredField]
        [Tooltip("The direction to reach")]
        public FsmVector2 targetDirection;

        [RequiredField]
        [Tooltip("Rotation speed in degrees per second")]
        public FsmFloat rotateSpeed;

        private Vector3 current;

        private Vector3 target;

        public override void Reset()
        {
            this.currentDirection = new FsmVector2
            {
                UseVariable = true
            };
            this.targetDirection = new FsmVector2
            {
                UseVariable = true
            };
            this.rotateSpeed = 360f;
        }

        public override void OnEnter()
        {
            this.current = new Vector3(this.currentDirection.Value.x, this.currentDirection.Value.y, 0f);
            this.target = new Vector3(this.targetDirection.Value.x, this.targetDirection.Value.y, 0f);
        }

        public override void OnUpdate()
        {
            this.current.x = this.currentDirection.Value.x;
            this.current.y = this.currentDirection.Value.y;
            this.current = Vector3.RotateTowards(this.current, this.target, this.rotateSpeed.Value * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
            this.currentDirection.Value = new Vector2(this.current.x, this.current.y);
        }
    }
}
