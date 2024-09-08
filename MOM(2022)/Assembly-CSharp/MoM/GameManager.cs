// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.GameManager
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class GameManager : Entity, IEnchantable
{
    public enum FocusFlag
    {
        None = 0,
        Movement = 1,
        Adventure = 2,
        Battle = 4,
        DisplayFeedback = 8,
        MAX = 15
    }

    public static GameManager instance;

    [ProtoIgnore]
    public Dictionary<global::WorldCode.Plane, List<global::MOM.Group>> groups = new Dictionary<global::WorldCode.Plane, List<global::MOM.Group>>();

    [ProtoIgnore]
    public Dictionary<global::WorldCode.Plane, List<global::MOM.Location>> locations = new Dictionary<global::WorldCode.Plane, List<global::MOM.Location>>();

    [ProtoIgnore]
    public Dictionary<global::WorldCode.Plane, HashSet<Vector3i>> pathfindingObstacles = new Dictionary<global::WorldCode.Plane, HashSet<Vector3i>>();

    [ProtoIgnore]
    public List<global::MOM.Group> registeredGroups = new List<global::MOM.Group>();

    [ProtoIgnore]
    public List<global::MOM.Location> registeredLocations = new List<global::MOM.Location>();

    [ProtoIgnore]
    public List<PlayerWizard> wizards;

    [ProtoIgnore]
    public List<Multitype<object, FocusFlag>> gameplayFocusObjects = new List<Multitype<object, FocusFlag>>();

    [ProtoIgnore]
    private Dictionary<object, string> debugPrevDict;

    [ProtoIgnore]
    private Dictionary<object, string> debugCurrentDict;

    [ProtoMember(1)]
    public int ID;

    [ProtoMember(2)]
    protected EnchantmentManager enchantmentManager;

    [ProtoMember(3)]
    public int rampagingMonsterAccumulator;

    [ProtoMember(4)]
    public bool allowPlaneSwitch;

    [ProtoMember(5)]
    public int worldCounterMagic;

    [ProtoMember(6)]
    public List<TerrainChange> terrainChanges;

    [ProtoMember(7)]
    public RoadManager arcanusRoads;

    [ProtoMember(8)]
    public RoadManager myrrorRoads;

    [ProtoMember(9)]
    public bool useManaInAutoresolves;

    [ProtoMember(10)]
    public bool customDBUsed;

    [ProtoMember(11)]
    public Reference<PlayerWizard> timeStopMaster;

    [ProtoMember(12)]
    public List<Vector3i> corruptedArcanus;

    [ProtoMember(13)]
    public List<Vector3i> corruptedMyrror;

    [ProtoMember(14)]
    public bool usedDevMenuInThisGame;

    [ProtoMember(15)]
    public bool achievementChaosNode;

    [ProtoMember(16)]
    public bool achievementNatureNode;

    [ProtoMember(17)]
    public bool achievementSorceryNode;

    [ProtoMember(18)]
    public int dlcSettings;

    [ProtoMember(20)]
    public float midgameMonsterAccumulator;

    [ProtoIgnore]
    private Queue<Callback> focusActionStack = new Queue<Callback>();

    [ProtoIgnore]
    public bool duringSaveReloading;

    [ProtoIgnore]
    public bool showGlobalsOnHUD;

    [ProtoIgnore]
    public bool showResourcesOnHUD;

    private GameManager()
    {
    }

    public static GameManager Get()
    {
        if (GameManager.instance == null)
        {
            GameManager entityOfType = EntityManager.GetEntityOfType<GameManager>();
            if (entityOfType != null)
            {
                GameManager.instance = entityOfType;
                entityOfType.wizards = EntityManager.GetEntitiesType<PlayerWizard>();
            }
            else
            {
                GameManager.instance = new GameManager();
                GameManager.instance.RegisterEntity();
                GameManager.instance.allowPlaneSwitch = true;
                GameManager.instance.useManaInAutoresolves = true;
                if (PlayerPrefs.HasKey("UseDLC"))
                {
                    GameManager.instance.dlcSettings = PlayerPrefs.GetInt("UseDLC");
                }
            }
        }
        return GameManager.instance;
    }

    [ProtoAfterDeserialization]
    public void AfterDeserialize()
    {
        if (this.enchantmentManager != null)
        {
            this.enchantmentManager.owner = this;
        }
    }

    public void PostLoad()
    {
        if (this.corruptedArcanus != null)
        {
            foreach (Vector3i corruptedArcanu in this.corruptedArcanus)
            {
                World.GetArcanus().GetHexAt(corruptedArcanu).ActiveHex = false;
            }
        }
        if (this.corruptedMyrror != null)
        {
            foreach (Vector3i item in this.corruptedMyrror)
            {
                World.GetMyrror().GetHexAt(item).ActiveHex = false;
            }
        }
        if (this.terrainChanges != null)
        {
            HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
            HashSet<Vector3i> hashSet2 = new HashSet<Vector3i>();
            foreach (TerrainChange terrainChange in this.terrainChanges)
            {
                terrainChange.ReApply();
                if (terrainChange.isArcanus && terrainChange.terrainChange != null)
                {
                    hashSet.Add(terrainChange.position);
                }
                else if (!terrainChange.isArcanus && terrainChange.terrainChange != null)
                {
                    hashSet2.Add(terrainChange.position);
                }
            }
            if (hashSet.Count > 0)
            {
                World.GetArcanus().RebuildUpdatedTerrains(hashSet);
            }
            if (hashSet2.Count > 0)
            {
                World.GetMyrror().RebuildUpdatedTerrains(hashSet2);
            }
        }
        this.registeredGroups = EntityManager.GetEntitiesType<global::MOM.Group>();
        if (this.registeredGroups != null)
        {
            this.groups.Clear();
            foreach (global::MOM.Group registeredGroup in this.registeredGroups)
            {
                global::WorldCode.Plane plane = registeredGroup.GetPlane();
                if (!this.groups.ContainsKey(plane))
                {
                    this.groups[plane] = new List<global::MOM.Group>();
                }
                this.groups[plane].Add(registeredGroup);
                global::MOM.Location locationHost = registeredGroup.GetLocationHost();
                if (locationHost != null && locationHost.otherPlaneLocation != null && !locationHost.GetPlane().arcanusType)
                {
                    locationHost.GetLocalGroup().groupUnits = locationHost.otherPlaneLocation.Get().GetLocalGroup().GetUnits();
                }
                if (registeredGroup.IsHumanPlayerFocusedOnPlane())
                {
                    registeredGroup.GetMapFormation();
                }
            }
        }
        this.registeredLocations = EntityManager.GetEntitiesType<global::MOM.Location>();
        if (this.registeredLocations != null)
        {
            this.locations.Clear();
            foreach (global::MOM.Location registeredLocation in this.registeredLocations)
            {
                global::WorldCode.Plane plane2 = registeredLocation.GetPlane();
                if (!this.locations.ContainsKey(plane2))
                {
                    this.locations[plane2] = new List<global::MOM.Location>();
                }
                this.locations[plane2].Add(registeredLocation);
                if (registeredLocation.discovered)
                {
                    registeredLocation.InitializeModel();
                }
                if (registeredLocation is TownLocation)
                {
                    (registeredLocation as TownLocation).InitializeAstralGate();
                }
            }
        }
        if (this.arcanusRoads != null)
        {
            this.arcanusRoads?.PostLoad(World.GetArcanus());
        }
        if (this.myrrorRoads != null)
        {
            this.myrrorRoads?.PostLoad(World.GetMyrror());
        }
        GameManager.InitializePathfindingObstacles();
    }

    public static void Destroy()
    {
        GameManager.instance = null;
    }

    public void RegisterGroup(global::MOM.Group g)
    {
        if (g.GetPlane() == null)
        {
            Debug.LogError("Registration of the group which is not assigned to a plane");
            return;
        }
        g.RegisterEntity();
        if (!this.groups.ContainsKey(g.GetPlane()))
        {
            this.groups[g.GetPlane()] = new List<global::MOM.Group>();
        }
        g.alive = true;
        this.groups[g.GetPlane()].Add(g);
        this.registeredGroups.Add(g);
        GameManager.SetPathfindingObstacleAt(g.GetPlane(), g.GetPosition(), value: true);
    }

    public void RegisterLocation(global::MOM.Location l)
    {
        if (l.plane == null)
        {
            Debug.LogError("Registration of the group which is not assigned to a plane");
            return;
        }
        l.RegisterEntity();
        if (!this.locations.ContainsKey(l.plane))
        {
            this.locations[l.plane] = new List<global::MOM.Location>();
        }
        this.locations[l.plane].Add(l);
        this.registeredLocations.Add(l);
        GameManager.SetPathfindingObstacleAt(l.plane, l.GetPosition(), value: true);
    }

    public void Unregister(global::MOM.Group g)
    {
        if (g == null)
        {
            return;
        }
        if (this.registeredGroups.Contains(g))
        {
            global::WorldCode.Plane plane = g.GetPlane();
            if (this.groups.ContainsKey(plane) && this.groups[plane].Contains(g))
            {
                this.groups[plane].Remove(g);
            }
            this.registeredGroups.Remove(g);
        }
        if (g.GetUnits() != null)
        {
            foreach (Reference<global::MOM.Unit> unit in g.GetUnits())
            {
                unit.Get().UnregisterEntity();
            }
        }
        g.UnregisterEntity();
        GameManager.UpdatePathfindingObstacleAt(g.GetPlane(), g.GetPosition());
    }

    public void Unregister(global::MOM.Location l)
    {
        if (this.registeredLocations.Contains(l))
        {
            global::WorldCode.Plane plane = l.GetPlane();
            if (this.locations.ContainsKey(plane) && this.locations[plane].Contains(l))
            {
                this.locations[plane].Remove(l);
            }
            this.registeredLocations.Remove(l);
            GameManager.UpdatePathfindingObstacleAt(l.plane, l.GetPosition());
        }
        l.UnregisterEntity();
    }

    public global::MOM.Group GetGroupAt(Vector3i pos, global::WorldCode.Plane p = null)
    {
        if (p == null)
        {
            p = World.GetActivePlane();
        }
        if (p == null)
        {
            return null;
        }
        if (this.groups.ContainsKey(p))
        {
            return this.groups[p].Find((global::MOM.Group o) => o.GetPosition() == pos);
        }
        return null;
    }

    public static List<global::MOM.Group> GetGroupsOfPlane(bool arcanus)
    {
        if (arcanus)
        {
            return GameManager.GetGroupsOfPlane(World.GetArcanus());
        }
        return GameManager.GetGroupsOfPlane(World.GetMyrror());
    }

    public static List<global::MOM.Group> GetGroupsOfPlane(global::WorldCode.Plane p)
    {
        if (GameManager.Get().groups != null && GameManager.Get().groups.ContainsKey(p))
        {
            return GameManager.Get().groups[p];
        }
        return new List<global::MOM.Group>();
    }

    public global::MOM.Location GetLocationAt(Vector3i pos, global::WorldCode.Plane p = null)
    {
        if (p == null)
        {
            p = World.GetActivePlane();
        }
        if (p == null)
        {
            return null;
        }
        if (this.locations.ContainsKey(p))
        {
            return this.locations[p].Find((global::MOM.Location o) => o.Position == pos);
        }
        return null;
    }

    public static List<global::MOM.Location> GetLocationsOfThePlane(bool arcanus)
    {
        if (arcanus)
        {
            return GameManager.GetLocationsOfThePlane(World.GetArcanus());
        }
        return GameManager.GetLocationsOfThePlane(World.GetMyrror());
    }

    public static List<global::MOM.Location> GetLocationsOfThePlane(global::WorldCode.Plane p)
    {
        if (p == null)
        {
            p = World.GetActivePlane();
        }
        if (p == null)
        {
            return null;
        }
        if (GameManager.Get().locations.ContainsKey(p))
        {
            return GameManager.Get().locations[p];
        }
        return null;
    }

    public static List<global::MOM.Location> GetLocationsOfWizard(int ID)
    {
        List<global::MOM.Location> list = new List<global::MOM.Location>();
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (registeredLocation.GetOwnerID() == ID)
            {
                list.Add(registeredLocation);
            }
        }
        return list;
    }

    public static List<global::MOM.Location> GetTownsOfWizard(int ID)
    {
        List<global::MOM.Location> list = new List<global::MOM.Location>();
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (registeredLocation.GetOwnerID() == ID && registeredLocation is TownLocation)
            {
                list.Add(registeredLocation);
            }
        }
        return list;
    }

    public static int GetWizardTownCount(int ID)
    {
        int num = 0;
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (registeredLocation.GetOwnerID() == ID && registeredLocation is TownLocation)
            {
                num++;
            }
        }
        return num;
    }

    public static int GetWizardLocationsCount(int ID)
    {
        int num = 0;
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (registeredLocation.GetOwnerID() == ID)
            {
                num++;
            }
        }
        return num;
    }

    public static List<global::MOM.Group> GetGroupsOfWizard(int ID)
    {
        List<global::MOM.Group> list = new List<global::MOM.Group>();
        foreach (global::MOM.Group registeredGroup in GameManager.Get().registeredGroups)
        {
            if (registeredGroup.GetOwnerID() == ID && (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() == null || !registeredGroup.plane.arcanusType))
            {
                list.Add(registeredGroup);
            }
        }
        return list;
    }

    public static PlayerWizard SelectWizard(Wizard w, List<Tag> books = null, List<Spell> spells = null, List<Trait> traits = null, bool custom = false)
    {
        GameManager.Get().wizards = new List<PlayerWizard>();
        if (spells != null && spells.Count == 0)
        {
            spells = null;
        }
        PlayerWizard playerWizard = new PlayerWizard(w, null, books, spells, traits, PlayerWizard.Color.None, custom);
        playerWizard.name = w.GetDescriptionInfo().GetLocalizedName();
        GameManager.Get().wizards.Add(playerWizard);
        playerWizard.AddNotification(new SummaryInfo
        {
            summaryType = SummaryInfo.SummaryType.eResearchAvailiable
        });
        return playerWizard;
    }

    public static PlayerWizard GetWizard(int id)
    {
        if (id <= 0 || GameManager.Get().wizards == null)
        {
            return null;
        }
        return GameManager.Get().wizards.Find((PlayerWizard o) => o.ID == id);
    }

    public static PlayerWizard GetHumanWizard()
    {
        return GameManager.GetWizard(PlayerWizard.HumanID());
    }

    public static List<PlayerWizard> GetWizards()
    {
        return GameManager.Get()?.wizards;
    }

    public static void InitializeAllWizards(int wizardCount = 5)
    {
        if (GameManager.Get() == null || GameManager.Get().wizards == null)
        {
            return;
        }
        List<Wizard> type = DataBase.GetType<Wizard>();
        if (wizardCount > type.Count)
        {
            wizardCount = type.Count;
        }
        List<Wizard> list = new List<Wizard>();
        List<Wizard> list2 = new List<Wizard>();
        foreach (Wizard item2 in type)
        {
            if (item2.traits != null && Array.Find(item2.traits, (Trait o) => o == (Trait)TRAIT.MYRRAN) != null)
            {
                list.Add(item2);
            }
            else
            {
                list2.Add(item2);
            }
        }
        while (GameManager.Get().wizards.Count < wizardCount)
        {
            Wizard w = null;
            PlayerWizard playerWizard = GameManager.Get().wizards.Find((PlayerWizard o) => o.GetTraits().Contains((Trait)TRAIT.MYRRAN));
            if (playerWizard != null && list2.Count > 1)
            {
                int index = global::UnityEngine.Random.Range(0, list2.Count);
                w = list2[index];
            }
            else
            {
                int index2 = global::UnityEngine.Random.Range(0, type.Count);
                w = type[index2];
            }
            if (GameManager.Get().wizards.Find((PlayerWizard o) => o.IsWizard(w)) == null)
            {
                bool myrranForced = playerWizard == null && GameManager.Get().wizards.Count == wizardCount - 1;
                PlayerWizardAI item = new PlayerWizardAI(w, myrranForced);
                GameManager.Get().wizards.Add(item);
            }
        }
        for (int i = 0; i < GameManager.Get().wizards.Count; i++)
        {
            GameManager.Get().wizards[i].GetPersonality();
        }
    }

    public static IEnumerator DoAITurn()
    {
        List<global::MOM.Group> groupsOfWizard = GameManager.GetGroupsOfWizard(PlayerWizard.HumanID());
        int value = TurnManager.GetTurnNumber() * 3 + 100;
        foreach (global::MOM.Group item in groupsOfWizard)
        {
            int value2 = item.GetValue();
            if (value < value2)
            {
                value = value2;
            }
        }
        List<PlayerWizard> ws = GameManager.GetWizards();
        for (int i = 0; i < ws.Count; i++)
        {
            if (ws[i] is PlayerWizardAI playerWizardAI && playerWizardAI.isAlive)
            {
                if (GameManager.Get()?.timeStopMaster != null && GameManager.Get()?.timeStopMaster.Get() != playerWizardAI)
                {
                    continue;
                }
                yield return playerWizardAI.PlayTurn(value);
                while (!GameManager.Get().IsFocusFree())
                {
                    yield return null;
                }
            }
            ws[i].ReportForLog();
        }
    }

    public EnchantmentManager GetEnchantmentManager()
    {
        if (this.enchantmentManager == null)
        {
            this.enchantmentManager = new EnchantmentManager(this);
        }
        return this.enchantmentManager;
    }

    public static void InitializePathfindingObstacles()
    {
        foreach (global::WorldCode.Plane plane in World.GetPlanes())
        {
            if (plane.battlePlane)
            {
                continue;
            }
            GameManager.GetPathfindingObstacles(plane).Clear();
            foreach (global::MOM.Group item in GameManager.GetGroupsOfPlane(plane))
            {
                GameManager.SetPathfindingObstacleAt(plane, item.GetPosition(), value: true);
            }
            foreach (global::MOM.Location item2 in GameManager.GetLocationsOfThePlane(plane))
            {
                GameManager.SetPathfindingObstacleAt(plane, item2.GetPosition(), value: true);
            }
        }
    }

    public static HashSet<Vector3i> GetPathfindingObstacles(global::WorldCode.Plane p)
    {
        if (GameManager.Get().pathfindingObstacles == null)
        {
            GameManager.Get().pathfindingObstacles = new Dictionary<global::WorldCode.Plane, HashSet<Vector3i>>();
        }
        if (!GameManager.Get().pathfindingObstacles.ContainsKey(p))
        {
            GameManager.Get().pathfindingObstacles[p] = new HashSet<Vector3i>();
        }
        return GameManager.Get().pathfindingObstacles[p];
    }

    public static void UpdatePathfindingObstacleAt(global::WorldCode.Plane p, Vector3i pos)
    {
        bool flag = false;
        foreach (global::MOM.Group item in GameManager.GetGroupsOfPlane(p))
        {
            if (item.alive && item.GetPosition() == pos)
            {
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            foreach (global::MOM.Location item2 in GameManager.GetLocationsOfThePlane(p))
            {
                if (item2.GetPosition() == pos)
                {
                    flag = true;
                    break;
                }
            }
        }
        GameManager.SetPathfindingObstacleAt(p, pos, flag);
    }

    public static void SetPathfindingObstacleAt(global::WorldCode.Plane p, Vector3i pos, bool value)
    {
        bool flag = GameManager.GetPathfindingObstacles(p).Contains(pos);
        if (value && !flag)
        {
            GameManager.GetPathfindingObstacles(p).Add(pos);
        }
        else if (!value && flag)
        {
            GameManager.GetPathfindingObstacles(p).Remove(pos);
        }
    }

    public PlayerWizard GetWizardOwner()
    {
        return null;
    }

    public static bool GroupAlive(IPlanePosition e)
    {
        if (e is global::MOM.Group)
        {
            return GameManager.Get().registeredGroups.Contains(e as global::MOM.Group);
        }
        if (e is global::MOM.Location)
        {
            return GameManager.Get().registeredLocations.Contains(e as global::MOM.Location);
        }
        Debug.LogError("Invalid type request! " + e);
        return false;
    }

    public bool IsFocusFree()
    {
        return this.gameplayFocusObjects.Count == 0;
    }

    public bool IsFocusFreeFrom(FocusFlag focusFlag)
    {
        if (focusFlag == FocusFlag.None)
        {
            return this.IsFocusFree();
        }
        return this.gameplayFocusObjects.Find((Multitype<object, FocusFlag> o) => o.t1 == focusFlag) == null;
    }

    public void TakeFocus(object o, FocusFlag type)
    {
        this.gameplayFocusObjects.Add(new Multitype<object, FocusFlag>(o, type));
    }

    public void FreeFocus(object o, bool throwErrors = true)
    {
        if (this.duringSaveReloading)
        {
            return;
        }
        Multitype<object, FocusFlag> multitype = this.gameplayFocusObjects.Find((Multitype<object, FocusFlag> k) => k.t0 == o);
        if (multitype != null)
        {
            this.gameplayFocusObjects.Remove(multitype);
            while (this.IsFocusFree() && this.focusActionStack.Count > 0)
            {
                this.focusActionStack.Dequeue()(null);
            }
        }
    }

    public void AddFocusCallback(Callback c)
    {
        this.focusActionStack.Enqueue(c);
    }

    public override int GetID()
    {
        return this.ID;
    }

    public override void SetID(int id)
    {
        this.ID = id;
    }

    public string GetName()
    {
        return global::DBUtils.Localization.Get("UI_GLOBAL", true);
    }

    public void RecordTerrainChange(TerrainChange tc)
    {
        if (this.terrainChanges == null)
        {
            this.terrainChanges = new List<TerrainChange>();
        }
        TerrainChange terrainChange = this.terrainChanges.Find((TerrainChange o) => o.isArcanus == tc.isArcanus && o.position == tc.position);
        if (terrainChange != null)
        {
            if (tc.updateResource)
            {
                terrainChange.resourceChange = tc.resourceChange;
            }
            if (tc.terrainChange != null)
            {
                terrainChange.terrainChange = tc.terrainChange;
            }
        }
        else
        {
            this.terrainChanges.Add(tc);
        }
    }

    public void FinishedIteratingEnchantments()
    {
    }

    public void AllowPlaneSwitch(bool value)
    {
        this.allowPlaneSwitch = value;
        HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
        if (!value)
        {
            foreach (global::MOM.Location item in GameManager.GetLocationsOfThePlane(arcanus: true))
            {
                if (item.otherPlaneLocation != null)
                {
                    hashSet.Add(item.GetPosition());
                }
            }
            World.GetArcanus().exclusionPoints = hashSet;
            World.GetMyrror().exclusionPoints = hashSet;
        }
        else
        {
            World.GetArcanus().exclusionPoints = null;
            World.GetMyrror().exclusionPoints = null;
        }
    }
}
