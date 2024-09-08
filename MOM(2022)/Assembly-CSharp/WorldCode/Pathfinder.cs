namespace WorldCode
{
    using MHUtils;
    using System;
    using System.Collections.Generic;

    public class Pathfinder
    {
        private static void AddNodeArea(SearcherData data, Vector3i from, Vector3i current)
        {
            int num = data.MovementCost(from, current);
            if ((num > 0) && (current != data.start))
            {
                int num2 = data.area.GetTotalValue(from) + num;
                int totalValue = data.area.GetTotalValue(current);
                if ((totalValue <= 0) || (totalValue > num2))
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

        private static void AtoBSearch(SearcherData data)
        {
            PotentNeightbours(data, data.start);
            while (true)
            {
                if (data.lPotentials.Count > 0)
                {
                    Vector3i bestNext = GetBestNext(data);
                    data.lPotentials.Remove(bestNext);
                    PotentNeightbours(data, bestNext);
                    if (data.lPotentials.Count <= 300)
                    {
                        continue;
                    }
                }
                return;
            }
        }

        private static bool CalculateCosts(SearcherData data, Vector3i from, Vector3i to)
        {
            int num = data.MovementCost(from, to);
            if ((num == 0) || (to == data.start))
            {
                return false;
            }
            int num2 = data.area.GetTotalValue(from) + num;
            int totalValue = data.area.GetTotalValue(to);
            if ((totalValue > 0) && (totalValue <= num2))
            {
                return false;
            }
            int num4 = num2 + data.rectArea.HexDistance(to, data.destination);
            data.area.SetTotalValue(to, num2);
            data.area.SetHeuristicValue(to, num4);
            return true;
        }

        public static List<Vector3i> FindArea(SearcherData data)
        {
            if (data.areaAnswer == null)
                data.areaAnswer = new List<Vector3i>();
            data.areaAnswer.Add(data.start);
            PotentNeightboursArea(data, data.start);
            while (data.qPotentials.Count > 0)
            {
                Vector3i from = data.qPotentials.Dequeue();
                PotentNeightboursArea(data, from);
            }
            return data.areaAnswer;
        }

        public static List<Vector3i> FindPath(SearcherData data)
        {
            if (!SearchPreparation(data))
            {
                return null;
            }
            int num = data.rectArea.HexDistance(data.start, data.destination);
            if (num == 0)
            {
                List<Vector3i> list1 = new List<Vector3i>();
                list1.Add(data.start);
                return list1;
            }
            if (num != 1)
            {
                AtoBSearch(data);
                return RecreatePath(data);
            }
            List<Vector3i> list2 = new List<Vector3i>();
            list2.Add(data.start);
            list2.Add(data.destination);
            return list2;
        }

        private static Vector3i GetBestNext(SearcherData data)
        {
            Vector3i v = data.lPotentials[0];
            PathfinderNode node = data.area.GetNode(v);
            for (int i = 1; i < data.lPotentials.Count; i++)
            {
                Vector3i vectori2 = data.lPotentials[i];
                PathfinderNode node2 = data.area.GetNode(vectori2);
                if (node.heuristicCost > node2.heuristicCost)
                {
                    v = vectori2;
                    node = node2;
                }
                else if ((node.heuristicCost == node2.heuristicCost) && (node.curentCost < node2.curentCost))
                {
                    v = vectori2;
                    node = node2;
                }
            }
            return v;
        }

        private static void PotentNeightbours(SearcherData data, Vector3i from)
        {
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i worldPosition = data.rectArea.KeepHorizontalInside(from + HexNeighbors.neighbours[i]);
                if (data.rectArea.IsInside(worldPosition, false) && (data.ValidLocation(worldPosition) && CalculateCosts(data, from, worldPosition)))
                {
                    data.lPotentials.Add(worldPosition);
                }
            }
        }

        private static void PotentNeightboursArea(SearcherData data, Vector3i from)
        {
            if ((data.maxCost < 0) || (data.area.GetTotalValue(from) < data.maxCost))
            {
                for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
                {
                    Vector3i key = data.rectArea.KeepHorizontalInside(from + HexNeighbors.neighbours[i]);
                    if (data.worldHexes.ContainsKey(key))
                    {
                        AddNodeArea(data, from, key);
                    }
                }
            }
        }

        private static List<Vector3i> RecreatePath(SearcherData data)
        {
            List<Vector3i> path = new List<Vector3i>();
            RecreatePath(data, ref path);
            return path;
        }

        private static void RecreatePath(SearcherData data, ref List<Vector3i> path)
        {
            if (data.area.GetTotalValue(data.destination) <= 0)
            {
                path.Clear();
            }
            else
            {
                Vector3i destination = data.destination;
                int totalValue = data.area.GetTotalValue(destination);
                path.Add(destination);
                Vector3i vectori2 = destination;
                while (destination != data.start)
                {
                    int index = 0;
                    while (true)
                    {
                        if (index >= HexNeighbors.neighbours.Length)
                        {
                            if (destination == vectori2)
                            {
                                path = null;
                                return;
                            }
                            destination = vectori2;
                            path.Add(destination);
                            break;
                        }
                        Vector3i v = data.rectArea.KeepHorizontalInside(destination + HexNeighbors.neighbours[index]);
                        int curentCost = data.area.GetNode(v).curentCost;
                        if (((curentCost != 0) || (v == data.start)) && (curentCost < totalValue))
                        {
                            vectori2 = v;
                            totalValue = curentCost;
                        }
                        index++;
                    }
                }
                path.Reverse();
            }
        }

        private static bool SearchPreparation(SearcherData data)
        {
            return (data.worldHexes.ContainsKey(data.start) && data.worldHexes.ContainsKey(data.destination));
        }
    }
}

