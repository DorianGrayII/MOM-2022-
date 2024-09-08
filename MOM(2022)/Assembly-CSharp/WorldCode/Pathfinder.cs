using System.Collections.Generic;
using MHUtils;

namespace WorldCode
{
    public class Pathfinder
    {
        public static List<Vector3i> FindPath(SearcherData data)
        {
            if (!Pathfinder.SearchPreparation(data))
            {
                return null;
            }
            switch (data.rectArea.HexDistance(data.start, data.destination))
            {
            case 0:
                return new List<Vector3i> { data.start };
            case 1:
                return new List<Vector3i> { data.start, data.destination };
            default:
                Pathfinder.AtoBSearch(data);
                return Pathfinder.RecreatePath(data);
            }
        }

        public static List<Vector3i> FindArea(SearcherData data)
        {
            if (data.areaAnswer == null)
            {
                data.areaAnswer = new List<Vector3i>();
            }
            data.areaAnswer.Add(data.start);
            Pathfinder.PotentNeightboursArea(data, data.start);
            while (data.qPotentials.Count > 0)
            {
                Vector3i from = data.qPotentials.Dequeue();
                Pathfinder.PotentNeightboursArea(data, from);
            }
            return data.areaAnswer;
        }

        private static bool SearchPreparation(SearcherData data)
        {
            if (data.worldHexes.ContainsKey(data.start))
            {
                return data.worldHexes.ContainsKey(data.destination);
            }
            return false;
        }

        private static void AtoBSearch(SearcherData data)
        {
            Pathfinder.PotentNeightbours(data, data.start);
            while (data.lPotentials.Count > 0)
            {
                Vector3i bestNext = Pathfinder.GetBestNext(data);
                data.lPotentials.Remove(bestNext);
                Pathfinder.PotentNeightbours(data, bestNext);
                if (data.lPotentials.Count > 300)
                {
                    break;
                }
            }
        }

        private static void PotentNeightbours(SearcherData data, Vector3i from)
        {
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i vector3i = data.rectArea.KeepHorizontalInside(from + HexNeighbors.neighbours[i]);
                if (data.rectArea.IsInside(vector3i) && data.ValidLocation(vector3i) && Pathfinder.CalculateCosts(data, from, vector3i))
                {
                    data.lPotentials.Add(vector3i);
                }
            }
        }

        private static bool CalculateCosts(SearcherData data, Vector3i from, Vector3i to)
        {
            int num = data.MovementCost(from, to);
            if (num == 0 || to == data.start)
            {
                return false;
            }
            int num2 = data.area.GetTotalValue(from) + num;
            int totalValue = data.area.GetTotalValue(to);
            if (totalValue > 0 && totalValue <= num2)
            {
                return false;
            }
            int value = num2 + data.rectArea.HexDistance(to, data.destination);
            data.area.SetTotalValue(to, num2);
            data.area.SetHeuristicValue(to, value);
            return true;
        }

        private static Vector3i GetBestNext(SearcherData data)
        {
            Vector3i vector3i = data.lPotentials[0];
            PathfinderNode pathfinderNode = data.area.GetNode(vector3i);
            for (int i = 1; i < data.lPotentials.Count; i++)
            {
                Vector3i vector3i2 = data.lPotentials[i];
                PathfinderNode node = data.area.GetNode(vector3i2);
                if (pathfinderNode.heuristicCost > node.heuristicCost)
                {
                    vector3i = vector3i2;
                    pathfinderNode = node;
                }
                else if (pathfinderNode.heuristicCost == node.heuristicCost && pathfinderNode.curentCost < node.curentCost)
                {
                    vector3i = vector3i2;
                    pathfinderNode = node;
                }
            }
            return vector3i;
        }

        private static List<Vector3i> RecreatePath(SearcherData data)
        {
            List<Vector3i> path = new List<Vector3i>();
            Pathfinder.RecreatePath(data, ref path);
            return path;
        }

        private static void RecreatePath(SearcherData data, ref List<Vector3i> path)
        {
            if (data.area.GetTotalValue(data.destination) <= 0)
            {
                path.Clear();
                return;
            }
            Vector3i vector3i = data.destination;
            int num = data.area.GetTotalValue(vector3i);
            path.Add(vector3i);
            Vector3i vector3i2 = vector3i;
            while (vector3i != data.start)
            {
                for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
                {
                    Vector3i vector3i3 = data.rectArea.KeepHorizontalInside(vector3i + HexNeighbors.neighbours[i]);
                    int curentCost = data.area.GetNode(vector3i3).curentCost;
                    if ((curentCost != 0 || !(vector3i3 != data.start)) && curentCost < num)
                    {
                        vector3i2 = vector3i3;
                        num = curentCost;
                    }
                }
                if (vector3i == vector3i2)
                {
                    path = null;
                    return;
                }
                vector3i = vector3i2;
                path.Add(vector3i);
            }
            path.Reverse();
        }

        private static void PotentNeightboursArea(SearcherData data, Vector3i from)
        {
            if (data.maxCost >= 0 && data.area.GetTotalValue(from) >= data.maxCost)
            {
                return;
            }
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i vector3i = data.rectArea.KeepHorizontalInside(from + HexNeighbors.neighbours[i]);
                if (data.worldHexes.ContainsKey(vector3i))
                {
                    Pathfinder.AddNodeArea(data, from, vector3i);
                }
            }
        }

        private static void AddNodeArea(SearcherData data, Vector3i from, Vector3i current)
        {
            int num = data.MovementCost(from, current);
            if (num <= 0 || current == data.start)
            {
                return;
            }
            int num2 = data.area.GetTotalValue(from) + num;
            int totalValue = data.area.GetTotalValue(current);
            if (totalValue <= 0 || totalValue > num2)
            {
                data.area.SetTotalValue(current, num2);
                if (data.ValidLocation(current))
                {
                    data.qPotentials.Enqueue(current);
                }
                if (totalValue == 0)
                {
                    data.areaAnswer.Add(current);
                }
            }
        }
    }
}
