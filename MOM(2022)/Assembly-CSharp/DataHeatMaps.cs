using DBDef;
using MHUtils;
using MOM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using UnityEngine;
using WorldCode;

public class DataHeatMaps
{
    private V3iRect rect;
    private MHThread curentThread;
    private WorldCode.Plane plane;
    private PlayerWizard requestingWizard;
    private int lastRequestTurn;
    private Multitype<bool, HeatMap>[] maps;

    public DataHeatMaps(WorldCode.Plane p)
    {
        this.plane = p;
        int num = 2;
        this.maps = new Multitype<bool, HeatMap>[num];
        for (int i = 0; i < 2; i++)
        {
            this.maps[i] = new Multitype<bool, HeatMap>(false, null);
        }
    }

    ~DataHeatMaps()
    {
        Debug.LogWarning("Heat maps destroyed for " + this.plane.planeSource.dbName);
    }

    public static DataHeatMaps Get(WorldCode.Plane plane)
    {
        return plane.GetDataHeatMaps();
    }

    public HeatMap GetHeatMap(HMType e)
    {
        return this.maps[(int) e].t1;
    }

    private bool IsBusy()
    {
        if ((this.curentThread != null) && !this.curentThread.IsFinished())
        {
            return true;
        }
        this.curentThread = null;
        return false;
    }

    public bool IsMapReady(HMType e, PlayerWizard wizard, bool createIfNotReady)
    {
        if (this.IsBusy())
        {
            return false;
        }
        if (!ReferenceEquals(wizard, this.requestingWizard) || (this.lastRequestTurn != TurnManager.GetTurnNumber()))
        {
            this.maps[(int) e].t0 = false;
            this.lastRequestTurn = TurnManager.GetTurnNumber();
            this.requestingWizard = wizard;
        }
        if (createIfNotReady)
        {
            this.UpdateHeatMap(e);
        }
        return (this.IsBusy() ? false : this.maps[(int) e].t0);
    }

    public bool IsMapReady(HMType e, int wizardID, bool createIfNotReady)
    {
        PlayerWizard wizard = GameManager.GetWizard(wizardID);
        if (wizard == null)
        {
            Debug.LogError("No wizard dound" + wizardID.ToString());
        }
        return this.IsMapReady(e, wizard, createIfNotReady);
    }

    public static void MakeMapDirty(WorldCode.Plane plane, HMType e)
    {
        Get(plane).maps[(int) e].t0 = false;
    }

    public int SettlementValue(Dictionary<Vector3i, Hex> hexes, List<Vector3i> towns, Vector3i pos)
    {
        ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(2);
        FInt zERO = FInt.ZERO;
        float num2 = 1f;
        int distToTown = DifficultySettingsData.GetTownDistance() + 1;
        if ((towns != null) && (towns.FindIndex(o => HexCoordinates.HexDistance(o, pos) < distToTown) > -1))
        {
            return 0;
        }
        bool flag = false;
        foreach (Vector3i vectori in rangeSimple)
        {
            Vector3i vectori2 = pos + vectori;
            Vector3i key = this.rect.KeepHorizontalInside(vectori2);
            if (hexes.ContainsKey(key))
            {
                Hex hex = hexes[key];
                zERO += hex.GetFood();
                if (hex.Resource != null)
                {
                    num2 += 0.15f;
                }
                if (hex.GetTerrain().terrainType == ETerrainType.Forest)
                {
                    num2 += 0.05f;
                }
                if (!hex.IsLand() && ((HexCoordinates.HexDistance(hex.Position, pos) == 1) && ((this.plane.waterBodies != null) && (this.plane.waterBodies.ContainsKey(hex.Position) && (this.plane.waterBodies[hex.Position] > 300)))))
                {
                    flag = true;
                }
            }
        }
        zERO *= num2;
        if (flag && (zERO > 6))
        {
            PlayerWizardAI ai = this.requestingWizard as PlayerWizardAI;
            if (ai != null)
            {
                zERO = (ai.GetPlaneVisibility(this.plane).knownLocations.Find(delegate (MOM.Location o) {
                    if (o.GetOwnerID() != ai.ID)
                    {
                        return false;
                    }
                    TownLocation location = o as TownLocation;
                    return (location != null) && (location.seaside && (PlanePositionExtension.GetDistanceTo(location, pos) < 10));
                }) != null) ? (zERO + new FInt(2.5f)) : (zERO + 5);
            }
        }
        return zERO.ToInt();
    }

