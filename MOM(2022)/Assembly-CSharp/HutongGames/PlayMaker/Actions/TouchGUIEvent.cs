namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Sends events when a GUI Texture or GUI Text is touched. Optionally filter by a fingerID."), Obsolete("GUIElement is part of the legacy UI system removed in 2019.3")]
    public class TouchGUIEvent : FsmStateAction
    {
        [RequiredField, ActionSection("Obsolete. Use Unity UI instead."), HutongGames.PlayMaker.Tooltip("The Game Object that owns the GUI Texture or GUI Text.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Only detect touches that match this fingerID, or set to None.")]
        public FsmInt fingerId;
        [ActionSection("Events"), HutongGames.PlayMaker.Tooltip("Event to send on touch began.")]
        public FsmEvent touchBegan;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch moved.")]
        public FsmEvent touchMoved;
        [HutongGames.PlayMaker.Tooltip("Event to send on stationary touch.")]
        public FsmEvent touchStationary;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch ended.")]
        public FsmEvent touchEnded;
        [HutongGames.PlayMaker.Tooltip("Event to send on touch cancel.")]
        public FsmEvent touchCanceled;
        [HutongGames.PlayMaker.Tooltip("Event to send if not touching (finger down but not over the GUI element)")]
        public FsmEvent notTouching;
        [ActionSection("Store Results"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the fingerId of the touch.")]
        public FsmInt storeFingerId;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the screen position where the GUI element was touched.")]
        public FsmVector3 storeHitPoint;
        [HutongGames.PlayMaker.Tooltip("Normalize the hit point screen coordinates (0-1).")]
        public FsmBool normalizeHitPoint;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the offset position of the hit.")]
        public FsmVector3 storeOffset;
        [HutongGames.PlayMaker.Tooltip("How to measure the offset.")]
        public OffsetOptions relativeTo;
        [HutongGames.PlayMaker.Tooltip("Normalize the offset.")]
        public FsmBool normalizeOffset;
        [ActionSection(""), HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private Vector3 touchStartPos;

        public override void OnEnter()
        {
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.fingerId = num1;
            this.touchBegan = null;
            this.touchMoved = null;
            this.touchStationary = null;
            this.touchEnded = null;
            this.touchCanceled = null;
            this.storeFingerId = null;
            this.storeHitPoint = null;
            this.normalizeHitPoint = false;
            this.storeOffset = null;
            this.relativeTo = OffsetOptions.Center;
            this.normalizeOffset = true;
            this.everyFrame = true;
        }

        public enum OffsetOptions
        {
            TopLeft,
            Center,
            TouchStart
        }
    }
}

