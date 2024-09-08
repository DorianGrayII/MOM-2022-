using System;
using System.Collections.Generic;
using System.Text;
using MHUtils;
using UnityEngine;

namespace WorldCode
{
    public class PathfinderV2 : MonoBehaviour
    {
        public static void FindPath(RequestDataV2 rd)
        {
            rd.fromID = rd.data.GetIndex(rd.from);
            rd.toID = rd.data.GetIndex(rd.to);
            if (rd.fromID == -1 || rd.toID == -1 || (rd.GetEntryCost(rd.from, rd.to) <= 0 && (!rd.IsWithin(rd.toID) || !rd.IsLocationOccupied(rd.toID))))
            {
                return;
            }
            bool num = rd.IsLocationOccupied(rd.toID);
            int cDOwnerID = SearcherDataV2.GetCDOwnerID(rd.data.units[rd.toID]);
            int cDOwnerID2 = SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo);
            if (num && cDOwnerID == cDOwnerID2 && (cDOwnerID2 < 0 || SearcherDataV2.GetCDUnitCount(rd.data.units[rd.toID]) + SearcherDataV2.GetCDUnitCount(rd.activeGroupComboInfo) > 9))
            {
                return;
            }
            rd.EnsureInitialized(rd.fromID);
            rd.SetGCost(rd.fromID, FInt.ZERO);
            rd.SetDangerForNode(rd.fromID, 0);
            rd.AddToOptionList(rd.fromID);
            int bestOption;
            do
            {
                bestOption = PathfinderV2.GetBestOption(rd);
                if (bestOption == -1)
                {
                    return;
                }
                rd.RemoveFromOptionList(bestOption);
            }
            while (!PathfinderV2.ConsiderNeighbors(rd, bestOption, rd.allowAllyPassMode));
            rd.pathFound = true;
        }

        public static void FindArea(RequestDataV2 rd, bool targetsOnly = false)
        {
            rd.areaStart = -1;
            rd.fromID = rd.data.GetIndex(rd.from);
            rd.toID = -1;
            rd.seekTargetsOnly = targetsOnly;
            if (rd.fromID == -1)
            {
                return;
            }
            rd.EnsureInitialized(rd.fromID);
            rd.SetGCost(rd.fromID, FInt.ZERO);
            rd.SetDangerForNode(rd.fromID, 0);
            if (rd.mpRange > 0)
            {
                rd.AddToOptionList(rd.fromID);
            }
            rd.AddToAreaList(rd.fromID);
            while (true)
            {
                int bestOption = PathfinderV2.GetBestOption(rd);
                if (bestOption == -1)
                {
                    break;
                }
                rd.RemoveFromOptionList(bestOption);
                PathfinderV2.ConsiderAreaNeighbors(rd, bestOption);
            }
            rd.pathFound = true;
        }

        private static int GetBestOption(RequestDataV2 rd)
        {
            FInt fInt = FInt.ONE * 10000;
            int result = -1;
            for (int num = rd.optionsStart; num > -1; num = rd.GetNextListIndex(num))
            {
                FInt sumCost = rd.GetSumCost(num);
                if (sumCost < fInt)
                {
                    result = num;
                    fInt = sumCost;
                }
            }
            return result;
        }

