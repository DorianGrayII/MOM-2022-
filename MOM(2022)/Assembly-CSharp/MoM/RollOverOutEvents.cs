namespace MOM
{
    using MHUtils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class RollOverOutEvents : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public object data;

        public void OnPointerEnter(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<RollOverOutEvents>(this, "OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<RollOverOutEvents>(this, "OnPointerExit");
        }
    }
}

