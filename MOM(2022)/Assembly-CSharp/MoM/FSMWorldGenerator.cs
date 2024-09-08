// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMWorldGenerator
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using HutongGames.PlayMaker;
using MHUtils;
using MOM;
using MOM.Adventures;
using UnityEngine;
using WorldCode;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMWorldGenerator : FSMStateBase
{
    public int worldGeneratorSeed;

    private bool finished;

    public override void OnEnter()
    {
        if (AdventureLibrary.currentLibrary != null)
        {
            AdventureLibrary.currentLibrary.AdventureLocalization();
        }
        Debug.Log("FSMWorldGenerator");
        this.finished = false;
        base.OnEnter();
        TerrainTextures.GetInstance(forceLoad: false);
        base.StartCoroutine(this.WorldBuilder());
    }

    private IEnumerator WorldBuilder()
    {
        if (World.Get().seed != 0)
        {
            Debug.Log("Loaded Seed: " + World.Get().seed);
            global::UnityEngine.Random.InitState(World.Get().seed);
        }
        else if (this.worldGeneratorSeed != 0)
        {
            Debug.Log("Fixed Seed: " + this.worldGeneratorSeed);
            global::UnityEngine.Random.InitState(this.worldGeneratorSeed);
            World.Get().seed = this.worldGeneratorSeed;
            World.Get().gameID = MHRandom.Get().GetIntMinMax();
        }
        else
        {
            int @int = new MHRandom(new global::System.Random()).GetInt(-2147483647, int.MaxValue);
            Debug.Log("Seed: " + @int);
            global::UnityEngine.Random.InitState(@int);
            World.Get().seed = @int;
            World.Get().gameID = MHRandom.Get().GetIntMinMax();
        }
        Settings.SetGameQualitySettings();
        Settings.GetMeshQuality(force: true);
        new Vector2i(40, 26);
        int num = ((EntityManager.GetEntityOfType<global::MOM.Location>() != null) ? World.Get().worldSizeSetting : DifficultySettingsData.GetSettingAsInt("UI_WORLD_SIZE"));
        Vector2i size;
        switch (num)
        {
        case 0:
            size = new Vector2i(48, 32);
            break;
        case 1:
            size = new Vector2i(40, 26);
            break;
        case 2:
            size = new Vector2i(32, 22);
            break;
        case 3:
            size = new Vector2i(30, 20);
            break;
        default:
            size = new Vector2i(32, 22);
            break;
        }
        World.Get().worldSizeSetting = num;
        MHRandom.seedSource = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        int planeSeed = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        MinimapManager.Get().RegisterEvents();
        global::WorldCode.Plane p = new global::WorldCode.Plane();
        PlaneSettings ps2 = default(PlaneSettings);
        ps2.NormalWorld(size);
        p.battlePlane = false;
        World.AddPlane(p);
        while (!Loading.ready)
        {
            yield return null;
        }
        bool useProto = false;
        ProtoLibrary instance = ProtoLibrary.GetInstance();
        if (World.Get().gameID == 0)
        {
            ProtoLibrary.Clear();
        }
        else if (instance != null && instance.mapResolution == Settings.GetMeshQuality() && instance.gameID == World.Get().gameID)
        {
            useProto = true;
        }
        else
        {
            yield return SaveManager.LoadCache();
            if (ProtoLibrary.GetInstance() != null)
            {
                useProto = true;
            }
        }
        if (useProto)
        {
            yield return p.RecreatePlane((global::DBDef.Plane)PLANE.ARCANUS, "Arcanus", planeSeed, size, ps2);
        }
        else
        {
            yield return p.InitializePlane((global::DBDef.Plane)PLANE.ARCANUS, "Arcanus", planeSeed, size, ps2);
        }
        MinimapManager.Get().TerrainDirty(arcanus: true);
        p.GetIslands();
        global::WorldCode.Plane p2 = new global::WorldCode.Plane();
        ps2 = default(PlaneSettings);
        ps2.NormalWorld(size);
        p2.battlePlane = false;
        World.AddPlane(p2);
        if (useProto)
        {
            yield return p2.RecreatePlane((global::DBDef.Plane)PLANE.MYRROR, "Myrror", planeSeed + 1, size, ps2);
        }
        else
        {
            yield return p2.InitializePlane((global::DBDef.Plane)PLANE.MYRROR, "Myrror", planeSeed + 1, size, ps2);
        }
        MinimapManager.Get().TerrainDirty(arcanus: false);
        p2.GetIslands();
        if (!useProto && ProtoLibrary.GetInstance() != null)
        {
            instance = ProtoLibrary.GetInstance();
            instance.arcanus = p.meshData;
            instance.myrror = p2.meshData;
            instance.mapResolution = p.meshQuality;
            ProtoLibrary.MeshIteration();
        }
        if (EntityManager.GetEntityOfType<global::MOM.Location>() == null)
        {
            FOW.Get().ResetMap(p.pathfindingArea.AreaWidth, p.pathfindingArea.AreaHeight, focusArcanus: true);
            List<Vector3i> connectingTowers = this.InitializeConnectingTowers(p, p2);
            MHTimer.StartNew();
            this.InitializeLocation(p, connectingTowers);
            MHTimer.StartNew();
            this.InitializeLocation(p2, connectingTowers);
            List<global::MOM.Location> locationsOfWizard = GameManager.GetLocationsOfWizard(PlayerWizard.HumanID());
            if (locationsOfWizard != null && locationsOfWizard.Count > 0)
            {
                World.ActivatePlane(locationsOfWizard[0].GetPlane());
            }
        }
        else
        {
            GameManager.Get().PostLoad();
            FOW.Get().ResetMap(p.pathfindingArea.AreaWidth, p.pathfindingArea.AreaHeight, focusArcanus: true, fullReset: false);
        }
        p.ClearSearcherData();
        p2.ClearSearcherData();
        this.finished = true;
        MHEventSystem.TriggerEvent<FSMWorldGenerator>(this, null);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (this.finished)
        {
            base.Fsm.Event("FINISHED");
        }
    }

    public List<Vector3i> InitializeConnectingTowers(global::WorldCode.Plane a, global::WorldCode.Plane b)
    {
        List<Vector3i> list = new List<Vector3i>(a.GetLandHexes());
        HashSet<Vector3i> landB = b.GetLandHexes();
        List<Vector3i> list2 = list.FindAll((Vector3i o) => landB.Contains(o));
        if (list2.Count < 7)
        {
            Debug.LogError("Produced world had not enough overlapping area! Start new world / report to MuHa Games");
            return new List<Vector3i>();
        }
        list2.RandomSort();
        List<Vector3i> list3 = new List<Vector3i>();
        int num = 0;
        int index = 0;
        for (int i = 0; i < list2.Count; i++)
        {
            Vector3i vector3i = list2[i];
            if (list3.Contains(vector3i))
            {
                continue;
            }
            int num2 = int.MaxValue;
            for (int j = 0; j < list3.Count; j++)
            {
                int distanceWrapping = a.GetDistanceWrapping(vector3i, list3[j]);
                if (num2 > distanceWrapping)
                {
                    num2 = distanceWrapping;
                }
            }
            if (num >= num2)
            {
                continue;
            }
            list3.Add(vector3i);
            if (list3.Count < 6)
            {
                num = 0;
                index = 0;
                continue;
            }
            if (list3.Count > 6)
            {
                list3.RemoveAt(index);
            }
            num = int.MaxValue;
            index = 0;
            for (int k = 0; k < list3.Count - 1; k++)
            {
                for (int l = k + 1; l < list3.Count; l++)
                {
                    int distanceWrapping2 = a.GetDistanceWrapping(list3[k], list3[l]);
                    if (num > distanceWrapping2)
                    {
                        num = distanceWrapping2;
                        index = l;
                    }
                }
            }
        }
        return list3;
    }

    public void InitializeLocation(global::WorldCode.Plane p, List<Vector3i> connectingTowers)
    {
        List<List<Vector3i>> list = p.GetIslands().FindAll((List<Vector3i> o) => o.Count > 120);
        List<Vector3i> list2 = new List<Vector3i>();
        List<Vector3i> list3 = new List<Vector3i>();
        int num = 0;
        int townDistance = DifficultySettingsData.GetTownDistance();
        List<PlayerWizard> wizards = GameManager.GetWizards();
        int count = wizards.Count;
        HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
        for (int k = 0; k < 10; k++)
        {
            num = 0;
            list2.Clear();
            list3.Clear();
            foreach (List<Vector3i> item in list)
            {
                int num2 = (item.Count + 20) / 80;
                int num3 = 0;
                int num4 = num2 * 2;
                int num5 = 80;
                List<Vector3i> list4 = new List<Vector3i>();
                while (num4 > 0 && num5 > 0)
                {
                    if (num3 >= num2)
                    {
                        num4--;
                    }
                    else
                    {
                        num5--;
                    }
                    int index = global::UnityEngine.Random.Range(0, item.Count);
                    Vector3i j = item[index];
                    Hex hexAt = p.GetHexAt(j);
                    if (!hexAt.HaveFlag(ETerrainType.Mountain) && hexAt.SeaAround(p) <= 1)
                    {
                        int num6 = connectingTowers.FindIndex((Vector3i o) => HexCoordinates.HexDistance(j, o) <= 2);
                        int num7 = list2.FindIndex((Vector3i o) => HexCoordinates.HexDistance(j, o) <= townDistance + 1);
                        if (num6 < 0 && num7 < 0 && p.GetFoodInArea(j) >= 6)
                        {
                            list2.Add(j);
                            list4.Add(j);
                            num3++;
                        }
                    }
                }
                num5 = 100;
                num4 = num2 * 10;
                num3 = 0;
                while (num4 > 0 && num5 > 0)
                {
                    if (num3 >= num2)
                    {
                        num4--;
                    }
                    else
                    {
                        num5--;
                    }
                    int index2 = global::UnityEngine.Random.Range(0, item.Count);
                    Vector3i i = item[index2];
                    if (!p.GetHexAt(i).HaveFlag(ETerrainType.Mountain))
                    {
                        int num8 = connectingTowers.FindIndex((Vector3i o) => HexCoordinates.HexDistance(i, o) <= 1);
                        int num9 = list4.FindIndex((Vector3i o) => HexCoordinates.HexDistance(i, o) <= 1);
                        if (num8 < 0 && num9 < 0)
                        {
                            list3.Add(i);
                            list4.Add(i);
                            num3++;
                        }
                    }
                }
            }
            Dictionary<Vector3i, int> values = new Dictionary<Vector3i, int>();
            foreach (Vector3i item2 in list2)
            {
                values[item2] = p.GetFoodInArea(item2);
            }
            list2.SortInPlace((Vector3i a, Vector3i b) => -values[a].CompareTo(values[b]));
            int num10 = 5;
            int num11 = 12;
            for (int l = 0; l < list2.Count; l++)
            {
                Vector3i key = list2[l];
                if (values[key] < num11)
                {
                    break;
                }
                num++;
            }
            if (num >= num10)
            {
                break;
            }
        }
        List<global::DBDef.Location> type = DataBase.GetType<global::DBDef.Location>();
        type = type.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.PlaneTower);
        global::DBDef.Location source;
        for (int m = 0; m < connectingTowers.Count; m++)
        {
            source = type[m % type.Count];
            global::MOM.Location.CreateLocation(connectingTowers[m], p, source, 0);
        }
        List<Vector3i> selectedLocations = new List<Vector3i>();
        List<Vector3i> range = list2.GetRange(0, num);
        range.RandomSort();
        List<Vector3i> range2 = range.GetRange(0, count);
        for (int n = count; n < range.Count; n++)
        {
            int num12 = int.MaxValue;
            int num13 = int.MaxValue;
            int num14 = int.MaxValue;
            Vector3i vector3i = range[n];
            Vector3i vector3i2 = Vector3i.invalid;
            Vector3i vector3i3 = Vector3i.invalid;
            for (int num15 = 0; num15 < count; num15++)
            {
                for (int num16 = 0; num16 < count; num16++)
                {
                    if (num15 != num16)
                    {
                        int distanceWrapping = p.GetDistanceWrapping(range2[num15], range2[num16]);
                        if (distanceWrapping < num14)
                        {
                            num14 = distanceWrapping;
                            vector3i2 = range2[num16];
                            vector3i3 = range2[num15];
                        }
                    }
                }
            }
            for (int num17 = 0; num17 < count; num17++)
            {
                int distanceWrapping2 = p.GetDistanceWrapping(range2[num17], vector3i);
                if (vector3i2 != range2[num17] && distanceWrapping2 < num12)
                {
                    num12 = distanceWrapping2;
                }
                if (vector3i3 != range2[num17] && distanceWrapping2 < num13)
                {
                    num13 = distanceWrapping2;
                }
            }
            if (num12 >= num13 && num12 > num14)
            {
                range2.Remove(vector3i2);
                range2.Add(vector3i);
            }
            else if (num13 > num12 && num13 > num14)
            {
                range2.Remove(vector3i3);
                range2.Add(vector3i);
            }
        }
        selectedLocations = range2;
        list2.RemoveAll((Vector3i o) => selectedLocations.Contains(o));
        List<Town> type2 = DataBase.GetType<Town>();
        bool flag = p.planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS;
        for (int num18 = 0; num18 < wizards.Count; num18++)
        {
            Vector3i position = selectedLocations[num18];
            PlayerWizard playerWizard = wizards[num18];
            Race race = playerWizard.mainRace.Get();
            if (playerWizard.HasTrait((Trait)TRAIT.MYRRAN) == flag || (playerWizard.HasTrait((Trait)TRAIT.MYRRAN_REFUGEE) && !flag))
            {
                continue;
            }
            Town source2 = type2.Find((Town o) => o.race == race);
            TownLocation townLocation = TownLocation.CreateLocation(position, p, source2, 4, playerWizard.ID);
            townLocation.AddBuilding((Building)BUILDING.BARRACKS);
            townLocation.AddBuilding((Building)BUILDING.SMITHY);
            townLocation.AddBuilding((Building)BUILDING.BUILDERS_HALL);
            if (playerWizard != null && playerWizard.townExtraBuilding != null && playerWizard.townExtraBuilding.Count > 0)
            {
                foreach (KeyValuePair<string, string> item3 in playerWizard.townExtraBuilding)
                {
                    DBClass dBClass = DataBase.Get(item3.Key, reportMissing: false);
                    if (dBClass != null && (bool)ScriptLibrary.Call(item3.Value, townLocation, (Building)dBClass, playerWizard))
                    {
                        townLocation.AddBuilding((Building)dBClass);
                    }
                }
            }
            DifficultyOption setting = DifficultySettingsData.GetSetting("UI_INITIAL_ECONOMY");
            if (setting.value == "1")
            {
                townLocation.AddBuilding((Building)BUILDING.SHRINE);
            }
            if (setting.value == "1" || setting.value == "2")
            {
                townLocation.AddBuilding((Building)BUILDING.GRANARY);
            }
            global::MOM.Group localGroup = townLocation.GetLocalGroup();
            object obj = ((playerWizard == null || string.IsNullOrEmpty(playerWizard.startingUnit)) ? ScriptLibrary.Call("StartingGroup", race, playerWizard) : ScriptLibrary.Call("StartingGroupWithExtraUnit", race, playerWizard, playerWizard.startingUnit));
            if (obj == null)
            {
                Debug.LogWarning("Location " + townLocation.source?.ToString() + " could not produce Starting group correctly");
            }
            else
            {
                foreach (Subrace item4 in obj as List<Subrace>)
                {
                    localGroup.AddUnit(global::MOM.Unit.CreateFrom(item4));
                }
                localGroup.Position = position;
                localGroup.GetMapFormation();
                foreach (Reference<global::MOM.Unit> unit in localGroup.GetUnits())
                {
                    playerWizard?.ModifyUnitSkillsByTraits(unit.Get());
                }
            }
            playerWizard.SetSummoningLocation(townLocation);
            playerWizard.SetTowerLocation(townLocation);
            playerWizard.AddTraitBaseEnchantmentsToNewBuildedTowns(townLocation);
        }
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_NUMBER_OF_NEUTRAL_TOWNS");
        if (settingAsInt < 4)
        {
            int num19 = settingAsInt - 1;
            list2.RandomSort();
            foreach (Vector3i item5 in list2)
            {
                num19++;
                if (num19 < settingAsInt)
                {
                    list3.Add(item5);
                    continue;
                }
                num19 = 0;
                List<Town> list5 = new List<Town>();
                list5 = ((!p.arcanusType) ? type2.FindAll((Town o) => !o.race.arcanusRace) : type2.FindAll((Town o) => o.race.arcanusRace));
                int index3 = global::UnityEngine.Random.Range(0, list5.Count);
                Town town = list5[index3];
                TownLocation townLocation2 = TownLocation.CreateLocation(item5, p, town, 4, 0);
                int curentScoreMultiplierCached = DifficultySettingsData.GetCurentScoreMultiplierCached();
                MHRandom mHRandom = new MHRandom();
                if (curentScoreMultiplierCached <= 50)
                {
                    townLocation2.Population = mHRandom.GetInt(1000, 4001);
                }
                else if (curentScoreMultiplierCached > 50 && curentScoreMultiplierCached <= 100)
                {
                    townLocation2.Population = mHRandom.GetInt(2000, 5001);
                }
                else if (mHRandom.GetFloat(0f, 1f) <= 0.2f)
                {
                    townLocation2.Population = mHRandom.GetInt(2000, 11000);
                }
                else
                {
                    townLocation2.Population = mHRandom.GetInt(2000, 5001);
                }
                FInt foodInArea = townLocation2.GetFoodInArea();
                if (townLocation2.Population > foodInArea * 1000)
                {
                    townLocation2.Population = foodInArea.ToInt() * 1000;
                }
                if (townLocation2.GetPopUnits() >= 2)
                {
                    townLocation2.AddBuilding((Building)BUILDING.BARRACKS);
                }
                if (townLocation2.GetPopUnits() >= 3)
                {
                    townLocation2.AddBuilding((Building)BUILDING.SMITHY);
                }
                if (townLocation2.GetPopUnits() >= 4)
                {
                    townLocation2.AddBuilding((Building)BUILDING.BUILDERS_HALL);
                }
                if (townLocation2.GetPopUnits() >= 5)
                {
                    townLocation2.AddBuilding((Building)BUILDING.ARMORY);
                }
                if (townLocation2.GetPopUnits() >= 7)
                {
                    townLocation2.AddBuilding((Building)BUILDING.GRANARY);
                }
                if (townLocation2.GetPopUnits() >= 8)
                {
                    if (townLocation2.race.Get() == (Race)RACE.HALFLINGS || townLocation2.race.Get() == (Race)RACE.DWARVES)
                    {
                        townLocation2.AddBuilding((Building)BUILDING.SHRINE);
                    }
                    else
                    {
                        townLocation2.AddBuilding((Building)BUILDING.SHRINE);
                    }
                }
                if (townLocation2.GetPopUnits() >= 9)
                {
                    townLocation2.AddBuilding((Building)BUILDING.CITY_WALLS);
                }
                global::MOM.Group localGroup2 = townLocation2.GetLocalGroup();
                foreach (Subrace item6 in (List<Subrace>)ScriptLibrary.Call("NeutralCityDefenders", town.race, townLocation2.GetPopUnits()))
                {
                    localGroup2.AddUnit(global::MOM.Unit.CreateFrom(item6));
                }
                localGroup2.Position = item5;
                localGroup2.GetMapFormation();
            }
        }
        int num20 = 0;
        int num21 = 0;
        int num22 = 0;
        GameplayConfiguration gameplayConfiguration = DataBase.Get<GameplayConfiguration>(GAMEPLAY_CONFIGURATION.DEFAULT);
        if (gameplayConfiguration != null && gameplayConfiguration.option != null)
        {
            if (p.planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS)
            {
                Gc gc = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Arcanus Chaos");
                if (gc != null)
                {
                    try
                    {
                        num20 = Convert.ToInt32(gc.setting);
                    }
                    catch
                    {
                        Debug.Log("Arcanus Chaos configuration contains invalid type, expected int");
                    }
                }
                gc = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Arcanus Nature");
                if (gc != null)
                {
                    try
                    {
                        num21 = Convert.ToInt32(gc.setting);
                    }
                    catch
                    {
                        Debug.Log("Arcanus Nature configuration contains invalid type, expected int");
                    }
                }
                gc = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Arcanus Sorcery");
                if (gc != null)
                {
                    try
                    {
                        num22 = Convert.ToInt32(gc.setting);
                    }
                    catch
                    {
                        Debug.Log("Arcanus Sorcery configuration contains invalid type, expected int");
                    }
                }
            }
            else
            {
                Gc gc2 = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Myrror Chaos");
                if (gc2 != null)
                {
                    try
                    {
                        num20 = Convert.ToInt32(gc2.setting);
                    }
                    catch
                    {
                        Debug.Log("Myrror Chaos configuration contains invalid type, expected int");
                    }
                }
                gc2 = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Myrror Nature");
                if (gc2 != null)
                {
                    try
                    {
                        num21 = Convert.ToInt32(gc2.setting);
                    }
                    catch
                    {
                        Debug.Log("Myrror Nature configuration contains invalid type, expected int");
                    }
                }
                gc2 = Array.Find(gameplayConfiguration.option, (Gc o) => o.name == "Myrror Sorcery");
                if (gc2 != null)
                {
                    try
                    {
                        num22 = Convert.ToInt32(gc2.setting);
                    }
                    catch
                    {
                        Debug.Log("Myrror Sorcery configuration contains invalid type, expected int");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("GameplayConfiguration is null or empty");
        }
        list3.RandomSort();
        list3 = new List<Vector3i>(list3);
        int num23 = 0;
        source = (MagicNode)(Enum)MAGIC_NODE.CHAOS;
        for (int num24 = 0; num24 < num20; num24++)
        {
            if (list3.Count <= num23)
            {
                Debug.LogError("Not enough minor locations for Magic nodes (Chaos " + num24 + ")");
                continue;
            }
            global::MOM.Location.CreateLocation(list3[num23], p, source, 0);
            hashSet.Add(list3[num23]);
            num23++;
        }
        source = (MagicNode)(Enum)MAGIC_NODE.NATURE;
        for (int num25 = 0; num25 < num21; num25++)
        {
            if (list3.Count <= num23)
            {
                Debug.LogError("Not enough minor locations for Magic nodes (Nature " + num25 + ")");
                continue;
            }
            global::MOM.Location.CreateLocation(list3[num23], p, source, 0);
            hashSet.Add(list3[num23]);
            num23++;
        }
        source = (MagicNode)(Enum)MAGIC_NODE.SORCERY;
        for (int num26 = 0; num26 < num22; num26++)
        {
            if (list3.Count <= num23)
            {
                Debug.LogError("Not enough minor locations for Magic nodes (Sorcery " + num26 + ")");
                continue;
            }
            global::MOM.Location.CreateLocation(list3[num23], p, source, 0);
            hashSet.Add(list3[num23]);
            num23++;
        }
        List<global::DBDef.Location> type3 = DataBase.GetType<global::DBDef.Location>();
        List<global::DBDef.Location> list6 = type3.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.WeakLair);
        type3 = type3.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.Lair);
        if (type3.Count < 1)
        {
            return;
        }
        int num27 = 0;
        int num28 = list3.Count;
        float settingAsFloat = DifficultySettingsData.GetSettingAsFloat("UI_LAIR_NUMBER_MULTIPLIER");
        if (settingAsFloat != 0f && settingAsFloat != 1f)
        {
            num28 = (int)((float)num28 * settingAsFloat);
        }
        for (int num29 = num23; num29 < num28; num29++)
        {
            if (num27 <= list3.Count / 2)
            {
                int index4 = global::UnityEngine.Random.Range(0, list6.Count);
                global::MOM.Location.CreateLocation(list3[num29], p, list6[index4], 0);
                num27++;
            }
            else
            {
                int index5 = global::UnityEngine.Random.Range(0, type3.Count);
                global::MOM.Location.CreateLocation(list3[num29], p, type3[index5], 0);
            }
        }
        if (settingAsFloat != -1f)
        {
            ScriptLibrary.Call("FSA_SpawnWeakLocations", p);
            ScriptLibrary.Call("FSA_SpawnStrongLocations", p);
        }
        p.RebuildUpdatedTerrains(hashSet);
        this.SpawnWaterUniqueLocations(p);
        this.SpawnWaterLocations(p, ELocationType.WaterLair, 1);
        this.SpawnWaterLocations(p, ELocationType.WeakWaterLair, 2);
        this.SpawnWaterLocations(p, ELocationType.StrongWaterLair, 2);
    }

    private void SpawnWaterLocations(global::WorldCode.Plane p, ELocationType lType, int lairReduction)
    {
        List<Vector3i> list = new List<Vector3i>(p.waterBodies.Keys);
        list.RandomSort();
        int num = list.Count / (lairReduction * 100);
        float settingAsFloat = DifficultySettingsData.GetSettingAsFloat("UI_WATER_LAIR_NUMBER_MULTIPLIER");
        if (settingAsFloat != 0f && settingAsFloat != 1f)
        {
            num = (int)((float)num * settingAsFloat);
        }
        List<global::DBDef.Location> list2 = DataBase.GetType<global::DBDef.Location>().FindAll((global::DBDef.Location o) => o.locationType == lType);
        if (list2.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            Vector3i hex = list[i];
            if (p.area.DistanceToBorder(hex) < 3 || p.GetWaterBodyFor(hex) == null || p.GetWaterBodyFor(hex).Count < 10)
            {
                continue;
            }
            bool flag = true;
            Vector3i[] neighbours = HexNeighbors.neighbours;
            foreach (Vector3i vector3i in neighbours)
            {
                if (p.IsLand(hex + vector3i))
                {
                    flag = false;
                }
            }
            if (!flag || GameManager.GetLocationsOfThePlane(p).Find((global::MOM.Location l) => l.GetDistanceTo(hex) <= 1) != null)
            {
                continue;
            }
            global::DBDef.Location location = list2[global::UnityEngine.Random.Range(0, list2.Count)];
            if (location != null)
            {
                global::MOM.Location.CreateLocation(hex, p, location, 0);
                num--;
                if (num <= 0)
                {
                    break;
                }
            }
        }
    }

    private void SpawnWaterUniqueLocations(global::WorldCode.Plane p)
    {
        List<Vector3i> list = new List<Vector3i>(p.waterBodies.Keys);
        list.RandomSort();
        if (p.arcanusType)
        {
            List<global::DBDef.Location> locWaterLairs = DataBase.GetType<global::DBDef.Location>().FindAll((global::DBDef.Location o) => o.locationType == ELocationType.WaterArcanusUnique);
            this.SpawnWaterLocationsOnce(p, locWaterLairs, list);
        }
        else
        {
            List<global::DBDef.Location> locWaterLairs2 = DataBase.GetType<global::DBDef.Location>().FindAll((global::DBDef.Location o) => o.locationType == ELocationType.WaterMyrrorUnique);
            this.SpawnWaterLocationsOnce(p, locWaterLairs2, list);
        }
    }

    private void SpawnWaterLocationsOnce(global::WorldCode.Plane p, List<global::DBDef.Location> locWaterLairs, List<Vector3i> waterHexes)
    {
        for (int i = 0; i < locWaterLairs.Count; i++)
        {
            for (int j = 0; j < waterHexes.Count; j++)
            {
                Vector3i hex = waterHexes[j];
                if (p.area.DistanceToBorder(hex) < 3 || p.GetWaterBodyFor(hex) == null || p.GetWaterBodyFor(hex).Count < 10)
                {
                    continue;
                }
                bool flag = true;
                Vector3i[] neighbours = HexNeighbors.neighbours;
                foreach (Vector3i vector3i in neighbours)
                {
                    if (p.IsLand(hex + vector3i))
                    {
                        flag = false;
                    }
                }
                if (flag && GameManager.GetLocationsOfThePlane(p).Find((global::MOM.Location l) => l.GetDistanceTo(hex) <= 1) == null)
                {
                    global::DBDef.Location location = locWaterLairs[global::UnityEngine.Random.Range(0, locWaterLairs.Count)];
                    if (location != null)
                    {
                        global::MOM.Location.CreateLocation(hex, p, location, 0);
                        break;
                    }
                }
            }
        }
    }
}
