namespace HutongGames.PlayMaker
{
    using HutongGames.PlayMaker.Actions;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [AddComponentMenu("PlayMaker/UI/UI Drop Event")]
    public class PlayMakerUiDropEvent : PlayMakerUiEventBase, IDropHandler, IEventSystemHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            base.SendEvent(FsmEvent.UiDrop);
        }
    }
}

