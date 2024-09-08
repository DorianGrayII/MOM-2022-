namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [ActionCategory(ActionCategory.UI), HutongGames.PlayMaker.Tooltip("The eventType will be executed on all components on the GameObject that can handle it.")]
    public class UiEventSystemExecuteEvent : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with  an IEventSystemHandler component (a UI button for example).")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The Type of handler to execute"), ObjectType(typeof(EventHandlers))]
        public FsmEnum eventHandler;
        [HutongGames.PlayMaker.Tooltip("Event Sent if execution was possible on GameObject")]
        public FsmEvent success;
        [HutongGames.PlayMaker.Tooltip("Event Sent if execution was NOT possible on GameObject because it can not handle the eventHandler selected")]
        public FsmEvent canNotHandleEvent;
        private GameObject go;

        private bool ExecuteEvent()
        {
            this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (this.go == null)
            {
                base.LogError("Missing GameObject ");
                return false;
            }
            switch (((EventHandlers) this.eventHandler.Value))
            {
                case EventHandlers.Submit:
                    if (!ExecuteEvents.CanHandleEvent<ISubmitHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<ISubmitHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                    break;

                case EventHandlers.beginDrag:
                    if (!ExecuteEvents.CanHandleEvent<IBeginDragHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IBeginDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.beginDragHandler);
                    break;

                case EventHandlers.cancel:
                    if (!ExecuteEvents.CanHandleEvent<ICancelHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<ICancelHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.cancelHandler);
                    break;

                case EventHandlers.deselectHandler:
                    if (!ExecuteEvents.CanHandleEvent<IDeselectHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IDeselectHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
                    break;

                case EventHandlers.dragHandler:
                    if (!ExecuteEvents.CanHandleEvent<IDragHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.dragHandler);
                    break;

                case EventHandlers.dropHandler:
                    if (!ExecuteEvents.CanHandleEvent<IDropHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IDropHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.dropHandler);
                    break;

                case EventHandlers.endDragHandler:
                    if (!ExecuteEvents.CanHandleEvent<IEndDragHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IEndDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.endDragHandler);
                    break;

                case EventHandlers.initializePotentialDrag:
                    if (!ExecuteEvents.CanHandleEvent<IInitializePotentialDragHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IInitializePotentialDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.initializePotentialDrag);
                    break;

                case EventHandlers.pointerClickHandler:
                    if (!ExecuteEvents.CanHandleEvent<IPointerClickHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IPointerClickHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                    break;

                case EventHandlers.pointerDownHandler:
                    if (!ExecuteEvents.CanHandleEvent<IPointerDownHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IPointerDownHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
                    break;

                case EventHandlers.pointerEnterHandler:
                    if (!ExecuteEvents.CanHandleEvent<IPointerEnterHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IPointerEnterHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
                    break;

                case EventHandlers.pointerExitHandler:
                    if (!ExecuteEvents.CanHandleEvent<IPointerExitHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IPointerExitHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
                    break;

                case EventHandlers.pointerUpHandler:
                    if (!ExecuteEvents.CanHandleEvent<IPointerUpHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IPointerUpHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
                    break;

                case EventHandlers.scrollHandler:
                    if (!ExecuteEvents.CanHandleEvent<IScrollHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IScrollHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.scrollHandler);
                    break;

                case EventHandlers.updateSelectedHandler:
                    if (!ExecuteEvents.CanHandleEvent<IUpdateSelectedHandler>(this.go))
                    {
                        return false;
                    }
                    ExecuteEvents.Execute<IUpdateSelectedHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.updateSelectedHandler);
                    break;

                default:
                    break;
            }
            return true;
        }

        public override void OnEnter()
        {
            base.Fsm.Event(this.ExecuteEvent() ? this.success : this.canNotHandleEvent);
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.eventHandler = EventHandlers.Submit;
            this.success = null;
            this.canNotHandleEvent = null;
        }

        public enum EventHandlers
        {
            Submit,
            beginDrag,
            cancel,
            deselectHandler,
            dragHandler,
            dropHandler,
            endDragHandler,
            initializePotentialDrag,
            pointerClickHandler,
            pointerDownHandler,
            pointerEnterHandler,
            pointerExitHandler,
            pointerUpHandler,
            scrollHandler,
            submitHandler,
            updateSelectedHandler
        }
    }
}

