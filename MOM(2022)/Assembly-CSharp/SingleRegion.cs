using System.Collections.Generic;
using System.Collections.ObjectModel;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

public class SingleRegion : IPlanePosition
{
    public Vector3i center;

    public int radius;

    public int value;

    public int dangers;

    public HashSet<Vector3i> area;

    public List<Vector3i> islandHexes;

    public List<TownLocation> locations;

    public V3iRect rectArea;

    public global::WorldCode.Plane plane;

    public SingleRegion enemyRegionAssignment;

    public void FindArea()
    {
        if (this.area == null)
        {
            this.area = new HashSet<Vector3i>();
        }
        this.area.Clear();
        ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(7);
        List<Vector3i> list = new List<Vector3i>();
        foreach (TownLocation location in this.locations)
        {
            Vector3i position = location.Position;
            list.Add(position);
            foreach (Vector3i item in rangeSimple)
            {
                Vector3i pos = item + position;
                Hex hexAtWrapped = location.GetPlane().GetHexAtWrapped(pos);
                if (hexAtWrapped != null)
                {
                    this.area.Add(hexAtWrapped.Position);
                }
            }
        }
        this.rectArea = new V3iRect(list);
    }

    public void FindValue()
    {
        this.value = 0;
        foreach (TownLocation location in this.locations)
        {
            this.value += location.GetStrategicValue();
        }
    }

    public Vector3 GetPhysicalPosition()
    {
        if (this.plane != null)
        {
            Chunk chunkFor = this.plane.GetChunkFor(this.GetPosition());
            Vector3 vector = HexCoordinates.HexToWorld3D(this.GetPosition());
            return chunkFor.go.transform.position + vector;
        }
        return Vector3.zero;
    }

    public global::WorldCode.Plane GetPlane()
    {
        return this.plane;
    }

    public Vector3i GetPosition()
    {
        return this.center;
    }

    public int TotalLocations()
    {
        return this.locations?.Count ?? 0;
    }
}
