namespace MOM
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class PlaySFX : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
    {
        public string clickEffect;
        public string rollOverEffect;
        public string rollOutEffect;
        public Selectable linkedComponent;
        public bool audioOnNonInteractible;
        private bool memoryInteractible;

        private void Awake()
        {
            if (this.linkedComponent == null)
            {
                this.linkedComponent = base.GetComponent<Selectable>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (this.memoryInteractible || (this.audioOnNonInteractible || ((this.linkedComponent == null) || this.linkedComponent.interactable)))
            {
                this.memoryInteractible = false;
                if (!string.IsNullOrEmpty(this.clickEffect))
                {
                    AudioLibrary.RequestSFX(this.clickEffect, 0f, 0f, 1f);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if ((this.linkedComponent == null) || this.linkedComponent.interactable)
            {
                this.memoryInteractible = true;
                if (!string.IsNullOrEmpty(this.rollOverEffect))
                {
                    AudioLibrary.RequestSFX(this.rollOverEffect, 0f, 0f, 1f);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.memoryInteractible = false;
            if (((this.linkedComponent == null) || this.linkedComponent.interactable) && !string.IsNullOrEmpty(this.rollOutEffect))
            {
                AudioLibrary.RequestSFX(this.rollOutEffect, 0f, 0f, 1f);
            }
        }
    }
}

