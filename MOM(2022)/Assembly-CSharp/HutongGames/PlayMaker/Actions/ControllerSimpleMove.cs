namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
    public class ControllerSimpleMove : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(CharacterController)), HutongGames.PlayMaker.Tooltip("The GameObject to move.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The movement vector.")]
        public FsmVector3 moveVector;
        [HutongGames.PlayMaker.Tooltip("Multiply the movement vector by a speed factor.")]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("Move in local or world space.")]
        public Space space;
        private GameObject previousGo;
        private CharacterController controller;

        public override void OnUpdate()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                if (ownerDefaultTarget != this.previousGo)
                {
                    this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
                    this.previousGo = ownerDefaultTarget;
                }
                if (this.controller != null)
                {
                    Vector3 vector = (this.space == Space.World) ? this.moveVector.get_Value() : ownerDefaultTarget.transform.TransformDirection(this.moveVector.get_Value());
                    this.controller.SimpleMove(vector * this.speed.Value);
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.moveVector = vector1;
            this.speed = 1f;
            this.space = Space.World;
        }
    }
}

