using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MOM;
using TMPro;
using UnityEngine;
using WorldCode;

namespace MHUtils.UI
{
    public class VerticalMarkerManager : MonoBehaviour
    {
        public enum MarkerType
        {
            markerTown = 0,
            markerArmy = 1,
            markerLocation = 2,
            markerCombatUnit = 3,
            markerMovement = 4,
            markerCombatEffect = 5,
            markerNode = 6,
            markerResource = 7,
            MAX = 8
        }

        private static VerticalMarkerManager instance;

        private static int ID;

        private GameObject markerLayer;

        private GameObject markerResourceLayer;

        public bool doUpdate = true;

        private Dictionary<object, MarkerData> markers = new Dictionary<object, MarkerData>();

        private Dictionary<MarkerType, Stack<GameObject>> markerPool = new Dictionary<MarkerType, Stack<GameObject>>();

        private bool showResources;

        private void Start()
        {
            VerticalMarkerManager.instance = this;
            VerticalMarkerManager.ID = global::UnityEngine.Random.Range(0, int.MaxValue);
            Debug.Log("MARKER ID " + VerticalMarkerManager.ID);
            this.markerLayer = UIManager.GetLayer(UIManager.Layer.UIMarkers);
            this.markerResourceLayer = UIManager.GetLayer(UIManager.Layer.UIMarkersResources);
            for (int i = 0; i < 8; i++)
            {
                this.markerPool[(MarkerType)i] = new Stack<GameObject>();
            }
            CanvasGroup orAddComponent = this.markerLayer.GetOrAddComponent<CanvasGroup>();
            orAddComponent.interactable = false;
            orAddComponent.blocksRaycasts = false;
            CanvasGroup orAddComponent2 = this.markerResourceLayer.GetOrAddComponent<CanvasGroup>();
            orAddComponent2.interactable = false;
            orAddComponent2.blocksRaycasts = false;
            MHEventSystem.RegisterListener<EnchantmentManager>(EnchantmentChanged, this);
        }

        private void OnDestroy()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        private void EnchantmentChanged(object sender, object e)
        {
            if (sender is EnchantmentManager enchantmentManager && enchantmentManager.owner is BattleUnit)
            {
                this.UpdateInfoOnMarker(enchantmentManager.owner);
            }
        }

