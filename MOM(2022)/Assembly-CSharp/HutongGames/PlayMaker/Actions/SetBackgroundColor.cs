namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Sets the Background Color used by the Camera.")]
    public class SetBackgroundColor : ComponentAction<Camera>
    {
        [RequiredField, CheckForComponent(typeof(Camera))]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        public FsmColor backgroundColor;
        public bool everyFrame;

        private void DoSetBackgroundColor()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.camera.backgroundColor = this.backgroundColor.get_Value();
            }
        }

        public override void OnEnter()
        {
            this.DoSetBackgroundColor();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoSetBackgroundColor();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.backgroundColor = (FsmColor) Color.black;
            this.everyFrame = false;
        }
    }
}

