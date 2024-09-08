namespace MOM
{
    using MHUtils;
    using MHUtils.UI;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
    {
        public object item;
        public object owner;
        public GameObject dragIcon;
        public Vector3 dragScale = Vector3.one;
        public DragAndDropItem dd;

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        private void OnDestroy()
        {
            if (this.dd != null)
            {
                this.dd.Destroy();
                this.dd = null;
            }
        }

        private void OnDisable()
        {
            if (this.dd != null)
            {
                this.dd.Destroy();
                this.dd = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                MHEventSystem.TriggerEvent<DragAndDrop>(this, this);
                GameObject obj2 = new GameObject("DragAndDropItem") {
                    transform = { parent = UIManager.GetLayer(UIManager.Layer.TopLayer).transform }
                };
                this.dd = obj2.AddComponent<DragAndDropItem>();
                this.dd.source = this;
                GameObject obj3 = (this.dragIcon == null) ? Instantiate<GameObject>(base.gameObject) : Instantiate<GameObject>(this.dragIcon);
                CanvasGroup local1 = obj3.AddComponent<CanvasGroup>();
                local1.interactable = false;
                local1.blocksRaycasts = false;
                local1.alpha = 0f;
                obj3.transform.SetParent(obj2.transform, false);
                obj3.transform.position = Vector3.zero;
                obj3.transform.localScale = this.dragScale;
            }
        }
    }
}