        private void LateUpdate()
        {
            if (!this.doUpdate || UIManager.Get() == null)
            {
                this.markerLayer.SetActive(value: false);
                return;
            }
            if (!this.markerLayer.activeSelf)
            {
                this.markerLayer.SetActive(value: true);
            }
            Vector3 localScale = UIManager.Get().transform.localScale;
            _ = Vector3.up * 2f;
            Camera main = Camera.main;
            global::WorldCode.Plane activePlane = World.GetActivePlane();
            if (activePlane == null)
            {
                return;
            }
            bool showResourcesOnHUD = GameManager.Get().showResourcesOnHUD;
            foreach (KeyValuePair<object, MarkerData> marker in this.markers)
            {
                if (marker.Value.displayObject == null)
                {
                    continue;
                }
                if (marker.Value.dirty)
                {
                    this.UpdateInfoOnMarker(marker.Key);
                }
                if (marker.Value.owner is global::MOM.Group)
                {
                    global::MOM.Group group = marker.Value.owner as global::MOM.Group;
                    Vector3 physicalPosition = group.GetPhysicalPosition();
                    if (marker.Value.Position != physicalPosition)
                    {
                        marker.Value.cellPosition = group.GetPosition();
                        marker.Value.Position = physicalPosition;
                    }
                }
                else if (marker.Value.owner is BattleUnit)
                {
                    IPlanePosition planePosition = marker.Value.owner as IPlanePosition;
                    if (marker.Value.cellPosition != planePosition.GetPosition())
                    {
                        marker.Value.cellPosition = planePosition.GetPosition();
                        marker.Value.Position = HexCoordinates.HexToWorld3D(marker.Value.cellPosition);
                    }
                }
                else if (marker.Value.owner is global::MOM.Location)
                {
                    global::MOM.Location location = marker.Value.owner as global::MOM.Location;
                    Vector3 physicalPosition2 = location.GetPhysicalPosition();
                    if (marker.Value.Position != physicalPosition2)
                    {
                        marker.Value.cellPosition = location.GetPosition();
                        marker.Value.Position = physicalPosition2;
                    }
                }
                else if (marker.Value.owner is Hex hex)
                {
                    if (!showResourcesOnHUD)
                    {
                        marker.Value.displayObject.SetActive(value: false);
                        continue;
                    }
                    marker.Value.displayObject.SetActive(value: true);
                    Vector3 vector = activePlane.GetChunkFor(hex.Position).go.transform.position + HexCoordinates.HexToWorld3D(hex.Position);
                    if (marker.Value.Position != vector)
                    {
                        marker.Value.cellPosition = hex.Position;
                        marker.Value.Position = vector;
                    }
                }
                Vector3 position = marker.Value.Position;
                _ = marker.Key;
                Vector3 vector2 = main.WorldToViewportPoint(position);
                Vector3 zero = Vector3.zero;
                float num = (float)Screen.width / localScale.x;
                float num2 = (float)Screen.height / localScale.y;
                float num3 = vector2.x * num;
                float num4 = vector2.y * num2;
                float num5 = 1f - vector2.y * 0.5f;
                zero.x = num3 - num / 2f;
                zero.y = num2 * num5 / 8f + num4 - num2 / 2f;
                if (marker.Value.container != null && marker.Value.displayObject != null)
                {
                    marker.Value.displayObject.transform.SetParent(marker.Value.container.transform.parent);
                    Debug.Log("positionChanged leave container");
                    if (marker.Value.container.transform.childCount == 0)
                    {
                        global::UnityEngine.Object.Destroy(marker.Value.container);
                    }
                    marker.Value.container = null;
                    marker.Value.displayObject.transform.SetAsLastSibling();
                }
                if (marker.Value.container != null && marker.Value.displayObject != null)
                {
                    marker.Value.container.transform.localPosition = zero;
                }
                else if (marker.Value.displayObject != null)
                {
                    marker.Value.displayObject.transform.localPosition = zero;
                }
            }
        }

        public static VerticalMarkerManager Get()
        {
            return VerticalMarkerManager.instance;
        }

        public static void MovementMode(bool on)
        {
            CanvasGroup component = VerticalMarkerManager.Get().markerResourceLayer.GetComponent<CanvasGroup>();
            if (on)
            {
                component.blocksRaycasts = false;
                component.alpha = 0.4f;
            }
            else
            {
                component.blocksRaycasts = true;
                component.alpha = 1f;
            }
        }

        public void DestroyMarker(object owner, bool removeFromMarkers = true)
        {
            if (owner != null && this.markers.ContainsKey(owner))
            {
                MarkerData markerData = this.markers[owner];
                this.markerPool[markerData.markerType].Push(markerData.displayObject);
                markerData.displayObject.SetActive(value: false);
                if (removeFromMarkers)
                {
                    this.markers.Remove(owner);
                }
            }
        }

        public void ClearBattleMarkers()
        {
            if (this.markers == null)
            {
                return;
            }
            List<BattleUnit> list = new List<BattleUnit>();
            foreach (KeyValuePair<object, MarkerData> marker in this.markers)
            {
                if (marker.Key is BattleUnit)
                {
                    list.Add(marker.Key as BattleUnit);
                }
            }
            list.ForEach(delegate(BattleUnit o)
            {
                this.DestroyMarker(o);
            });
        }

