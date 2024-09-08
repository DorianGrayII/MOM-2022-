namespace WorldCode
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class PathfinderV2 : MonoBehaviour
    {
        private static void ConsiderAreaNeighbors(RequestDataV2 rd, int index)
        {
            FInt gCost = rd.GetGCost(index);
            int steps = rd.GetSteps(index);
            PathNode fromN = rd.GetNode(index);
            int dangerSum = rd.GetDangerSum(index);
            int num4 = 0;
            while (true)
            {
                while (true)
                {
                    if (num4 >= 6)
                    {
                        return;
                    }
                    int num5 = (index * 12) + (num4 * 2);
                    int num6 = rd.data.nodeNeighbours[num5];
                    if (num6 != -1)
                    {
                        if (rd.exclusionPoints != null)
                        {
                            Vector3i position = rd.GetPosition(num6);
                            if (rd.exclusionPoints.Contains(position))
                            {
                                break;
                            }
                        }
                        int num7 = rd.data.nodeNeighbours[num5 + 1];
                        rd.EnsureInitialized(num6);
                        PathNode node = rd.GetNode(num6);
                        FInt oNE = rd.RoadMPCost(fromN.pos, node.pos);
                        if (oNE == 0)
                        {
                            oNE = rd.GetEntryCost(fromN, node, num7 > 0, true);
                        }
                        if ((oNE <= 0) && rd.IsTransportReady(num6, node))
                        {
                            oNE = FInt.ONE;
                        }
                        if (rd.movementPool > 0)
                        {
                            FInt num11 = gCost;
                            if (num11 < rd.activeTurnMovementPool)
                            {
                                if ((oNE + num11) >= rd.activeTurnMovementPool)
                                {
                                    oNE = rd.activeTurnMovementPool - num11;
                                }
                            }
                            else
                            {
                                num11 -= rd.activeTurnMovementPool;
                                FInt num12 = ((FInt) rd.movementPool) - (num11 % rd.movementPool);
                                if (num12 == rd.movementPool)
                                {
                                    num12 += rd.movementPool;
                                }
                                if (num12 < oNE)
                                {
                                    oNE = num12;
                                }
                            }
                        }
                        bool flag = false;
                        bool flag2 = rd.IsLocationOccupied(index, false, false);
                        bool flag3 = rd.IsLocationOccupied(num6, false, false);
                        FInt num9 = oNE + gCost;
                        int cDOwnerID = SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo);
                        if (!flag3 && ((cDOwnerID == -1) && ((rd.data.walls != null) && rd.data.walls[num6])))
                        {
                            flag3 = true;
                        }
                        if (flag3)
                        {
                            if (cDOwnerID != SearcherDataV2.GetCDOwnerID(rd.data.units[num6]))
                            {
                                if ((rd.fromID != index) & flag2)
                                {
                                    break;
                                }
                            }
                            else if ((gCost < rd.activeTurnMovementPool) && ((num9 >= rd.activeTurnMovementPool) && flag3))
                            {
                                if ((cDOwnerID < 0) || ((SearcherDataV2.GetCDUnitCount(rd.activeGroupComboInfo) + SearcherDataV2.GetCDUnitCount(rd.data.units[num6])) > 9))
                                {
                                    break;
                                }
                                flag = true;
                            }
                        }
                        if ((oNE > 0) && (node.gCost > num9))
                        {
                            rd.AddToAreaList(num6);
                            rd.SetCameFrom(num6, index);
                            rd.SetCameFromStepCount(num6, steps + 1);
                            rd.SetGCost(num6, num9);
                            rd.SetDangerForNode(num6, dangerSum);
                            if (!flag && ((rd.mpRange > num9) && !rd.IsLocationOccupied(num6, true, false)))
                            {
                                rd.AddToOptionList(num6);
                            }
                        }
                    }
                    break;
                }
                num4++;
            }
        }

        private static bool ConsiderNeighbors(RequestDataV2 rd, int index, bool allowAllyPass)
        {
            PathNode fromN = rd.GetNode(index);
            FInt sumCost = rd.GetSumCost(index);
            FInt gCost = rd.GetGCost(index);
            int steps = rd.GetSteps(index);
            int dangerSum = rd.GetDangerSum(index);
            int num5 = 0;
            while (true)
            {
                while (true)
                {
                    if (num5 >= 6)
                    {
                        return false;
                    }
                    int num6 = (index * 12) + (num5 * 2);
                    int num7 = rd.data.nodeNeighbours[num6];
                    if (num7 != -1)
                    {
                        if (rd.exclusionPoints != null)
                        {
                            Vector3i position = rd.GetPosition(num7);
                            if (rd.exclusionPoints.Contains(position))
                            {
                                break;
                            }
                        }
                        int num8 = rd.data.nodeNeighbours[num6 + 1];
                        rd.EnsureInitialized(num7);
                        int num9 = rd.GetDangerSum(num7);
                        if (((dangerSum + rd.GetLocalDanger(num7)) <= num9) && (rd.GetSumCost(num7) >= sumCost))
                        {
                            PathNode node = rd.GetNode(num7);
                            FInt oNE = rd.RoadMPCost(fromN.pos, node.pos);
                            if (oNE == 0)
                            {
                                oNE = rd.GetEntryCost(fromN, node, num8 > 0, true);
                            }
                            if ((oNE <= 0) && rd.IsTransportReady(num7, node))
                            {
                                oNE = FInt.ONE;
                            }
                            if (rd.avoidLandingOnTarget && ((num7 == rd.toID) && (fromN.water != node.water)))
                            {
                                oNE = FInt.ZERO;
                            }
                            FInt num11 = gCost;
                            if (rd.movementPool > 0)
                            {
                                if (num11 < rd.activeTurnMovementPool)
                                {
                                    if ((oNE + num11) >= rd.activeTurnMovementPool)
                                    {
                                        oNE = rd.activeTurnMovementPool - num11;
                                    }
                                }
                                else
                                {
                                    num11 -= rd.activeTurnMovementPool;
                                    FInt num12 = ((FInt) rd.movementPool) - (num11 % rd.movementPool);
                                    if (num12 == rd.movementPool)
                                    {
                                        num12 += rd.movementPool;
                                    }
                                    if (num12 < oNE)
                                    {
                                        oNE = num12;
                                    }
                                }
                            }
                            bool flag = rd.IsLocationOccupied(index, false, false);
                            if (rd.IsLocationOccupied(num7, false, false))
                            {
                                if (SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo) != SearcherDataV2.GetCDOwnerID(rd.data.units[num7]))
                                {
                                    if ((rd.fromID != index) & flag)
                                    {
                                        break;
                                    }
                                }
                                else if (gCost < rd.activeTurnMovementPool)
                                {
                                    if (num7 == rd.toID)
                                    {
                                        if ((SearcherDataV2.GetCDUnitCount(rd.activeGroupComboInfo) + SearcherDataV2.GetCDUnitCount(rd.data.units[num7])) > 9)
                                        {
                                            break;
                                        }
                                    }
                                    else if ((gCost + oNE) >= rd.activeTurnMovementPool)
                                    {
                                        break;
                                    }
                                }
                            }
                            if ((oNE > 0) && (node.gCost > (oNE + gCost)))
                            {
                                rd.SetCameFrom(num7, index);
                                rd.SetCameFromStepCount(num7, steps + 1);
                                rd.SetGCost(num7, oNE + gCost);
                                rd.SetDangerForNode(num7, dangerSum);
                                if (num7 == rd.toID)
                                {
                                    return true;
                                }
                                if ((steps < 90) && !rd.IsLocationOccupied(num7, allowAllyPass, false))
                                {
                                    rd.AddToOptionList(num7);
                                }
                            }
                        }
                    }
                    break;
                }
                num5++;
            }
        }

        public static void FindArea(RequestDataV2 rd, bool targetsOnly)
        {
            rd.areaStart = -1;
            rd.fromID = rd.data.GetIndex(rd.from);
            rd.toID = -1;
            rd.seekTargetsOnly = targetsOnly;
            if (rd.fromID != -1)
            {
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
                    int bestOption = GetBestOption(rd);
                    if (bestOption == -1)
                    {
                        rd.pathFound = true;
                        return;
                    }
                    rd.RemoveFromOptionList(bestOption);
                    ConsiderAreaNeighbors(rd, bestOption);
                }
            }
        }

        public static void FindPath(RequestDataV2 rd)
        {
            rd.fromID = rd.data.GetIndex(rd.from);
            rd.toID = rd.data.GetIndex(rd.to);
            if (((rd.fromID != -1) && (rd.toID != -1)) && ((rd.GetEntryCost(rd.from, rd.to, false, null) > 0) || (rd.IsWithin(rd.toID) && rd.IsLocationOccupied(rd.toID, false, false))))
            {
                int cDOwnerID = SearcherDataV2.GetCDOwnerID(rd.data.units[rd.toID]);
                int num2 = SearcherDataV2.GetCDOwnerID(rd.activeGroupComboInfo);
                if (!rd.IsLocationOccupied(rd.toID, false, false) || ((cDOwnerID != num2) || ((num2 >= 0) && ((SearcherDataV2.GetCDUnitCount(rd.data.units[rd.toID]) + SearcherDataV2.GetCDUnitCount(rd.activeGroupComboInfo)) <= 9))))
                {
                    rd.EnsureInitialized(rd.fromID);
                    rd.SetGCost(rd.fromID, FInt.ZERO);
                    rd.SetDangerForNode(rd.fromID, 0);
                    rd.AddToOptionList(rd.fromID);
                    while (true)
                    {
                        int bestOption = GetBestOption(rd);
                        if (bestOption == -1)
                        {
                            return;
                        }
                        rd.RemoveFromOptionList(bestOption);
                        if (ConsiderNeighbors(rd, bestOption, rd.allowAllyPassMode))
                        {
                            rd.pathFound = true;
                            return;
                        }
                    }
                }
            }
        }

        private static int GetBestOption(RequestDataV2 rd)
        {
            FInt num = FInt.ONE * 0x2710;
            int num2 = -1;
            for (int i = rd.optionsStart; i > -1; i = rd.GetNextListIndex(i))
            {
                FInt sumCost = rd.GetSumCost(i);
                if (sumCost < num)
                {
                    num2 = i;
                    num = sumCost;
                }
            }
            return num2;
        }

        public static unsafe List<Vector3i> GetDirectLine(Vector3i start, Vector3i end)
        {
            Vector3i vectori = end - start;
            Vector3i vectori2 = new Vector3i((vectori.x < 0) ? -1 : 1, (vectori.y < 0) ? -1 : 1, (vectori.z < 0) ? -1 : 1);
            Vector3i item = start;
            List<Vector3i> list = new List<Vector3i>(HexCoordinates.HexDistance(start, end)) {
                item
            };
            while (true)
            {
                if (item != end)
                {
                    if (item.x == end.x)
                    {
                        vectori2.x = 0;
                    }
                    if (item.y == end.y)
                    {
                        vectori2.y = 0;
                    }
                    if (item.z == end.z)
                    {
                        vectori2.z = 0;
                    }
                    Vector3i vectori4 = item;
                    Vector3i vectori5 = item + vectori2;
                    Vector3i vectori6 = end - vectori4;
                    Vector3i vectori7 = end - vectori5;
                    Vector3 vector = new Vector3(MathF.Abs(((float) vectori6.x) / (vectori.x + 0.01f)), MathF.Abs(((float) vectori6.y) / (vectori.y + 0.01f)), MathF.Abs(((float) vectori6.z) / (vectori.z + 0.01f)));
                    Vector3 vector2 = new Vector3(MathF.Abs(((float) vectori7.x) / (vectori.x + 0.01f)), MathF.Abs(((float) vectori7.y) / (vectori.y + 0.01f)), MathF.Abs(((float) vectori7.z) / (vectori.z + 0.01f)));
                    Vector3 vector3 = vector2 + ((vector - vector2) / 2f);
                    if ((vector3.x < vector3.y) && (vector3.x < vector3.z))
                    {
                        short* numPtr1 = &item.y;
                        numPtr1[0] = (short) (numPtr1[0] + ((vectori.y < 0) ? ((short) (-1)) : ((short) 1)));
                        short* numPtr2 = &item.z;
                        numPtr2[0] = (short) (numPtr2[0] + ((vectori.z < 0) ? ((short) (-1)) : ((short) 1)));
                    }
                    else if (vector3.y < vector3.z)
                    {
                        short* numPtr3 = &item.x;
                        numPtr3[0] = (short) (numPtr3[0] + ((vectori.x < 0) ? ((short) (-1)) : ((short) 1)));
                        short* numPtr4 = &item.z;
                        numPtr4[0] = (short) (numPtr4[0] + ((vectori.z < 0) ? ((short) (-1)) : ((short) 1)));
                    }
                    else
                    {
                        short* numPtr5 = &item.x;
                        numPtr5[0] = (short) (numPtr5[0] + ((vectori.x < 0) ? ((short) (-1)) : ((short) 1)));
                        short* numPtr6 = &item.y;
                        numPtr6[0] = (short) (numPtr6[0] + ((vectori.y < 0) ? ((short) (-1)) : ((short) 1)));
                    }
                    list.Add(item);
                    if (list.Count <= 0x3e8)
                    {
                        continue;
                    }
                }
                return list;
            }
        }

        private static void RectordLocalReach(RequestDataV2 rd, int index)
        {
            if (rd.debugLog == null)
                rd.debugLog = new StringBuilder();
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
                    string[] textArray1 = new string[] { "[", i.ToString(), "]#", node.gCost.ToString(), ",", node.hCost.ToString(), "#", node.pos.ToString() };
                    rd.debugLog.Append(string.Concat(textArray1));
                }
                if (rd.GetCameFrom(num) == -1)
                {
                    return;
                }
            }
        }
    }
}