    public void StartDangerValuationHeatMaps()
    {
        this.rect = this.plane.area;
        this.maps[1].t1 = new HeatMap(this.rect);
        if (this.requestingWizard != null)
        {
            Dictionary<int, float> relationship = new Dictionary<int, float>();
            if (this.requestingWizard.GetDiplomacy().statusses != null)
            {
                foreach (KeyValuePair<int, DiplomaticStatus> pair in this.requestingWizard.GetDiplomacy().statusses)
                {
                    float num = pair.Value.openWar ? 1f : Mathf.Clamp((float) (((float) -(pair.Value.GetRelationship() - 50)) / 100f), (float) 0f, (float) 1f);
                    relationship[pair.Key] = num;
                }
            }
            List<MOM.Group> list = GameManager.GetGroupsOfPlane(this.plane).FindAll(o => (o.GetOwnerID() > 0) && relationship.ContainsKey(o.GetOwnerID()));
            this.ThreadedDangerValuationHeatMaps(new Multitype<List<MOM.Group>, Dictionary<int, float>>(list, relationship));
        }
    }

    public void StartSettlementValuationHeatMaps()
    {
        this.rect = this.plane.area;
        this.maps[0].t1 = new HeatMap(this.rect);
        List<Vector3i> list = new List<Vector3i>();
        foreach (MOM.Location location in GameManager.GetLocationsOfThePlane(this.plane))
        {
            if (location is TownLocation)
            {
                list.Add(location.GetPosition());
            }
        }
        Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>> o = new Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>>(this.plane.GetHexes(), this.plane.GetLandHexes(), list);
        this.ThreadedSettlementValuationHeatMaps(o);
    }

    private void ThreadedDangerValuationHeatMaps(object o)
    {
        Multitype<List<MOM.Group>, Dictionary<int, float>> multitype1 = o as Multitype<List<MOM.Group>, Dictionary<int, float>>;
        Dictionary<int, float> dictionary = multitype1.t1;
        HeatMap map = this.maps[1].t1;
        foreach (MOM.Group local1 in multitype1.t0)
        {
            int ownerID = local1.GetOwnerID();
            int num2 = local1.GetValue();
            List<Vector3i> surroundingArea = PlanePositionExtension.GetSurroundingArea(local1, 5);
            float num3 = dictionary[ownerID];
            if (local1.GetLocationHostSmart() != null)
            {
                num3 *= 0.8f;
            }
            int num4 = (int) (num2 * num3);
            foreach (Vector3i vectori in surroundingArea)
            {
                int num5 = map.GetValue(vectori);
                if (num4 > num5)
                {
                    map.SetValue(vectori, num4);
                }
            }
        }
        map.valid = true;
    }

    private void ThreadedSettlementValuationHeatMaps(object o)
    {
        Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>> multitype1 = o as Multitype<Dictionary<Vector3i, Hex>, HashSet<Vector3i>, List<Vector3i>>;
        Dictionary<Vector3i, Hex> hexes = multitype1.t0;
        List<Vector3i> towns = multitype1.t2;
        HeatMap map = this.maps[0].t1;
        foreach (Vector3i vectori in multitype1.t1)
        {
            int num = this.SettlementValue(hexes, towns, vectori);
            map.SetValue(vectori, num);
        }
        map.valid = true;
    }

    private void UpdateHeatMap(HMType e)
    {
        if (!this.maps[(int) e].t0)
        {
            if (e == HMType.SettlementValue)
            {
                this.StartSettlementValuationHeatMaps();
            }
            else if (e == HMType.DangerMap)
            {
                this.StartDangerValuationHeatMaps();
            }
            this.maps[(int) e].t0 = true;
        }
    }

    public enum HMType
    {
        SettlementValue,
        DangerMap,
        MAX
    }
}

