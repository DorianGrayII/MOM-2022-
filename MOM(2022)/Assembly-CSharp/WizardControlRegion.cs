// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WizardControlRegion
using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MOM;
using WorldCode;

public class WizardControlRegion
{
    public int owner;

    public bool regionsDirty;

    public bool regionsUpdating;

    private List<SingleRegion> regions;

    public WizardControlRegion(int owner)
    {
        this.regionsDirty = true;
        this.owner = owner;
    }

    public IEnumerator GetRegionsAsync(List<SingleRegion> regions)
    {
        if (!this.regionsUpdating)
        {
            this.Update();
        }
        while (this.regionsUpdating)
        {
            yield return null;
        }
        regions.Clear();
        regions.AddRange(this.regions);
    }

    private void Update()
    {
        this.regionsUpdating = true;
        if (this.regionsDirty)
        {
            this._FindRegions();
        }
        this.UpdateValue();
        this.regionsDirty = false;
        this.regionsUpdating = false;
    }

    private void _FindRegions()
    {
        if (this.regions == null)
        {
            this.regions = new List<SingleRegion>();
        }
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (entity.Value is TownLocation townLocation && townLocation.owner == this.owner)
            {
                Hex hex = townLocation.GetPlane().GetHexAt(townLocation.GetPosition());
                Plane plane = townLocation.GetPlane();
                List<Vector3i> isl = plane.GetIslands().Find((List<Vector3i> o) => o.Contains(hex.Position));
                SingleRegion singleRegion = this.regions.Find((SingleRegion o) => o.islandHexes == isl);
                if (singleRegion == null)
                {
                    singleRegion = new SingleRegion();
                    singleRegion.islandHexes = isl;
                    singleRegion.locations = new List<TownLocation>();
                    singleRegion.plane = plane;
                    this.regions.Add(singleRegion);
                }
                singleRegion.locations.Add(townLocation);
            }
        }
        for (int num = this.regions.Count - 1; num >= 0; num--)
        {
            List<TownLocation> locations = this.regions[num].locations;
            while (locations.Count > 1)
            {
                List<TownLocation> list = new List<TownLocation>(locations.Count);
                TownLocation item = locations[locations.Count - 1];
                list.Add(item);
                locations.Remove(item);
                for (int i = 0; i < list.Count; i++)
                {
                    TownLocation obj = list[i];
                    for (int num2 = locations.Count - 1; num2 >= 0; num2--)
                    {
                        item = locations[num2];
                        if (obj.GetDistanceTo(item) <= 7)
                        {
                            list.Add(item);
                            locations.Remove(item);
                        }
                    }
                }
                if (locations.Count > 0)
                {
                    SingleRegion singleRegion2 = new SingleRegion();
                    singleRegion2.islandHexes = this.regions[num].islandHexes;
                    singleRegion2.locations = list;
                    singleRegion2.plane = this.regions[num].plane;
                    this.regions.Add(singleRegion2);
                }
                else
                {
                    this.regions[num].locations = list;
                }
            }
        }
        foreach (SingleRegion region in this.regions)
        {
            region.FindArea();
        }
    }

    private void UpdateValue()
    {
        foreach (SingleRegion region in this.regions)
        {
            region.FindValue();
        }
    }
}
