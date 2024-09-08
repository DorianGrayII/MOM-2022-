using UnityEngine;
using UnityEngine.EventSystems;

namespace MHUtils.UI
{
    public class GridItemInteractiveClick : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
    {
        public GridItemManager owner;

        public int itemIndex;

        public object itemDisplaySource;

        public object itemData;

        public void OnPointerClick(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<GridItemInteractiveClick>(this, this.owner);
        }
    }
}
