using UnityEngine;
using UnityEngine.EventSystems;

namespace MHUtils.UI
{
    public class GridItemInteractiveRollOver : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
    {
        public GridItemManager owner;

        public int itemIndex;

        public object itemDisplaySource;

        public object itemData;

        public void OnPointerEnter(PointerEventData eventData)
        {
            MHEventSystem.TriggerEvent<GridItemInteractiveRollOver>(this, this.owner);
        }
    }
}
