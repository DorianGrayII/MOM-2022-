namespace MOM
{
    using DBDef;
    using DBEnum;
    using DBUtils;
    using MHUtils;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class BuildingListItem : ListItem, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
    {
        public TextMeshProUGUI labelBuildingName;
        public TextMeshProUGUI labelGoldUpkeep;
        public TextMeshProUGUI labelManaUpkeep;
        public RawImage iconBuilding;
        public Button btDemolish;
        public GameObject goGoldUpkeep;
        public GameObject goManaUpkeep;
        [Tooltip("If true a roll over tooltip will be displayed when hovering")]
        public bool supportTooltip = true;
        private Building building;
        private TownLocation town;
        private TownScreen townScreen;
        private RolloverObject tooltip;

        public void Awake()
        {
            this.townScreen = base.GetComponentInParent<TownScreen>();
            this.btDemolish.onClick.AddListener(() => Sell(this.town, this.building, this.townScreen));
            if (this.supportTooltip)
            {
                this.tooltip = GameObjectUtils.GetOrAddComponent<RolloverObject>(base.gameObject);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            TownMap componentInChildren;
            if (this.townScreen != null)
            {
                componentInChildren = this.townScreen.GetComponentInChildren<TownMap>();
            }
            else
            {
                TownScreen townScreen = this.townScreen;
                componentInChildren = null;
            }
            TownMap local2 = componentInChildren;
            if (local2 == null)
            {
                TownMap local3 = local2;
            }
            else
            {
                local2.Select(this.building);
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            TownMap componentInChildren;
            if (this.townScreen != null)
            {
                componentInChildren = this.townScreen.GetComponentInChildren<TownMap>();
            }
            else
            {
                TownScreen townScreen = this.townScreen;
                componentInChildren = null;
            }
            TownMap local2 = componentInChildren;
            if (local2 == null)
            {
                TownMap local3 = local2;
            }
            else
            {
                local2.ClearSelection();
            }
        }

        public static void Sell(TownLocation town, Building building, TownScreen townScreen)
        {
            Building objA = town.CanRemoveBuilding(building, false);
            if (objA != null)
            {
                string localizedName = building.GetDescriptionInfo().GetLocalizedName();
                if (!ReferenceEquals(objA, building))
                {
                    object[] parameters = new object[] { objA.GetDescriptionInfo().GetLocalizedName() };
                    PopupGeneral.OpenPopup(townScreen, localizedName, DBUtils.Localization.Get("UI_CAN_NOT_SELL_DEPENDENCY", true, parameters), "UI_OKAY", null, null, null, null, null, null);
                }
                else if (!ReferenceEquals(objA, (Building) BUILDING.ASTRAL_GATE) && (!ReferenceEquals(objA, (Building) BUILDING.EARTH_GATE) && (!ReferenceEquals(objA, (Building) BUILDING.NATURES_EYE) && (!ReferenceEquals(objA, (Building) BUILDING.ALTAR_OF_BATTLE) && !ReferenceEquals(objA, (Building) BUILDING.STREAM_OF_LIFE)))))
                {
                    object[] parameters = new object[] { localizedName };
                    PopupGeneral.OpenPopup(townScreen, localizedName, DBUtils.Localization.Get("UI_CAN_NOT_SELL", true, parameters), "UI_OKAY", null, null, null, null, null, null);
                }
                else
                {
                    object[] parameters = new object[] { localizedName };
                    PopupGeneral.OpenPopup(townScreen, localizedName, DBUtils.Localization.Get("UI_CAN_NOT_SELL_ENCHANTMENT", true, parameters), "UI_OKAY", null, null, null, null, null, null);
                }
            }
            else
            {
                Building[] buildingArray = null;
                CraftingItem first = town.craftingQueue.GetFirst();
                DescriptionInfo descriptionInfo = null;
                if (first != null)
                {
                    if (first.craftedBuilding != null)
                    {
                        Building[] parentBuildingRequired;
                        Building local1 = first.craftedBuilding.Get();
                        if (local1 != null)
                        {
                            parentBuildingRequired = local1.parentBuildingRequired;
                        }
                        else
                        {
                            Building local2 = local1;
                            parentBuildingRequired = null;
                        }
                        buildingArray = parentBuildingRequired;
                        descriptionInfo = first.craftedBuilding.Get().GetDescriptionInfo();
                    }
                    else if (first.craftedUnit != null)
                    {
                        Building[] requiredBuildings;
                        DescriptionInfo descriptionInfo;
                        DBDef.Unit local3 = first.craftedUnit.Get();
                        if (local3 != null)
                        {
                            requiredBuildings = local3.requiredBuildings;
                        }
                        else
                        {
                            DBDef.Unit local4 = local3;
                            requiredBuildings = null;
                        }
                        buildingArray = requiredBuildings;
                        DBDef.Unit local5 = first.craftedUnit.Get();
                        if (local5 != null)
                        {
                            descriptionInfo = local5.GetDescriptionInfo();
                        }
                        else
                        {
                            DBDef.Unit local6 = local5;
                            descriptionInfo = null;
                        }
                        descriptionInfo = descriptionInfo;
                    }
                }
                string message = "";
                if (buildingArray != null)
                {
                    Building[] buildingArray2 = buildingArray;
                    for (int i = 0; i < buildingArray2.Length; i++)
                    {
                        if (ReferenceEquals(buildingArray2[i], building))
                        {
                            object[] objArray4 = new object[] { descriptionInfo.GetLocalizedName() };
                            message = DBUtils.Localization.Get("UI_SELL_BUILDING_STOP_CRAFT", true, objArray4) + "\n\n";
                        }
                    }
                }
                int cost = building.buildCost / 3;
                object[] parameters = new object[] { cost };
                message = message + DBUtils.Localization.Get("UI_SELL_BUILDING_SURE", true, parameters);
                PopupGeneral.OpenPopup(townScreen, building.GetDescriptionInfo().GetLocalizedName(), message, "UI_SELL", delegate (object o) {
                    PlayerWizard humanWizard = GameManager.GetHumanWizard();
                    humanWizard.money += cost;
                    town.RemoveBuilding(building, true);
                    HUD hud = HUD.Get();
                    bool flag = town.craftingQueue.SanitizeRequirementsAfterRemovalOfBuilding(building);
                    if (hud)
                    {
                        hud.UpdateHUD();
                    }
                    if (townScreen != null)
                    {
                        if (flag)
                        {
                            townScreen.UpdateRightPanel();
                        }
                        else
                        {
                            townScreen.UpdateBuildingsTab();
                        }
                        townScreen.UpdateTopPanel(true);
                        townScreen.UpdateCenterVisuals();
                        townScreen.UpdateCraftingManager();
                    }
                }, "UI_CANCEL", null, null, null, null);
            }
        }

        public override void Set(object o, object data, int index)
        {
            DBReference<Building> reference = o as DBReference<Building>;
            this.building = (reference != null) ? reference.Get() : (o as Building);
            this.town = data as TownLocation;
            this.labelBuildingName.text = this.building.GetDescriptionInfo().GetLocalizedName();
            this.labelGoldUpkeep.text = "-" + this.building.upkeepCost.ToString();
            this.labelManaUpkeep.text = "-" + this.building.upkeepManaCost.ToString();
            if (this.building.upkeepManaCost == 0)
            {
                this.goManaUpkeep.SetActive(false);
            }
            else
            {
                this.goManaUpkeep.SetActive(true);
            }
            if (this.building.upkeepCost == 0)
            {
                this.goGoldUpkeep.SetActive(false);
            }
            else
            {
                this.goGoldUpkeep.SetActive(true);
            }
            this.iconBuilding.texture = DescriptionInfoExtension.GetTexture(this.building);
            this.btDemolish.gameObject.SetActive(ReferenceEquals(this.town.CanRemoveBuilding(this.building, false), null));
            if (this.tooltip)
            {
                this.tooltip.source = this.building;
            }
        }
    }
}