        public void Addmarker(object owner)
        {
            if (this.markers.ContainsKey(owner) || (owner is IPlanePosition && (owner as IPlanePosition).GetPlane() != World.GetActivePlane()))
            {
                return;
            }
            if (owner is TerrainMarkers terrainMarkers)
            {
                GameObject unusedMarker = this.GetUnusedMarker(MarkerType.markerMovement);
                MarkerData markerData = new MarkerData();
                markerData.cellPosition = terrainMarkers.destination;
                markerData.displayObject = unusedMarker;
                markerData.markerType = MarkerType.markerMovement;
                markerData.owner = terrainMarkers;
                markerData.Position = HexCoordinates.HexToWorld3D(markerData.cellPosition);
                unusedMarker.transform.SetAsLastSibling();
                this.markers.Add(terrainMarkers, markerData);
                this.UpdateInfoOnMarker(terrainMarkers);
            }
            if (owner is Hex owner2)
            {
                Hex hex = owner as Hex;
                if (hex.Resource != null)
                {
                    MarkerType markerType = MarkerType.markerResource;
                    GameObject unusedMarker2 = this.GetUnusedMarker(markerType);
                    MarkerData markerData2 = new MarkerData();
                    markerData2.cellPosition = hex.Position;
                    markerData2.displayObject = unusedMarker2;
                    markerData2.markerType = markerType;
                    markerData2.owner = hex;
                    markerData2.Position = HexCoordinates.HexToWorld3D(markerData2.cellPosition);
                    unusedMarker2.GetComponent<MarkerResource>().resourceIcon.texture = hex.Resource.Get().descriptionInfo.GetTexture();
                    this.markers.Add(hex, markerData2);
                    this.UpdateInfoOnMarker(owner2);
                }
            }
            else if (owner is CombatEffect combatEffect)
            {
                GameObject unusedMarker3 = this.GetUnusedMarker(MarkerType.markerCombatEffect);
                MarkerData markerData3 = new MarkerData();
                markerData3.cellPosition = combatEffect.unit.GetPosition();
                markerData3.displayObject = unusedMarker3;
                markerData3.markerType = MarkerType.markerCombatEffect;
                markerData3.owner = combatEffect;
                markerData3.Position = HexCoordinates.HexToWorld3D(markerData3.cellPosition);
                this.markers.Add(combatEffect, markerData3);
                this.UpdateInfoOnMarker(combatEffect);
            }
            else if (owner is global::MOM.Location)
            {
                global::MOM.Location location = owner as global::MOM.Location;
                if (location is TownLocation && !location.skipMarker)
                {
                    MarkerType markerType2 = MarkerType.markerTown;
                    TownLocation townLocation = location as TownLocation;
                    GameObject unusedMarker4 = this.GetUnusedMarker(markerType2);
                    MarkerData markerData4 = new MarkerData();
                    markerData4.cellPosition = location.GetPosition();
                    markerData4.displayObject = unusedMarker4;
                    markerData4.markerType = markerType2;
                    markerData4.owner = location;
                    markerData4.Position = HexCoordinates.HexToWorld3D(markerData4.cellPosition);
                    MarkerTown component = unusedMarker4.GetComponent<MarkerTown>();
                    GameManager.GetWizard(location.GetOwnerID());
                    PlayerWizard.Color color = this.GetColor(location.GetOwnerID());
                    component.goBrown.SetActive(color == PlayerWizard.Color.None);
                    component.goBlue.SetActive(color == PlayerWizard.Color.Blue);
                    component.goGreen.SetActive(color == PlayerWizard.Color.Green);
                    component.goPurple.SetActive(color == PlayerWizard.Color.Purple);
                    component.goRed.SetActive(color == PlayerWizard.Color.Red);
                    component.goYellow.SetActive(color == PlayerWizard.Color.Yellow);
                    component.labelName.text = townLocation.name;
                    this.markers.Add(location, markerData4);
                    component.gridEnchantments.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
                    {
                        EnchantmentInstance enchantmentInstance3 = source as EnchantmentInstance;
                        CombatUnitEnchantmentItem component7 = itemSource.GetComponent<CombatUnitEnchantmentItem>();
                        EEnchantmentCategory enchantmentType3 = enchantmentInstance3.GetEnchantmentType();
                        component7.good.SetActive(enchantmentType3 == EEnchantmentCategory.Positive);
                        component7.bad.SetActive(enchantmentType3 == EEnchantmentCategory.Negative);
                        component7.other.SetActive(enchantmentType3 == EEnchantmentCategory.None);
                    });
                    this.UpdateInfoOnMarker(location);
                }
                else
                {
                    if (!location.skipMarker)
                    {
                        MarkerType markerType3 = MarkerType.markerLocation;
                        GameObject unusedMarker5 = this.GetUnusedMarker(markerType3);
                        MarkerData markerData5 = new MarkerData();
                        markerData5.cellPosition = location.GetPosition();
                        markerData5.displayObject = unusedMarker5;
                        markerData5.markerType = markerType3;
                        markerData5.owner = location;
                        markerData5.Position = HexCoordinates.HexToWorld3D(markerData5.cellPosition);
                        this.markers.Add(location, markerData5);
                    }
                    if (location.locationType == ELocationType.Node && !this.markers.ContainsKey(location.GetLocalGroup()))
                    {
                        owner = location.GetLocalGroup();
                    }
                    if (location.locationType == ELocationType.Node)
                    {
                        MarkerType markerType4 = MarkerType.markerNode;
                        GameObject unusedMarker6 = this.GetUnusedMarker(markerType4);
                        MarkerNode component2 = unusedMarker6.GetComponent<MarkerNode>();
                        MarkerData markerData6 = new MarkerData();
                        PlayerWizard wizard = GameManager.GetWizard(location.melding?.meldOwner ?? 0);
                        if (wizard != null)
                        {
                            component2.nodeOwnerColour.color = WizardColors.GetColor(wizard);
                        }
                        else
                        {
                            component2.nodeOwnerColour.color = new Color32(132, 121, 104, byte.MaxValue);
                        }
                        component2.labelNodePower.text = location.NodePowerIncome().ToString();
                        markerData6.cellPosition = location.GetPosition();
                        markerData6.displayObject = unusedMarker6;
                        markerData6.markerType = markerType4;
                        markerData6.owner = location;
                        markerData6.Position = HexCoordinates.HexToWorld3D(markerData6.cellPosition);
                        this.markers.Add(location, markerData6);
                        if (!component2.gridEnchantments.gameObject.activeInHierarchy)
                        {
                            component2.gridEnchantments.gameObject.SetActive(value: true);
                        }
                        component2.gridEnchantments.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
                        {
                            EnchantmentInstance enchantmentInstance2 = source as EnchantmentInstance;
                            CombatUnitEnchantmentItem component6 = itemSource.GetComponent<CombatUnitEnchantmentItem>();
                            EEnchantmentCategory enchantmentType2 = enchantmentInstance2.GetEnchantmentType();
                            component6.good.SetActive(enchantmentType2 == EEnchantmentCategory.Positive);
                            component6.bad.SetActive(enchantmentType2 == EEnchantmentCategory.Negative);
                            component6.other.SetActive(enchantmentType2 == EEnchantmentCategory.None);
                        });
                        this.UpdateInfoOnMarker(location);
                    }
                }
            }
            if (owner is global::MOM.Group)
            {
                global::MOM.Group group = owner as global::MOM.Group;
                GameObject unusedMarker7 = this.GetUnusedMarker(MarkerType.markerArmy);
                MarkerData markerData7 = new MarkerData();
                markerData7.cellPosition = group.GetPosition();
                markerData7.displayObject = unusedMarker7;
                markerData7.markerType = MarkerType.markerArmy;
                markerData7.owner = group;
                markerData7.Position = HexCoordinates.HexToWorld3D(markerData7.cellPosition);
                PlayerWizard.Color color2 = this.GetColor(group.GetOwnerID());
                MarkerArmy component3 = unusedMarker7.GetComponent<MarkerArmy>();
                component3.goBrown.SetActive(color2 == PlayerWizard.Color.None);
                component3.goBlue.SetActive(color2 == PlayerWizard.Color.Blue);
                component3.goGreen.SetActive(color2 == PlayerWizard.Color.Green);
                component3.goPurple.SetActive(color2 == PlayerWizard.Color.Purple);
                component3.goRed.SetActive(color2 == PlayerWizard.Color.Red);
                component3.goYellow.SetActive(color2 == PlayerWizard.Color.Yellow);
                this.markers.Add(group, markerData7);
                this.UpdateInfoOnMarker(group);
            }
            else if (owner is BattleUnit)
            {
                MarkerType markerType5 = MarkerType.markerCombatUnit;
                BattleUnit battleUnit = owner as BattleUnit;
                GameObject unusedMarker8 = this.GetUnusedMarker(markerType5);
                MarkerData markerData8 = new MarkerData();
                markerData8.cellPosition = battleUnit.GetPosition();
                markerData8.displayObject = unusedMarker8;
                markerData8.markerType = markerType5;
                markerData8.owner = battleUnit;
                markerData8.Position = HexCoordinates.HexToWorld3D(markerData8.cellPosition);
                PlayerWizard.Color color3 = this.GetColor(battleUnit.GetWizardOwner()?.ID ?? 0);
                MarkerCombatUnit component4 = unusedMarker8.GetComponent<MarkerCombatUnit>();
                component4.goBrown.SetActive(color3 == PlayerWizard.Color.None);
                component4.goBlue.SetActive(color3 == PlayerWizard.Color.Blue);
                component4.goGreen.SetActive(color3 == PlayerWizard.Color.Green);
                component4.goPurple.SetActive(color3 == PlayerWizard.Color.Purple);
                component4.goRed.SetActive(color3 == PlayerWizard.Color.Red);
                component4.goYellow.SetActive(color3 == PlayerWizard.Color.Yellow);
                bool active = false;
                bool active2 = false;
                bool flag = false;
                Battle battle = Battle.GetBattle();
                if (battle != null)
                {
                    active = !battle.landBattle && battleUnit.GetAttFinal(TAG.CAN_WALK) > 0 && battleUnit.GetAttFinal(TAG.CAN_SWIM) < 0;
                    active2 = battle.landBattle && battleUnit.GetAttFinal(TAG.CAN_WALK) < 0 && battleUnit.GetAttFinal(TAG.CAN_SWIM) > 0;
                }
                flag = battleUnit.GetAttFinal(TAG.CAN_FLY) > 0;
                component4.goWaterOnly.SetActive(active2);
                component4.goGroundOnly.SetActive(active);
                component4.goFlying.SetActive(flag);
                component4.classIcon.texture = AssetManager.Get<Texture2D>(battleUnit.dbSource.Get().marker);
                component4.sliderHp.value = battleUnit.GetTotalHpPercent();
                component4.sliderHp.gameObject.SetActive(battleUnit.GetTotalHpPercent() != 1f);
                component4.gridEnchantments.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
                {
                    EnchantmentInstance enchantmentInstance = source as EnchantmentInstance;
                    CombatUnitEnchantmentItem component5 = itemSource.GetComponent<CombatUnitEnchantmentItem>();
                    EEnchantmentCategory enchantmentType = enchantmentInstance.GetEnchantmentType();
                    component5.good.SetActive(enchantmentType == EEnchantmentCategory.Positive);
                    component5.bad.SetActive(enchantmentType == EEnchantmentCategory.Negative);
                    component5.other.SetActive(enchantmentType == EEnchantmentCategory.None);
                });
                this.markers.Add(battleUnit, markerData8);
                this.UpdateInfoOnMarker(owner);
            }
        }

        private PlayerWizard.Color GetColor(int wizard)
        {
            if (wizard < 1)
            {
                return PlayerWizard.Color.None;
            }
            PlayerWizard wizard2 = GameManager.GetWizard(wizard);
            PlayerWizard.Color result = PlayerWizard.Color.None;
            if (wizard2 != null)
            {
                result = wizard2.color;
            }
            return result;
        }

        public void ResetSystem()
        {
            this.Clearmarkers();
            if (this.markerPool != null)
            {
                foreach (KeyValuePair<MarkerType, Stack<GameObject>> item in this.markerPool)
                {
                    while (item.Value.Count > 0)
                    {
                        global::UnityEngine.Object.Destroy(item.Value.Pop());
                    }
                }
            }
            this.markerPool?.Clear();
        }

        public void Clearmarkers()
        {
            if (this.markers == null)
            {
                return;
            }
            foreach (KeyValuePair<object, MarkerData> marker in this.markers)
            {
                this.DestroyMarker(marker.Key, removeFromMarkers: false);
            }
            this.markers.Clear();
        }

        public void InitializeMarkers(global::WorldCode.Plane p)
        {
            List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(p);
            if (locationsOfThePlane != null)
            {
                foreach (global::MOM.Location item in locationsOfThePlane)
                {
                    if (item.IsMarkerVisible())
                    {
                        this.Addmarker(item);
                    }
                }
            }
            List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(p);
            if (groupsOfPlane != null)
            {
                foreach (global::MOM.Group item2 in groupsOfPlane)
                {
                    if (item2.locationHost == null && item2.IsMarkerVisible())
                    {
                        this.Addmarker(item2);
                    }
                }
            }
            foreach (KeyValuePair<Vector3i, Hex> hex in p.GetHexes())
            {
                if (hex.Value.Resource != null && FOW.Get().IsVisible(hex.Key, p))
                {
                    this.Addmarker(hex.Value);
                }
            }
        }

        public void MarkDirtyInfoOnMarker(object owner, bool onlyIfHashChanged = false)
        {
            if (this.markers.ContainsKey(owner))
            {
                this.markers[owner].dirty = true;
            }
        }

        public void UpdateInfoOnMarker(object owner, bool onlyIfHashChanged = false)
        {
            if (!this.markers.ContainsKey(owner))
            {
                return;
            }
            MarkerData markerData = this.markers[owner];
            GameObject displayObject = markerData.displayObject;
            markerData.dirty = false;
            if (owner is TerrainMarkers terrainMarkers)
            {
                displayObject.GetComponentInChildren<TextMeshProUGUI>().text = terrainMarkers.turns.ToString();
            }
            else if (owner is CombatEffect ce)
            {
                displayObject.GetComponent<OverheadEffect>().Set(ce);
            }
            if (owner is Hex)
            {
                Hex hex = owner as Hex;
                if (hex.Resource != null)
                {
                    displayObject.GetComponent<MarkerResource>().resourceIcon.texture = hex.Resource.Get().descriptionInfo.GetTexture();
                }
            }
            else if (owner is global::MOM.Group)
            {
                global::MOM.Group group = owner as global::MOM.Group;
                if (onlyIfHashChanged && markerData.groupHash == group.GetHash())
                {
                    return;
                }
                if (group.locationHost != null)
                {
                    if (group.GetOwnerID() != PlayerWizard.HumanID() || group.Action != global::MOM.Group.GroupActions.Guard)
                    {
                        displayObject.SetActive(value: false);
                        return;
                    }
                    displayObject.SetActive(value: true);
                }
                MarkerArmy component = displayObject.GetComponent<MarkerArmy>();
                component.labelArmySize.text = group.GetUnits().Count.ToString();
                if (group.GetOwnerID() == PlayerWizard.HumanID() && group.IsGroupInvisible())
                {
                    component.goUnitInvisible.SetActive(value: true);
                    component.goUnitVisible.SetActive(value: false);
                }
                else if (group.IsGroupInvisible())
                {
                    component.gameObject.SetActive(value: false);
                }
                else
                {
                    component.gameObject.SetActive(value: true);
                    component.goUnitInvisible.SetActive(value: false);
                    component.goUnitVisible.SetActive(value: false);
                }
                component.goGuard.SetActive(group.Action == global::MOM.Group.GroupActions.Guard);
                component.goFrame.SetActive(group.Action != global::MOM.Group.GroupActions.Guard);
                component.goWork.SetActive(group.isActivelyBuilding);
                component.goTaskTime.SetActive(group.isActivelyBuilding);
                if (group.engineerManager != null)
                {
                    component.labelTaskTimeLeft.text = group.engineerManager.TurnsOfWorkLeft().ToString();
                }
                else if (group.purificationManager != null)
                {
                    component.labelTaskTimeLeft.text = group.purificationManager.TurnsOfWorkLeft().ToString();
                }
                FInt fInt = group.CurentMP();
                if (group.GetOwnerID() == PlayerWizard.HumanID() && fInt == FInt.ZERO)
                {
                    component.canvasMarker.alpha = 0.75f;
                }
                else
                {
                    component.canvasMarker.alpha = 1f;
                }
                markerData.groupHash = group.GetHash();
            }
            else if (owner is global::MOM.Location)
            {
                global::MOM.Location location = owner as global::MOM.Location;
                if (location is TownLocation townLocation)
                {
                    MarkerTown component2 = displayObject.GetComponent<MarkerTown>();
                    PlayerWizard wizard = GameManager.GetWizard(location.GetOwnerID());
                    bool active = wizard != null && wizard.wizardTower == location;
                    component2.goCapital.SetActive(active);
                    bool active2 = wizard != null && townLocation.GetRebels() > 0 && townLocation.GetPopUnits() / 2 <= townLocation.GetRebels();
                    component2.goUnrestWarning.SetActive(active2);
                    bool active3 = wizard != null && townLocation.CalculateFoodIncome() < townLocation.GetPopUnits();
                    component2.goStarvationWarning.SetActive(active3);
                    if (wizard != null && wizard.ID == PlayerWizard.HumanID())
                    {
                        component2.goSummoningCircle.SetActive(wizard.summoningCircle == townLocation);
                        component2.goPopulation.SetActive(value: true);
                        component2.goProduction.SetActive(value: true);
                        CraftingItem first = townLocation.craftingQueue.GetFirst();
                        if (first != null)
                        {
                            int num = townLocation.CalculateProductionIncome();
                            if (num <= 0)
                            {
                                component2.labelBuildTime.text = "-";
                            }
                            else
                            {
                                float f = (float)(first.requirementValue - first.progress) / (float)num;
                                component2.labelBuildTime.text = Mathf.Max(1f, (float)Mathf.CeilToInt(f)).ToString();
                                if (component2.labelBuildTime.text == "0")
                                {
                                    component2.labelBuildTime.text = "-";
                                }
                            }
                            component2.riCurrentTask.texture = first.GetDI().GetTexture();
                        }
                        else
                        {
                            component2.labelBuildTime.text = "-";
                            component2.riCurrentTask.texture = null;
                            component2.goProduction.SetActive(value: false);
                        }
                        townLocation.GetLocalGroup();
                        component2.goGuard.SetActive(townLocation.GetUnits().Count > 0);
                        component2.labelNumberOfUnits.text = townLocation.GetUnits().Count.ToString();
                        component2.labelName.text = townLocation.name;
                        component2.labelPopulation.text = townLocation.GetPopUnits().ToString();
                        if (townLocation.GetPopIncreaseTime() < 1)
                        {
                            component2.labelPopIncreaseTime.text = "-";
                        }
                        else
                        {
                            component2.labelPopIncreaseTime.text = townLocation.GetPopIncreaseTime().ToString();
                        }
                    }
                    else
                    {
                        component2.goGuard.SetActive(value: false);
                        component2.goPopulation.SetActive(value: false);
                        component2.goProduction.SetActive(value: false);
                        component2.goSummoningCircle.SetActive(value: false);
                    }
                    List<EnchantmentInstance> list = new List<EnchantmentInstance>(townLocation.GetEnchantments());
                    list = list.FindAll((EnchantmentInstance o) => !o.buildingEnchantment);
                    component2.gridEnchantments.UpdateGrid(list);
                    return;
                }
                MarkerNode component3 = displayObject.GetComponent<MarkerNode>();
                if (component3 != null)
                {
                    PlayerWizard wizard2 = GameManager.GetWizard(location.melding?.meldOwner ?? 0);
                    if (wizard2 != null)
                    {
                        component3.nodeOwnerColour.color = WizardColors.GetColor(wizard2);
                    }
                    else
                    {
                        component3.nodeOwnerColour.color = new Color32(132, 121, 104, byte.MaxValue);
                    }
                    List<EnchantmentInstance> items = new List<EnchantmentInstance>(location.GetEnchantments());
                    component3.gridEnchantments.UpdateGrid(items);
                }
            }
            else if (owner is BattleUnit)
            {
                BattleUnit battleUnit = owner as BattleUnit;
                MarkerCombatUnit component4 = displayObject.GetComponent<MarkerCombatUnit>();
                component4.sliderHp.value = battleUnit.GetTotalHpPercent();
                component4.sliderHp.gameObject.SetActive(battleUnit.GetTotalHpPercent() != 1f);
                bool active4 = false;
                bool active5 = false;
                bool flag = false;
                Battle battle = Battle.GetBattle();
                if (battle != null)
                {
                    active4 = !battle.landBattle && battleUnit.GetAttFinal(TAG.CAN_WALK) > 0 && battleUnit.GetAttFinal(TAG.CAN_SWIM) < 0;
                    active5 = battle.landBattle && battleUnit.GetAttFinal(TAG.CAN_WALK) < 0 && battleUnit.GetAttFinal(TAG.CAN_SWIM) > 0;
                }
                flag = battleUnit.GetAttFinal(TAG.CAN_FLY) > 0;
                component4.goWaterOnly.SetActive(active5);
                component4.goGroundOnly.SetActive(active4);
                component4.goFlying.SetActive(flag);
                component4.experience.Set(battleUnit);
                List<EnchantmentInstance> enchantmentsWithRemotes = battleUnit.GetEnchantmentManager().GetEnchantmentsWithRemotes(visibleOnly: true);
                component4.gridEnchantments.UpdateGrid(enchantmentsWithRemotes);
                if (battleUnit.ownerID == PlayerWizard.HumanID() && battleUnit.IsInvisibleUnit())
                {
                    component4.goUnitInvisible.SetActive(!battleUnit.currentlyVisible);
                    component4.goUnitVisible.SetActive(battleUnit.currentlyVisible);
                }
                else
                {
                    component4.goUnitInvisible.SetActive(value: false);
                    component4.goUnitVisible.SetActive(value: false);
                }
                if (battleUnit.Mp == FInt.ZERO)
                {
                    component4.canvasMarker.alpha = 0.75f;
                }
                else
                {
                    component4.canvasMarker.alpha = 1f;
                }
                component4.goSpellcaster.SetActive(BattleHUD.Get()?.activeCaster == battleUnit);
            }
        }

        public void UpdateMarkerColors(object owner)
        {
            if (owner != null && this.markers.ContainsKey(owner))
            {
                if (owner is BattleUnit)
                {
                    BattleUnit obj = owner as BattleUnit;
                    GameObject displayObject = this.markers[owner].displayObject;
                    PlayerWizard.Color color = this.GetColor(obj.GetWizardOwner()?.ID ?? 0);
                    MarkerCombatUnit component = displayObject.GetComponent<MarkerCombatUnit>();
                    component.goBrown.SetActive(color == PlayerWizard.Color.None);
                    component.goBlue.SetActive(color == PlayerWizard.Color.Blue);
                    component.goGreen.SetActive(color == PlayerWizard.Color.Green);
                    component.goPurple.SetActive(color == PlayerWizard.Color.Purple);
                    component.goRed.SetActive(color == PlayerWizard.Color.Red);
                    component.goYellow.SetActive(color == PlayerWizard.Color.Yellow);
                }
                else if (owner is global::MOM.Group)
                {
                    global::MOM.Group group = owner as global::MOM.Group;
                    GameObject displayObject2 = this.markers[owner].displayObject;
                    int ownerID = group.GetOwnerID();
                    PlayerWizard.Color color2 = this.GetColor(ownerID);
                    MarkerArmy component2 = displayObject2.GetComponent<MarkerArmy>();
                    component2.goBrown.SetActive(color2 == PlayerWizard.Color.None);
                    component2.goBlue.SetActive(color2 == PlayerWizard.Color.Blue);
                    component2.goGreen.SetActive(color2 == PlayerWizard.Color.Green);
                    component2.goPurple.SetActive(color2 == PlayerWizard.Color.Purple);
                    component2.goRed.SetActive(color2 == PlayerWizard.Color.Red);
                    component2.goYellow.SetActive(color2 == PlayerWizard.Color.Yellow);
                }
            }
        }

        public void UpdateSpelcasterIcon(object owner, bool active)
        {
            if (owner != null && owner is BattleUnit && this.markers.ContainsKey(owner))
            {
                this.markers[owner].displayObject.GetComponent<MarkerCombatUnit>().goSpellcaster.SetActive(active);
            }
        }

        public void UpdateSpelcasterIcon(List<BattleUnit> owners, bool active)
        {
            foreach (BattleUnit owner in owners)
            {
                if (owner != null && this.markers.ContainsKey(owner))
                {
                    this.markers[owner].displayObject.GetComponent<MarkerCombatUnit>().goSpellcaster.SetActive(active);
                }
            }
        }

        private GameObject GetUnusedMarker(MarkerType t)
        {
            if (!this.markerPool.ContainsKey(t))
            {
                this.markerPool[t] = new Stack<GameObject>();
            }
            if (this.markerPool[t].Count > 0)
            {
                GameObject obj = this.markerPool[t].Pop();
                obj.SetActive(value: true);
                return obj;
            }
            return GameObjectUtils.Instantiate(UIReferences.GetMarkerGOSource(t), this.markerLayer.transform);
        }
    }
}
