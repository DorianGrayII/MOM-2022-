using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class CraftingQueue
    {
        [ProtoMember(1)]
        public List<CraftingItem> craftingItems = new List<CraftingItem>();

        [ProtoMember(2)]
        public Reference<TownLocation> owner;

        [ProtoMember(3)]
        public int tempProgress;

        [ProtoMember(4)]
        public bool repeatUnit;

        public CraftingQueue()
        {
        }

        public CraftingQueue(TownLocation tl)
        {
            this.owner = tl;
            this.SanitizeQueue();
            this.MarkerUpdate();
        }

        public void RemoveItem(CraftingItem item)
        {
            int num = this.craftingItems.IndexOf(item);
            if (item != null && num > -1)
            {
                if (num == 0)
                {
                    this.repeatUnit = false;
                }
                this.craftingItems.Remove(item);
                if (item.progress > 0)
                {
                    this.tempProgress = item.progress;
                }
            }
            this.SanitizeQueue();
            this.TryToTransferTempProgress();
            this.MarkerUpdate();
        }

        public void MoveRight(CraftingItem item)
        {
            if (item == null || !this.craftingItems.Contains(item))
            {
                return;
            }
            int num = this.craftingItems.IndexOf(item);
            if (this.CanMoveItem(left: false, num, item))
            {
                if (num < this.craftingItems.Count - 1)
                {
                    if (num == 0)
                    {
                        this.repeatUnit = false;
                    }
                    if (item.progress > 0)
                    {
                        this.craftingItems[num + 1].progress = item.progress;
                        item.progress = 0;
                    }
                    this.craftingItems.RemoveAt(num);
                    this.craftingItems.Insert(num + 1, item);
                }
                this.SanitizeQueue();
                this.MarkerUpdate();
            }
            else
            {
                PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_CANNOT_MOVE_QUEUE_ITEM", "UI_OK");
            }
        }

        public bool CanMoveRight(CraftingItem item)
        {
            if (item == null || !this.craftingItems.Contains(item))
            {
                return false;
            }
            return this.craftingItems.IndexOf(item) < this.craftingItems.Count - 1;
        }

        public void MoveLeft(CraftingItem item)
        {
            if (item == null || !this.craftingItems.Contains(item))
            {
                return;
            }
            int num = this.craftingItems.IndexOf(item);
            if (this.CanMoveItem(left: true, num, item))
            {
                if (num > 0)
                {
                    if (num == 1)
                    {
                        this.repeatUnit = false;
                    }
                    if (this.craftingItems[num - 1].progress > 0)
                    {
                        item.progress = this.craftingItems[num - 1].progress;
                        this.craftingItems[num - 1].progress = 0;
                    }
                    this.craftingItems.RemoveAt(num);
                    this.craftingItems.Insert(num - 1, item);
                }
                this.SanitizeQueue();
                this.MarkerUpdate();
            }
            else
            {
                PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_CANNOT_MOVE_QUEUE_ITEM", "UI_OK");
            }
        }

        private bool CanMoveItem(bool left, int movingItemIndex, CraftingItem movingItem)
        {
            if (this.craftingItems != null && this.craftingItems.Count > 0)
            {
                if (left)
                {
                    global::DBDef.Unit unit = movingItem.craftedUnit?.Get();
                    if (unit != null && unit.requiredBuildings != null)
                    {
                        Building[] requiredBuildings = unit.requiredBuildings;
                        foreach (Building v4 in requiredBuildings)
                        {
                            if (!this.owner.Get().HaveBuilding(v4))
                            {
                                int num = this.craftingItems.FindIndex((CraftingItem k) => k.craftedBuilding?.Get() == v4);
                                if (num >= movingItemIndex - 1 || num <= -1)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        Building building = movingItem.craftedBuilding?.Get();
                        if (building != null && building.parentBuildingRequired != null)
                        {
                            Building[] requiredBuildings = building.parentBuildingRequired;
                            foreach (Building v3 in requiredBuildings)
                            {
                                if (!this.owner.Get().HaveBuilding(v3))
                                {
                                    int num2 = this.craftingItems.FindIndex((CraftingItem k) => k.craftedBuilding?.Get() == v3);
                                    if (num2 >= movingItemIndex - 1 || num2 <= -1)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = movingItemIndex + 1; j < this.craftingItems.Count; j++)
                    {
                        CraftingItem craftingItem = this.craftingItems[j];
                        global::DBDef.Unit unit2 = craftingItem.craftedUnit?.Get();
                        Building[] requiredBuildings;
                        if (unit2 != null && unit2.requiredBuildings != null)
                        {
                            requiredBuildings = unit2.requiredBuildings;
                            foreach (Building v2 in requiredBuildings)
                            {
                                if (this.owner.Get().HaveBuilding(v2))
                                {
                                    continue;
                                }
                                int num3 = this.craftingItems.FindIndex((CraftingItem k) => k.craftedBuilding?.Get() == v2);
                                if (num3 == movingItemIndex)
                                {
                                    if (movingItemIndex + 1 >= j)
                                    {
                                        return false;
                                    }
                                }
                                else if (num3 >= j || num3 <= -1)
                                {
                                    return false;
                                }
                            }
                            continue;
                        }
                        Building building2 = craftingItem.craftedBuilding?.Get();
                        if (building2 == null || building2.parentBuildingRequired == null)
                        {
                            continue;
                        }
                        requiredBuildings = building2.parentBuildingRequired;
                        foreach (Building v in requiredBuildings)
                        {
                            if (this.owner.Get().HaveBuilding(v))
                            {
                                continue;
                            }
                            int num4 = this.craftingItems.FindIndex((CraftingItem k) => k.craftedBuilding?.Get() == v);
                            if (num4 == movingItemIndex)
                            {
                                if (movingItemIndex + 1 >= j)
                                {
                                    return false;
                                }
                            }
                            else if (num4 >= j || num4 <= -1)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool CanMoveLeft(CraftingItem item)
        {
            if (item == null || !this.craftingItems.Contains(item))
            {
                return false;
            }
            return this.craftingItems.IndexOf(item) > 0;
        }

        public void ReplaceFirstIndexItem(Building building)
        {
            CraftingItem first = this.GetFirst();
            this.craftingItems.Remove(first);
            if (first.progress > 0)
            {
                this.tempProgress = first.progress;
            }
            CraftingItem item = new CraftingItem(building, this.owner.Get().buildingDiscount);
            this.craftingItems.Insert(0, item);
            this.repeatUnit = false;
            this.TryToTransferTempProgress();
            this.MarkerUpdate();
        }

        public void AddItem(Building building)
        {
            CraftingItem craftingItem = this.craftingItems.Find((CraftingItem o) => o.requirementValue < 1);
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
            }
            CraftingItem item = new CraftingItem(building, this.owner.Get().buildingDiscount);
            this.craftingItems.Add(item);
            this.TryToTransferTempProgress();
            this.MarkerUpdate();
        }

        public void InsertFirstItem(Building building)
        {
            CraftingItem craftingItem = this.craftingItems.Find((CraftingItem o) => o.requirementValue < 1);
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
            }
            CraftingItem item = new CraftingItem(building, this.owner.Get().buildingDiscount);
            this.craftingItems.Insert(0, item);
            this.repeatUnit = false;
            if (this.craftingItems.Count > 5)
            {
                this.craftingItems.RemoveRange(5, this.craftingItems.Count - 5);
            }
            this.MarkerUpdate();
        }

        public void AddItem(global::DBDef.Unit unit)
        {
            CraftingItem craftingItem = this.craftingItems.Find((CraftingItem o) => o.requirementValue < 1);
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
            }
            CraftingItem item = new CraftingItem(unit, this.owner.Get().GetUnitDiscount(unit));
            this.craftingItems.Add(item);
            if (this.owner.Get().GetOwnerID() == PlayerWizard.HumanID())
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.owner);
            }
            this.TryToTransferTempProgress();
            this.MarkerUpdate();
        }

        public void ReplaceFirstIndexItem(global::DBDef.Unit unit)
        {
            CraftingItem first = this.GetFirst();
            this.craftingItems.Remove(first);
            if (first.progress > 0)
            {
                this.tempProgress = first.progress;
            }
            CraftingItem item = new CraftingItem(unit, this.owner.Get().GetUnitDiscount(unit));
            this.craftingItems.Insert(0, item);
            this.repeatUnit = false;
            this.TryToTransferTempProgress();
            this.MarkerUpdate();
        }

        public void InsertFirstItem(global::DBDef.Unit unit, int Xtimes)
        {
            CraftingItem craftingItem = this.craftingItems.Find((CraftingItem o) => o.requirementValue < 1);
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
            }
            for (int i = 0; i < Xtimes; i++)
            {
                CraftingItem item = new CraftingItem(unit, this.owner.Get().GetUnitDiscount(unit));
                this.craftingItems.Insert(0, item);
            }
            this.repeatUnit = false;
            if (this.craftingItems.Count > 5)
            {
                this.craftingItems.RemoveRange(5, this.craftingItems.Count - 5);
            }
            this.MarkerUpdate();
        }

        public bool ReturnWorkToMilitaryUnit()
        {
            CraftingItem craftingItem = null;
            foreach (CraftingItem craftingItem2 in this.craftingItems)
            {
                if (!(craftingItem2.craftedUnit == null) && craftingItem2.craftedUnit != GameplayHelper.GetTownProducedSettler(this.owner.Get().GetSourceTown()) && craftingItem2.craftedUnit != GameplayHelper.GetTownProducedEngineer(this.owner.Get().GetSourceTown()))
                {
                    craftingItem = craftingItem2;
                    break;
                }
            }
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
                this.craftingItems.Insert(0, craftingItem);
                this.repeatUnit = false;
                return true;
            }
            return false;
        }

        public bool ReturnWorkToSettlerUnit()
        {
            CraftingItem craftingItem = null;
            foreach (CraftingItem craftingItem2 in this.craftingItems)
            {
                if (craftingItem2.craftedUnit == GameplayHelper.GetTownProducedSettler(this.owner.Get().GetSourceTown()))
                {
                    craftingItem = craftingItem2;
                    break;
                }
            }
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
                this.craftingItems.Insert(0, craftingItem);
                this.repeatUnit = false;
                return true;
            }
            return false;
        }

        public bool ReturnWorkToTransportUnit()
        {
            Tag t = (Tag)TAG.TRANSPORTER;
            CraftingItem craftingItem = null;
            foreach (CraftingItem craftingItem2 in this.craftingItems)
            {
                if (craftingItem2.craftedUnit != null && Array.Find(craftingItem2.craftedUnit.Get().tags, (CountedTag k) => k.tag == t) != null)
                {
                    craftingItem = craftingItem2;
                    break;
                }
            }
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
                this.craftingItems.Insert(0, craftingItem);
                return true;
            }
            return false;
        }

        public bool ReturnWorkToEngineerUnit()
        {
            CraftingItem craftingItem = null;
            foreach (CraftingItem craftingItem2 in this.craftingItems)
            {
                if (craftingItem2.craftedUnit == GameplayHelper.GetTownProducedEngineer(this.owner.Get().GetSourceTown()))
                {
                    craftingItem = craftingItem2;
                    break;
                }
            }
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
                this.craftingItems.Insert(0, craftingItem);
                return true;
            }
            return false;
        }

        public bool ReturnWorkToMilitaryBuilding()
        {
            CraftingItem craftingItem = null;
            foreach (CraftingItem craftingItem2 in this.craftingItems)
            {
                if (!(craftingItem2.craftedBuilding == null) && GameplayHelper.Get().GetMilitaryBuildings(this.owner.Get().GetSourceTown()).Contains(craftingItem2.craftedBuilding))
                {
                    craftingItem = craftingItem2;
                    break;
                }
            }
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
                this.craftingItems.Insert(0, craftingItem);
                return true;
            }
            return false;
        }

        public bool ReturnWorkToEconomyBuilding()
        {
            CraftingItem craftingItem = null;
            foreach (CraftingItem craftingItem2 in this.craftingItems)
            {
                if (!(craftingItem2.craftedBuilding == null) && GameplayHelper.Get().GetEconomyBuildings(this.owner.Get().GetSourceTown()).Contains(craftingItem2.craftedBuilding))
                {
                    craftingItem = craftingItem2;
                    break;
                }
            }
            if (craftingItem != null)
            {
                this.craftingItems.Remove(craftingItem);
                this.craftingItems.Insert(0, craftingItem);
                return true;
            }
            return false;
        }

        public bool HaveQueueSpace()
        {
            return this.craftingItems.Count < 5;
        }

        public CraftingItem GetFirst()
        {
            if (this.craftingItems != null && this.craftingItems.Count > 0)
            {
                return this.craftingItems[0];
            }
            return null;
        }

        public void AdvanceQueue()
        {
            if (this.craftingItems == null)
            {
                Debug.LogError("craftingItems == null!");
            }
            else
            {
                if (this.owner.Get().GetOwnerID() < 1)
                {
                    return;
                }
                int num = this.owner.Get().CalculateProductionIncome();
                if (this.owner.Get().GetOwnerID() > 1)
                {
                    int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
                    num += settingAsInt - 1;
                    num = (int)((float)num * (1f + (float)(settingAsInt - 1) * 0.1f));
                }
                if (this.craftingItems.Count != 0)
                {
                    this.craftingItems[0].GetDI().GetLocalizedName();
                }
                if (this.craftingItems.Count != 0)
                {
                    _ = this.craftingItems[0].requirementValue;
                }
                if (this.craftingItems.Count != 0)
                {
                    this.craftingItems[0].Progress();
                }
                while (this.craftingItems.Count > 0 && (num > 0 || this.craftingItems[0].progress == this.craftingItems[0].requirementValue))
                {
                    CraftingItem craftingItem = this.craftingItems[0];
                    int requirementValue = craftingItem.requirementValue;
                    int progress = craftingItem.progress;
                    int num2 = Mathf.Max(0, requirementValue - progress);
                    if (num2 > num)
                    {
                        craftingItem.progress += num;
                        break;
                    }
                    num -= num2;
                    craftingItem.progress = requirementValue;
                    if (craftingItem.craftedUnit != null)
                    {
                        Unit unit = Unit.CreateFrom((global::DBDef.Unit)craftingItem.craftedUnit);
                        PlayerWizard wizardOwner = this.owner.Get().GetWizardOwner();
                        if (this.owner != null && this.owner.Get() != null && craftingItem.craftedUnit.Get().populationCost > 0)
                        {
                            TownLocation townLocation = this.owner.Get();
                            this.owner.Get().Population -= craftingItem.craftedUnit.Get().populationCost;
                            if (townLocation.Population > 0 && townLocation.Population - craftingItem.craftedUnit.Get().populationCost < 1000)
                            {
                                townLocation.craftingQueue.CleanUnitFromQueue(unit);
                            }
                        }
                        int value = 0;
                        this.owner.Get().ProcessIntigerScripts(EEnchantmentType.NewUnitXpModifier, ref value);
                        unit.xp = value;
                        this.owner.Get().TriggerScripts(EEnchantmentType.NewUnitModifier, unit);
                        if (unit.GetAttributes().GetFinal(TAG.CAN_WALK) > 0 || unit.GetAttributes().GetFinal(TAG.CAN_FLY) > 0 || this.owner.Get().GetLocalGroup().transporter != null)
                        {
                            this.owner.Get().GetLocalGroup().AddUnit(unit);
                        }
                        else if (unit.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0)
                        {
                            Vector3i position = this.owner.Get().GetPosition();
                            global::WorldCode.Plane plane = this.owner.Get().GetPlane();
                            plane.GetIslands();
                            bool flag = false;
                            for (int i = 1; i < 4; i++)
                            {
                                ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(i, i);
                                List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(plane);
                                Vector3i target = Vector3i.invalid;
                                bool flag2 = false;
                                foreach (Vector3i item in rangeSimple)
                                {
                                    Vector3i vector3i = position + item;
                                    Hex hexAt = plane.GetHexAt(vector3i);
                                    if (hexAt == null || hexAt.IsLand())
                                    {
                                        continue;
                                    }
                                    if (target != Vector3i.invalid)
                                    {
                                        if (plane.waterBodies.ContainsKey(target) && plane.waterBodies.ContainsKey(hexAt.Position) && (plane.waterBodies[target] < plane.waterBodies[hexAt.Position] || (flag2 && plane.waterBodies[target] == plane.waterBodies[hexAt.Position])))
                                        {
                                            Group group = groupsOfPlane.Find((Group o) => o.GetPosition() == target);
                                            flag2 = ((group != null && group.GetOwnerID() != this.owner.Get().GetOwnerID()) ? true : false);
                                            target = hexAt.Position;
                                        }
                                    }
                                    else
                                    {
                                        target = vector3i;
                                        Group group2 = groupsOfPlane.Find((Group o) => o.GetPosition() == target);
                                        flag2 = ((group2 != null && group2.GetOwnerID() != this.owner.Get().GetOwnerID()) ? true : false);
                                    }
                                }
                                if (target != Vector3i.invalid)
                                {
                                    Group group3 = groupsOfPlane.Find((Group o) => o.Position == target);
                                    if (group3 == null)
                                    {
                                        Group group4 = new Group(plane, this.owner.Get().GetOwnerID());
                                        group4.Position = target;
                                        group4.AddUnit(unit);
                                        group4.GetMapFormation();
                                        flag = true;
                                        break;
                                    }
                                    if (group3.GetOwnerID() == this.owner.Get().GetOwnerID() && group3.GetUnits().Count < 9)
                                    {
                                        group3.AddUnit(unit);
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag)
                            {
                                unit.Destroy();
                            }
                        }
                        else
                        {
                            this.owner.Get().GetLocalGroup().AddUnit(unit);
                        }
                        unit?.GetAttributes().SetDirty();
                        if (wizardOwner.IsHuman)
                        {
                            SummaryInfo s = new SummaryInfo
                            {
                                summaryType = SummaryInfo.SummaryType.eBuildingProgress,
                                location = (TownLocation)this.owner,
                                name = ((TownLocation)this.owner).GetName(),
                                graphic = unit.GetDescriptionInfo().graphic,
                                unit = unit
                            };
                            wizardOwner.AddNotification(s);
                        }
                    }
                    else if (craftingItem.craftedBuilding != null)
                    {
                        DBReference<Building> craftedBuilding = craftingItem.craftedBuilding;
                        if (requirementValue <= 0)
                        {
                            break;
                        }
                        this.owner.Get().AddBuilding(craftingItem.craftedBuilding);
                        PlayerWizard wizardOwner2 = this.owner.Get().GetWizardOwner();
                        if (wizardOwner2.IsHuman)
                        {
                            SummaryInfo s2 = new SummaryInfo
                            {
                                summaryType = SummaryInfo.SummaryType.eBuildingProgress,
                                location = (TownLocation)this.owner,
                                name = ((TownLocation)this.owner).GetName(),
                                graphic = craftingItem.craftedBuilding.Get().GetDescriptionInfo().graphic,
                                building = craftedBuilding
                            };
                            wizardOwner2.AddNotification(s2);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Unknown craft, neither unit, nor building.");
                    }
                    if (!this.repeatUnit)
                    {
                        this.craftingItems.RemoveAt(0);
                    }
                    else
                    {
                        craftingItem.progress = 0;
                    }
                    this.SanitizeQueue();
                }
                this.MarkerUpdate();
            }
        }

        public void SanitizeQueue()
        {
            bool flag = false;
            if (this.craftingItems == null)
            {
                this.craftingItems = new List<CraftingItem>();
            }
            for (int i = 0; i < this.craftingItems.Count; i++)
            {
                CraftingItem craftingItem = this.craftingItems[i];
                global::DBDef.Unit unit = craftingItem.craftedUnit?.Get();
                Building[] requiredBuildings;
                if (unit != null && unit.requiredBuildings != null)
                {
                    requiredBuildings = unit.requiredBuildings;
                    foreach (Building v2 in requiredBuildings)
                    {
                        if (!this.owner.Get().HaveBuilding(v2))
                        {
                            int num = this.craftingItems.FindIndex((CraftingItem k) => k.craftedBuilding?.Get() == v2);
                            if (num >= i || num <= -1)
                            {
                                flag = true;
                                this.craftingItems.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                    continue;
                }
                Building building = craftingItem.craftedBuilding?.Get();
                if (building == null)
                {
                    continue;
                }
                if (this.owner.Get().HaveBuilding(building))
                {
                    flag = true;
                    this.craftingItems.RemoveAt(i);
                    i--;
                    break;
                }
                if (building.parentBuildingRequired == null)
                {
                    continue;
                }
                requiredBuildings = building.parentBuildingRequired;
                foreach (Building v in requiredBuildings)
                {
                    if (!this.owner.Get().HaveBuilding(v))
                    {
                        int num2 = this.craftingItems.FindIndex((CraftingItem k) => k.craftedBuilding?.Get() == v);
                        if (num2 >= i || num2 <= -1)
                        {
                            flag = true;
                            this.craftingItems.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }
            if (flag)
            {
                PlayerWizard wizardOwner = this.owner.Get().GetWizardOwner();
                if (wizardOwner != null && wizardOwner.IsHuman)
                {
                    PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_REQUIREMENTS_NOT_MET", "UI_OK");
                }
            }
            if (this.craftingItems.Count < 1)
            {
                if (this.owner.Get().GetPopUnits() >= this.owner.Get().MaxPopulation())
                {
                    this.craftingItems.Add(new CraftingItem((Building)BUILDING.TRADE_GOODS));
                }
                else
                {
                    this.craftingItems.Add(new CraftingItem((Building)BUILDING.HOUSING));
                }
                PlayerWizard wizardOwner2 = this.owner.Get().GetWizardOwner();
                if (wizardOwner2 != null && wizardOwner2.IsHuman && !this.owner.Get().autoManaged && !this.owner.Get().IsAnOutpost())
                {
                    SummaryInfo s = new SummaryInfo
                    {
                        summaryType = SummaryInfo.SummaryType.eConstructionQueueComplete,
                        location = (TownLocation)this.owner,
                        name = ((TownLocation)this.owner).GetName()
                    };
                    wizardOwner2.AddNotification(s);
                }
            }
            if (this.craftingItems.Count > 5)
            {
                this.craftingItems = this.craftingItems.GetRange(0, 5);
            }
            if (this.owner.Get().autoManaged || (this.GetFirst() != null && this.GetFirst().craftedUnit == null))
            {
                this.repeatUnit = false;
            }
        }

        public void RecaclulateUnitsCostInQueue()
        {
            if (this.craftingItems.Count <= 0)
            {
                return;
            }
            foreach (CraftingItem craftingItem in this.craftingItems)
            {
                if (craftingItem.craftedUnit != null)
                {
                    FInt unitDiscount = this.owner.Get().GetUnitDiscount(craftingItem.craftedUnit);
                    global::DBDef.Unit unit = craftingItem.craftedUnit.Get();
                    craftingItem.requirementValue = (unit.constructionCost * (FInt.ONE - unitDiscount)).ToInt();
                }
            }
        }

        public void RecaclulateItemCostInQueue()
        {
            if (this.craftingItems.Count <= 0)
            {
                return;
            }
            foreach (CraftingItem craftingItem in this.craftingItems)
            {
                if (craftingItem.craftedUnit != null)
                {
                    FInt unitDiscount = this.owner.Get().GetUnitDiscount(craftingItem.craftedUnit);
                    global::DBDef.Unit unit = craftingItem.craftedUnit.Get();
                    craftingItem.requirementValue = (unit.constructionCost * (FInt.ONE - unitDiscount)).ToInt();
                }
                if (craftingItem.craftedBuilding != null)
                {
                    FInt buildingDiscount = this.owner.Get().buildingDiscount;
                    Building building = craftingItem.craftedBuilding.Get();
                    craftingItem.requirementValue = (building.buildCost * (FInt.ONE - buildingDiscount)).ToInt();
                }
            }
        }

        private void MarkerUpdate()
        {
            if (this.owner.Get().GetOwnerID() == PlayerWizard.HumanID())
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this.owner);
            }
        }

        public bool SanitizeRequirementsAfterRemovalOfBuilding(Building b)
        {
            bool result = false;
            for (int i = 0; i < this.craftingItems.Count; i++)
            {
                CraftingItem craftingItem = this.craftingItems[i];
                Building[] array = null;
                if (craftingItem.craftedBuilding != null)
                {
                    array = craftingItem.craftedBuilding.Get().parentBuildingRequired;
                }
                if (craftingItem.craftedUnit != null)
                {
                    array = craftingItem.craftedUnit.Get().requiredBuildings;
                }
                if (array == null)
                {
                    continue;
                }
                Building[] array2 = array;
                foreach (Building building in array2)
                {
                    if (b == building)
                    {
                        this.craftingItems.RemoveAt(i);
                        if (i == 0)
                        {
                            this.repeatUnit = false;
                        }
                        i--;
                        result = true;
                        break;
                    }
                }
            }
            this.SanitizeQueue();
            this.MarkerUpdate();
            return result;
        }

        public void CleanUnitFromQueue(Unit unit)
        {
            for (int num = this.craftingItems.Count - 1; num >= 0; num--)
            {
                if (this.craftingItems[num].craftedUnit != null && this.craftingItems[num].craftedUnit.Get() == unit.dbSource)
                {
                    this.RemoveItem(this.craftingItems[num]);
                }
            }
            this.SanitizeQueue();
            this.MarkerUpdate();
        }

        public void CleanBuildingFromQueue(Building building)
        {
            for (int num = this.craftingItems.Count - 1; num >= 0; num--)
            {
                if (this.craftingItems[num].craftedBuilding != null && this.craftingItems[num].craftedBuilding.Get() == building)
                {
                    this.RemoveItem(this.craftingItems[num]);
                }
            }
        }

        public void ClearTempProgres()
        {
            this.tempProgress = 0;
        }

        public void TryToTransferTempProgress()
        {
            if (this.tempProgress > 0 && this.craftingItems[0].requirementValue > 0)
            {
                this.craftingItems[0].progress = this.tempProgress;
                this.ClearTempProgres();
            }
        }

        public void ResetCraftingQueue()
        {
            this.craftingItems = new List<CraftingItem>();
            this.repeatUnit = false;
            this.ClearTempProgres();
            this.SanitizeQueue();
            this.MarkerUpdate();
        }
    }
}
