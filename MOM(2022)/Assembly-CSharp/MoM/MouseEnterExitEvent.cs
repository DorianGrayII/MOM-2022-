using UnityEngine;
using UnityEngine.EventSystems;

namespace MOM
{
    public class MouseEnterExitEvent : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public Callback mouseEnter;

        public Callback mouseExit;

        public object data;

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.mouseEnter?.Invoke(this.data);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.mouseExit?.Invoke(this.data);
        }
    }
}
