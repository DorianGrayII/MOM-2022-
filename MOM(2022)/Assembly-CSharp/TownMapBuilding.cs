using MOM;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TownMapBuilding : RolloverObject
{
    public TownMap townMap;
    public DBReference<Building> building;

    public void Awake()
    {
        base.titleOnlyUntilRightClick = true;
        base.useMouseLocation = false;
        base.anchor = new Vector2(0.5f, 1f);
        base.position = new Vector2(0.5f, 0f);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (this.building.dbName != "SUMMONING_CIRCLE")
        {
            base.source = this.building.Get();
        }
        else
        {
            base.overrideTitle = "DES_EXTRA_SUMMONING_CIRCLE";
            base.overrideDescription = "DES_EXTRA_SUMMONING_CIRCLE_DES";
            base.source = null;
        }
        this.townMap.Select(this.building);
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        this.townMap.ClearSelection();
        base.OnPointerExit(eventData);
    }
}

