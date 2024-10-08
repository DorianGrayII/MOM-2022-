using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Pointer Events")]
    public class PlayMakerUiPointerEvents : PlayMakerUiEventBase, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            base.SendEvent(FsmEvent.UiPointerClick);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            base.SendEvent(FsmEvent.UiPointerDown);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            base.SendEvent(FsmEvent.UiPointerEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            base.SendEvent(FsmEvent.UiPointerExit);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            base.SendEvent(FsmEvent.UiPointerUp);
        }
    }
}
