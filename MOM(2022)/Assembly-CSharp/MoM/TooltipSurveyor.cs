using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using WorldCode;

namespace MOM
{
    public class TooltipSurveyor : TooltipBase
    {
        public TextMeshProUGUI terrainName;

        public TextMeshProUGUI food;

        public TextMeshProUGUI production;

        public TextMeshProUGUI gold;

        public TextMeshProUGUI locationName;

        public TextMeshProUGUI locationHeldBy;

        public TextMeshProUGUI cannotBuildReason;

        public TextMeshProUGUI maxPopulation;

        public TextMeshProUGUI maxProductionBonus;

        public TextMeshProUGUI maxGoldBonus;

        public TextMeshProUGUI defenders;

        public TextMeshProUGUI movementCost;

        public GridItemManager enchSkillsGrid;

        public GameObject goTerrainBonuses;

        public GameObject goFood;

        public GameObject goProduction;

        public GameObject goGold;

        public EnchantmentInfo enchantmentInfo;

        public GameObject goTileCorrupted;

        public GameObject goRoad;

        public GameObject goEnchantedRoad;

        public GameObject goLocation;

        public GameObject goLocationUnexplored;

        public GameObject goCannotBuildCity;

        public GameObject goCanBuildCity;

        public GameObject goPopBonus;

        public GameObject goProdBonus;

        public GameObject goGoldBonus;

        public List<EnchantmentInfo> enchantmentInfos = new List<EnchantmentInfo>();

        public ResourceListItem resourceListItem;

        public RoadManager rm;

        private static Hex currentHex;

        private static bool isOpen;

        private List<GameObject> defenderDuplicates = new List<GameObject>();

        public static void DoUpdate(bool openAllowed)
        {
            if (openAllowed)
            {
                Vector3 clickWorldPosition = CameraController.GetClickWorldPosition(flat: true, mousePosition: true, checkForUILock: true);
                Vector3i invalid = Vector3i.invalid;
                Hex hex = null;
                global::WorldCode.Plane activePlane = World.GetActivePlane();
                if (clickWorldPosition != -Vector3.one)
                {
                    invalid = HexCoordinates.GetHexCoordAt(clickWorldPosition);
                    if (activePlane != null)
                    {
                        invalid = activePlane.area.KeepHorizontalInside(invalid);
                        if (FOW.Get().IsDiscovered(invalid, activePlane))
                        {
                            hex = activePlane.GetHexAt(invalid);
                        }
                    }
                }
                if (hex != null)
                {
                    if (!TooltipSurveyor.isOpen)
                    {
                        TooltipSurveyor.isOpen = true;
                        TooltipSurveyor.currentHex = hex;
                        CursorsLibrary.SetMode(CursorsLibrary.Mode.Surveyor);
                        TooltipBase.OpenSurveyor();
                    }
                    else if (hex != TooltipSurveyor.currentHex)
                    {
                        TooltipSurveyor.currentHex = hex;
                        TooltipBase.RefreshInstance();
                    }
                }
                else
                {
                    openAllowed = false;
                }
            }
            if (!openAllowed && TooltipSurveyor.isOpen)
            {
                TooltipSurveyor.isOpen = false;
                TooltipBase.Close();
            }
        }

        public void Awake()
        {
            this.enchantmentInfos.Add(this.enchantmentInfo);
            ScreenBase.LocalizeTextFields(base.gameObject);
        }

