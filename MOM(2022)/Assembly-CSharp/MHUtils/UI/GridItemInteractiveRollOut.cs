using UnityEngine;
using UnityEngine.EventSystems;

namespace MHUtils.UI
{
    public class GridItemInteractiveRollOut : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
    {
        public GridItemManager owner;

        public int itemIndex;

        public object itemDisplaySource;

        public object itemData;

        public void OnPointerExit(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<GridItemInteractiveRollOut>(this, this.owner);
        }
    }
}
