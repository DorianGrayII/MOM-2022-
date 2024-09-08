namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GUIElement), HutongGames.PlayMaker.Tooltip("Performs a Hit Test on a Game Object with a GUITexture or GUIText component."), Obsolete("GUIElement is part of the legacy UI system removed in 2019.3")]
    public class GUIElementHitTest : FsmStateAction
    {
        [RequiredField, ActionSection("Obsolete. Use Unity UI instead."), HutongGames.PlayMaker.Tooltip("The GameObject that has a GUITexture or GUIText component.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Specify camera or use MainCamera as default.")]
        public Camera camera;
        [HutongGames.PlayMaker.Tooltip("A vector position on screen. Usually stored by actions like GetTouchInfo, or World To Screen Point.")]
        public FsmVector3 screenPoint;
        [HutongGames.PlayMaker.Tooltip("Specify screen X coordinate.")]
        public FsmFloat screenX;
        [HutongGames.PlayMaker.Tooltip("Specify screen Y coordinate.")]
        public FsmFloat screenY;
        [HutongGames.PlayMaker.Tooltip("Whether the specified screen coordinates are normalized (0-1).")]
        public FsmBool normalized;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Hit Test is true.")]
        public FsmEvent hitEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result of the Hit Test in a bool variable (true/false).")]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you want to wait for the hit test to return true.")]
        public FsmBool everyFrame;
        private GameObject gameObjectCached;

        public override void OnEnter()
        {
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.camera = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.screenPoint = vector1;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.screenX = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.screenY = num2;
            this.normalized = true;
            this.hitEvent = null;
            this.everyFrame = true;
        }
    }
}

