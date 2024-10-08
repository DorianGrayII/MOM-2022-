using UnityEngine;
using UnityEngine.EventSystems;

namespace MHUtils.UI
{
    public class DraggableSurface : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
    {
        private Vector2 startPosition;

        private Vector3 initialLocalPosition;

        public Transform draggedTransform;

        public Transform dragingSpace;

        private Callback dragStarted;

        private Callback dragUpdate;

        private Callback dragEnd;

        public void Initialize(Transform draggedTransform, Callback dragStarted = null, Callback dragUpdate = null, Callback dragEnd = null)
        {
            this.draggedTransform = draggedTransform;
            this.dragingSpace = draggedTransform.parent;
            this.dragStarted = dragStarted;
            this.dragUpdate = dragUpdate;
            this.dragEnd = dragEnd;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.startPosition = eventData.position;
            this.initialLocalPosition = this.draggedTransform.localPosition;
            if (this.dragStarted != null)
            {
                this.dragStarted(null);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 vector = eventData.position - this.startPosition;
            Vector3 lossyScale = this.dragingSpace.lossyScale;
            Vector3 vector2 = this.initialLocalPosition + new Vector3(vector.x / lossyScale.x, vector.y / lossyScale.z, 0f);
            if (this.dragUpdate == null)
            {
                this.draggedTransform.localPosition = vector2;
            }
            else
            {
                this.dragUpdate(vector2);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 vector = eventData.position - this.startPosition;
            Vector3 lossyScale = this.dragingSpace.lossyScale;
            Vector3 vector2 = this.initialLocalPosition + new Vector3(vector.x / lossyScale.x, vector.y / lossyScale.z, 0f);
            if (this.dragEnd == null)
            {
                this.draggedTransform.localPosition = vector2;
            }
            else
            {
                this.dragEnd(vector2);
            }
        }
    }
}
