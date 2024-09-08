namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a 2d or 3d position.")]
    public class LookAt2d : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The 2d position to Look At.")]
        public FsmVector2 vector2Target;
        [HutongGames.PlayMaker.Tooltip("The 3d position to Look At. If not set to none, will be added to the 2d target")]
        public FsmVector3 vector3Target;
        [HutongGames.PlayMaker.Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;
        [Title("Draw Debug Line"), HutongGames.PlayMaker.Tooltip("Draw a debug line from the GameObject to the Target.")]
        public FsmBool debug;
        [HutongGames.PlayMaker.Tooltip("Color to use for the debug line.")]
        public FsmColor debugLineColor;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame = true;

        private void DoLookAt()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 end = new Vector3(this.vector2Target.get_Value().x, this.vector2Target.get_Value().y, 0f);
                if (!this.vector3Target.IsNone)
                {
                    end += this.vector3Target.get_Value();
                }
                Vector3 vector2 = end - ownerDefaultTarget.transform.position;
                vector2.Normalize();
                float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
                ownerDefaultTarget.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
                if (this.debug.Value)
                {
                    Debug.DrawLine(ownerDefaultTarget.transform.position, end, this.debugLineColor.get_Value());
                }
            }
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

        public override void Reset()
        {
            this.gameObject = null;
            this.vector2Target = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.vector3Target = vector1;
            this.debug = false;
            this.debugLineColor = (FsmColor) Color.green;
            this.everyFrame = true;
        }
    }
}

