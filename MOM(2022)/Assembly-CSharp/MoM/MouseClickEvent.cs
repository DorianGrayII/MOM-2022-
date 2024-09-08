namespace MOM
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MouseClickEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public Callback mouseLeftClick;
        public Callback mouseRightClick;
        public Callback mouseCenterClick;
        public Callback mouseDoubleClick;
        public object data;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.mouseLeftClick == null)
                {
                    Callback mouseLeftClick = this.mouseLeftClick;
                }
                else
                {
                    this.mouseLeftClick(this.data);
                }
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (this.mouseRightClick == null)
                {
                    Callback mouseRightClick = this.mouseRightClick;
                }
                else
                {
                    this.mouseRightClick(this.data);
                }
            }
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                if (this.mouseCenterClick == null)
                {
                    Callback mouseCenterClick = this.mouseCenterClick;
                }
                else
                {
                    this.mouseCenterClick(this.data);
                }
            }
            if (eventData.clickCount == 2)
            {
                if (this.mouseDoubleClick == null)
                {
                    Callback mouseDoubleClick = this.mouseDoubleClick;
                }
                else
                {
                    this.mouseDoubleClick(this.data);
                }
            }
        }
    }
}

