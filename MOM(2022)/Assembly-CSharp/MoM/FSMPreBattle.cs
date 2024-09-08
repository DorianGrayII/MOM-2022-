// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.FSMPreBattle
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using HutongGames.PlayMaker;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

[ActionCategory(ActionCategory.GameLogic)]
public class FSMPreBattle : FSMStateBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        base.StartCoroutine(FSMPreBattle.LoadMap(delegate
        {
            base.Finish();
        }));
    }

    public static IEnumerator LoadMap(Action finished)
    {
        Battle battle = Battle.GetBattle();
        if (battle == null)
        {
            Debug.LogError("No battle class defined to start battle mode ");
            yield break;
        }
        Vector2i size = new Vector2i(12, 6);
        PlaneBattle p = new PlaneBattle();
        PlaneSettings settings = default(PlaneSettings);
        global::MOM.Group group = ((battle.gDefender != null) ? battle.gDefender : battle.gAttacker);
        if (group == null)
        {
            Debug.LogError("Battle among two groups without world position! cannot take hex map source for the map generation!");
            yield break;
        }
        global::MOM.Location loc = battle.gDefender?.GetLocationHostSmart();
        string customMap = (loc?.source.Get() as global::DBDef.Location)?.customBattleMap;
        float? customLighting = (loc?.source.Get() as global::DBDef.Location)?.customBattleLighting;
        global::WorldCode.Plane plane = (battle.battleOnArcanus ? World.GetArcanus() : World.GetMyrror());
        Hex hexAt = plane.GetHexAt(group.GetPosition());
        if (customMap != null || !battle.landBattle || hexAt == null || !hexAt.IsLand())
        {
            settings.WaterBattle(size, null);
        }
        else
        {
            settings.LandBattle(size, hexAt);
        }
        p.battlePlane = true;
        World.AddPlane(p);
        yield return p.InitializePlane(plane.planeSource, "battle", global::UnityEngine.Random.Range(int.MinValue, int.MaxValue), size, settings);
        FSMPreBattle.CustomMapSection(battle, p, customMap, customLighting);
        World.ActivatePlane(p);
        battle.plane = p;
        foreach (BattleUnit attackerUnit in battle.attackerUnits)
        {
            attackerUnit.GetOrCreateFormation();
            attackerUnit.CheckIfRaftNeeded();
        }
        foreach (BattleUnit defenderUnit in battle.defenderUnits)
        {
            defenderUnit.GetOrCreateFormation();
            defenderUnit.CheckIfRaftNeeded();
        }
        battle.UpdateInvisibility();
        CameraController.CenterAt(Vector3i.zero);
        if (battle.gDefender?.GetLocationHostSmart() != null)
        {
            if (loc is TownLocation)
            {
                TownLocation townLocation = loc as TownLocation;
                if (townLocation.buildings != null)
                {
                    if (townLocation.buildings.Contains((Building)BUILDING.CITY_WALLS))
                    {
                        battle.AddWalls(null);
                    }
                    if (townLocation.GetWizardOwner() != null && townLocation.GetWizardOwner().wizardTower != null && townLocation.GetWizardOwner().wizardTower.Get() == townLocation)
                    {
                        battle.AddMageTower();
                    }
                }
                FSMPreBattle.AddBuildings(battle, townLocation);
            }
            GameObject gameObject = AssetManager.Get<GameObject>("Battle_" + (loc.source.Get() as IDescriptionInfoType)?.GetDescriptionInfo().graphic);
            if (gameObject != null)
            {
                global::WorldCode.Plane plane2 = battle.plane;
                if (plane2.exclusionPoints == null)
                {
                    plane2.exclusionPoints = new HashSet<Vector3i>();
                }
                Vector3i vector3i = new Vector3i(-10, 10);
                foreach (Vector3i item in HexNeighbors.GetRange(vector3i, 2))
                {
                    battle.plane.exclusionPoints.Add(item);
                }
                Chunk chunkFor = battle.plane.GetChunkFor(vector3i);
                GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
                MHZombieMemoryDetector.Track(gameObject2);
                battle.SetHexModelPosition(gameObject2, vector3i);
            }
        }
        battle.plane.ClearSearcherData();
        finished?.Invoke();
    }

    private static void CustomMapSection(Battle battle, global::WorldCode.Plane p, string customMap, float? customLighting)
    {
        if (customMap == null)
        {
            return;
        }
        GameObject gameObject = AssetManager.Get<GameObject>(customMap);
        if (!(gameObject != null))
        {
            return;
        }
        Chunk chunkFor = p.GetChunkFor(Vector3i.zero);
        GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
        MHZombieMemoryDetector.Track(gameObject2);
        Material material = World.GetWater(p.arcanusType)?.GetComponent<MeshRenderer>()?.material;
        if (material != null)
        {
            material.SetFloat("_UseBorderWave", 0f);
        }
        LightController.SetInstensity(customLighting.HasValue ? customLighting.Value : 0f);
        Texture2D textureData = gameObject2.GetComponent<CustomMapData>().textureData;
        Color32[] pixels = textureData.GetPixels32();
        Vector2i vector2i = Vector2i.zero;
        Vector2i vector2i2 = Vector2i.zero;
        for (int i = 0; i < textureData.width; i++)
        {
            for (int j = 0; j < textureData.height; j++)
            {
                Color32 color = pixels[i + j * textureData.width];
                if (color.g == 0)
                {
                    if (color.r == byte.MaxValue)
                    {
                        vector2i = new Vector2i(i, j);
                    }
                    else if (color.r == 127)
                    {
                        vector2i2 = new Vector2i(i, j);
                    }
                }
            }
        }
        Vector2 vector = new Vector2((float)vector2i.x / (float)textureData.width, (float)vector2i.y / (float)textureData.height);
        Vector2 vector2 = new Vector2((float)vector2i2.x / (float)textureData.width, (float)vector2i2.y / (float)textureData.height) - vector;
        Vector2 vector3 = HexCoordinates.HexToWorld(new Vector3i(0, 0, 0));
        Vector2 vector4 = HexCoordinates.HexToWorld(new Vector3i(-11, 12, -1)) - vector3;
        Vector2 vector5 = vector2 / vector4;
        foreach (KeyValuePair<Vector3i, Hex> hex in p.GetHexes())
        {
            Vector2 vector6 = HexCoordinates.HexToWorld(hex.Key);
            Vector2 vector7 = vector + vector6 * vector5;
            Vector2i vector2i3 = new Vector2i((int)((float)textureData.width * vector7.x), (int)((float)textureData.height * vector7.y));
            Color pixel = textureData.GetPixel(vector2i3.x, vector2i3.y);
            if (pixel.g != 0f)
            {
                Debug.LogError("Custom map data is corrupted, detected green channel at " + hex.Key.ToString());
                break;
            }
            if (pixel.b == 0f)
            {
                if (p.exclusionPoints == null)
                {
                    p.exclusionPoints = new HashSet<Vector3i>();
                }
                p.exclusionPoints.Add(hex.Key);
            }
            else if (pixel.b == 127f)
            {
                hex.Value.customMPCost = 2;
            }
            else if (pixel.b == 255f)
            {
                hex.Value.customMPCost = 1;
            }
        }
        p.ClearSearcherData();
    }

    private static void AddBuildings(Battle battle, TownLocation tl)
    {
        string text = "";
        Race race = tl.race.Get();
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
        List<Vector3i> list = new List<Vector3i>(battle.plane.GetHexes().Keys).FindAll((Vector3i o) => o.x < 0 && o.y > 5 && o.z < 3);
        list.RandomSort();
        int num = 0;
        for (int i = 0; i < tl.GetPopUnits(); i++)
        {
            bool flag = true;
            while (true)
            {
                if (list.Count <= num)
                {
                    flag = false;
                    break;
                }
                HashSet<Vector3i> exclusionPoints = battle.plane.exclusionPoints;
                if (exclusionPoints == null || !exclusionPoints.Contains(list[num]))
                {
                    break;
                }
                num++;
            }
            if (flag)
            {
                string graphic = "House" + (i % 3 + 1) + text;
                FSMPreBattle.AddHouse(battle, graphic, list[num]);
                num++;
                continue;
            }
            break;
        }
    }

    private static void AddHouse(Battle battle, string graphic, Vector3i position)
    {
        Chunk chunkFor = battle.plane.GetChunkFor(position);
        GameObject gameObject = AssetManager.Get<GameObject>(graphic);
        if (gameObject == null)
        {
            Debug.LogError("Model " + graphic + " is missing!");
            return;
        }
        GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
        if (!(gameObject2 == null))
        {
            MHZombieMemoryDetector.Track(gameObject2);
            battle.SetHexModelPosition(gameObject2, position);
            if (battle.houses == null)
            {
                battle.houses = new List<Vector3i>();
            }
            battle.houses.Add(position);
            global::WorldCode.Plane plane = battle.plane;
            if (plane.exclusionPoints == null)
            {
                plane.exclusionPoints = new HashSet<Vector3i>();
            }
            battle.plane.exclusionPoints.Add(position);
        }
    }
}
