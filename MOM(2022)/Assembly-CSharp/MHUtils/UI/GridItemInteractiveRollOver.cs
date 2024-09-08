namespace MHUtils.UI
{
    using MHUtils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

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

