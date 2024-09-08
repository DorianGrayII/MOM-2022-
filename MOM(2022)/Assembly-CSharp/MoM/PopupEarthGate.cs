// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupEarthGate
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class PopupEarthGate : ScreenBase
{
    private static PopupEarthGate instance;

    public Button btClose;

    public Button btConfirm;

    public GridItemManager gridCities;

    public RawImage minimap;

    private TownLocation startTown;

    private TownLocation destinationTown;

    public static void OpenPopup(ScreenBase parent, TownLocation tl)
    {
        PopupEarthGate.instance = UIManager.Open<PopupEarthGate>(UIManager.Layer.Popup, parent);
        PopupEarthGate.instance.startTown = tl;
        PopupEarthGate.instance.destinationTown = tl;
        PopupEarthGate.instance.gridCities.CustomDynamicItem(PopupEarthGate.instance.UnitItem, PopupEarthGate.instance.BaseUpdate);
        PopupEarthGate.instance.BaseUpdate();
        MinimapManager.Get().FocusMinimap(tl.GetPlane(), tl.GetPosition());
    }

    private void UnitItem(GameObject itemSource, object source, object data, int index)
    {
        TownLocation tl = source as TownLocation;
        IconAndNameListItem orAddComponent = itemSource.GetOrAddComponent<IconAndNameListItem>();
        orAddComponent.icon.texture = tl.race.Get().GetDescriptionInfo().GetTexture();
        orAddComponent.label.text = tl.name;
        Toggle orAddComponent2 = itemSource.GetOrAddComponent<Toggle>();
        orAddComponent2.onValueChanged.RemoveAllListeners();
        orAddComponent2.isOn = tl == this.startTown;
        orAddComponent2.onValueChanged.AddListener(delegate(bool b)
        {
            if (b)
            {
                MinimapManager.Get().FocusMinimap(tl.GetPlane(), tl.GetPosition());
                this.destinationTown = tl;
            }
        });
        itemSource.GetOrAddComponent<MouseClickEvent>().mouseDoubleClick = delegate
        {
            this.TransferUnits();
        };
    }

    private void BaseUpdate()
    {
        List<global::MOM.Location> locationsOfWizard = GameManager.GetLocationsOfWizard(this.startTown.GetOwnerID());
        locationsOfWizard = locationsOfWizard.FindAll((global::MOM.Location o) => o is TownLocation && (o as TownLocation).HaveBuilding((Building)BUILDING.EARTH_GATE) && o.GetPlane() == this.startTown.GetPlane());
        PopupEarthGate.instance.gridCities.UpdateGrid(locationsOfWizard);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btClose)
        {
            UIManager.Close(this);
        }
        else if (s == this.btConfirm)
        {
            this.TransferUnits();
        }
    }

    private void TransferUnits()
    {
        UIManager.Close(this);
        bool flag = false;
        if (this.destinationTown == null || this.destinationTown == this.startTown)
        {
            return;
        }
        List<global::MOM.Unit> selectedUnits = TownScreen.Get().GetSelectedUnits();
        if (selectedUnits != null && selectedUnits.Count > 0 && selectedUnits.Count < this.startTown.GetLocalGroup().GetUnits().Count)
        {
            foreach (global::MOM.Unit item in selectedUnits)
            {
                if (item.Mp > 0)
                {
                    item.Mp -= 1;
                    this.startTown.GetLocalGroup().TransferUnit(this.destinationTown.GetLocalGroup(), item);
                    flag = true;
                }
            }
            this.startTown.GetLocalGroup().UpdateHash();
            this.destinationTown.GetLocalGroup().UpdateHash();
        }
        else
        {
            if (this.startTown.GetLocalGroup().CurentMP() > 0)
            {
                foreach (Reference<global::MOM.Unit> unit in this.startTown.GetLocalGroup().GetUnits())
                {
                    unit.Get().Mp = unit.Get().Mp - 1;
                }
                this.startTown.GetLocalGroup().TransferUnits(this.destinationTown.GetLocalGroup());
                flag = true;
            }
            this.startTown.GetLocalGroup().UpdateHash();
            this.destinationTown.GetLocalGroup().UpdateHash();
        }
        if (flag)
        {
            bool flag2 = this.destinationTown.IsAnOutpost();
            string e = (flag2 ? "OutpostScreen" : "TownScreen");
            TownScreen townScreen = TownScreen.Get();
            if (townScreen != null)
            {
                if (flag2)
                {
                    townScreen.Close();
                    FSMMapGame.Get()?.MapGameEvents(this.destinationTown, e);
                }
                else
                {
                    townScreen.SetTown(this.destinationTown);
                    townScreen.UpdateAll(ignoreTowns: true);
                    FSMSelectionManager.Get().Select(this.destinationTown, focus: true);
                }
            }
            else
            {
                FSMMapGame.Get()?.MapGameEvents(this.destinationTown, e);
            }
        }
        else
        {
            PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_EARTH_GATE_NO_MP", "UI_OK");
        }
    }

    public override IEnumerator Closing()
    {
        PopupEarthGate.instance = null;
        yield return base.Closing();
    }

    public override IEnumerator PostClose()
    {
        yield return base.PostClose();
        MinimapManager.Get().SetPlane(World.GetActivePlane());
    }
}
