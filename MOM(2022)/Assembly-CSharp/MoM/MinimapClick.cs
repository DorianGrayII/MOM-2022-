using MHUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using WorldCode;

namespace MOM
{
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

        private Vector3 ScaleUsingMinimapSettings(Vector2 click, RectTransform t)
        {
            global::WorldCode.Plane arcanus = World.GetArcanus();
            if (arcanus == null)
            {
                return Vector3.zero;
            }
            float x = click.x / t.rect.width;
            float z = click.y / t.rect.height;
            Vector3 result = new Vector3(x, 0f, z);
            result /= 0.9f;
            Vector2 vector = HexCoordinates.HexToWorld(arcanus.area.A00);
            Vector2 vector2 = HexCoordinates.HexToWorld(arcanus.area.A11);
            result.x *= vector2.x - vector.x;
            result.z *= vector2.y - vector.y;
            return result;
        }

        private void UpdateOnPress(PointerEventData eventData)
        {
            if (MinimapManager.Get().MapInZoomoutMode())
            {
                RectTransform component = base.GetComponent<RectTransform>();
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(component, eventData.position, eventData.pressEventCamera, out var localPoint))
                {
                    CameraController.CenterAt(this.ScaleUsingMinimapSettings(localPoint, component));
                }
            }
        }
    }
}
