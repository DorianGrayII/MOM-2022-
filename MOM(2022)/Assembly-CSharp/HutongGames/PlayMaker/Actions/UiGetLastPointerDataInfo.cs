using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("Gets pointer data on the last System event.")]
    public class UiGetLastPointerDataInfo : FsmStateAction
    {
        public static PointerEventData lastPointerEventData;

        [Tooltip("Number of clicks in a row.")]
        [UIHint(UIHint.Variable)]
        public FsmInt clickCount;

        [Tooltip("The last time a click event was sent.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat clickTime;

        [Tooltip("Pointer delta since last update.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 delta;

        [Tooltip("Is a drag operation currently occuring.")]
        [UIHint(UIHint.Variable)]
        public FsmBool dragging;

        [Tooltip("The InputButton for this event.")]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(PointerEventData.InputButton))]
        public FsmEnum inputButton;

        [Tooltip("Is the pointer being pressed? (Not documented by Unity)")]
        [UIHint(UIHint.Variable)]
        public FsmBool eligibleForClick;

        [Tooltip("The camera associated with the last OnPointerEnter event.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject enterEventCamera;

        [Tooltip("The camera associated with the last OnPointerPress event.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject pressEventCamera;

        [Tooltip("Is the pointer moving.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isPointerMoving;

        [Tooltip("Is scroll being used on the input device.")]
        [UIHint(UIHint.Variable)]
        public FsmBool isScrolling;

        [Tooltip("The GameObject for the last press event.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject lastPress;

        [Tooltip("The object that is receiving OnDrag.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject pointerDrag;

        [Tooltip("The object that received 'OnPointerEnter'.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject pointerEnter;

        [Tooltip("Id of the pointer (touch id).")]
        [UIHint(UIHint.Variable)]
        public FsmInt pointerId;

        [Tooltip("The GameObject that received the OnPointerDown.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject pointerPress;

        [Tooltip("Current pointer position.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 position;

        [Tooltip("Position of the press.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 pressPosition;

        [Tooltip("The object that the press happened on even if it can not handle the press event.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject rawPointerPress;

        [Tooltip("The amount of scroll since the last update.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 scrollDelta;

        [Tooltip("Is the event used?")]
        [UIHint(UIHint.Variable)]
        public FsmBool used;

        [Tooltip("Should a drag threshold be used?")]
        [UIHint(UIHint.Variable)]
        public FsmBool useDragThreshold;

        [Tooltip("The normal of the last raycast in world coordinates.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 worldNormal;

        [Tooltip("The world position of the last raycast.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 worldPosition;

        public override void Reset()
        {
            this.clickCount = null;
            this.clickTime = null;
            this.delta = null;
            this.dragging = null;
            this.inputButton = PointerEventData.InputButton.Left;
            this.eligibleForClick = null;
            this.enterEventCamera = null;
            this.pressEventCamera = null;
            this.isPointerMoving = null;
            this.isScrolling = null;
            this.lastPress = null;
            this.pointerDrag = null;
            this.pointerEnter = null;
            this.pointerId = null;
            this.pointerPress = null;
            this.position = null;
            this.pressPosition = null;
            this.rawPointerPress = null;
            this.scrollDelta = null;
            this.used = null;
            this.useDragThreshold = null;
            this.worldNormal = null;
            this.worldPosition = null;
        }

        public override void OnEnter()
        {
            if (UiGetLastPointerDataInfo.lastPointerEventData == null)
            {
                base.Finish();
                return;
            }
            if (!this.clickCount.IsNone)
            {
                this.clickCount.Value = UiGetLastPointerDataInfo.lastPointerEventData.clickCount;
            }
            if (!this.clickTime.IsNone)
            {
                this.clickTime.Value = UiGetLastPointerDataInfo.lastPointerEventData.clickTime;
            }
            if (!this.delta.IsNone)
            {
                this.delta.Value = UiGetLastPointerDataInfo.lastPointerEventData.delta;
            }
            if (!this.dragging.IsNone)
            {
                this.dragging.Value = UiGetLastPointerDataInfo.lastPointerEventData.dragging;
            }
            if (!this.inputButton.IsNone)
            {
                this.inputButton.Value = UiGetLastPointerDataInfo.lastPointerEventData.button;
            }
            if (!this.eligibleForClick.IsNone)
            {
                this.eligibleForClick.Value = UiGetLastPointerDataInfo.lastPointerEventData.eligibleForClick;
            }
            if (!this.enterEventCamera.IsNone)
            {
                this.enterEventCamera.Value = UiGetLastPointerDataInfo.lastPointerEventData.enterEventCamera.gameObject;
            }
            if (!this.isPointerMoving.IsNone)
            {
                this.isPointerMoving.Value = UiGetLastPointerDataInfo.lastPointerEventData.IsPointerMoving();
            }
            if (!this.isScrolling.IsNone)
            {
                this.isScrolling.Value = UiGetLastPointerDataInfo.lastPointerEventData.IsScrolling();
            }
            if (!this.lastPress.IsNone)
            {
                this.lastPress.Value = UiGetLastPointerDataInfo.lastPointerEventData.lastPress;
            }
            if (!this.pointerDrag.IsNone)
            {
                this.pointerDrag.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerDrag;
            }
            if (!this.pointerEnter.IsNone)
            {
                this.pointerEnter.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerEnter;
            }
            if (!this.pointerId.IsNone)
            {
                this.pointerId.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerId;
            }
            if (!this.pointerPress.IsNone)
            {
                this.pointerPress.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerPress;
            }
            if (!this.position.IsNone)
            {
                this.position.Value = UiGetLastPointerDataInfo.lastPointerEventData.position;
            }
            if (!this.pressEventCamera.IsNone)
            {
                this.pressEventCamera.Value = UiGetLastPointerDataInfo.lastPointerEventData.pressEventCamera.gameObject;
            }
            if (!this.pressPosition.IsNone)
            {
                this.pressPosition.Value = UiGetLastPointerDataInfo.lastPointerEventData.pressPosition;
            }
            if (!this.rawPointerPress.IsNone)
            {
                this.rawPointerPress.Value = UiGetLastPointerDataInfo.lastPointerEventData.rawPointerPress;
            }
            if (!this.scrollDelta.IsNone)
            {
                this.scrollDelta.Value = UiGetLastPointerDataInfo.lastPointerEventData.scrollDelta;
            }
            if (!this.used.IsNone)
            {
                this.used.Value = UiGetLastPointerDataInfo.lastPointerEventData.used;
            }
            if (!this.useDragThreshold.IsNone)
            {
                this.useDragThreshold.Value = UiGetLastPointerDataInfo.lastPointerEventData.useDragThreshold;
            }
            if (!this.worldNormal.IsNone)
            {
                this.worldNormal.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerCurrentRaycast.worldNormal;
            }
            if (!this.worldPosition.IsNone)
            {
                this.worldPosition.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerCurrentRaycast.worldPosition;
            }
            base.Finish();
        }
    }
}
