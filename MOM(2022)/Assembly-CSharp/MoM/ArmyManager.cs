// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.ArmyManager
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class ArmyManager : ScreenBase
{
    public Button btClose;

    public Button btManageItems;

    public Button btViewArmy;

    public GridItemManager gridArmies;

    public ArmyGrid gridHeroes;

    public GridItemManager gridUnusedItems;

    public TextMeshProUGUI heading;

    public TextMeshProUGUI headingArtefacts;

    public TextMeshProUGUI upkeepGold;

    public TextMeshProUGUI upkeepFood;

    public TextMeshProUGUI upkeepMana;

    public RawImage minimap;

    private List<global::MOM.Group> groupList;

    private Coroutine updateArmiesCoroutine;

    private Coroutine updateVaultCoroutine;

    public override IEnumerator PreStart()
    {
        MHEventSystem.RegisterListener<global::MOM.Unit>(UnitChanged, this);
        MHEventSystem.RegisterListener<global::MOM.Group>(delegate(object sender, object o)
        {
            global::MOM.Group item = sender as global::MOM.Group;
            if (this.groupList.Contains(item) && this.updateArmiesCoroutine == null)
            {
                this.updateArmiesCoroutine = base.StartCoroutine(this.ASyncUpdateArmies());
            }
        }, this);
        MHEventSystem.RegisterListener<EquipmentSlot>(delegate
        {
            if (this.updateVaultCoroutine == null)
            {
                this.updateVaultCoroutine = base.StartCoroutine(this.ASyncUpdateVault());
            }
        }, this);
        MHEventSystem.RegisterListener<global::MOM.Artefact>(ArtefactSmashed, this);
        this.gridArmies.onSelectionChange = delegate
        {
            global::MOM.Group selectedObject = this.gridArmies.GetSelectedObject<global::MOM.Group>();
            if (selectedObject != null)
            {
                MinimapManager.Get().FocusMinimap(selectedObject.GetPlane(), selectedObject.GetPosition());
            }
        };
        this.gridArmies.CustomDynamicItem(GroupItem, UpdateGridArmies);
        this.UpdateGridArmies();
        this.UpdateHeroesList();
        this.gridUnusedItems.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            global::MOM.Artefact a = source as global::MOM.Artefact;
            GameObjectUtils.FindByNameGetComponent<RawImage>(itemSource, "ItemIcon").texture = AssetManager.Get<Texture2D>(a.graphic);
            itemSource.GetOrAddComponent<RolloverObject>().source = a;
            itemSource.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
            {
                AudioLibrary.RequestSFX("SmashArtefact");
                PopupGeneral.OpenPopup(null, "UI_SMASH_ARTEFACT", global::DBUtils.Localization.Get("UI_SMASH_ARTEFACT_DES", true, a.name, a.GetValue() / 2), "UI_OK", global::MOM.Artefact.SmashArtefact, "UI_CANCEL", null, null, null, a);
            };
        }, UpdateVault);
        this.UpdateVault();
        this.UpdateUpkeep();
        this.heading.text = global::DBUtils.Localization.Get("UI_THE_ARMIES_OF", true) + GameManager.GetHumanWizard().name;
        AudioLibrary.RequestSFX("OpenArmyScreen");
        yield return base.PreStart();
    }

    private void OnDestroy()
    {
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
    }

    private IEnumerator ASyncUpdateArmies()
    {
        yield return new WaitForEndOfFrame();
        this.UpdateGridArmies();
        this.updateArmiesCoroutine = null;
    }

    private IEnumerator ASyncUpdateVault()
    {
        yield return new WaitForEndOfFrame();
        this.UpdateVault();
        this.updateVaultCoroutine = null;
    }

    private void UpdateUpkeep()
    {
        List<global::MOM.Group> groupsOfWizard = GameManager.GetGroupsOfWizard(PlayerWizard.HumanID());
        FInt zERO = FInt.ZERO;
        FInt zERO2 = FInt.ZERO;
        FInt zERO3 = FInt.ZERO;
        Tag t = (Tag)TAG.UPKEEP_GOLD;
        Tag t2 = (Tag)TAG.UPKEEP_FOOD;
        FInt zERO4 = FInt.ZERO;
        FInt zERO5 = FInt.ZERO;
        FInt zERO6 = FInt.ZERO;
        if (groupsOfWizard != null)
        {
            foreach (global::MOM.Group item in groupsOfWizard)
            {
                foreach (Reference<global::MOM.Unit> unit in item.GetUnits())
                {
                    zERO += unit.Get().GetAttributes().GetFinal(t);
                    zERO2 += unit.Get().GetAttributes().GetFinal(t2);
                    zERO3 += unit.Get().GetManaUpkeep();
                    zERO4 += unit.Get().GetUpkeepChannelerManaDiscount();
                    zERO5 += unit.Get().GetUpkeepConjuerManaDiscount();
                    zERO6 += unit.Get().GetUpkeepNatureSummonerManaDiscount();
                }
            }
        }
        zERO3 -= zERO4.ReturnRoundedFloor() + zERO5.ReturnRoundedFloor() + zERO6.ReturnRoundedFloor();
        this.upkeepGold.text = zERO.ToInt().ToString();
        this.upkeepFood.text = zERO2.ToInt().ToString();
        this.upkeepMana.text = zERO3.ToInt().ToString();
    }

    private void ArtefactSmashed(object sender, object e)
    {
        if (e.ToString() == "Smashed")
        {
            this.UpdateVault();
        }
    }

    public void UnitChanged(object sender, object e)
    {
        this.UpdateUpkeep();
    }

    private void UpdateGridArmies()
    {
        this.groupList = new List<global::MOM.Group>(GameManager.Get().registeredGroups);
        this.groupList = this.groupList.FindAll((global::MOM.Group o) => o.GetOwnerID() == PlayerWizard.HumanID() && o.GetUnits().Count > 0 && (o.GetLocationHost()?.otherPlaneLocation?.Get() == null || o.plane.arcanusType));
        this.gridArmies.UpdateGrid(this.groupList);
        global::MOM.Group selectedObject = this.gridArmies.GetSelectedObject<global::MOM.Group>();
        if (selectedObject != null)
        {
            MinimapManager.Get().FocusMinimap(selectedObject.GetPlane(), selectedObject.GetPosition());
        }
    }

    private void UpdateHeroesList()
    {
        List<Reference<global::MOM.Unit>> heroes = GameManager.GetHumanWizard().heroes;
        this.gridHeroes.SetUnits(heroes);
    }

    private void UpdateVault()
    {
        this.gridUnusedItems.UpdateGrid(GameManager.GetHumanWizard().artefacts);
        int count = GameManager.GetHumanWizard().artefacts.Count;
        this.headingArtefacts.text = global::DBUtils.Localization.Get("UI_UNUSED_ARTEFACTS", true) + " (" + count + ")";
    }

    private void GroupItem(GameObject itemSource, object source, object data, int index)
    {
        ArmyListItem2 component = itemSource.GetComponent<ArmyListItem2>();
        global::MOM.Group group = source as global::MOM.Group;
        component.labelArmyID.text = (index + 1).ToString();
        component.labelArmySize.text = (group?.GetUnits()?.Count).GetValueOrDefault() + "/9";
        component.gridUnits.SetUnits(group.GetUnits());
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btManageItems)
        {
            UIManager.Open<HeroEquip>(UIManager.Layer.Popup, this);
        }
        else if (s == this.btViewArmy)
        {
            global::MOM.Group selectedObject = this.gridArmies.GetSelectedObject<global::MOM.Group>();
            if (selectedObject != null)
            {
                ArmyManager screen = UIManager.GetScreen<ArmyManager>(UIManager.Layer.Standard);
                if (screen == null)
                {
                    return;
                }
                UIManager.Close(screen);
                TownScreen townScreen = TownScreen.Get();
                if (townScreen != null)
                {
                    townScreen.Close();
                }
                FSMSelectionManager.Get().Select(selectedObject, focus: true);
            }
        }
        if (s.name == "ButtonClose")
        {
            UIManager.Close(this);
        }
    }

    public override IEnumerator PostClose()
    {
        yield return base.PostClose();
        MinimapManager.Get().SetPlane(World.GetActivePlane());
    }
}
