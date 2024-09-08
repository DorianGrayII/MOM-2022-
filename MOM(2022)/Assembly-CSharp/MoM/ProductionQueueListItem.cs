namespace MOM
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ProductionQueueListItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public RawImage icon;
        public Button btRemoveFromQueue;
        public Button btMoveRight;
        public Button btMoveLeft;
        public GameObject buttons;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.buttons != null)
            {
                this.buttons.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this.buttons != null)
            {
                this.buttons.SetActive(false);
            }
        }
    }
}

