namespace MHUtils.UI
{
    using MHUtils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

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

