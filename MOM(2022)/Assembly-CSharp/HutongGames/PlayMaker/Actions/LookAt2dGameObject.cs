namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a Target.")]
    public class LookAt2dGameObject : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The GameObject to Look At.")]
        public FsmGameObject targetObject;
        [HutongGames.PlayMaker.Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;
        [Title("Draw Debug Line"), HutongGames.PlayMaker.Tooltip("Draw a debug line from the GameObject to the Target.")]
        public FsmBool debug;
        [HutongGames.PlayMaker.Tooltip("Color to use for the debug line.")]
        public FsmColor debugLineColor;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame = true;
        private GameObject go;
        private GameObject goTarget;

        private void DoLookAt()
        {
            this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            this.goTarget = this.targetObject.get_Value();
            if ((this.go != null) && (this.targetObject != null))
            {
                Vector3 vector = this.goTarget.transform.position - this.go.transform.position;
                vector.Normalize();
                float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
                this.go.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
                if (this.debug.Value)
                {
                    Debug.DrawLine(this.go.transform.position, this.goTarget.transform.position, this.debugLineColor.get_Value());
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
            this.targetObject = null;
            this.debug = false;
            this.debugLineColor = (FsmColor) Color.green;
            this.everyFrame = true;
        }
    }
}

