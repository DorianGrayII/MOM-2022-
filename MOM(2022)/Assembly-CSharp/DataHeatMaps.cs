using System.Collections.Generic;
using System.Collections.ObjectModel;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

public class DataHeatMaps
{
    public enum HMType
    {
        SettlementValue = 0,
        DangerMap = 1,
        MAX = 2
    }

    private V3iRect rect;

    private MHThread curentThread;

    private global::WorldCode.Plane plane;

    private PlayerWizard requestingWizard;

    private int lastRequestTurn;

    private Multitype<bool, HeatMap>[] maps;

    public DataHeatMaps(global::WorldCode.Plane p)
    {
        this.plane = p;
        int num = 2;
        this.maps = new Multitype<bool, HeatMap>[num];
        for (int i = 0; i < 2; i++)
        {
            this.maps[i] = new Multitype<bool, HeatMap>(t0: false, null);
        }
    }

    ~DataHeatMaps()
    {
        Debug.LogWarning("Heat maps destroyed for " + this.plane.planeSource.dbName);
    }

    private void UpdateHeatMap(HMType e)
    {
        if (!this.maps[(int)e].t0)
        {
            switch (e)
            {
            case HMType.SettlementValue:
                this.StartSettlementValuationHeatMaps();
                break;
            case HMType.DangerMap:
                this.StartDangerValuationHeatMaps();
                break;
            }
            this.maps[(int)e].t0 = true;
        }
    }

