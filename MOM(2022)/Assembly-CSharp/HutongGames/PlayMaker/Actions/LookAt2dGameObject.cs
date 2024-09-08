using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a Target.")]
    public class LookAt2dGameObject : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The GameObject to Look At.")]
        public FsmGameObject targetObject;

        [Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;

        [Title("Draw Debug Line")]
        [Tooltip("Draw a debug line from the GameObject to the Target.")]
        public FsmBool debug;

        [Tooltip("Color to use for the debug line.")]
        public FsmColor debugLineColor;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame = true;

        private GameObject go;

        private GameObject goTarget;

        public override void Reset()
        {
            this.gameObject = null;
            this.targetObject = null;
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
            this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            this.goTarget = this.targetObject.Value;
            if (!(this.go == null) && this.targetObject != null)
            {
                Vector3 vector = this.goTarget.transform.position - this.go.transform.position;
                vector.Normalize();
                float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
                this.go.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
                if (this.debug.Value)
                {
                    Debug.DrawLine(this.go.transform.position, this.goTarget.transform.position, this.debugLineColor.Value);
                }
            }
        }
    }
}
