// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AINeutralManager
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

public class AINeutralManager
{
    private const int RAMPAGING_MIDGAME_NORMAL = 6000;

    private const int RAMPAGING_MIDGAME_HARD = 10000;

    public static float CreateMidgameThreat(MHRandom rand, float acc)
    {
        List<global::MOM.Location> list = GameManager.Get().registeredLocations.FindAll((global::MOM.Location o) => o.locationType == ELocationType.MidGameLair);
        float num = list.Count;
        if (num == 0f)
        {
            return 0f;
        }
        float num2 = GameManager.Get().registeredGroups.FindAll((global::MOM.Group o) => o.midGameCrisis).Count;
        acc = acc + num * 0.4f - num2;
        acc = Mathf.Max(0f, acc);
        if (acc > 1.5f && rand.GetFloat(0f, 1f) * 10f < acc)
        {
            global::MOM.Location location = list[rand.GetInt(0, list.Count)];
            Vector3i position = location.GetPosition();
            global::WorldCode.Plane plane = location.GetPlane();
            bool flag = false;
            Vector3i vector3i = Vector3i.zero;
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i vector3i2 = HexNeighbors.neighbours[i];
                Vector3i p = position + vector3i2;
                if (GameManager.Get().registeredGroups.Find((global::MOM.Group o) => o.GetPosition() == p && o.GetPlane() == plane) == null && (!flag || !(rand.GetFloat(0f, 1f) < 0.66f)))
                {
                    flag = true;
                    vector3i = p;
                }
            }
            if (!flag)
            {
                return acc;
            }
            global::DBDef.Location location2 = location.source.Get() as global::DBDef.Location;
            global::MOM.Group group = null;
            if (!string.IsNullOrEmpty(location2?.rampagingScript))
            {
                int num3 = 6000;
                DifficultyOption setting = DifficultySettingsData.GetSetting("UI_MID_GAME_AWAKE");
                if (setting.value == "1" || setting.value == "2")
                {
                    num3 = 6000;
                }
                else if (setting.value == "3" || setting.value == "4")
                {
                    num3 = 10000;
                }
                group = ScriptLibrary.Call(location2.rampagingScript, vector3i, plane, num3) as global::MOM.Group;
                group.Position = vector3i;
                if (group.IsModelVisible())
                {
                    group.GetMapFormation();
                }
            }
            if (group == null)
            {
                return acc;
            }
            group.midGameCrisis = true;
            foreach (Reference<global::MOM.Unit> unit in group.GetUnits())
            {
                if (!unit.Get().CanTravelOverWater())
                {
                    unit.Get().attributes.AddToBase(TAG.CAN_SWIM, FInt.ONE);
                    unit.Get().attributes.SetDirty();
                }
            }
            acc = Mathf.Max(0f, acc - 5f);
        }
        return acc;
    }

    public static IEnumerator UpdateMidGameGroups(int targetPlayerA, int targetPlayerB)
    {
        List<global::MOM.Group> list = GameManager.Get().registeredGroups.FindAll((global::MOM.Group o) => o.midGameCrisis);
        if (targetPlayerA == 0)
        {
            targetPlayerA = PlayerWizard.HumanID();
        }
        if (targetPlayerB == 0)
        {
            targetPlayerB = PlayerWizard.HumanID();
        }
        foreach (global::MOM.Group item in list)
        {
            if (item.GetUnits().Count == 0)
            {
                item.midGameCrisis = false;
                continue;
            }
            int num = 1000;
            global::MOM.Group group = null;
            int num2 = 1000;
            global::MOM.Group group2 = null;
            foreach (global::MOM.Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetPlane() != item.GetPlane() || (registeredGroup.locationHost != null && !(registeredGroup.locationHost.Get() is TownLocation) && registeredGroup.GetUnits().Count == 0))
                {
                    continue;
                }
                int distanceTo = registeredGroup.GetDistanceTo(item.GetPosition());
                if (registeredGroup.GetOwnerID() == targetPlayerA || registeredGroup.GetOwnerID() == targetPlayerB)
                {
                    if (distanceTo < num)
                    {
                        num = distanceTo;
                        group = registeredGroup;
                    }
                }
                else if (registeredGroup.GetOwnerID() > 0 && distanceTo < num2)
                {
                    num2 = distanceTo;
                    group2 = registeredGroup;
                }
            }
            if (group == null)
            {
                group = group2;
            }
            if (item != null && group != null)
            {
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(item.GetPlane(), item.GetPosition(), group.GetPosition(), item);
                PathfinderV2.FindPath(requestDataV);
                List<Vector3i> path = requestDataV.GetPath();
                if (path != null && path.Count >= 2)
                {
                    item.MoveViaPath(path, mergeCollidedAlliedGroups: false);
                    yield return AINeutralManager.TrackMovement(item);
                }
            }
        }
    }

    public static int ResolveRuins(MHRandom random, int acc, float scale, int treshold, int increase)
    {
        if (TurnManager.GetTurnNumber() < 20)
        {
            return acc;
        }
        int num = acc - treshold;
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_NEUTRAL_ARMIES");
        float num2 = (float)(settingAsInt * 200) + (float)(TurnManager.GetTurnNumber() * 2 + num * 15) * scale;
        List<global::MOM.Location> list = GameManager.Get().registeredLocations.FindAll((global::MOM.Location o) => !(o is TownLocation) && o.GetOwnerID() <= 0 && o.GetUnits().Count > 0 && o.locationType != ELocationType.MidGameLair);
        if (list != null && list.Count > 0)
        {
            global::WorldCode.Plane plane = GameManager.GetHumanWizard().wizardTower?.Get().GetPlane();
            int @int = random.GetInt(0, list.Count);
            global::MOM.Location location = list[@int];
            int num3 = 1 + settingAsInt;
            for (int i = 0; i < num3; i++)
            {
                int num4 = 0;
                int num5 = 0;
                @int = random.GetInt(0, list.Count);
                global::MOM.Location location2 = list[@int];
                if (location.GetPlane() == plane)
                {
                    num4++;
                }
                if (location2.GetPlane() == plane)
                {
                    num5++;
                }
                global::MOM.Location location3 = GameManager.Get().registeredLocations.Find((global::MOM.Location o) => o.GetPlane() == location.GetPlane() && o.GetOwnerID() == PlayerWizard.HumanID() && o.GetPlane().GetDistanceWrapping(o.GetPosition(), location.GetPosition()) < 10);
                global::MOM.Location location4 = GameManager.Get().registeredLocations.Find((global::MOM.Location o) => o.GetPlane() == location2.GetPlane() && o.GetOwnerID() == PlayerWizard.HumanID() && o.GetPlane().GetDistanceWrapping(o.GetPosition(), location2.GetPosition()) < 10);
                if (location3 != null)
                {
                    num4++;
                }
                if (location4 != null)
                {
                    num5++;
                }
                if (num4 < num5)
                {
                    location = location2;
                }
            }
            if (location != null)
            {
                global::DBDef.Location location5 = location.source.Get() as global::DBDef.Location;
                if (location5.rampagingScript == null)
                {
                    return acc;
                }
                if (!location5.rampagingScript.StartsWith("RAM"))
                {
                    Debug.LogWarning("Rampaging monsters are using methods starting 'RAM'. This is temporary lock to ensure crashing scripts are not in use for this process");
                    return acc;
                }
                List<Vector3i> list2 = null;
                if (location.GetPlane().GetHexAt(location.GetPosition()).IsLand())
                {
                    RequestDataV2 requestDataV = RequestDataV2.CreateRequest(location.GetPlane(), location.GetPosition(), new FInt(15), null);
                    PathfinderV2.FindArea(requestDataV);
                    list2 = requestDataV.GetArea();
                }
                else
                {
                    list2 = HexNeighbors.GetRange(location.GetPosition(), 15);
                }
                List<global::MOM.Location> list3 = GameManager.GetLocationsOfThePlane(location.GetPlane().arcanusType).FindAll((global::MOM.Location o) => o is TownLocation);
                bool flag = false;
                global::MOM.Location location6 = null;
                foreach (Vector3i v in list2)
                {
                    global::MOM.Location location7 = list3.Find((global::MOM.Location o) => o.GetPosition() == v);
                    if (location7 != null && location7.GetOwnerID() > 0)
                    {
                        flag = true;
                        if (location6 == null || ((location6.GetOwnerID() != PlayerWizard.HumanID() || location7.GetOwnerID() == PlayerWizard.HumanID()) && location6.GetLocalGroup().GetValue() > location7.GetLocalGroup().GetValue()))
                        {
                            location6 = location7;
                        }
                    }
                }
                if (!flag)
                {
                    return acc;
                }
                global::MOM.Group group = (global::MOM.Group)ScriptLibrary.Call(location5.rampagingScript, location.GetPosition(), location.GetPlane(), (int)num2);
                if (group.GetUnits().Count < 1)
                {
                    Debug.Log("FAILED Attempt to spawn with " + num2 + ", plane " + location.GetPlane().arcanusType + ", " + location.GetPosition().ToString());
                    group.Destroy();
                    return acc + random.GetInt(1, increase);
                }
                Debug.Log("SUCCESS Attempt to spawn with " + num2 + ", plane " + location.GetPlane().arcanusType + ", " + location.GetPosition().ToString());
                group.Position = location.GetPosition();
                group.aiNeturalExpedition = new AINeutralExpedition(group, null, location6);
                group.NewTurn();
                return acc - treshold;
            }
        }
        return acc;
    }

    public static IEnumerator ResolveTowns()
    {
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        foreach (global::MOM.Location v in GameManager.Get().registeredLocations)
        {
            if (v is TownLocation && v.GetOwnerID() < 1)
            {
                if ((double)(Time.realtimeSinceStartup - realtimeSinceStartup) > 0.01)
                {
                    yield return null;
                    realtimeSinceStartup = Time.realtimeSinceStartup;
                }
                TownLocation townLocation = v as TownLocation;
                if (townLocation.aiForNeutralTown == null)
                {
                    townLocation.aiForNeutralTown = new AINeutralTown(townLocation);
                }
                townLocation.aiForNeutralTown.ResolveTurn();
            }
        }
    }

    public static IEnumerator ResolveExpeditions()
    {
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            yield return null;
        }
        List<global::MOM.Group> list = GameManager.Get().registeredGroups.FindAll((global::MOM.Group o) => o.locationHost == null && o.GetOwnerID() < 1);
        int aiDifficulty = DifficultySettingsData.GetSettingAsInt("UI_DIFF_NEUTRAL_ARMIES");
        foreach (global::MOM.Group v in list)
        {
            if (v.alive)
            {
                if (v.aiNeturalExpedition == null)
                {
                    v.aiNeturalExpedition = new AINeutralExpedition(v);
                    v.RegainTo1MP();
                }
                yield return v.aiNeturalExpedition.ResolveTurn(aiDifficulty);
                yield return AINeutralManager.TrackMovement(v);
            }
        }
    }

    private static IEnumerator TrackMovement(global::MOM.Group v)
    {
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            if (GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle) && v.GetOwnerID() != GameManager.GetHumanWizard().GetID() && !v.IsGroupInvisible() && Settings.GetData().GetFollowAIMovement() && FOW.Get().IsVisible(v.GetPosition(), v.GetPlane()))
            {
                if (World.GetActivePlane() != v.GetPlane())
                {
                    World.ActivatePlane(v.GetPlane());
                }
                CharacterActor characterActor = v.GetMapFormation()?.GetCharacterActors().Find((CharacterActor o) => o != null);
                if (characterActor != null)
                {
                    CameraController.CenterAt(characterActor.transform.localPosition);
                }
            }
            yield return null;
        }
    }
}
