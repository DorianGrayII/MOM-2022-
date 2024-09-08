using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Sets the Main Camera.")]
    public class SetMainCamera : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Camera))]
        [Tooltip("The GameObject to set as the main camera (should have a Camera component).")]
        public FsmGameObject gameObject;

        public override void Reset()
        {
            this.gameObject = null;
        }

        public override void OnEnter()
        {
            if (this.gameObject.Value != null)
            {
                if (Camera.main != null)
                {
                    Camera.main.gameObject.tag = "Untagged";
                }
                this.gameObject.Value.tag = "MainCamera";
            }
            base.Finish();
        }
    }
}
