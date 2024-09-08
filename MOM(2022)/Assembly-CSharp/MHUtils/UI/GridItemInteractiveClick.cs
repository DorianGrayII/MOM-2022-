namespace MHUtils.UI
{
    using MHUtils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

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

