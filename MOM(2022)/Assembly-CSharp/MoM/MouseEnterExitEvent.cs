namespace MOM
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MouseEnterExitEvent : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public Callback mouseEnter;
        public Callback mouseExit;
        public object data;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.mouseEnter == null)
            {
                Callback mouseEnter = this.mouseEnter;
            }
            else
            {
                this.mouseEnter(this.data);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.mouseExit == null)
            {
                Callback mouseExit = this.mouseExit;
            }
            else
            {
                this.mouseExit(this.data);
            }
        }
    }
}