        public override void Refresh()
        {
            global::WorldCode.Plane activePlane = World.GetActivePlane();
            Terrain terrain = TooltipSurveyor.currentHex.GetTerrain();
            this.terrainName.text = terrain.GetDILocalizedName();
            if (activePlane.GetRoadManagers() != null)
            {
                RoadManager roadManagers = activePlane.GetRoadManagers();
                FInt roadAt = roadManagers.GetRoadAt(TooltipSurveyor.currentHex.Position);
                RoadManager.RoadType roadTypeAt = roadManagers.GetRoadTypeAt(TooltipSurveyor.currentHex.Position);
                this.goRoad.SetActive(roadTypeAt == RoadManager.RoadType.Normal);
                this.goEnchantedRoad.SetActive(roadTypeAt == RoadManager.RoadType.Enchanted);
                if (roadAt > FInt.ZERO)
                {
                    this.movementCost.text = roadAt.ToString(1);
                }
                else
                {
                    this.movementCost.text = terrain.movementCost.ToString();
                }
            }
            else
            {
                this.movementCost.text = terrain.movementCost.ToString();
            }
            bool flag = false;
            FInt fInt = TooltipSurveyor.currentHex.GetFood();
            this.goFood.SetActive(fInt > 0);
            flag = this.goFood.activeSelf;
            this.food.text = fInt.ToString();
            FInt fInt2 = terrain.production;
            this.goProduction.SetActive(fInt2 > 0);
            flag |= this.goProduction.activeSelf;
            this.production.text = "+" + fInt2.ToIntX100() + "%";
            FInt goldProduction = terrain.goldProduction;
            this.gold.text = "+" + goldProduction.ToIntX100() + "%";
            this.goGold.SetActive(goldProduction > 0);
            flag |= this.goGold.activeSelf;
            this.goTileCorrupted.SetActive(!TooltipSurveyor.currentHex.ActiveHex);
            flag |= this.goTileCorrupted.activeSelf;
            this.goTerrainBonuses.SetActive(flag);
            List<Enchantment> list = TooltipSurveyor.currentHex.CoastRiverBonusEnchantments();
            while (this.enchantmentInfos.Count < list.Count)
            {
                EnchantmentInfo component = Object.Instantiate(this.enchantmentInfo.gameObject, this.enchantmentInfo.transform.parent).GetComponent<EnchantmentInfo>();
                this.enchantmentInfos.Add(component);
            }
            for (int i = 0; i < this.enchantmentInfos.Count; i++)
            {
                EnchantmentInfo enchantmentInfo = this.enchantmentInfos[i];
                bool flag2 = i < list.Count;
                enchantmentInfo.gameObject.SetActive(flag2);
                if (flag2)
                {
                    enchantmentInfo.Set(list[i]);
                }
            }
            this.enchSkillsGrid.CustomDynamicItem(EnchItem);
            bool flag3 = false;
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(activePlane);
            if (locationsOfThePlane != null)
            {
                Location location = locationsOfThePlane.Find((Location o) => o.GetPosition() == TooltipSurveyor.currentHex.Position);
                if (location != null)
                {
                    List<EnchantmentInstance> list2 = location.GetEnchantments().FindAll((EnchantmentInstance o) => !o.source.Get().hideEnch && !o.buildingEnchantment);
                    if (list2 != null)
                    {
                        this.enchSkillsGrid.UpdateGrid(list2);
                        flag3 = true;
                    }
                }
            }
            if (!flag3)
            {
                this.enchSkillsGrid.UpdateGrid(new List<EnchantmentInstance>());
            }
            this.resourceListItem.gameObject.SetActive(TooltipSurveyor.currentHex.Resource != null);
            if (TooltipSurveyor.currentHex.Resource != null)
            {
                Resource o2 = TooltipSurveyor.currentHex.Resource.Get();
                this.resourceListItem.Set(o2, null, -1);
                this.resourceListItem.image.material = (TooltipSurveyor.currentHex.ActiveHex ? null : UIReferences.GetGrayscale());
            }
            Location locationAt = GameManager.Get().GetLocationAt(TooltipSurveyor.currentHex.Position);
            this.goLocation.SetActive(locationAt != null);
            if (locationAt != null)
            {
                bool flag4 = locationAt.IsExplored();
                this.goLocationUnexplored.SetActive(!flag4);
                this.locationName.text = locationAt.GetName();
                this.locationHeldBy.gameObject.SetActive(value: false);
                PlayerWizard wizardOwner = locationAt.GetWizardOwner();
                if (wizardOwner != null)
                {
                    this.locationHeldBy.gameObject.SetActive(value: true);
                    this.locationHeldBy.color = WizardColors.GetColor(wizardOwner);
                    this.locationHeldBy.text = global::DBUtils.Localization.Get("UI_HELD_BY", true, wizardOwner.GetName());
                }
                if (locationAt.locationType == ELocationType.Node)
                {
                    this.locationName.text = locationAt.GetName() + " (" + global::DBUtils.Localization.Get("UI_NODE_POWER", true) + locationAt.NodePowerIncome() + ")";
                    if (locationAt.melding != null && locationAt.melding.meldOwner > 0)
                    {
                        PlayerWizard wizard = GameManager.GetWizard(locationAt.melding.meldOwner);
                        this.locationHeldBy.color = WizardColors.GetColor(wizard);
                        this.locationHeldBy.text = global::DBUtils.Localization.Get("UI_MELDED_BY", true, wizard.GetName());
                    }
                    else
                    {
                        this.locationHeldBy.gameObject.SetActive(value: false);
                    }
                }
                foreach (GameObject defenderDuplicate in this.defenderDuplicates)
                {
                    Object.Destroy(defenderDuplicate);
                }
                TownLocation townLocation = locationAt as TownLocation;
                this.defenders.gameObject.SetActive(value: false);
                if (townLocation != null && townLocation.owner != PlayerWizard.HumanID())
                {
                    flag4 = townLocation.CanSeeInside();
                    if (!flag4)
                    {
                        this.defenders.gameObject.SetActive(value: true);
                        this.defenders.text = global::DBUtils.Localization.Get("UI_UNKNOWN_TOWN_UNITS", true);
                    }
                }
                if (flag4)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>();
                    foreach (Reference<Unit> unit in locationAt.GetUnits())
                    {
                        string key = unit.Get().GetName();
                        dictionary.TryGetValue(key, out var value);
                        dictionary[key] = value + 1;
                    }
                    int num = this.defenders.transform.GetSiblingIndex() + 1;
                    foreach (KeyValuePair<string, int> item in dictionary)
                    {
                        GameObject gameObject = Object.Instantiate(this.defenders.gameObject, this.defenders.transform.parent);
                        gameObject.transform.SetSiblingIndex(num++);
                        gameObject.SetActive(value: true);
                        gameObject.GetComponent<TextMeshProUGUI>().text = item.Value + "x " + item.Key;
                        this.defenderDuplicates.Add(gameObject);
                    }
                }
            }
            string text = Location.CanBuildTownAtLocation(activePlane, TooltipSurveyor.currentHex.Position);
            this.goCannotBuildCity.SetActive(text != null);
            this.cannotBuildReason.text = global::DBUtils.Localization.Get(text, true);
            this.goCanBuildCity.SetActive(text == null);
            if (text != null)
            {
                return;
            }
            List<Vector3i> range = HexNeighbors.GetRange(TooltipSurveyor.currentHex.Position, 2);
            FInt zERO = FInt.ZERO;
            FInt zERO2 = FInt.ZERO;
            FInt zERO3 = FInt.ZERO;
            foreach (Vector3i item2 in range)
            {
                Hex hexAt = activePlane.GetHexAt(item2);
                if (hexAt != null)
                {
                    Terrain terrain2 = hexAt.GetTerrain();
                    zERO += hexAt.GetFood();
                    if (terrain2 != null)
                    {
                        zERO2 += terrain2.production;
                        zERO3 += terrain2.goldProduction;
                    }
                }
            }
            this.maxPopulation.text = zERO.ToInt().ToString();
            this.goPopBonus.SetActive(zERO != FInt.ZERO);
            this.maxProductionBonus.text = "+" + zERO2.ToIntX100() + "%";
            this.goProdBonus.SetActive(zERO2 != FInt.ZERO);
            this.maxGoldBonus.text = "+" + zERO3.ToIntX100() + "%";
            this.goGoldBonus.SetActive(zERO3 != FInt.ZERO);
        }

        private void EnchItem(GameObject itemSource, object source, object data, int index)
        {
            EnchantmentInstance enchantmentInstance = source as EnchantmentInstance;
            itemSource.GetComponent<SimpleListItem>().icon.texture = enchantmentInstance.source.Get().GetTexture();
        }
    }
}
