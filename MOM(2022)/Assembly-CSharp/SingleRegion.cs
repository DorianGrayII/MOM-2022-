using MHUtils;
using MOM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public WorldCode.Plane plane;
    public SingleRegion enemyRegionAssignment;

    public void FindArea()
    {
        if (this.area == null)
            this.area = new HashSet<Vector3i>();
        this.area.Clear();
        ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(7);
        List<Vector3i> includedPoints = new List<Vector3i>();
        foreach (TownLocation location in this.locations)
        {
            Vector3i position = location.Position;
            includedPoints.Add(position);
            using (IEnumerator<Vector3i> enumerator2 = rangeSimple.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    Vector3i pos = enumerator2.Current + position;
                    Hex hexAtWrapped = location.GetPlane().GetHexAtWrapped(pos);
                    if (hexAtWrapped != null)
                    {
                        this.area.Add(hexAtWrapped.Position);
                    }
                }
            }
        }
        this.rectArea = new V3iRect(includedPoints);
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
        if (this.plane == null)
        {
            return Vector3.zero;
        }
        Vector3 vector = HexCoordinates.HexToWorld3D(this.GetPosition());
        return (this.plane.GetChunkFor(this.GetPosition()).go.transform.position + vector);
    }

    public WorldCode.Plane GetPlane()
    {
        return this.plane;
    }

    public Vector3i GetPosition()
    {
        return this.center;
    }

    public int TotalLocations()
    {
        if (this.locations != null)
        {
            return this.locations.Count;
        }
        List<TownLocation> locations = this.locations;
        return 0;
    }
}

