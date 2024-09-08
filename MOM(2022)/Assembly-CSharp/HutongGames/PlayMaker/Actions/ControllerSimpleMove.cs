using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Character)]
    [Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
    public class ControllerSimpleMove : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(CharacterController))]
        [Tooltip("The GameObject to move.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The movement vector.")]
        public FsmVector3 moveVector;

        [Tooltip("Multiply the movement vector by a speed factor.")]
        public FsmFloat speed;

        [Tooltip("Move in local or world space.")]
        public Space space;

        private GameObject previousGo;

        private CharacterController controller;

        public override void Reset()
        {
            this.gameObject = null;
            this.moveVector = new FsmVector3
            {
                UseVariable = true
            };
            this.speed = 1f;
            this.space = Space.World;
        }

        public override void OnUpdate()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                if (ownerDefaultTarget != this.previousGo)
                {
                    this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
                    this.previousGo = ownerDefaultTarget;
                }
                if (this.controller != null)
                {
                    Vector3 vector = ((this.space == Space.World) ? this.moveVector.Value : ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value));
                    this.controller.SimpleMove(vector * this.speed.Value);
                }
            }
        }
    }
}
