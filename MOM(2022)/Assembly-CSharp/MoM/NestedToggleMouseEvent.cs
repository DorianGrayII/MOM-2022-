using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MOM
{
    public class NestedToggleMouseEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Toggle toggle;

        public void Awake()
        {
            GameObject gameObject = base.gameObject.transform.parent.gameObject;
            this.toggle = gameObject.GetComponentInParent<Toggle>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.toggle != null)
            {
                this.toggle.OnPointerClick(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.toggle != null)
            {
                this.toggle.OnPointerEnter(eventData);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.toggle != null)
            {
                this.toggle.OnPointerExit(eventData);
            }
        }
    }
}
