using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.UI)]
    [Tooltip("The eventType will be executed on all components on the GameObject that can handle it.")]
    public class UiEventSystemExecuteEvent : FsmStateAction
    {
        public enum EventHandlers
        {
            Submit = 0,
            beginDrag = 1,
            cancel = 2,
            deselectHandler = 3,
            dragHandler = 4,
            dropHandler = 5,
            endDragHandler = 6,
            initializePotentialDrag = 7,
            pointerClickHandler = 8,
            pointerDownHandler = 9,
            pointerEnterHandler = 10,
            pointerExitHandler = 11,
            pointerUpHandler = 12,
            scrollHandler = 13,
            submitHandler = 14,
            updateSelectedHandler = 15
        }

        [RequiredField]
        [Tooltip("The GameObject with  an IEventSystemHandler component (a UI button for example).")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The Type of handler to execute")]
        [ObjectType(typeof(EventHandlers))]
        public FsmEnum eventHandler;

        [Tooltip("Event Sent if execution was possible on GameObject")]
        public FsmEvent success;

        [Tooltip("Event Sent if execution was NOT possible on GameObject because it can not handle the eventHandler selected")]
        public FsmEvent canNotHandleEvent;

        private GameObject go;

        public override void Reset()
        {
            this.gameObject = null;
            this.eventHandler = EventHandlers.Submit;
            this.success = null;
            this.canNotHandleEvent = null;
        }

        public override void OnEnter()
        {
            base.Fsm.Event(this.ExecuteEvent() ? this.success : this.canNotHandleEvent);
            base.Finish();
        }

        private bool ExecuteEvent()
        {
            this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (this.go == null)
            {
                base.LogError("Missing GameObject ");
                return false;
            }
            switch ((EventHandlers)(object)this.eventHandler.Value)
            {
            case EventHandlers.Submit:
                if (!ExecuteEvents.CanHandleEvent<ISubmitHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                break;
            case EventHandlers.beginDrag:
                if (!ExecuteEvents.CanHandleEvent<IBeginDragHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.beginDragHandler);
                break;
            case EventHandlers.cancel:
                if (!ExecuteEvents.CanHandleEvent<ICancelHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.cancelHandler);
                break;
            case EventHandlers.deselectHandler:
                if (!ExecuteEvents.CanHandleEvent<IDeselectHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
                break;
            case EventHandlers.dragHandler:
                if (!ExecuteEvents.CanHandleEvent<IDragHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.dragHandler);
                break;
            case EventHandlers.dropHandler:
                if (!ExecuteEvents.CanHandleEvent<IDropHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.dropHandler);
                break;
            case EventHandlers.endDragHandler:
                if (!ExecuteEvents.CanHandleEvent<IEndDragHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.endDragHandler);
                break;
            case EventHandlers.initializePotentialDrag:
                if (!ExecuteEvents.CanHandleEvent<IInitializePotentialDragHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.initializePotentialDrag);
                break;
            case EventHandlers.pointerClickHandler:
                if (!ExecuteEvents.CanHandleEvent<IPointerClickHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                break;
            case EventHandlers.pointerDownHandler:
                if (!ExecuteEvents.CanHandleEvent<IPointerDownHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
                break;
            case EventHandlers.pointerUpHandler:
                if (!ExecuteEvents.CanHandleEvent<IPointerUpHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
                break;
            case EventHandlers.pointerEnterHandler:
                if (!ExecuteEvents.CanHandleEvent<IPointerEnterHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
                break;
            case EventHandlers.pointerExitHandler:
                if (!ExecuteEvents.CanHandleEvent<IPointerExitHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
                break;
            case EventHandlers.scrollHandler:
                if (!ExecuteEvents.CanHandleEvent<IScrollHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.scrollHandler);
                break;
            case EventHandlers.updateSelectedHandler:
                if (!ExecuteEvents.CanHandleEvent<IUpdateSelectedHandler>(this.go))
                {
                    return false;
                }
                ExecuteEvents.Execute(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.updateSelectedHandler);
                break;
            }
            return true;
        }
    }
}
