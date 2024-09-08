using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;

namespace WorldCode
{
    public class SearcherData
    {
        public static Queue<SearcherData> cachedSearcherDatas = new Queue<SearcherData>();

        public HashSet<Vector3i> discountLocations;

        public HashSet<Vector3i> worldLand;

        public Dictionary<Vector3i, Hex> worldHexes;

        public HashSet<Vector3i> invalidLocations;

        public List<Vector3i> avoidance;

        public Dictionary<Vector3i, int> avoidanceCost;

        public List<Vector3i> lPotentials = new List<Vector3i>();

        public Queue<Vector3i> qPotentials = new Queue<Vector3i>();

        public Vector3i start;

        public Vector3i destination;

        public PathfinderArea area;

        public V3iRect rectArea;

        public List<Vector3i> areaAnswer;

        public int maxCost;

        public bool landMovement;

        public bool waterMovement;

        public bool mountainMovement;

        public bool forestMovement;

        public bool nonCorporealMovement;

        public SearcherData(Plane p, Vector3i start, Vector3i destination, HashSet<Vector3i> invalidLocations = null, HashSet<Vector3i> discountLocations = null)
        {
            this.start = start;
            this.destination = destination;
            this.worldLand = p.GetLandHexes();
            this.worldHexes = p.GetHexes();
            this.invalidLocations = invalidLocations;
            this.discountLocations = discountLocations;
            this.rectArea = p.pathfindingArea;
            this.area = new PathfinderArea(p.pathfindingArea.AreaWidth, p.pathfindingArea.AreaHeight);
            this.Initialize(useHeuristic: true);
        }

        public SearcherData(Plane p, Vector3i start, int maxRange, HashSet<Vector3i> invalidLocations = null, HashSet<Vector3i> discountLocations = null)
        {
            this.start = start;
            this.destination = -1 * Vector3i.one;
            this.worldLand = p.GetLandHexes();
            this.worldHexes = p.GetHexes();
            this.invalidLocations = invalidLocations;
            this.discountLocations = discountLocations;
            this.maxCost = maxRange;
            this.rectArea = p.pathfindingArea;
            this.area = new PathfinderArea(p.pathfindingArea.AreaWidth, p.pathfindingArea.AreaHeight);
            this.Initialize(useHeuristic: false);
        }

        public void FillMovementCost(IPlanePosition source)
        {
            if (source is IAttributable)
            {
                this.FillMovementCost(source as IAttributable);
            }
            else if (source is global::MOM.Group)
            {
                this.FillMovementCost(source as global::MOM.Group);
            }
            else if (!(source is global::MOM.Location))
            {
                Debug.LogError("Unknown type " + source);
            }
        }

        public void FillMovementCost(IAttributable source)
        {
            bool flag = source.GetAttributes().GetFinal(TAG.CAN_FLY) > 0;
            this.landMovement = flag || source.GetAttributes().GetFinal(TAG.CAN_WALK) > 0;
            this.waterMovement = flag || source.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0;
            this.mountainMovement = source.GetAttributes().GetFinal(TAG.MOUNTAINEER) > 0;
            this.forestMovement = source.GetAttributes().GetFinal(TAG.FORESTER) > 0;
            this.nonCorporealMovement = flag || source.GetAttributes().GetFinal(TAG.NON_CORPOREAL) > 0;
        }

        public void FillMovementCost(global::MOM.Group source)
        {
            this.landMovement = source.landMovement;
            this.waterMovement = source.waterMovement;
            this.mountainMovement = source.mountainMovement;
            this.forestMovement = source.forestMovement;
            this.nonCorporealMovement = source.nonCorporealMovement;
        }

        private void Initialize(bool useHeuristic)
        {
            if (useHeuristic)
            {
                this.area.SetHeuristicValue(this.start, this.rectArea.HexDistance(this.start, this.destination));
            }
        }

        public int MovementCost(Vector3i fromNode, Vector3i toNode)
        {
            int num = 0;
            if (this.avoidance != null)
            {
                if (this.avoidanceCost == null)
                {
                    this.avoidanceCost = new Dictionary<Vector3i, int>();
                }
                if (!this.avoidanceCost.ContainsKey(toNode))
                {
                    int num2 = int.MaxValue;
                    foreach (Vector3i item in this.avoidance)
                    {
                        int num3 = this.rectArea.HexDistance(toNode, item);
                        if (num3 < num2)
                        {
                            num2 = num3;
                        }
                    }
                    this.avoidanceCost[toNode] = Mathf.Max(0, 4 - num2 >> 1);
                }
                num = this.avoidanceCost[toNode];
            }
            if (this.discountLocations != null && this.discountLocations.Contains(toNode))
            {
                return 1 + num;
            }
            int num4 = 0;
            Hex hex = this.worldHexes[toNode];
            int num5 = hex.MovementCost();
            if (!hex.IsLand())
            {
                if (!this.waterMovement)
                {
                    return 0;
                }
            }
            else
            {
                if (!this.landMovement)
                {
                    return 0;
                }
                if (num5 > 1)
                {
                    if (this.nonCorporealMovement)
                    {
                        num5 = 1;
                    }
                    else if (this.mountainMovement && (hex.HaveFlag(ETerrainType.Mountain) || hex.HaveFlag(ETerrainType.Hill)))
                    {
                        num5 = 1;
                    }
                    else if (this.forestMovement && hex.HaveFlag(ETerrainType.Forest))
                    {
                        num5 = 1;
                    }
                }
            }
            if (hex.viaRiver != null && !this.waterMovement)
            {
                for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
                {
                    if (hex.viaRiver[i] && HexNeighbors.neighbours[i] + hex.Position == fromNode)
                    {
                        num4++;
                        break;
                    }
                }
            }
            return num5 + num4 + num;
        }

        public bool ValidLocation(Vector3i pos)
        {
            if (this.invalidLocations == null || pos == this.destination)
            {
                return true;
            }
            if (this.invalidLocations.Contains(pos))
            {
                return false;
            }
            return true;
        }

        public int DestinationCost()
        {
            if (this.area != null)
            {
                return this.area.GetNode(this.destination).curentCost;
            }
            return 0;
        }

        public int TotalCostTo(Vector3i pos)
        {
            if (this.area != null)
            {
                return this.area.GetNode(pos).curentCost;
            }
            return 0;
        }
    }
}
