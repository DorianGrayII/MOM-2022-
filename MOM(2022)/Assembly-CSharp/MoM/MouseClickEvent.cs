using UnityEngine;
using UnityEngine.EventSystems;

namespace MOM
{
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
                this.mouseLeftClick?.Invoke(this.data);
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                this.mouseRightClick?.Invoke(this.data);
            }
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                this.mouseCenterClick?.Invoke(this.data);
            }
            if (eventData.clickCount == 2)
            {
                this.mouseDoubleClick?.Invoke(this.data);
            }
        }
    }
}
