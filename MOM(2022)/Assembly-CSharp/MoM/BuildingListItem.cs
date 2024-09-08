using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MOM
{
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
            this.btDemolish.onClick.AddListener(delegate
            {
                BuildingListItem.Sell(this.town, this.building, this.townScreen);
            });
            if (this.supportTooltip)
            {
                this.tooltip = base.gameObject.GetOrAddComponent<RolloverObject>();
            }
        }

        public static void Sell(TownLocation town, Building building, TownScreen townScreen)
        {
            Building building2 = town.CanRemoveBuilding(building);
            if (building2 != null)
            {
                string localizedName = building.GetDescriptionInfo().GetLocalizedName();
                if (building2 == building)
                {
                    if (building2 == (Building)BUILDING.ASTRAL_GATE || building2 == (Building)BUILDING.EARTH_GATE || building2 == (Building)BUILDING.NATURES_EYE || building2 == (Building)BUILDING.ALTAR_OF_BATTLE || building2 == (Building)BUILDING.STREAM_OF_LIFE)
                    {
                        PopupGeneral.OpenPopup(townScreen, localizedName, global::DBUtils.Localization.Get("UI_CAN_NOT_SELL_ENCHANTMENT", true, localizedName), "UI_OKAY");
                    }
                    else
                    {
                        PopupGeneral.OpenPopup(townScreen, localizedName, global::DBUtils.Localization.Get("UI_CAN_NOT_SELL", true, localizedName), "UI_OKAY");
                    }
                }
                else
                {
                    PopupGeneral.OpenPopup(townScreen, localizedName, global::DBUtils.Localization.Get("UI_CAN_NOT_SELL_DEPENDENCY", true, building2.GetDescriptionInfo().GetLocalizedName()), "UI_OKAY");
                }
                return;
            }
            Building[] array = null;
            CraftingItem first = town.craftingQueue.GetFirst();
            DescriptionInfo descriptionInfo = null;
            if (first != null)
            {
                if (first.craftedBuilding != null)
                {
                    array = first.craftedBuilding.Get()?.parentBuildingRequired;
                    descriptionInfo = first.craftedBuilding.Get().GetDescriptionInfo();
                }
                else if (first.craftedUnit != null)
                {
                    array = first.craftedUnit.Get()?.requiredBuildings;
                    descriptionInfo = first.craftedUnit.Get()?.GetDescriptionInfo();
                }
            }
            string text = "";
            if (array != null)
            {
                Building[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    if (array2[i] == building)
                    {
                        text = global::DBUtils.Localization.Get("UI_SELL_BUILDING_STOP_CRAFT", true, descriptionInfo.GetLocalizedName()) + "\n\n";
                    }
                }
            }
            int cost = building.buildCost / 3;
            text += global::DBUtils.Localization.Get("UI_SELL_BUILDING_SURE", true, cost);
            PopupGeneral.OpenPopup(townScreen, building.GetDescriptionInfo().GetLocalizedName(), text, "UI_SELL", delegate
            {
                GameManager.GetHumanWizard().money += cost;
                town.RemoveBuilding(building);
                HUD hUD = HUD.Get();
                bool flag = town.craftingQueue.SanitizeRequirementsAfterRemovalOfBuilding(building);
                if ((bool)hUD)
                {
                    hUD.UpdateHUD();
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
                    townScreen.UpdateTopPanel();
                    townScreen.UpdateCenterVisuals();
                    townScreen.UpdateCraftingManager();
                }
            }, "UI_CANCEL");
        }

        public override void Set(object o, object data, int index)
        {
            DBReference<Building> dBReference = o as DBReference<Building>;
            if (dBReference == null)
            {
                this.building = o as Building;
            }
            else
            {
                this.building = dBReference.Get();
            }
            this.town = data as TownLocation;
            this.labelBuildingName.text = this.building.GetDescriptionInfo().GetLocalizedName();
            this.labelGoldUpkeep.text = "-" + this.building.upkeepCost;
            this.labelManaUpkeep.text = "-" + this.building.upkeepManaCost;
            if (this.building.upkeepManaCost == 0)
            {
                this.goManaUpkeep.SetActive(value: false);
            }
            else
            {
                this.goManaUpkeep.SetActive(value: true);
            }
            if (this.building.upkeepCost == 0)
            {
                this.goGoldUpkeep.SetActive(value: false);
            }
            else
            {
                this.goGoldUpkeep.SetActive(value: true);
            }
            this.iconBuilding.texture = this.building.GetTexture();
            this.btDemolish.gameObject.SetActive(this.town.CanRemoveBuilding(this.building) == null);
            if ((bool)this.tooltip)
            {
                this.tooltip.source = this.building;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            (this.townScreen?.GetComponentInChildren<TownMap>())?.Select(this.building);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            (this.townScreen?.GetComponentInChildren<TownMap>())?.ClearSelection();
        }
    }
}
