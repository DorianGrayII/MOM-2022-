// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.TownLocation
using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class TownLocation : global::MOM.Location
{
    [ProtoMember(1)]
    public DBReference<Race> race;

    [ProtoMember(2)]
    private int population;

    [ProtoIgnore]
    private int _farmerStorage;

    [ProtoMember(4)]
    public int isNatureProtected;

    [ProtoMember(5)]
    public int isSorceryProtected;

    [ProtoMember(6)]
    public int isChaosProtected;

    [ProtoMember(7)]
    public int isLifeProtected;

    [ProtoMember(8)]
    public int isDeathProtected;

    [ProtoMember(9)]
    public FInt foodInArea;

    [ProtoMember(11)]
    public CraftingQueue craftingQueue;

    [ProtoMember(12)]
    public List<DBReference<Building>> buildings = new List<DBReference<Building>>();

    [ProtoMember(13)]
    private int townRange;

    [ProtoMember(14)]
    public bool seaside;

    [ProtoMember(15)]
    public FInt taxMultiplier;

    [ProtoMember(16)]
    public int turnsUndefended;

    [ProtoMember(17)]
    public FInt productionInArea;

    [ProtoMember(18)]
    public bool autoManaged;

    [ProtoMember(19)]
    public FInt unitDiscount;

    [ProtoMember(20)]
    public FInt buildingDiscount;

    [ProtoMember(21)]
    public int conqueredTurn;

    [ProtoIgnore]
    public AINeutralTown aiForNeutralTown;

    [ProtoIgnore]
    public List<Resource> resources;

    [ProtoIgnore]
    public List<Resource> potencialResources;

    [ProtoIgnore]
    public List<Multitype<Resource, bool>> resourceAndState;

    [ProtoIgnore]
    public int preBattlePopulation;

    [ProtoIgnore]
    public int preBattleValue;

    [ProtoIgnore]
    private int waterHexInClosesetVicinity = -1;

    [ProtoIgnore]
    public List<global::MOM.Location> townLocations;

    [ProtoIgnore]
    private float unrest;

    [ProtoIgnore]
    public List<Building> tempDestroyedBuilding;

    [ProtoIgnore]
    public int roadBonus;

    [ProtoIgnore]
    private int rebelModifier;

    public static TownLocation lastBattle;

    public const int OUTPOST_POPULATION = 1000;

    [ProtoMember(3)]
    public int farmers
    {
        get
        {
            return this._farmerStorage;
        }
        set
        {
            this._farmerStorage = value;
            if (value < 0)
            {
                Debug.LogError("Negative value set");
            }
        }
    }

    public int Population
    {
        get
        {
            return this.population;
        }
        set
        {
            int num = this.population;
            if (this.population >= 1000 && value < 1000)
            {
                this.population = 1000;
            }
            else
            {
                this.population = value;
            }
            if (this.population < 0)
            {
                Debug.LogError("Population changed into negative Old: " + num + ", New: " + value + " Turn: " + TurnManager.GetTurnNumber());
            }
            if (num != 0 && num < 1000 && this.population >= 1000 && base.owner == PlayerWizard.HumanID())
            {
                PlayerWizard humanWizard = GameManager.GetHumanWizard();
                SummaryInfo s = new SummaryInfo
                {
                    summaryType = SummaryInfo.SummaryType.eOutpostToTown,
                    location = this,
                    name = this.GetName()
                };
                humanWizard.AddNotification(s);
            }
            if (base.ID <= 0 || this.population / 1000 == num / 1000)
            {
                return;
            }
            int val = this.MinFarmers();
            int popUnits = this.GetPopUnits();
            int rebels = this.GetRebels();
            int val2 = popUnits - rebels;
            PlayerWizard wizardOwner = base.GetWizardOwner();
            if (wizardOwner != null)
            {
                if (wizardOwner.CalculateFoodIncome(includeUpkeep: true, null, reportStaration: false) <= 0 || this.CalculateFoodIncome() < popUnits)
                {
                    this.farmers = Math.Min(this.farmers + 1, val2);
                    val = Math.Min(val, val2);
                    this.farmers = Mathf.Max(this.farmers, val);
                }
            }
            else
            {
                val = Math.Min(val, val2);
                this.farmers = val;
            }
        }
    }

    public float OutpostProgression()
    {
        return Mathf.Clamp01((float)this.Population / 1000f);
    }

    [ProtoAfterDeserialization]
    public void AfterDeserialization()
    {
        if (this.buildings == null)
        {
            this.buildings = new List<DBReference<Building>>();
        }
        if (this.craftingQueue != null)
        {
            this.craftingQueue.owner = this;
        }
        this.SetUnrestDirty();
    }

    public int RazeGold()
    {
        int num = 0;
        if (this.buildings != null)
        {
            foreach (DBReference<Building> building in this.buildings)
            {
                num += building.Get().buildCost;
            }
        }
        float num2 = num / 5;
        return 10 + (int)num2;
    }

    public override void SetOwnerID(int id, int attackerID = -1, bool collateralDamage = false)
    {
        if (id != base.owner && base.owner == PlayerWizard.HumanID())
        {
            PlayerWizard wizardOwner = base.GetWizardOwner();
            SummaryInfo s = new SummaryInfo
            {
                summaryType = SummaryInfo.SummaryType.eTownLost,
                location = this,
                name = this.GetName()
            };
            wizardOwner.AddNotification(s);
        }
        if (attackerID == PlayerWizard.HumanID() && id == 0)
        {
            AchievementManager.Progress(AchievementManager.Achievement.CleansingTheLand);
        }
        base.SetOwnerID(id, attackerID, collateralDamage);
        this.SetUnrestDirty();
        if (base.plane != null)
        {
            PlayerWizard wizardOwner2 = base.GetWizardOwner();
            if (wizardOwner2 == null)
            {
                base.plane.GetMarkers_().ChangeTownArea(this.GetSurroundingArea(2), Color.black, show: false);
            }
            else
            {
                base.plane.GetMarkers_().ChangeTownArea(this.GetSurroundingArea(2), wizardOwner2.GetColor());
            }
        }
        if (id > 0)
        {
            this.aiForNeutralTown = null;
            base.locationTactic = new AILocationTactic(this);
            int a = this.MinFarmers();
            int rebels = this.GetRebels();
            int popUnits = this.GetPopUnits();
            int b = Mathf.Max(0, popUnits - rebels);
            this.farmers = Mathf.Min(a, b);
        }
        else
        {
            this.aiForNeutralTown = new AINeutralTown(this);
            base.locationTactic = null;
        }
        if (collateralDamage)
        {
            if (this.IsAnOutpost())
            {
                this.population /= 2;
            }
            else
            {
                int num = Mathf.Min((this.population - 1000) / 4, 3000);
                this.population -= num;
            }
            if (this.buildings != null)
            {
                List<DBReference<Building>> list = this.buildings.FindAll((DBReference<Building> o) => o.Get().buildCost > 0);
                if (list.Count > 0)
                {
                    int index = global::UnityEngine.Random.Range(0, list.Count);
                    this.RemoveBuilding(list[index]);
                }
            }
        }
        this.conqueredTurn = TurnManager.GetTurnNumber();
    }

    public override int ConquerGold()
    {
        int num = 0;
        if (base.owner > 0)
        {
            PlayerWizard wizard = GameManager.GetWizard(base.owner);
            int num2 = 0;
            List<global::MOM.Location> locationsOfWizard = GameManager.GetLocationsOfWizard(base.owner);
            int num3 = Mathf.FloorToInt(((float)this.preBattlePopulation + Mathf.Epsilon) / 1000f);
            foreach (global::MOM.Location item in locationsOfWizard)
            {
                if (item is TownLocation townLocation)
                {
                    num2 = ((townLocation == this) ? (num2 + num3) : (num2 + townLocation.GetPopUnits()));
                }
            }
            if (num2 > 0)
            {
                num = ((!(wizard is PlayerWizardAI)) ? (num3 * wizard.money / num2) : Mathf.Min(num3 * 100, num3 * wizard.money / num2));
            }
        }
        else
        {
            MHRandom mHRandom = new MHRandom(TurnManager.GetTurnNumber() + base.Position.SqMagnitude());
            for (int num4 = this.GetPopUnits(); num4 > 0; num4--)
            {
                num += mHRandom.GetInt(1, 11);
            }
        }
        return num;
    }

    public override int LoseFame()
    {
        return (int)this.GetTownSize();
    }

    public int RazeFame()
    {
        return this.LoseFame();
    }

    public override int ConquerFame()
    {
        int num = 0;
        PlayerWizard wizard = GameManager.GetWizard(base.owner);
        if (wizard != null)
        {
            if (wizard.wizardTower?.Get() == this)
            {
                num += 5;
            }
            if (GameManager.GetLocationsOfWizard(base.owner).FindAll((global::MOM.Location o) => o is TownLocation).Count < 2)
            {
                num += 5;
            }
        }
        return num + Mathf.Max(0, (int)(-2 + this.GetTownSize()));
    }

    public void EnchantmentsChanged()
    {
        this.UpdateOwnerModels();
        this.SetUnrestDirty();
    }

    public void GroupChanged()
    {
        this.SetUnrestDirty();
        VerticalMarkerManager.Get().MarkDirtyInfoOnMarker(this);
    }

    public void Raze(int attackerID)
    {
        bool flag = base.owner == attackerID;
        if (attackerID > 0 && base.GetOwnerID() != attackerID)
        {
            int num = this.ConquerGold();
            PlayerWizard wizard = GameManager.GetWizard(base.GetOwnerID());
            if (wizard != null)
            {
                wizard.money -= num;
            }
            wizard = GameManager.GetWizard(attackerID);
            if (wizard.traitThePirat <= 0)
            {
                wizard.TakeFame(this.RazeFame());
            }
            if (!flag)
            {
                int num2 = this.RazeGold();
                if (wizard.traitThePirat > 0)
                {
                    wizard.AddFame(this.ConquerFame());
                    num2 *= wizard.traitThePirat;
                    if (wizard.GetID() == PlayerWizard.HumanID())
                    {
                        AdventureOutcomeDelta adventureOutcomeDelta = new AdventureOutcomeDelta();
                        adventureOutcomeDelta.Store();
                        ScriptLibrary.Call("GenerateRazeReward", this, wizard, base.preBattleAttackers);
                        List<AdventureOutcomeDelta.Outcome> outcomes = adventureOutcomeDelta.GetOutcomes();
                        UIManager.Get().PopupEvents(this, outcomes);
                    }
                    else
                    {
                        ScriptLibrary.Call("GenerateRazeReward", this, wizard, base.preBattleAttackers);
                    }
                }
                wizard.money += num2;
                wizard.money += num;
            }
        }
        if (flag)
        {
            global::MOM.Group group = base.GetLocalGroup();
            global::MOM.Group group2 = new global::MOM.Group(base.GetPlane(), attackerID);
            group2.Position = group.GetPosition();
            group?.TransferUnits(group2);
            if (group2.IsModelVisible())
            {
                group2.GetMapFormation();
            }
        }
        this.SetOwnerID(0, attackerID);
        Vector3i item = base.GetPosition();
        this.Destroy();
        if (!flag && base.preBattleAttackers != null)
        {
            base.preBattleAttackers.EnsureHasMP();
            base.preBattleAttackers.MoveViaPath(new List<Vector3i>
            {
                base.preBattleAttackers.GetPosition(),
                item
            }, mergeCollidedAlliedGroups: false);
        }
    }

    public override void Conquer(global::MOM.Group g)
    {
        this.craftingQueue.ResetCraftingQueue();
        base.Conquer(g);
        this.RemoveWizardTowerBonus();
    }

    public static TownLocation CreateLocation(Vector3i position, global::WorldCode.Plane p, Town source, int size, int owner)
    {
        int num = 0;
        num = ((size != 0) ? (size * 1000) : 300);
        p.isSettlerDataReady = false;
        TownLocation i = new TownLocation();
        i.plane = p;
        i.planeSerializableReference = p.planeSource;
        i.Position = position;
        i.source = source;
        i.owner = owner;
        i.race = source.race;
        i.Population = num;
        i.townRange = TownLocation.GetGeneralTownRange();
        i.taxMultiplier = source.taxMultiplier;
        i.SetUnrestDirty();
        i.UpdateModelName();
        TownName townName = DataBase.GetType<TownName>().Find((TownName o) => o.race == i.race.Get());
        List<global::MOM.Location> locations = GameManager.Get().registeredLocations;
        List<string> list = new List<string>(townName.names);
        list.RandomSort();
        string value = list.Find((string o) => locations.FindIndex((global::MOM.Location k) => k is TownLocation && (k as TownLocation).name == o) == -1);
        if (!string.IsNullOrEmpty(value))
        {
            i.name = value;
        }
        else
        {
            i.name = list[0];
        }
        p.ClearSearcherData();
        GameManager.Get().RegisterLocation(i);
        i.craftingQueue = new CraftingQueue(i);
        List<Enchantment> list2 = p.GetHexAt(position).CoastRiverBonusEnchantments();
        if (list2 != null)
        {
            foreach (Enchantment item in list2)
            {
                i.AddEnchantment(item, null);
            }
        }
        if (i.GetSourceTown().enchantmentData != null)
        {
            Enchantment[] enchantmentData = i.GetSourceTown().enchantmentData;
            foreach (Enchantment e in enchantmentData)
            {
                i.AddEnchantment(e, null);
            }
        }
        foreach (Vector3i item2 in HexNeighbors.GetRangeSimple(1))
        {
            Vector3i pos = p.area.KeepHorizontalInside(position + item2);
            Hex hexAt = p.GetHexAt(pos);
            if (hexAt != null && !hexAt.IsLand())
            {
                i.seaside = true;
                break;
            }
        }
        if (owner > 0)
        {
            i.farmers = i.MinFarmers();
            PlayerWizard wizard = GameManager.GetWizard(owner);
            wizard.AddTraitBaseEnchantmentsToNewBuildedTowns(i);
            wizard.TriggerScripts(EEnchantmentType.WizardOrGlobalToTownEnchantment, i);
        }
        GameManager.Get().TriggerScripts(EEnchantmentType.WizardOrGlobalToTownEnchantment, i);
        DataHeatMaps.MakeMapDirty(p, DataHeatMaps.HMType.SettlementValue);
        p.GetRoadManagers().SetRoadMode(position, p);
        if (EnchantmentRegister.GetByWizard(GameManager.GetHumanWizard()).Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.AWARENESS) != null && i.plane == World.GetActivePlane())
        {
            FOW.Get().UpdateFogForPlane(World.GetActivePlane());
        }
        return i;
    }

    public Tax GetTaxRank()
    {
        Tax tax = GameManager.GetWizard(base.GetOwnerID())?.GetTaxRank();
        if (tax == null)
        {
            tax = DataBase.Get<Tax>(TAX.NONE);
        }
        return tax;
    }

    public List<Vector3i> GetArea()
    {
        return HexNeighbors.GetRange(base.GetPosition(), 2);
    }

    public FInt GetFoodInArea()
    {
        if (this.foodInArea == FInt.ZERO)
        {
            this.foodInArea = new FInt(base.GetPlane().GetFoodInArea(base.GetPosition()));
        }
        return this.foodInArea;
    }

    public void ResetTownResources()
    {
        this.resources = null;
        this.foodInArea = FInt.ZERO;
        this.productionInArea = FInt.ZERO;
    }

    public int GetForestCountInArea()
    {
        int num = 0;
        global::WorldCode.Plane plane = base.GetPlane();
        foreach (Vector3i item in this.GetArea())
        {
            Hex hexAt = plane.GetHexAt(item);
            if (hexAt != null && hexAt.GetTerrain().terrainType == ETerrainType.Forest)
            {
                num++;
            }
        }
        return num;
    }

    public int GetPlainsCountInArea()
    {
        int num = 0;
        global::WorldCode.Plane plane = base.GetPlane();
        foreach (Vector3i item in this.GetArea())
        {
            Hex hexAt = plane.GetHexAt(item);
            if (hexAt != null && hexAt.GetTerrain().terrainType == ETerrainType.GrassLand)
            {
                num++;
            }
        }
        return num;
    }

    public int GetMiningCountInArea()
    {
        int num = 0;
        global::WorldCode.Plane plane = base.GetPlane();
        foreach (Vector3i item in this.GetArea())
        {
            Hex hexAt = plane.GetHexAt(item);
            if (hexAt != null && (hexAt.GetTerrain().terrainType == ETerrainType.Mountain || hexAt.GetTerrain().terrainType == ETerrainType.Hill || (hexAt.Resource != null && hexAt.Resource != (Resource)RESOURCE.WILD_GAME && hexAt.Resource != (Resource)RESOURCE.NIGHTSHADE && hexAt.Resource != (Resource)RESOURCE.MAMMOTH && hexAt.Resource != (Resource)RESOURCE.FISH && hexAt.Resource != (Resource)RESOURCE.REEF && hexAt.Resource != (Resource)RESOURCE.PEARLS)))
            {
                num++;
            }
        }
        return num;
    }

    public int GetNodeCountInArea()
    {
        int num = 0;
        foreach (global::MOM.Location townLocation in this.GetTownLocations())
        {
            if (townLocation.locationType == ELocationType.Node)
            {
                num++;
            }
        }
        return num;
    }

    public FInt GetProductionInArea()
    {
        if (this.productionInArea == FInt.ZERO)
        {
            this.productionInArea = global::MOM.Location.GetProductionInArea(base.GetPlane(), base.GetPosition(), this.GetTownRange());
            this.unitDiscount = this.GetUnitDiscountInArea();
            this.buildingDiscount = this.GetBuildingDiscountInArea();
            this.craftingQueue.RecaclulateItemCostInQueue();
        }
        return this.productionInArea;
    }

    public FInt GetUnitDiscountInArea()
    {
        FInt zERO = FInt.ZERO;
        foreach (Resource resource in this.GetResources())
        {
            zERO += resource.bonusTypes.unitProductionCost;
            if (this.GetSourceTown().buildingResourceBonus != null)
            {
                BuildingResourceBonus[] buildingResourceBonus = this.GetSourceTown().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus2 in buildingResourceBonus)
                {
                    if (resource == buildingResourceBonus2.resource)
                    {
                        zERO += buildingResourceBonus2.bonusTypes.unitProductionCost;
                    }
                }
            }
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().buildingResourceBonus == null)
                {
                    continue;
                }
                BuildingResourceBonus[] buildingResourceBonus = building.Get().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus3 in buildingResourceBonus)
                {
                    if (buildingResourceBonus3.resource == resource)
                    {
                        zERO += buildingResourceBonus3.bonusTypes.unitProductionCost;
                    }
                }
            }
        }
        return FInt.Min(zERO, (FInt)0.9f);
    }

    public FInt GetBuildingDiscountInArea()
    {
        FInt zERO = FInt.ZERO;
        foreach (Resource resource in this.GetResources())
        {
            zERO += resource.bonusTypes.buildingProductionCost;
            if (this.GetSourceTown().buildingResourceBonus != null)
            {
                BuildingResourceBonus[] buildingResourceBonus = this.GetSourceTown().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus2 in buildingResourceBonus)
                {
                    if (resource == buildingResourceBonus2.resource)
                    {
                        zERO += buildingResourceBonus2.bonusTypes.buildingProductionCost;
                    }
                }
            }
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().buildingResourceBonus == null)
                {
                    continue;
                }
                BuildingResourceBonus[] buildingResourceBonus = building.Get().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus3 in buildingResourceBonus)
                {
                    if (buildingResourceBonus3.resource == resource)
                    {
                        zERO += buildingResourceBonus3.bonusTypes.buildingProductionCost;
                    }
                }
            }
        }
        return FInt.Min(zERO, (FInt)0.9f);
    }

    public List<Resource> GetResources()
    {
        this.UpdateCachedResources();
        return this.resources;
    }

    public List<Resource> GetPotentialResources()
    {
        this.UpdateCachedResources();
        return this.potencialResources;
    }

    public List<Multitype<Resource, bool>> GetResourceAndState()
    {
        this.UpdateCachedResources();
        return this.resourceAndState;
    }

    private void UpdateCachedResources()
    {
        if (this.resources != null)
        {
            return;
        }
        this.resources = new List<Resource>();
        this.potencialResources = new List<Resource>();
        this.resourceAndState = new List<Multitype<Resource, bool>>();
        global::WorldCode.Plane plane = base.GetPlane();
        foreach (Vector3i item in this.GetSurroundingArea(this.GetTownRange()))
        {
            Hex hexAt = plane.GetHexAt(plane.GetPositionWrapping(item));
            if (hexAt != null)
            {
                if (hexAt.Resource != null && hexAt.ActiveHex)
                {
                    this.resources.Add(hexAt.Resource);
                }
                if (hexAt.Resource != null)
                {
                    this.potencialResources.Add(hexAt.Resource);
                    this.resourceAndState.Add(new Multitype<Resource, bool>(hexAt.Resource, hexAt.ActiveHex));
                }
            }
        }
        this.unitDiscount = this.GetUnitDiscountInArea();
        this.buildingDiscount = this.GetBuildingDiscountInArea();
    }

    public List<global::MOM.Location> GetTownLocations()
    {
        this.UpdateCachedTownLocations();
        return this.townLocations;
    }

    public void UpdateTownLocations()
    {
        this.townLocations = null;
        this.UpdateCachedTownLocations();
    }

    private void UpdateCachedTownLocations()
    {
        if (this.townLocations != null)
        {
            return;
        }
        this.townLocations = new List<global::MOM.Location>();
        global::WorldCode.Plane p = base.GetPlane();
        foreach (Vector3i v in this.GetSurroundingArea(this.GetTownRange()))
        {
            global::MOM.Location location = GameManager.GetLocationsOfThePlane(p).Find((global::MOM.Location o) => o.GetPosition() == v);
            if (location != null)
            {
                this.townLocations.Add(location);
            }
        }
    }

    public void EndTurnUpdate()
    {
        int num = this.PopulationIncreasePerTurn();
        if (this.IsAnOutpost())
        {
            foreach (Resource resource in this.GetResources())
            {
                num += resource.outpostGrowth;
            }
            if (num < 20)
            {
                num = 20;
            }
        }
        this.Population += num;
        this.ChangeTownModel();
        if (this.GetUnits().Count > 0)
        {
            this.turnsUndefended = 0;
        }
        else
        {
            this.turnsUndefended++;
        }
    }

    public int MaxPopulation()
    {
        int value = this.GetFoodInArea().ToInt();
        this.ProcessIntigerScripts(EEnchantmentType.MaximumPopulationModifier, ref value);
        this.ProcessIntigerScripts(EEnchantmentType.MaximumPopulationModifierMP, ref value);
        return value;
    }

    public FInt AvaliableFood()
    {
        return FInt.ONE;
    }

    public int GetPopUnits()
    {
        return Mathf.FloorToInt(((float)this.Population + Mathf.Epsilon) / 1000f);
    }

    public int GetWorkers()
    {
        return this.GetPopUnits() - this.GetFarmers() - this.GetRebels();
    }

    public int GetFarmers()
    {
        this.farmers = Mathf.Min(this.farmers, this.GetPopUnits() - this.GetRebels());
        return this.farmers;
    }

    public void SetUnrestDirty()
    {
        this.unrest = float.MinValue;
    }

    private static FInt BaseTension(Race townRace, Race wizardRace, bool useFallback)
    {
        if (wizardRace == null)
        {
            return FInt.ZERO;
        }
        if (townRace.raceTension != null)
        {
            RaceTension[] raceTension = townRace.raceTension;
            foreach (RaceTension raceTension2 in raceTension)
            {
                if (raceTension2.race == wizardRace)
                {
                    return raceTension2.value;
                }
            }
        }
        if (townRace == wizardRace)
        {
            return FInt.ZERO;
        }
        if (!useFallback)
        {
            return FInt.MAX;
        }
        if (townRace.tensionFallback != null)
        {
            FInt fInt = TownLocation.BaseTension(townRace.tensionFallback, wizardRace, useFallback: false);
            if (fInt != FInt.MAX)
            {
                return fInt;
            }
        }
        if (wizardRace.tensionFallback != null)
        {
            FInt fInt2 = TownLocation.BaseTension(townRace, wizardRace.tensionFallback, useFallback: false);
            if (fInt2 != FInt.MAX)
            {
                return fInt2;
            }
        }
        if (townRace.tensionFallback != null && wizardRace.tensionFallback != null)
        {
            FInt fInt3 = TownLocation.BaseTension(townRace.tensionFallback, wizardRace.tensionFallback, useFallback: false);
            if (fInt3 != FInt.MAX)
            {
                return fInt3;
            }
        }
        return wizardRace.tensionFallback2;
    }

    public float GetUnrest(StatDetails sd = null)
    {
        if (this.unrest == float.MinValue || sd != null)
        {
            FInt rebelion = this.GetTaxRank().rebelion;
            sd?.Add("UI_TOWN_UNREST_FROM_TAX", rebelion, allowZero: true);
            Town sourceTown = this.GetSourceTown();
            PlayerWizard wizard = GameManager.GetWizard(base.GetOwnerID());
            FInt fInt = TownLocation.BaseTension(sourceTown.race, wizard?.mainRace.Get(), useFallback: true);
            rebelion += fInt;
            sd?.Add("UI_TOWN_UNREST_FROM_TENSION", fInt);
            this.unrest = rebelion.ToFloat();
            float num = this.unrest;
            this.ProcessFloatScripts(EEnchantmentType.UnrestModifier, ref this.unrest);
            sd?.Add("UI_TOWN_UNREST_FROM_ENCHANTMENTS", new FInt(this.unrest - num));
            num = this.unrest;
            this.ProcessFloatScripts(EEnchantmentType.UnrestModifierMP, ref this.unrest);
            sd?.Add("UI_TOWN_UNREST_FROM_ENCHANTMENTS_MULTIPLIER", new FInt(this.unrest - num));
            if (wizard != null && wizard.percentUnrestModifier != 0f)
            {
                float num2 = wizard.percentUnrestModifier.ToFloat();
                this.unrest += num2;
                sd?.Add("UI_TOWN_UNREST_FROM_TRAITS", new FInt(num2));
            }
            this.unrest = Mathf.Clamp01(this.unrest);
        }
        return this.unrest;
    }

    public int GetRebels(StatDetails sd = null)
    {
        float num = this.GetUnrest();
        int popUnits = this.GetPopUnits();
        int value = (int)Mathf.Round((float)popUnits * (num + Mathf.Epsilon));
        string id = global::DBUtils.Localization.Get("UI_REBELS_FROM_UNREST", true, popUnits, new FInt(num));
        sd?.Add(id, value, allowZero: true);
        int num2 = value;
        this.ProcessIntigerScripts(EEnchantmentType.RebelsModifier, ref value);
        sd?.Add("UI_REBELS_MODIFIER_FROM_ENCHANTMENTS", value - num2);
        PlayerWizard wizard = GameManager.GetWizard(base.GetOwnerID());
        if (wizard != null)
        {
            this.rebelModifier = 0;
            this.rebelModifier += wizard.rebelModifier.ToInt();
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().tags == null)
                {
                    continue;
                }
                Tag tag = Array.Find(building.Get().tags, (Tag o) => o == (Tag)TAG.DIVINE_INFERNAL_BONUS);
                if (wizard.GetTraits() != null)
                {
                    Trait trait = wizard.GetTraits().Find((Trait o) => o == (Trait)TRAIT.DIVINE_POWER || o == (Trait)TRAIT.INFERNAL_POWER);
                    if (tag != null && trait != null)
                    {
                        this.rebelModifier += -1;
                        break;
                    }
                }
            }
        }
        sd?.Add("UI_TOWN_REBEL_REDUCTION_FROM_WIZARD", this.rebelModifier);
        int num3 = 0;
        foreach (Reference<global::MOM.Unit> unit in this.GetUnits())
        {
            if (unit.Get().GetAttFinal(TAG.FANTASTIC_CLASS) == 0)
            {
                num3++;
            }
        }
        if (FSMSelectionManager.Get() != null && FSMSelectionManager.Get().GetSelectedGroup() is global::MOM.Group group && group.beforeMovingAway == this)
        {
            foreach (Reference<global::MOM.Unit> unit2 in group.GetUnits())
            {
                if (unit2.Get().GetAttFinal(TAG.FANTASTIC_CLASS) == 0)
                {
                    num3++;
                }
            }
        }
        this.rebelModifier += -num3 / 2;
        sd?.Add("UI_TOWN_REBEL_REDUCTION_FROM_GARRISON", -num3 / 2);
        value += this.rebelModifier;
        return Mathf.Clamp(value, 0, this.GetPopUnits());
    }

    public int CalculateMoneyIncome(bool includeUpkeep = false, StatDetails details = null)
    {
        if (this.IsAnOutpost())
        {
            return 0;
        }
        int num = (int)(this.GetTaxRank().income.ToFloat() * (float)(this.GetPopUnits() - this.GetRebels()));
        num *= this.taxMultiplier.ToInt();
        if (details != null)
        {
            details?.Add("UI_GOLD_DETAILS_TOWN_TAXES", num);
        }
        int num2 = num;
        foreach (Resource resource in this.GetResources())
        {
            num += resource.bonusTypes.money.ToInt();
            if (this.GetSourceTown().buildingResourceBonus != null)
            {
                BuildingResourceBonus[] buildingResourceBonus = this.GetSourceTown().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus2 in buildingResourceBonus)
                {
                    if (resource == buildingResourceBonus2.resource)
                    {
                        num += buildingResourceBonus2.bonusTypes.money.ToInt();
                    }
                }
            }
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().buildingResourceBonus == null)
                {
                    continue;
                }
                BuildingResourceBonus[] buildingResourceBonus = building.Get().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus3 in buildingResourceBonus)
                {
                    if (buildingResourceBonus3.resource == resource)
                    {
                        num += buildingResourceBonus3.bonusTypes.money.ToInt();
                    }
                }
            }
        }
        if (details != null)
        {
            details?.Add("UI_GOLD_DETAILS_TOWN_RESOURCES", num - num2);
        }
        num2 = num;
        int upkeep = 0;
        this.ProcessIntigerScripts(EEnchantmentType.GoldModifier, ref num, ref upkeep);
        this.ProcessIntigerScripts(EEnchantmentType.GoldModifierMP, ref num, ref upkeep);
        details?.Add("UI_GOLD_DETAILS_TOWN_INCOME", num - num2);
        details?.Add("UI_GOLD_DETAILS_TOWN_UPKEEP", upkeep);
        num += upkeep;
        if (includeUpkeep && this.buildings != null)
        {
            num2 = num;
            foreach (DBReference<Building> building2 in this.buildings)
            {
                num -= building2.Get().upkeepCost;
            }
            details?.Add("UI_GOLD_DETAILS_TOWN_UPKEEP", num - num2);
        }
        num2 = num;
        float num3 = this.IncomeProduction();
        if (num3 > 0f)
        {
            int num4 = this.CalculateProductionIncome();
            num += Mathf.FloorToInt(num3 * (float)num4);
        }
        details?.Add("UI_GOLD_DETAILS_TOWN_PRODUCTION", num - num2);
        if (includeUpkeep)
        {
            HashSet<Vector3i> roadNetworkForTown = base.GetPlane().GetRoadManagers().GetRoadNetworkForTown(this);
            if (roadNetworkForTown != null)
            {
                float num5 = 0f;
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (entity.Value != this && entity.Value is TownLocation townLocation && townLocation.GetOwnerID() == base.GetOwnerID() && townLocation.GetPlane() == base.GetPlane() && roadNetworkForTown.Contains(townLocation.GetPosition()))
                    {
                        num5 += this.GetRoadIncomeGain(townLocation);
                    }
                }
                if (num5 > 0f)
                {
                    if (this.seaside)
                    {
                        num5 += 0.1f;
                    }
                    if (base.GetPlane().GetHexAt(base.GetPosition()).viaRiver != null)
                    {
                        num5 += 0.2f;
                    }
                    if (this.race == (Race)RACE.NOMADS)
                    {
                        num5 += 0.5f;
                    }
                    num5 = Mathf.Min((float)this.GetPopUnits() * 0.03f, num5);
                    int num6 = (int)((float)num * num5);
                    if (num6 > 0)
                    {
                        num += num6;
                        details?.Add("UI_GOLD_DETAILS_ROAD_NETWORK", num6);
                        if (details != null || !base.GetWizardOwner().IsHuman)
                        {
                            this.roadBonus = num6;
                        }
                    }
                }
            }
        }
        return num;
    }

    public float GetRoadIncomeGain(TownLocation provider)
    {
        float num = 0f;
        int popUnits = provider.GetPopUnits();
        if (this.race == provider.race)
        {
            return num + 0.005f * (float)popUnits;
        }
        return num + 0.01f * (float)popUnits;
    }

    public int CalculateFoodIncome(StatDetails sd = null)
    {
        if (this.IsAnOutpost())
        {
            return 0;
        }
        int value = this.FoodProductionOnFoodSlots().ToInt();
        sd?.Add("UI_TOWN_FOOD_PRODUCTION", value, allowZero: true);
        int num = value;
        foreach (Resource resource in this.GetResources())
        {
            int num2 = resource.bonusTypes.food.ToInt();
            value += num2;
            if (this.GetSourceTown().buildingResourceBonus != null)
            {
                BuildingResourceBonus[] buildingResourceBonus = this.GetSourceTown().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus2 in buildingResourceBonus)
                {
                    if (resource == buildingResourceBonus2.resource)
                    {
                        num2 = buildingResourceBonus2.bonusTypes.food.ToInt();
                        value += num2;
                    }
                }
            }
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().buildingResourceBonus == null)
                {
                    continue;
                }
                BuildingResourceBonus[] buildingResourceBonus = building.Get().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus3 in buildingResourceBonus)
                {
                    if (buildingResourceBonus3.resource == resource)
                    {
                        num2 = buildingResourceBonus3.bonusTypes.food.ToInt();
                        value += num2;
                    }
                }
            }
        }
        sd?.Add("UI_TOWN_FOOD_RESOURCE_BONUS", value - num);
        num = value;
        this.ProcessIntigerScripts(EEnchantmentType.FoodModifier, ref value);
        sd?.Add("UI_TOWN_FOOD_ENCHANTMENTS", value - num);
        num = value;
        this.ProcessIntigerScripts(EEnchantmentType.FoodModifierMP, ref value);
        sd?.Add("UI_TOWN_FOOD_ENCHANTMENT_MULTIPLIER", value - num);
        return value;
    }

    public int CalculateFoodFinalIncome(bool reportStarvation = true)
    {
        int num = this.CalculateFoodIncome();
        int popUnits = this.GetPopUnits();
        int result = num - popUnits;
        if (base.owner == PlayerWizard.HumanID())
        {
            GameManager.GetHumanWizard().TrackStarvation(this, reportStarvation && num - popUnits < 0);
        }
        return result;
    }

    public int CalculateResearchIncomeLimited(StatDetails sd = null)
    {
        if (this.IsAnOutpost())
        {
            return 0;
        }
        int value = 0;
        this.ProcessIntigerScripts(EEnchantmentType.ResearchModifier, ref value);
        sd?.Add("UI_TOWN_RESEARCH_FROM_ENCHANTMENTS", value, allowZero: true);
        return value;
    }

    public int CalculateManaIncomeLimited()
    {
        if (this.IsAnOutpost())
        {
            return 0;
        }
        int value = 0;
        this.ProcessIntigerScripts(EEnchantmentType.ManaModifier, ref value);
        this.ProcessIntigerScripts(EEnchantmentType.ManaModifierMP, ref value);
        return value;
    }

    public int CalculatePowerIncome(StatDetails sd = null)
    {
        if (this.IsAnOutpost())
        {
            return 0;
        }
        Town sourceTown = this.GetSourceTown();
        PlayerWizard wizardOwner = base.GetWizardOwner();
        FInt value = FInt.ZERO;
        if (wizardOwner != null)
        {
            this.ProcessFIntScripts(EEnchantmentType.PowerModifierReligious, ref value);
            FInt fInt = value;
            sd?.Add("UI_TOWN_POWER_FROM_RELIGIOUS", fInt, allowZero: true);
            EnchantmentInstance enchantmentInstance = this.GetEnchantments().Find((EnchantmentInstance o) => o.source.Get() == (Enchantment)ENCH.DARK_RITUALS);
            EnchantmentInstance enchantmentInstance2 = wizardOwner.GetEnchantments().Find((EnchantmentInstance o) => o.source.Get() == (Enchantment)ENCH.LIFE_MOON || o.source.Get() == (Enchantment)ENCH.DEATH_MOON);
            if (enchantmentInstance != null)
            {
                value += fInt;
                sd?.Add("UI_TOWN_POWER_FROM_DARK_RITUAL", fInt);
            }
            if (enchantmentInstance2 != null)
            {
                float num = (int)ScriptLibrary.Call("UTIL_GetStringParameterValue", enchantmentInstance2.parameters);
                num /= 100f;
                FInt fInt2 = fInt * num;
                value += fInt2;
                sd?.Add("UI_TOWN_POWER_FROM_LIFE_AND_DEATH_MOON", fInt2);
            }
            Enchantment nLifeMoon = (Enchantment)ENCH.TAKE_LIFE_MOON;
            Enchantment nDeathMoon = (Enchantment)ENCH.TAKE_DEATH_MOON;
            EnchantmentInstance enchantmentInstance3 = wizardOwner.GetEnchantments().Find((EnchantmentInstance o) => o.source.Get() == nLifeMoon || o.source.Get() == nDeathMoon);
            if (enchantmentInstance3 != null)
            {
                float num2 = (int)ScriptLibrary.Call("UTIL_GetStringParameterValue", enchantmentInstance3.parameters);
                num2 /= 100f;
                num2 = Mathf.Min(num2, 1f);
                value -= fInt * num2;
                sd?.Add("UI_TOWN_POWER_FROM_LIFE_AND_DEATH_MOON", fInt * num2);
            }
            if (wizardOwner.townsPowerPercentBonus > 0)
            {
                int num3 = (value * wizardOwner.townsPowerPercentBonus).ToInt();
                value += num3;
                sd?.Add("UI_TOWN_POWER_FROM_TRAITS", num3);
            }
        }
        FInt fInt3 = this.GetWorkers() * sourceTown.worker.powerProduction + this.GetFarmers() * sourceTown.farmer.powerProduction + this.GetRebels() * sourceTown.rebel.powerProduction;
        value += fInt3;
        sd?.Add("UI_TOWN_POWER_FROM_POPULATION", fInt3);
        FInt fInt4 = value;
        foreach (Resource resource in this.GetResources())
        {
            value += resource.bonusTypes.power;
            if (this.GetSourceTown().buildingResourceBonus != null)
            {
                BuildingResourceBonus[] buildingResourceBonus = this.GetSourceTown().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus2 in buildingResourceBonus)
                {
                    if (resource == buildingResourceBonus2.resource)
                    {
                        value += buildingResourceBonus2.bonusTypes.power;
                    }
                }
            }
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().buildingResourceBonus == null)
                {
                    continue;
                }
                BuildingResourceBonus[] buildingResourceBonus = building.Get().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus3 in buildingResourceBonus)
                {
                    if (buildingResourceBonus3.resource == resource)
                    {
                        value += buildingResourceBonus3.bonusTypes.power;
                    }
                }
            }
        }
        sd?.Add("UI_TOWN_POWER_FROM_RESOURCES", value - fInt4);
        fInt4 = value;
        if (base.GetWizardOwner() != null)
        {
            if (this.townLocations == null)
            {
                this.UpdateTownLocations();
            }
            foreach (global::MOM.Location townLocation in this.townLocations)
            {
                if (townLocation.locationType == ELocationType.Node && townLocation.melding != null && townLocation.melding.meldOwner == base.GetOwnerID())
                {
                    FInt value2 = FInt.ZERO;
                    this.ProcessFIntScripts(EEnchantmentType.PowerModifierNode, ref value2);
                    value += value2;
                }
            }
        }
        sd?.Add("UI_TOWN_POWER_FROM_POWER_NODES", value - fInt4);
        fInt4 = value;
        this.ProcessFIntScripts(EEnchantmentType.PowerModifier, ref value);
        sd?.Add("UI_TOWN_POWER_FROM_ENCHANTMENTS", value - fInt4);
        fInt4 = value;
        this.ProcessFIntScripts(EEnchantmentType.PowerModifierMP, ref value);
        sd?.Add("UI_TOWN_POWER_FROM_ENCHANTMENT_MULTIPLIERS", value - fInt4);
        return value.ToInt();
    }

    public int CalculateProductionIncome(StatDetails sd = null)
    {
        Town sourceTown = this.GetSourceTown();
        FInt fInt = this.GetWorkers() * sourceTown.worker.production + this.GetFarmers() * sourceTown.farmer.production + this.GetRebels() * sourceTown.rebel.production;
        sd?.Add("UI_TOWN_PRODUCTION_BASE", fInt, allowZero: true);
        int num = fInt.ToInt();
        FInt fInt2 = this.GetProductionInArea() * fInt;
        int value = Mathf.Max((fInt + fInt2).ToInt(), 1);
        sd?.Add("UI_TOWN_PRODUCTION_AREA_BONUS", value - num);
        int num2 = value;
        this.ProcessIntigerScripts(EEnchantmentType.ProductionModifier, ref value);
        sd?.Add("UI_TOWN_PRODUCTION_ENCHANTMENT_BONUS", value - num2);
        num2 = value;
        this.ProcessIntigerScripts(EEnchantmentType.ProductionModifierMP, ref value);
        value = ((value >= 0) ? value : 0);
        sd?.Add("UI_TOWN_PRODUCTION_ENCHANTMENT_MULTIPLIER_BONUS", value - num2);
        return value;
    }

    public int PopulationIncreasePerTurn()
    {
        int num = this.CalculatePopulationIncome(this.CalculateFoodIncome() - this.GetPopUnits());
        if (base.GetWizardOwner() != null)
        {
            if (base.GetWizardOwner() == GameManager.GetHumanWizard())
            {
                float settingAsFloat = DifficultySettingsData.GetSettingAsFloat("UI_OUR_TOWN_GROW_MULTIPLIER");
                if ((double)settingAsFloat > 1.0)
                {
                    num = (int)((float)num * settingAsFloat);
                }
            }
            else
            {
                float settingAsFloat2 = DifficultySettingsData.GetSettingAsFloat("UI_FOE_TOWN_GROW_MULTIPLIER");
                if ((double)settingAsFloat2 > 1.0)
                {
                    num = (int)((float)num * settingAsFloat2);
                }
            }
        }
        return num;
    }

    private int CalculatePopulationIncome(int foodIncome)
    {
        int num = 0;
        if (foodIncome < 0)
        {
            num += foodIncome * 50;
        }
        else
        {
            int num2 = (this.MaxPopulation() - this.GetPopUnits() + 1) / 2;
            num = ((!(this.GetFoodInArea() > this.GetPopUnits())) ? (num + num2 * 5) : (num + num2 * 10));
            Town sourceTown = this.GetSourceTown();
            if (sourceTown != null)
            {
                if (sourceTown.populationGrowth > 0 && num2 > 0)
                {
                    num += sourceTown.populationGrowth;
                }
                else if (sourceTown.populationGrowth < 0)
                {
                    num += sourceTown.populationGrowth;
                }
            }
            float num3 = this.HousingProduction();
            if (num3 > 0f && num > 0)
            {
                num += (int)((float)num * num3);
            }
            this.ProcessIntigerScripts(EEnchantmentType.PopulationGrowModifier, ref num);
            this.ProcessIntigerScripts(EEnchantmentType.PopulationGrowModifierMP, ref num);
        }
        if (this.GetPopUnits() >= this.MaxPopulation() && num > 0)
        {
            return 0;
        }
        return num;
    }

    public int MinFarmers()
    {
        int popUnits = this.GetPopUnits();
        int rebels = this.GetRebels();
        this.GetSourceTown();
        int num = this.farmers;
        int num2 = 0;
        FInt availbleFood = this.GetFoodInArea();
        int num3 = 0;
        foreach (Resource resource in this.GetResources())
        {
            num3 += resource.bonusTypes.food.ToInt();
            if (this.GetSourceTown().buildingResourceBonus != null)
            {
                BuildingResourceBonus[] buildingResourceBonus = this.GetSourceTown().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus2 in buildingResourceBonus)
                {
                    if (resource == buildingResourceBonus2.resource)
                    {
                        num3 += buildingResourceBonus2.bonusTypes.food.ToInt();
                    }
                }
            }
            foreach (DBReference<Building> building in this.buildings)
            {
                if (building.Get().buildingResourceBonus == null)
                {
                    continue;
                }
                BuildingResourceBonus[] buildingResourceBonus = building.Get().buildingResourceBonus;
                foreach (BuildingResourceBonus buildingResourceBonus3 in buildingResourceBonus)
                {
                    if (buildingResourceBonus3.resource == resource)
                    {
                        num3 += buildingResourceBonus3.bonusTypes.food.ToInt();
                    }
                }
            }
        }
        while (true)
        {
            int workers = popUnits - rebels - num;
            int num4 = this.FoodProductionOnFoodSlots(availbleFood, num, workers, rebels).ToInt();
            num4 += num3;
            this.ProcessIntigerScripts(EEnchantmentType.FoodModifier, ref num4);
            this.ProcessIntigerScripts(EEnchantmentType.FoodModifierMP, ref num4);
            if (num4 < popUnits)
            {
                if (num2 < 0)
                {
                    return num + 1;
                }
                num2 = 1;
                num++;
                continue;
            }
            if (num4 <= popUnits || num <= 0)
            {
                break;
            }
            if (num2 > 0)
            {
                return num;
            }
            num2 = -1;
            num--;
        }
        return num;
    }

    public Town GetSourceTown()
    {
        return base.source.Get() as Town;
    }

    public List<Building> PossibleBuildings(bool atThisMoment)
    {
        if (!(base.source.Get() is Town town))
        {
            return null;
        }
        if (!atThisMoment)
        {
            return new List<Building>(town.possibleBuildings);
        }
        List<Building> list = new List<Building>();
        if (town.possibleBuildings != null)
        {
            Building[] possibleBuildings = town.possibleBuildings;
            foreach (Building building in possibleBuildings)
            {
                if (this.buildings.Contains(building) || this.craftingQueue.craftingItems.Find((CraftingItem o) => o.craftedBuilding == building) != null || (building.marineBuilding && !this.seaside))
                {
                    continue;
                }
                if (building.parentBuildingRequired != null)
                {
                    bool flag = true;
                    Building[] parentBuildingRequired = building.parentBuildingRequired;
                    foreach (Building req in parentBuildingRequired)
                    {
                        if (!this.buildings.Contains(req) && (base.owner != PlayerWizard.HumanID() || this.craftingQueue.craftingItems.Find((CraftingItem o) => o.craftedBuilding == req) == null))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        continue;
                    }
                }
                if (building == (Building)BUILDING.HOUSING && this.GetPopUnits() >= this.MaxPopulation())
                {
                    continue;
                }
                if (building == (Building)BUILDING.SAWMILL)
                {
                    if (this.GetForestCountInArea() == 0)
                    {
                        continue;
                    }
                }
                else if (building == (Building)BUILDING.STABLES || building == (Building)BUILDING.FANTASTIC_STABLES)
                {
                    if (this.GetPlainsCountInArea() == 0)
                    {
                        continue;
                    }
                }
                else if (building == (Building)BUILDING.MINERS_GUILD || building == (Building)BUILDING.DWARF_MINERS_GUILD)
                {
                    if (this.GetMiningCountInArea() == 0)
                    {
                        continue;
                    }
                }
                else if (building == (Building)BUILDING.MAGIC_COLLECTOR && this.GetNodeCountInArea() == 0)
                {
                    continue;
                }
                list.Add(building);
            }
        }
        return list;
    }

    public List<Building> GetPossibleBuildingsDisplayList()
    {
        if (!(base.source.Get() is Town town))
        {
            return null;
        }
        List<Building> list = new List<Building>();
        if (town.possibleBuildings != null)
        {
            Building[] possibleBuildings2 = town.possibleBuildings;
            foreach (Building building in possibleBuildings2)
            {
                if (!this.buildings.Contains(building) && this.craftingQueue.craftingItems.Find((CraftingItem o) => o.craftedBuilding == building) == null && (!building.marineBuilding || this.seaside))
                {
                    list.Add(building);
                }
            }
        }
        List<Building> possibleBuildings = this.PossibleBuildings(atThisMoment: true);
        list.Sort(delegate(Building x, Building y)
        {
            bool flag = possibleBuildings.Contains(x);
            bool flag2 = possibleBuildings.Contains(y);
            return (flag == flag2) ? x.buildCost.CompareTo(y.buildCost) : (-flag.CompareTo(flag2));
        });
        return list;
    }

    public List<global::DBDef.Unit> PossibleUnits(bool ignoreBuildingReq = false)
    {
        if (!(base.source.Get() is Town town))
        {
            return null;
        }
        Race race = (Race)RACE.NON_RACIAL;
        List<global::DBDef.Unit> type = DataBase.GetType<global::DBDef.Unit>();
        List<global::DBDef.Unit> list = new List<global::DBDef.Unit>();
        foreach (global::DBDef.Unit item in type)
        {
            if (item.race != town.race && item.race != race)
            {
                continue;
            }
            if (!ignoreBuildingReq && item.requiredBuildings != null)
            {
                bool flag = true;
                Building[] requiredBuildings = item.requiredBuildings;
                foreach (Building req in requiredBuildings)
                {
                    if (!this.buildings.Contains(req) && (base.owner != PlayerWizard.HumanID() || this.craftingQueue.craftingItems.Find((CraftingItem o) => o.craftedBuilding == req) == null))
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                {
                    continue;
                }
            }
            if (item.populationCost <= 0 || this.Population - item.populationCost >= 1000)
            {
                list.Add(item);
            }
        }
        return list;
    }

    public List<global::DBDef.Unit> GetPossibleUnitsDisplayList()
    {
        if (!(base.source.Get() is Town town))
        {
            return null;
        }
        Race race = (Race)RACE.NON_RACIAL;
        List<global::DBDef.Unit> type = DataBase.GetType<global::DBDef.Unit>();
        List<global::DBDef.Unit> list = new List<global::DBDef.Unit>();
        foreach (global::DBDef.Unit item in type)
        {
            if (item.race != town.race && item.race != race)
            {
                continue;
            }
            if (item.requiredBuildings != null)
            {
                int num = 0;
                for (int i = 0; i < item.requiredBuildings.Length; i++)
                {
                    Building[] possibleBuildings = town.possibleBuildings;
                    foreach (Building building in possibleBuildings)
                    {
                        if (item.requiredBuildings[0] == building)
                        {
                            num++;
                        }
                    }
                }
                if (num != item.requiredBuildings.Length)
                {
                    continue;
                }
            }
            list.Add(item);
        }
        List<global::DBDef.Unit> possibleUnits = this.PossibleUnits();
        list.Sort(delegate(global::DBDef.Unit x, global::DBDef.Unit y)
        {
            bool flag = possibleUnits.Contains(x);
            bool flag2 = possibleUnits.Contains(y);
            return (flag == flag2) ? x.constructionCost.CompareTo(y.constructionCost) : (-flag.CompareTo(flag2));
        });
        return list;
    }

    public void AddBuilding(Building b)
    {
        this.buildings.Add(b);
        if (b.enchantments != null)
        {
            Enchantment[] enchantments = b.enchantments;
            foreach (Enchantment e in enchantments)
            {
                this.AddEnchantment(e, null).buildingEnchantment = true;
            }
        }
        this.unitDiscount = this.GetUnitDiscountInArea();
        this.buildingDiscount = this.GetBuildingDiscountInArea();
        this.craftingQueue.RecaclulateUnitsCostInQueue();
        this.SetUnrestDirty();
    }

    public Building CanRemoveBuilding(Building b, bool allowFortress = false)
    {
        if (b == (Building)BUILDING.FORTRESS)
        {
            if (!allowFortress)
            {
                return b;
            }
            return null;
        }
        if (b == (Building)BUILDING.ASTRAL_GATE)
        {
            return b;
        }
        if (b == (Building)BUILDING.EARTH_GATE)
        {
            return b;
        }
        if (b == (Building)BUILDING.NATURES_EYE)
        {
            return b;
        }
        if (b == (Building)BUILDING.ALTAR_OF_BATTLE)
        {
            return b;
        }
        if (b == (Building)BUILDING.STREAM_OF_LIFE)
        {
            return b;
        }
        foreach (DBReference<Building> building in this.buildings)
        {
            Building[] parentBuildingRequired = building.Get().parentBuildingRequired;
            if (parentBuildingRequired == null)
            {
                continue;
            }
            Building[] array = parentBuildingRequired;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == b)
                {
                    return building;
                }
            }
        }
        return null;
    }

    public bool IsRegularBuilding(Building b, bool skipFortress = true)
    {
        if (b == (Building)BUILDING.ASTRAL_GATE)
        {
            return false;
        }
        if (b == (Building)BUILDING.EARTH_GATE)
        {
            return false;
        }
        if (b == (Building)BUILDING.NATURES_EYE)
        {
            return false;
        }
        if (b == (Building)BUILDING.ALTAR_OF_BATTLE)
        {
            return false;
        }
        if (b == (Building)BUILDING.STREAM_OF_LIFE)
        {
            return false;
        }
        if (b == (Building)BUILDING.FORTRESS)
        {
            if (skipFortress)
            {
                return false;
            }
            return true;
        }
        return true;
    }

    public bool RemoveBuildingSpecial(Building b, bool skipFortress = true)
    {
        if (skipFortress && b == (Building)BUILDING.FORTRESS)
        {
            return false;
        }
        if (!this.buildings.Contains(b))
        {
            return false;
        }
        if (this.tempDestroyedBuilding == null)
        {
            this.tempDestroyedBuilding = new List<Building>();
        }
        this.tempDestroyedBuilding.Add(b);
        return true;
    }

    public void AttemptToDestroyAwaitingBuildings()
    {
        if (this.tempDestroyedBuilding == null)
        {
            return;
        }
        foreach (Building v in this.tempDestroyedBuilding)
        {
            DBReference<Building> dBReference = this.buildings.Find((DBReference<Building> o) => o.Get() == v);
            if (dBReference != null)
            {
                this.RemoveBuilding(dBReference);
            }
        }
        this.tempDestroyedBuilding = null;
    }

    public override void FinishedIteratingEnchantments()
    {
        this.AttemptToDestroyAwaitingBuildings();
        base.FinishedIteratingEnchantments();
    }

    public bool RemoveBuilding(Building b, bool skipFortress = true)
    {
        if (skipFortress && b == (Building)BUILDING.FORTRESS)
        {
            return false;
        }
        if (!this.buildings.Contains(b))
        {
            return false;
        }
        this.buildings.Remove(b);
        if (b.enchantments != null)
        {
            Enchantment[] enchantments = b.enchantments;
            foreach (Enchantment e in enchantments)
            {
                this.RemoveEnchantment(e);
            }
        }
        this.unitDiscount = this.GetUnitDiscountInArea();
        this.buildingDiscount = this.GetBuildingDiscountInArea();
        this.craftingQueue.SanitizeRequirementsAfterRemovalOfBuilding(b);
        this.SetUnrestDirty();
        return true;
    }

    public override int GetStrategicValue()
    {
        int num = 150;
        if (this.buildings != null)
        {
            foreach (DBReference<Building> building in this.buildings)
            {
                num += building.Get().buildCost / 15;
            }
        }
        return num;
    }

    public FInt FoodProductionOnFoodSlots()
    {
        FInt availbleFood = this.GetFoodInArea();
        int num = this.GetFarmers();
        int workers = this.GetWorkers();
        int rebels = this.GetRebels();
        return this.FoodProductionOnFoodSlots(availbleFood, num, workers, rebels);
    }

    public FInt FoodProductionOnFoodSlots(FInt availbleFood, int farmers, int workers, int rebels)
    {
        Town sourceTown = this.GetSourceTown();
        FInt fInt = workers * sourceTown.worker.farmer + farmers * sourceTown.farmer.farmer + rebels * sourceTown.rebel.farmer;
        if (fInt > availbleFood)
        {
            FInt fInt2 = fInt - availbleFood;
            fInt = availbleFood + fInt2 / 2;
        }
        return fInt;
    }

    public float GetMilitaryEconomy()
    {
        if (this.buildings == null || this.buildings.Count < 1)
        {
            return 0f;
        }
        Town t = base.source.Get() as Town;
        List<Building> militaryBuildings = GameplayHelper.Get().GetMilitaryBuildings(t);
        int num = 0;
        foreach (DBReference<Building> building in this.buildings)
        {
            if (militaryBuildings.Contains(building))
            {
                num++;
            }
        }
        return (float)num / (float)militaryBuildings.Count;
    }

    public float GetNonMilitaryEconomy()
    {
        if (this.buildings == null || this.buildings.Count < 1)
        {
            return 0f;
        }
        Town t = base.source.Get() as Town;
        HashSet<Building> economyBuildings = GameplayHelper.Get().GetEconomyBuildings(t);
        int num = 0;
        foreach (DBReference<Building> building in this.buildings)
        {
            if (economyBuildings.Contains(building))
            {
                num++;
            }
        }
        return (float)num / (float)economyBuildings.Count;
    }

    public bool HaveBuilding(Building b)
    {
        if (this.buildings != null)
        {
            return this.buildings.Find((DBReference<Building> o) => o.Get() == b) != null;
        }
        return false;
    }

    public bool IsAnOutpost()
    {
        return this.Population < 1000;
    }

    public int GetTownRange()
    {
        return TownLocation.GetGeneralTownRange();
    }

    public static int GetGeneralTownRange()
    {
        return 2;
    }

    public int GetPopIncreaseTime()
    {
        if (this.PopulationIncreasePerTurn() > 0)
        {
            return Mathf.CeilToInt((1000f - ((float)this.Population - (float)this.GetPopUnits() * 1000f)) / (float)this.PopulationIncreasePerTurn());
        }
        return Mathf.FloorToInt(((float)this.Population - (float)this.GetPopUnits() * 1000f) / (float)this.PopulationIncreasePerTurn());
    }

    public float HousingProduction()
    {
        if (this.craftingQueue.craftingItems.Count > 0 && this.craftingQueue.craftingItems[0].craftedBuilding != null && this.craftingQueue.craftingItems[0].craftedBuilding == (Building)BUILDING.HOUSING)
        {
            float num = this.GetWorkers();
            float num2 = this.GetPopUnits();
            float num3 = num / num2;
            num3 += 0.1f;
            if (this.buildings.Contains((Building)BUILDING.BUILDERS_HALL))
            {
                num3 += 0.15f;
            }
            if (this.buildings.Contains((Building)BUILDING.SAWMILL))
            {
                num3 += 0.1f;
            }
            return num3;
        }
        return 0f;
    }

    public float IncomeProduction()
    {
        if (this.craftingQueue.craftingItems.Count > 0 && this.craftingQueue.craftingItems[0].craftedBuilding != null && this.craftingQueue.craftingItems[0].craftedBuilding == (Building)BUILDING.TRADE_GOODS)
        {
            return 0.5f;
        }
        return 0f;
    }

    public void TurnToRuin()
    {
        base.DestroyMarkers();
        if (base.model != null)
        {
            global::UnityEngine.Object.Destroy(base.model);
        }
        List<global::DBDef.Location> type = DataBase.GetType<global::DBDef.Location>();
        type = type.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.Ruins);
        int index = global::UnityEngine.Random.Range(0, type.Count);
        global::MOM.Location location = global::MOM.Location.CreateLocation(base.GetPosition(), base.GetPlane(), type[index], 0);
        List<Reference<global::MOM.Unit>> units = location.localGroup.Get().GetUnits();
        for (int num = units.Count - 1; num > 0; num--)
        {
            units[num].Get().Destroy();
        }
        base.localGroup.Get().TransferUnits(location.localGroup.Get());
        GameManager.Get().Unregister(this);
    }

    private bool UpdateModelName()
    {
        if (!(base.source.Get() is Town town))
        {
            Debug.LogError("Missing town source for Town Location " + base.source);
            return false;
        }
        string text = "";
        Race race = town.race;
        text = race.visualGroup;
        if (string.IsNullOrEmpty(text))
        {
            if (race == (Race)RACE.GNOLLS || race == (Race)RACE.NOMADS || race == (Race)RACE.BARBARIANS)
            {
                text = "Primitive";
            }
            if (race == (Race)RACE.TROLLS || race == (Race)RACE.KLACKONS || race == (Race)RACE.LIZARDMEN)
            {
                text = "Nature";
            }
            if (race == (Race)RACE.DRACONIANS || race == (Race)RACE.HIGH_ELVES || race == (Race)RACE.DARK_ELVES)
            {
                text = "Magical";
            }
            if (race == (Race)RACE.DWARVES || race == (Race)RACE.ORCS || race == (Race)RACE.BEASTMEN)
            {
                text = "Warlike";
            }
            else if (race == (Race)RACE.HALFLINGS || race == (Race)RACE.HIGH_MEN || string.IsNullOrEmpty(text))
            {
                text = "";
            }
        }
        switch (this.GetTownSize())
        {
        case TownSize.City:
            if (base.modelName != town.graphic.city + text)
            {
                base.modelName = town.graphic.city + text;
                return true;
            }
            break;
        case TownSize.Town:
            if (base.modelName != town.graphic.town + text)
            {
                base.modelName = town.graphic.town + text;
                return true;
            }
            break;
        case TownSize.Village:
            if (base.modelName != town.graphic.village + text)
            {
                base.modelName = town.graphic.village + text;
                return true;
            }
            break;
        case TownSize.Hamlet:
            if (base.modelName != town.graphic.hamlet + text)
            {
                base.modelName = town.graphic.hamlet + text;
                return true;
            }
            break;
        case TownSize.Settlement:
            if (base.modelName != town.graphic.settlement + text)
            {
                base.modelName = town.graphic.settlement + text;
                return true;
            }
            break;
        case TownSize.Outpost:
            if (base.modelName != town.graphic.outpost + text)
            {
                base.modelName = town.graphic.outpost + text;
                return true;
            }
            break;
        }
        return false;
    }

    public void ChangeTownModel()
    {
        if (base.discovered && this.UpdateModelName())
        {
            global::UnityEngine.Object.Destroy(base.model);
            this.InitializeModel();
        }
    }

    public void UpdateTownArea()
    {
        if (base.GetPlane() != null)
        {
            PlayerWizard wizardOwner = base.GetWizardOwner();
            if (wizardOwner != null)
            {
                base.GetPlane().GetMarkers_().ChangeTownArea(this.GetSurroundingArea(2), wizardOwner.GetColor());
            }
            else
            {
                base.GetPlane().GetMarkers_().ChangeTownArea(this.GetSurroundingArea(2), Color.white, show: false);
            }
        }
    }

    public override void InitializeModel()
    {
        base.InitializeModel();
        this.UpdateTownArea();
    }

    public override void Destroy()
    {
        if (base.plane != null)
        {
            DataHeatMaps.MakeMapDirty(base.plane, DataHeatMaps.HMType.SettlementValue);
            base.plane.GetMarkers_().ChangeTownArea(this.GetSurroundingArea(2), Color.black, show: false);
        }
        base.Destroy();
    }

    public override string GetName()
    {
        return base.name;
    }

    public TownSize GetTownSize()
    {
        TownSize townSize = TownSize.Outpost;
        if (!this.IsAnOutpost())
        {
            townSize++;
            if (this.Population >= 5000)
            {
                townSize++;
                if (this.Population >= 9000)
                {
                    townSize++;
                    if (this.Population >= 13000)
                    {
                        townSize++;
                        if (this.Population >= 17000)
                        {
                            townSize++;
                        }
                    }
                }
            }
        }
        return townSize;
    }

    public bool CanSeeInside()
    {
        List<Vector3i> surroundingArea = this.GetSurroundingArea();
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(base.GetPlane());
        int num = PlayerWizard.HumanID();
        foreach (global::MOM.Group item in groupsOfPlane)
        {
            if (item.GetOwnerID() == num && surroundingArea.Contains(item.GetPosition()))
            {
                return true;
            }
        }
        return false;
    }

    public override void UpdateOwnerModels()
    {
        base.UpdateOwnerModels();
        if (!(base.model != null))
        {
            return;
        }
        bool active = this.HaveBuilding((Building)BUILDING.CITY_WALLS);
        foreach (Transform item in base.model.transform)
        {
            if (item.name.StartsWith("Wall"))
            {
                item.gameObject.SetActive(active);
            }
            else if (item.name == "EnchantmentWallOfFire")
            {
                item.gameObject.SetActive(this.HaveWallOfFire());
            }
            else if (item.name == "EnchantmentWallOfDarkness")
            {
                item.gameObject.SetActive(this.HaveWallOfDarkness());
            }
            else if (item.name == "Fortress")
            {
                PlayerWizard wizardOwner = base.GetWizardOwner();
                bool active2 = wizardOwner != null && wizardOwner.wizardTower != null && wizardOwner.wizardTower.Get() == this;
                item.gameObject.SetActive(active2);
            }
        }
    }

    public void InitializeAstralGate()
    {
        if (this.HaveBuilding((Building)BUILDING.ASTRAL_GATE))
        {
            World.GetOtherPlane(base.GetPlane()).AddDecor("AstralGate", base.Position);
        }
    }

    private bool HaveWallOfFire()
    {
        List<EnchantmentInstance> enchantments = base.GetEnchantmentManager().GetEnchantments();
        bool result = false;
        if (enchantments != null)
        {
            result = enchantments.Find((EnchantmentInstance o) => o.source.Get() == (Enchantment)ENCH.WALL_OF_FIRE) != null;
        }
        return result;
    }

    private bool HaveWallOfDarkness()
    {
        List<EnchantmentInstance> enchantments = base.GetEnchantmentManager().GetEnchantments();
        bool result = false;
        if (enchantments != null)
        {
            result = enchantments.Find((EnchantmentInstance o) => o.source.Get() == (Enchantment)ENCH.WALL_OF_DARKNESS) != null;
        }
        return result;
    }

    public string GetReportForLog()
    {
        CraftingItem first = this.craftingQueue.GetFirst();
        return string.Format("Town {0}({10}) Pop: {1}->{2}/{3}/{4} Def: {5}({6}) Prod: {7}({8}/{9})", base.GetPosition(), this.GetPopUnits(), this.GetFarmers(), this.GetWorkers(), this.GetRebels(), base.GetLocalGroup().GetUnits().Count, base.GetLocalGroup().GetValue(), (first != null) ? first.GetDI().graphic : "-", (first != null) ? first.progress.ToString() : "-", (first != null) ? first.requirementValue.ToString() : "-", base.GetPlane().arcanusType);
    }

    public int WaterInClosestVicinity()
    {
        if (this.waterHexInClosesetVicinity < 0)
        {
            this.waterHexInClosesetVicinity = 0;
            foreach (Vector3i item in this.GetSurroundingArea())
            {
                Hex hexAtWrapped = base.GetPlane().GetHexAtWrapped(item);
                if (hexAtWrapped == null || !hexAtWrapped.IsLand())
                {
                    this.waterHexInClosesetVicinity++;
                }
            }
        }
        return this.waterHexInClosesetVicinity;
    }

    public void AutoManageUpdate()
    {
        if (this.IsAnOutpost() || base.owner != PlayerWizard.HumanID() || (this.craftingQueue.craftingItems != null && this.craftingQueue.craftingItems.Count >= 1 && this.craftingQueue.GetFirst().requirementValue > 0))
        {
            return;
        }
        while (this.craftingQueue.craftingItems.Count > 1)
        {
            this.craftingQueue.RemoveItem(this.craftingQueue.GetFirst());
            this.craftingQueue.repeatUnit = false;
        }
        if (this.GetUnits().Count < 9 && this.GetUnits().Count < 1 + this.GetPopUnits() / 5)
        {
            List<global::DBDef.Unit> list = this.PossibleUnits().FindAll((global::DBDef.Unit o) => o.tags == null || (Array.Find(o.tags, (CountedTag k) => k.tag == (Tag)TAG.CONSTRUCTION_UNIT) == null && Array.Find(o.tags, (CountedTag k) => k.tag == (Tag)TAG.SHIP) == null));
            list.Sort((global::DBDef.Unit a, global::DBDef.Unit b) => -a.constructionCost.CompareTo(b.constructionCost));
            if (list.Count > 0)
            {
                this.craftingQueue.AddItem(list[0]);
                return;
            }
        }
        List<Building> list2 = this.PossibleBuildings(atThisMoment: true).FindAll((Building o) => o.buildCost > 0);
        if (list2.Count < 1)
        {
            return;
        }
        bool num = (float)this.GetFarmers() > (float)this.GetPopUnits() * 0.5f;
        bool flag = this.GetPopUnits() < 5;
        if (num)
        {
            list2.FindAll((Building o) => o.tags != null && Array.Find(o.tags, (Tag k) => k == (Tag)TAG.FOOD_PRODUCTION) != null);
            if (list2.Count < 1)
            {
                list2 = this.PossibleBuildings(atThisMoment: true);
            }
        }
        if (flag)
        {
            list2.FindAll((Building o) => o.tags != null && Array.Find(o.tags, (Tag k) => k == (Tag)TAG.FOOD_PRODUCTION || k == (Tag)TAG.INCREASE_PRODUCTION || k == (Tag)TAG.GOLD_PRODUCTION) != null);
            if (list2.Count < 1)
            {
                list2 = this.PossibleBuildings(atThisMoment: true);
            }
        }
        list2.Sort((Building a, Building b) => a.buildCost.CompareTo(b.buildCost));
        if (list2.Count > 0)
        {
            this.craftingQueue.AddItem(list2[0]);
        }
    }

    public void RemoveWizardTowerBonus()
    {
        if (this.GetEnchantments() == null)
        {
            return;
        }
        List<EnchantmentInstance> list = new List<EnchantmentInstance>();
        foreach (EnchantmentInstance enchantment in this.GetEnchantments())
        {
            if (enchantment.source.Get().wizardTowerBonus)
            {
                list.Add(enchantment);
            }
        }
        foreach (EnchantmentInstance item in list)
        {
            this.RemoveEnchantment(item);
        }
    }

    public FInt GetUnitDiscount(global::DBDef.Unit unit)
    {
        PlayerWizard wizardOwner = base.GetWizardOwner();
        FInt zERO = FInt.ZERO;
        if (wizardOwner != null && wizardOwner.seaMasterTrait && unit.GetTag(TAG.SHIP) > FInt.ZERO)
        {
            zERO += (FInt)0.2f;
        }
        return this.unitDiscount + zERO;
    }
}
