using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MOM
{
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

        private void OnDisable()
        {
            if (this.dd != null)
            {
                this.dd.Destroy();
                this.dd = null;
            }
        }

        private void OnDestroy()
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
                GameObject gameObject = new GameObject("DragAndDropItem");
                gameObject.transform.parent = UIManager.GetLayer(UIManager.Layer.TopLayer).transform;
                this.dd = gameObject.AddComponent<DragAndDropItem>();
                this.dd.source = this;
                GameObject gameObject2 = ((!(this.dragIcon != null)) ? Object.Instantiate(base.gameObject) : Object.Instantiate(this.dragIcon));
                CanvasGroup canvasGroup = gameObject2.AddComponent<CanvasGroup>();
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0f;
                gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
                gameObject2.transform.position = Vector3.zero;
                gameObject2.transform.localScale = this.dragScale;
            }
        }
    }
}
