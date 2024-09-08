namespace MOM.Adventures
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class EditorDraggableHeader : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
    {
        private Vector2 startPosition;
        private Vector3 initialLocalPosition;
        private RectTransform draggedTransform;
        private RectTransform dragingSpace;
        public Callback dragStarted;
        public Callback dragUpdate;
        public Callback dragEnd;

        public void Initialize(RectTransform draggedTransform, int transformPopLimit, Callback dragStarted, Callback dragUpdate, Callback dragEnd)
        {
            this.draggedTransform = draggedTransform;
            this.dragingSpace = draggedTransform.parent as RectTransform;
            this.dragStarted = dragStarted;
            this.dragUpdate = dragUpdate;
            this.dragEnd = dragEnd;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.startPosition = eventData.position - eventData.delta;
            this.initialLocalPosition = this.draggedTransform.localPosition;
            this.draggedTransform.SetAsLastSibling();
            if (this.dragStarted != null)
            {
                this.dragStarted(null);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 vector = eventData.position - this.startPosition;
            Vector3 lossyScale = this.dragingSpace.lossyScale;
            Vector3 o = this.initialLocalPosition + new Vector3(vector.x / lossyScale.x, vector.y / lossyScale.z, 0f);
            this.draggedTransform.localPosition = o;
            if (this.dragUpdate != null)
            {
                this.dragUpdate(o);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 vector = eventData.position - this.startPosition;
            Vector3 lossyScale = this.dragingSpace.lossyScale;
            Vector3 o = this.initialLocalPosition + new Vector3(vector.x / lossyScale.x, vector.y / lossyScale.z, 0f);
            if (this.dragEnd != null)
            {
                this.dragEnd(o);
            }
        }
    }
}

