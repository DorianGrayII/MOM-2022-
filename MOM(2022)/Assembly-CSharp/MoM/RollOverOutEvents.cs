using MHUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MOM
{
    public class RollOverOutEvents : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public object data;

        public void OnPointerEnter(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<RollOverOutEvents>(this, "OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<RollOverOutEvents>(this, "OnPointerExit");
        }
    }
}
