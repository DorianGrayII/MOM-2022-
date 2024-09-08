using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a 2d or 3d position.")]
    public class LookAt2d : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The 2d position to Look At.")]
        public FsmVector2 vector2Target;

        [Tooltip("The 3d position to Look At. If not set to none, will be added to the 2d target")]
        public FsmVector3 vector3Target;

        [Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;

        [Title("Draw Debug Line")]
        [Tooltip("Draw a debug line from the GameObject to the Target.")]
        public FsmBool debug;

        [Tooltip("Color to use for the debug line.")]
        public FsmColor debugLineColor;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame = true;

        public override void Reset()
        {
            this.gameObject = null;
            this.vector2Target = null;
            this.vector3Target = new FsmVector3
            {
                UseVariable = true
            };
            this.debug = false;
            this.debugLineColor = Color.green;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.DoLookAt();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoLookAt();
        }

        private void DoLookAt()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                Vector3 vector = new Vector3(this.vector2Target.Value.x, this.vector2Target.Value.y, 0f);
                if (!this.vector3Target.IsNone)
                {
                    vector += this.vector3Target.Value;
                }
                Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
                vector2.Normalize();
                float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
                ownerDefaultTarget.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
                if (this.debug.Value)
                {
                    Debug.DrawLine(ownerDefaultTarget.transform.position, vector, this.debugLineColor.Value);
                }
            }
        }
    }
}