    public void StartSettlementValuationHeatMaps()
    {
        this.rect = this.plane.area;
        this.maps[0].t1 = new HeatMap(this.rect);
        List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.plane);
        List<Vector3i> list = new List<Vector3i>();
        foreach (global::MOM.Location item in locationsOfThePlane)
        {
            if (item is TownLocation)
            {
                list.Add(item.GetPosition());
            }
        }
        Dictionary<Vector3i, Hex> hexes = this.plane.GetHexes();
        HashSet<Vector3i> landHexes = this.plane.GetLandHexes();
        Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>> o = new Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>>(hexes, landHexes, list);
        this.ThreadedSettlementValuationHeatMaps(o);
    }

    private void ThreadedSettlementValuationHeatMaps(object o)
    {
        Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>> obj = o as Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>>;
        Dictionary<Vector3i, Hex> t = obj.t0;
        HashSet<Vector3i> t2 = obj.t1;
        List<Vector3i> t3 = obj.t2;
        HeatMap t4 = this.maps[0].t1;
        foreach (Vector3i item in t2)
        {
            int value = this.SettlementValue(t, t3, item);
            t4.SetValue(item, value);
        }
        t4.valid = true;
    }

    public int SettlementValue(Dictionary<Vector3i, Hex> hexes, List<Vector3i> towns, Vector3i pos)
    {
        ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(2);
        FInt zERO = FInt.ZERO;
        float num = 1f;
        int distToTown = DifficultySettingsData.GetTownDistance() + 1;
        if (towns != null && towns.FindIndex((Vector3i o) => HexCoordinates.HexDistance(o, pos) < distToTown) > -1)
        {
            return 0;
        }
        bool flag = false;
        foreach (Vector3i item in rangeSimple)
        {
            Vector3i pos2 = pos + item;
            Vector3i key = this.rect.KeepHorizontalInside(pos2);
            if (hexes.ContainsKey(key))
            {
                Hex hex = hexes[key];
                zERO += hex.GetFood();
                if (hex.Resource != null)
                {
                    num += 0.15f;
                }
                if (hex.GetTerrain().terrainType == ETerrainType.Forest)
                {
                    num += 0.05f;
                }
                if (!hex.IsLand() && HexCoordinates.HexDistance(hex.Position, pos) == 1 && this.plane.waterBodies != null && this.plane.waterBodies.ContainsKey(hex.Position) && this.plane.waterBodies[hex.Position] > 300)
                {
                    flag = true;
                }
            }
        }
        zERO *= num;
        if (flag && zERO > 6)
        {
            PlayerWizardAI ai = this.requestingWizard as PlayerWizardAI;
            if (ai != null)
            {
                if (ai.GetPlaneVisibility(this.plane).knownLocations.Find((global::MOM.Location o) => o.GetOwnerID() == ai.ID && o is TownLocation townLocation && townLocation.seaside && townLocation.GetDistanceTo(pos) < 10) == null)
                {
                    zERO += 5;
                }
                else
                {
                    zERO += new FInt(2.5f);
                }
            }
        }
        return zERO.ToInt();
    }

    public void StartDangerValuationHeatMaps()
    {
        this.rect = this.plane.area;
        this.maps[1].t1 = new HeatMap(this.rect);
        if (this.requestingWizard == null)
        {
            return;
        }
        Dictionary<int, float> relationship = new Dictionary<int, float>();
        if (this.requestingWizard.GetDiplomacy().statusses != null)
        {
            foreach (KeyValuePair<int, DiplomaticStatus> statuss in this.requestingWizard.GetDiplomacy().statusses)
            {
                float value = (statuss.Value.openWar ? 1f : Mathf.Clamp((float)(-(statuss.Value.GetRelationship() - 50)) / 100f, 0f, 1f));
                relationship[statuss.Key] = value;
            }
        }
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.plane);
        groupsOfPlane = groupsOfPlane.FindAll((global::MOM.Group o) => o.GetOwnerID() > 0 && relationship.ContainsKey(o.GetOwnerID()));
        this.ThreadedDangerValuationHeatMaps(new Multitype<List<global::MOM.Group>, Dictionary<int, float>>(groupsOfPlane, relationship));
    }

    private void ThreadedDangerValuationHeatMaps(object o)
    {
        Multitype<List<global::MOM.Group>, Dictionary<int, float>> obj = o as Multitype<List<global::MOM.Group>, Dictionary<int, float>>;
        List<global::MOM.Group> t = obj.t0;
        Dictionary<int, float> t2 = obj.t1;
        HeatMap t3 = this.maps[1].t1;
        foreach (global::MOM.Group item in t)
        {
            int ownerID = item.GetOwnerID();
            int value = item.GetValue();
            List<Vector3i> surroundingArea = item.GetSurroundingArea(5);
            float num = t2[ownerID];
            if (item.GetLocationHostSmart() != null)
            {
                num *= 0.8f;
            }
            int num2 = (int)((float)value * num);
            foreach (Vector3i item2 in surroundingArea)
            {
                int value2 = t3.GetValue(item2);
                if (num2 > value2)
                {
                    t3.SetValue(item2, num2);
                }
            }
        }
        t3.valid = true;
    }

    private bool IsBusy()
    {
        if (this.curentThread != null && !this.curentThread.IsFinished())
        {
            return true;
        }
        this.curentThread = null;
        return false;
    }

    public static DataHeatMaps Get(global::WorldCode.Plane plane)
    {
        return plane.GetDataHeatMaps();
    }

    public static void MakeMapDirty(global::WorldCode.Plane plane, HMType e)
    {
        DataHeatMaps.Get(plane).maps[(int)e].t0 = false;
    }

    public bool IsMapReady(HMType e, int wizardID, bool createIfNotReady = true)
    {
        PlayerWizard wizard = GameManager.GetWizard(wizardID);
        if (wizard == null)
        {
            Debug.LogError("No wizard dound" + wizardID);
        }
        return this.IsMapReady(e, wizard, createIfNotReady);
    }

    public bool IsMapReady(HMType e, PlayerWizard wizard = null, bool createIfNotReady = true)
    {
        if (this.IsBusy())
        {
            return false;
        }
        if (wizard != this.requestingWizard || this.lastRequestTurn != TurnManager.GetTurnNumber())
        {
            this.maps[(int)e].t0 = false;
            this.lastRequestTurn = TurnManager.GetTurnNumber();
            this.requestingWizard = wizard;
        }
        if (createIfNotReady)
        {
            this.UpdateHeatMap(e);
        }
        if (!this.IsBusy())
        {
            return this.maps[(int)e].t0;
        }
        return false;
    }

    public HeatMap GetHeatMap(HMType e)
    {
        return this.maps[(int)e].t1;
    }
}
