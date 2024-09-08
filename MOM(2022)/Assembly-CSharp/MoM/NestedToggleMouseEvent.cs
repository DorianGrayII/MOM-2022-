namespace MOM
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class NestedToggleMouseEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Toggle toggle;

        public void Awake()
        {
            this.toggle = base.gameObject.transform.parent.gameObject.GetComponentInParent<Toggle>();
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