        private static bool ConsiderNeighbors(RequestDataV2 rd, int index, bool allowAllyPass)
        {
            PathNode node = rd.GetNode(index);
            FInt sumCost = rd.GetSumCost(index);
            FInt gCost = rd.GetGCost(index);
            int steps = rd.GetSteps(index);
            int dangerSum = rd.GetDangerSum(index);
            for (int i = 0; i < 6; i++)
            {
                int num = index * 12 + i * 2;
                int num2 = rd.data.nodeNeighbours[num];
                if (num2 == -1)
                {
                    continue;
                }
                if (rd.exclusionPoints != null)
                {
                    Vector3i position = rd.GetPosition(num2);
                    if (rd.exclusionPoints.Contains(position))
                    {
                        continue;
                    }
                }
                int num3 = rd.data.nodeNeighbours[num + 1];
                rd.EnsureInitialized(num2);
                int dangerSum2 = rd.GetDangerSum(num2);
                if (dangerSum + rd.GetLocalDanger(num2) > dangerSum2 || rd.GetSumCost(num2) < sumCost)
                {
                    continue;
                }
                PathNode node2 = rd.GetNode(num2);
                FInt fInt = rd.RoadMPCost(node.pos, node2.pos);
                if (fInt == 0)
                {
                    fInt = rd.GetEntryCost(node, node2, num3 > 0, includeDangers: true);
                }
                if (fInt <= 0 && rd.IsTransportReady(num2, node2))
                {
                    fInt = FInt.ONE;
                }
                if (rd.avoidLandingOnTarget && num2 == rd.toID && node.water != node2.water)
                {
                    fInt = FInt.ZERO;
                }
                FInt fInt2 = gCost;
                if (rd.movementPool > 0)
                {
                    if (fInt2 < rd.activeTurnMovementPool)
                    {
                        if (fInt + fInt2 >= rd.activeTurnMovementPool)
                        {
                            fInt = rd.activeTurnMovementPool - fInt2;
                        }
                    }
                    else
                    {
                        fInt2 -= rd.activeTurnMovementPool;
                        FInt fInt3 = rd.movementPool - fInt2 % rd.movementPool;
                        if (fInt3 == rd.movementPool)
                        {
                            fInt3 += rd.movementPool;
                        }
                        if (fInt3 < fInt)
                        {
                            fInt = fInt3;
                        }
                    }
                }
                bool flag = rd.IsLocationOccupied(index);
                if (rd.IsLocationOccupied(num2))
                {
                    int cDOwnerID = SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo);
                    int cDOwnerID2 = SearcherDataV2.GetCDOwnerID(rd.data.units[num2]);
                    if (cDOwnerID != cDOwnerID2)
                    {
                        if (rd.fromID != index && flag)
                        {
                            continue;
                        }
                    }
                    else if (gCost < rd.activeTurnMovementPool)
                    {
                        if (num2 == rd.toID)
                        {
                            int cDUnitCount = SearcherDataV2.GetCDUnitCount(rd.activeGroupComboInfo);
                            int cDUnitCount2 = SearcherDataV2.GetCDUnitCount(rd.data.units[num2]);
                            if (cDUnitCount + cDUnitCount2 > 9)
                            {
                                continue;
                            }
                        }
                        else if (gCost + fInt >= rd.activeTurnMovementPool)
                        {
                            continue;
                        }
                    }
                }
                if (fInt > 0 && node2.gCost > fInt + gCost)
                {
                    rd.SetCameFrom(num2, index);
                    rd.SetCameFromStepCount(num2, steps + 1);
                    rd.SetGCost(num2, fInt + gCost);
                    rd.SetDangerForNode(num2, dangerSum);
                    if (num2 == rd.toID)
                    {
                        return true;
                    }
                    if (steps < 90 && !rd.IsLocationOccupied(num2, allowAllyPass))
                    {
                        rd.AddToOptionList(num2);
                    }
                }
            }
            return false;
        }

        private static void ConsiderAreaNeighbors(RequestDataV2 rd, int index)
        {
            FInt gCost = rd.GetGCost(index);
            int steps = rd.GetSteps(index);
            PathNode node = rd.GetNode(index);
            int dangerSum = rd.GetDangerSum(index);
            for (int i = 0; i < 6; i++)
            {
                int num = index * 12 + i * 2;
                int num2 = rd.data.nodeNeighbours[num];
                if (num2 == -1)
                {
                    continue;
                }
                if (rd.exclusionPoints != null)
                {
                    Vector3i position = rd.GetPosition(num2);
                    if (rd.exclusionPoints.Contains(position))
                    {
                        continue;
                    }
                }
                int num3 = rd.data.nodeNeighbours[num + 1];
                rd.EnsureInitialized(num2);
                PathNode node2 = rd.GetNode(num2);
                FInt fInt = rd.RoadMPCost(node.pos, node2.pos);
                if (fInt == 0)
                {
                    fInt = rd.GetEntryCost(node, node2, num3 > 0, includeDangers: true);
                }
                if (fInt <= 0 && rd.IsTransportReady(num2, node2))
                {
                    fInt = FInt.ONE;
                }
                if (rd.movementPool > 0)
                {
                    FInt fInt2 = gCost;
                    if (fInt2 < rd.activeTurnMovementPool)
                    {
                        if (fInt + fInt2 >= rd.activeTurnMovementPool)
                        {
                            fInt = rd.activeTurnMovementPool - fInt2;
                        }
                    }
                    else
                    {
                        fInt2 -= rd.activeTurnMovementPool;
                        FInt fInt3 = rd.movementPool - fInt2 % rd.movementPool;
                        if (fInt3 == rd.movementPool)
                        {
                            fInt3 += rd.movementPool;
                        }
                        if (fInt3 < fInt)
                        {
                            fInt = fInt3;
                        }
                    }
                }
                bool flag = false;
                bool flag2 = rd.IsLocationOccupied(index);
                bool flag3 = rd.IsLocationOccupied(num2);
                FInt fInt4 = fInt + gCost;
                int cDOwnerID = SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo);
                if (!flag3 && cDOwnerID == -1 && rd.data.walls != null && rd.data.walls[num2])
                {
                    flag3 = true;
                }
                if (flag3)
                {
                    int cDOwnerID2 = SearcherDataV2.GetCDOwnerID(rd.data.units[num2]);
                    if (cDOwnerID != cDOwnerID2)
                    {
                        if (rd.fromID != index && flag2)
                        {
                            continue;
                        }
                    }
                    else if (gCost < rd.activeTurnMovementPool && fInt4 >= rd.activeTurnMovementPool && flag3)
                    {
                        if (cDOwnerID < 0)
                        {
                            continue;
                        }
                        int cDUnitCount = SearcherDataV2.GetCDUnitCount(rd.activeGroupComboInfo);
                        int cDUnitCount2 = SearcherDataV2.GetCDUnitCount(rd.data.units[num2]);
                        if (cDUnitCount + cDUnitCount2 > 9)
                        {
                            continue;
                        }
                        flag = true;
                    }
                }
                if (fInt > 0 && node2.gCost > fInt4)
                {
                    rd.AddToAreaList(num2);
                    rd.SetCameFrom(num2, index);
                    rd.SetCameFromStepCount(num2, steps + 1);
                    rd.SetGCost(num2, fInt4);
                    rd.SetDangerForNode(num2, dangerSum);
                    if (!flag && rd.mpRange > fInt4 && !rd.IsLocationOccupied(num2, allowAllyPass: true))
                    {
                        rd.AddToOptionList(num2);
                    }
                }
            }
        }

        private static void RectordLocalReach(RequestDataV2 rd, int index)
        {
            if (rd.debugLog == null)
            {
                rd.debugLog = new StringBuilder();
            }
            rd.debugLog.AppendLine();
            int num = index;
            for (int i = 0; i < 11; i++)
            {
                if (i == 10)
                {
                    rd.debugLog.Append("...");
                }
                else
                {
                    PathNode node = rd.GetNode(num);
                    StringBuilder debugLog = rd.debugLog;
                    string[] obj = new string[8]
                    {
                        "[",
                        i.ToString(),
                        "]#",
                        null,
                        null,
                        null,
                        null,
                        null
                    };
                    FInt gCost = node.gCost;
                    obj[3] = gCost.ToString();
                    obj[4] = ",";
                    gCost = node.hCost;
                    obj[5] = gCost.ToString();
                    obj[6] = "#";
                    Vector3i pos = node.pos;
                    obj[7] = pos.ToString();
                    debugLog.Append(string.Concat(obj));
                }
                num = rd.GetCameFrom(num);
                if (num == -1)
                {
                    break;
                }
            }
        }

        public static List<Vector3i> GetDirectLine(Vector3i start, Vector3i end)
        {
            Vector3i vector3i = end - start;
            Vector3i vector3i2 = new Vector3i((vector3i.x >= 0) ? 1 : (-1), (vector3i.y >= 0) ? 1 : (-1), (vector3i.z >= 0) ? 1 : (-1));
            Vector3i vector3i3 = start;
            List<Vector3i> list = new List<Vector3i>(HexCoordinates.HexDistance(start, end));
            list.Add(vector3i3);
            while (vector3i3 != end)
            {
                if (vector3i3.x == end.x)
                {
                    vector3i2.x = 0;
                }
                if (vector3i3.y == end.y)
                {
                    vector3i2.y = 0;
                }
                if (vector3i3.z == end.z)
                {
                    vector3i2.z = 0;
                }
                Vector3i vector3i4 = vector3i3;
                Vector3i vector3i5 = vector3i3 + vector3i2;
                Vector3i vector3i6 = end - vector3i4;
                Vector3i vector3i7 = end - vector3i5;
                Vector3 vector = new Vector3(Mathf.Abs((float)vector3i6.x / ((float)vector3i.x + 0.01f)), Mathf.Abs((float)vector3i6.y / ((float)vector3i.y + 0.01f)), Mathf.Abs((float)vector3i6.z / ((float)vector3i.z + 0.01f)));
                Vector3 vector2 = new Vector3(Mathf.Abs((float)vector3i7.x / ((float)vector3i.x + 0.01f)), Mathf.Abs((float)vector3i7.y / ((float)vector3i.y + 0.01f)), Mathf.Abs((float)vector3i7.z / ((float)vector3i.z + 0.01f)));
                Vector3 vector3 = vector2 + (vector - vector2) / 2f;
                if (vector3.x < vector3.y && vector3.x < vector3.z)
                {
                    vector3i3.y += (short)((vector3i.y >= 0) ? 1 : (-1));
                    vector3i3.z += (short)((vector3i.z >= 0) ? 1 : (-1));
                }
                else if (vector3.y < vector3.z)
                {
                    vector3i3.x += (short)((vector3i.x >= 0) ? 1 : (-1));
                    vector3i3.z += (short)((vector3i.z >= 0) ? 1 : (-1));
                }
                else
                {
                    vector3i3.x += (short)((vector3i.x >= 0) ? 1 : (-1));
                    vector3i3.y += (short)((vector3i.y >= 0) ? 1 : (-1));
                }
                list.Add(vector3i3);
                if (list.Count > 1000)
                {
                    break;
                }
            }
            return list;
        }
    }
}
