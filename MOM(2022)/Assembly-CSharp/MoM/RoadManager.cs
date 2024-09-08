using System.Collections.Generic;
using MHUtils;
using ProtoBuf;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class RoadManager
    {
        public enum RoadType
        {
            None = 0,
            Enchanted = 1,
            Normal = 5
        }

        [ProtoMember(1)]
        public NetDictionary<Vector3i, FInt> roadMap;

        [ProtoIgnore]
        public List<HashSet<Vector3i>> networks;

        [ProtoIgnore]
        public Plane owner;

        public RoadManager()
        {
        }

        public RoadManager(Plane p)
        {
            this.owner = p;
        }

        public void SetRoadMode(Vector3i v, Plane plane)
        {
            if (plane.arcanusType)
            {
                this.SetRoadMode(v, RoadType.Normal);
            }
            else
            {
                this.SetRoadMode(v, RoadType.Enchanted);
            }
            this.networks = null;
        }

        public void SetRoadMode(Vector3i v, RoadType rType)
        {
            this.SetRoadMode(v, new FInt((float)rType * 0.1f));
        }

        public void SetRoadMode(Vector3i v, FInt mode)
        {
            if (this.roadMap == null)
            {
                this.roadMap = new NetDictionary<Vector3i, FInt>();
            }
            if (mode == 0 && this.roadMap.ContainsKey(v))
            {
                this.roadMap.Remove(v);
            }
            else if (mode != 0)
            {
                this.roadMap[v] = mode;
            }
            this.UpdateAt(v);
            this.owner.GetMarkers_().UpdateMarkers();
            this.networks = null;
        }

        public FInt GetRoadAt(Vector3i v)
        {
            if (this.roadMap == null)
            {
                this.roadMap = new NetDictionary<Vector3i, FInt>();
            }
            if (this.roadMap.ContainsKey(v))
            {
                return this.roadMap[v];
            }
            return FInt.ZERO;
        }

        public bool[] GetNeighbours(Vector3i v)
        {
            bool[] array = new bool[6];
            if (this.roadMap == null)
            {
                return array;
            }
            bool flag = false;
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i v2 = v + HexNeighbors.neighbours[i];
                if (this.GetRoadAt(v2) != 0)
                {
                    flag = true;
                    array[i] = true;
                }
                else
                {
                    array[i] = false;
                }
            }
            if (!flag)
            {
                array[0] = true;
            }
            return array;
        }

        public void UpdateAt(Vector3i v)
        {
            if (this.GetRoadAt(v) == 0)
            {
                this.owner.GetMarkers_().SetAdvancedMarker(v, TerrainMarkers.MarkerType.Roads, visible: false);
            }
            else
            {
                TerrainMarkers.MarkerType type = ((this.GetRoadTypeAt(v) == RoadType.Normal) ? TerrainMarkers.MarkerType.Roads : TerrainMarkers.MarkerType.Roads2);
                this.owner.GetMarkers_().SetAdvancedMarker(v, type, visible: true, this.GetNeighbours(v));
            }
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i vector3i = v + HexNeighbors.neighbours[i];
                if (this.GetRoadAt(vector3i) != 0)
                {
                    TerrainMarkers.MarkerType type2 = ((this.GetRoadTypeAt(vector3i) == RoadType.Normal) ? TerrainMarkers.MarkerType.Roads : TerrainMarkers.MarkerType.Roads2);
                    this.owner.GetMarkers_().SetAdvancedMarker(vector3i, type2, visible: true, this.GetNeighbours(vector3i));
                }
            }
        }

        public void PostLoad(Plane owner)
        {
            this.owner = owner;
            if (this.roadMap == null)
            {
                return;
            }
            foreach (KeyValuePair<Vector3i, FInt> item in this.roadMap)
            {
                this.UpdateAt(item.Key);
            }
        }

        public List<HashSet<Vector3i>> GetNetworks()
        {
            if (this.networks == null)
            {
                this.networks = new List<HashSet<Vector3i>>();
                List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.owner);
                HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
                foreach (Location item in locationsOfThePlane)
                {
                    if (item is TownLocation townLocation && this.GetRoadNetworkForTown(townLocation) == null)
                    {
                        this.RecursiveSeek(townLocation.GetPosition(), hashSet);
                        if (hashSet.Count > 5)
                        {
                            this.networks.Add(hashSet);
                            hashSet = new HashSet<Vector3i>();
                        }
                        else
                        {
                            hashSet.Clear();
                        }
                    }
                }
            }
            return this.networks;
        }

        private void RecursiveSeek(Vector3i pos, HashSet<Vector3i> network)
        {
            if (this.roadMap.ContainsKey(pos) && !network.Contains(pos))
            {
                network.Add(pos);
                Vector3i[] neighbours = HexNeighbors.neighbours;
                foreach (Vector3i vector3i in neighbours)
                {
                    Vector3i pos2 = pos + vector3i;
                    this.RecursiveSeek(pos2, network);
                }
            }
        }

        public HashSet<Vector3i> GetRoadNetworkForTown(TownLocation tl)
        {
            foreach (HashSet<Vector3i> network in this.GetNetworks())
            {
                if (network.Contains(tl.GetPosition()))
                {
                    return network;
                }
            }
            return null;
        }

        public RoadType GetRoadTypeAt(Vector3i v)
        {
            if (this.roadMap == null)
            {
                this.roadMap = new NetDictionary<Vector3i, FInt>();
            }
            if (this.roadMap.ContainsKey(v))
            {
                if (!(this.roadMap[v] > 0.2f))
                {
                    return RoadType.Enchanted;
                }
                return RoadType.Normal;
            }
            return RoadType.None;
        }
    }
}
