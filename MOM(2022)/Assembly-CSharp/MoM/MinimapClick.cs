namespace MOM
{
    using MHUtils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using WorldCode;

    public class MinimapClick : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            this.UpdateOnPress(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.UpdateOnPress(eventData);
        }

        private unsafe Vector3 ScaleUsingMinimapSettings(Vector2 click, RectTransform t)
        {
            WorldCode.Plane arcanus = World.GetArcanus();
            if (arcanus == null)
            {
                return Vector3.zero;
            }
            Vector3 vector = new Vector3(click.x / t.rect.width, 0f, click.y / t.rect.height);
            vector /= 0.9f;
            Vector2 vector2 = HexCoordinates.HexToWorld(arcanus.area.A00);
            Vector2 vector3 = HexCoordinates.HexToWorld(arcanus.area.A11);
            float* singlePtr1 = &vector.x;
            singlePtr1[0] *= vector3.x - vector2.x;
            float* singlePtr2 = &vector.z;
            singlePtr2[0] *= vector3.y - vector2.y;
            return vector;
        }

        private void UpdateOnPress(PointerEventData eventData)
        {
            if (MinimapManager.Get().MapInZoomoutMode())
            {
                Vector2 vector;
                RectTransform component = base.GetComponent<RectTransform>();
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(component, eventData.position, eventData.pressEventCamera, out vector))
                {
                    CameraController.CenterAt(this.ScaleUsingMinimapSettings(vector, component), 0f);
                }
            }
        }
    }
}

