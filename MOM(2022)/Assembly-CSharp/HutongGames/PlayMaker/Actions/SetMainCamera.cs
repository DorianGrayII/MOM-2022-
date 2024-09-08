namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Camera), HutongGames.PlayMaker.Tooltip("Sets the Main Camera.")]
    public class SetMainCamera : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(Camera)), HutongGames.PlayMaker.Tooltip("The GameObject to set as the main camera (should have a Camera component).")]
        public FsmGameObject gameObject;

        public override void OnEnter()
        {
            if (this.gameObject.get_Value() != null)
            {
                if (Camera.main != null)
                {
                    Camera.main.gameObject.tag = "Untagged";
                }
                this.gameObject.get_Value().tag = "MainCamera";
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
        }
    }
}

