namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Activates a Camera in the scene.")]
    public class CutToCamera : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The Camera to activate.")]
        public Camera camera;
        [HutongGames.PlayMaker.Tooltip("Makes the camera the new MainCamera. The old MainCamera will be untagged.")]
        public bool makeMainCamera;
        [HutongGames.PlayMaker.Tooltip("Cut back to the original MainCamera when exiting this state.")]
        public bool cutBackOnExit;
        private Camera oldCamera;

        public override void OnEnter()
        {
            if (this.camera == null)
            {
                base.LogError("Missing camera!");
            }
            else
            {
                this.oldCamera = Camera.main;
                SwitchCamera(Camera.main, this.camera);
                if (this.makeMainCamera)
                {
                    this.camera.tag = "MainCamera";
                }
                base.Finish();
            }
        }

        public override void OnExit()
        {
            if (this.cutBackOnExit)
            {
                SwitchCamera(this.camera, this.oldCamera);
            }
        }

        public override void Reset()
        {
            this.camera = null;
            this.makeMainCamera = true;
            this.cutBackOnExit = false;
        }

        private static void SwitchCamera(Camera camera1, Camera camera2)
        {
            if (camera1 != null)
            {
                camera1.enabled = false;
            }
            if (camera2 != null)
            {
                camera2.enabled = true;
            }
        }
    }
}

