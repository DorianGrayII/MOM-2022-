// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.RiverFactory
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;
using WorldCode;

public class RiverFactory
{
    public static bool CreateRiver(MHRandom random, PlaneMeshData meshData, Vector3i pos)
    {
        Vertex start = meshData.GetLocalCells()[pos].Get().corners[random.GetInt(0, 6)].Get();
        List<Vertex> list = null;
        int num;
        for (num = 10; num >= 0; num--)
        {
            list = RiverFactory.GenerateSingleRiver(random, meshData, start);
            if (list != null && list.Count > 3)
            {
                break;
            }
        }
        if (num < 0)
        {
            return false;
        }
        int riverCount = meshData.RiverCount;
        foreach (Vertex item in list)
        {
            item.riverIndex = riverCount;
            item.isRiver = true;
            for (int i = 0; i < item.parentCells.Length; i++)
            {
                if (item.parentCells[i] != null)
                {
                    item.parentCells[i].hex.SetFlag(ETerrainType.RiverBank);
                }
            }
        }
        meshData.AddRiver(list);
        for (int j = 0; j < list.Count - 1; j++)
        {
            Vertex vertex = list[j];
            Vertex vertex2 = list[j + 1];
            MeshCell meshCell = null;
            MeshCell meshCell2 = null;
            for (int k = 0; k < vertex.parentCells.Length; k++)
            {
                if (vertex.parentCells[k] == null)
                {
                    continue;
                }
                for (int l = 0; l < vertex2.parentCells.Length; l++)
                {
                    if (vertex2.parentCells[l] != null && vertex.parentCells[k] == vertex2.parentCells[l])
                    {
                        if (meshCell == null)
                        {
                            meshCell = vertex.parentCells[k];
                        }
                        else
                        {
                            meshCell2 = vertex.parentCells[k];
                        }
                    }
                }
            }
            if (meshCell == null || meshCell2 == null)
            {
                continue;
            }
            for (int m = 0; m < meshCell.neighbourCells.Length; m++)
            {
                if (meshCell.neighbourCells[m].Get() == meshCell2)
                {
                    if (meshCell.hex.viaRiver == null)
                    {
                        meshCell.hex.viaRiver = new bool[6];
                    }
                    int num2 = m;
                    meshCell.hex.viaRiver[num2] = true;
                    meshCell.viaRiverFlags |= 1 << num2;
                    if (meshCell2.hex.viaRiver == null)
                    {
                        meshCell2.hex.viaRiver = new bool[6];
                    }
                    num2 = (m + 3) % 6;
                    meshCell2.hex.viaRiver[num2] = true;
                    meshCell2.viaRiverFlags |= 1 << num2;
                }
            }
        }
        return true;
    }

    private static List<Vertex> GenerateSingleRiver(MHRandom random, PlaneMeshData planeMeshData, Vertex start)
    {
        List<Vertex> list = new List<Vertex>();
        list.Add(start);
        Vertex vertex = start;
        Vertex previousRiverPoint = null;
        Vector3 prefDirection = Vector3.zero;
        while (vertex.parentCells[0] != null && vertex.parentCells[1] != null && vertex.parentCells[2] != null)
        {
            Vertex vertex2 = RiverFactory.NextRiverDirectionGuided(random, vertex, previousRiverPoint, ref prefDirection);
            previousRiverPoint = vertex;
            vertex = vertex2;
            if (list.Contains(vertex))
            {
                int num = list.IndexOf(vertex);
                list.RemoveRange(num + 1, list.Count - num - 1);
                previousRiverPoint = ((num <= 0) ? null : list[num - 1]);
                continue;
            }
            list.Add(vertex);
            if (vertex.isRiver)
            {
                return null;
            }
            bool flag = false;
            int num2 = 0;
            for (int i = 0; i < vertex.parentCells.Length; i++)
            {
                MeshCell meshCell = vertex.parentCells[i];
                if (meshCell == null)
                {
                    flag = true;
                    break;
                }
                if (meshCell.hex.HaveFlag(ETerrainType.Sea))
                {
                    list.Add(meshCell.center);
                    flag = true;
                    break;
                }
                if (!meshCell.hex.HaveAnyFlag(6))
                {
                    num2++;
                }
            }
            if (num2 < 2)
            {
                return null;
            }
            if (flag)
            {
                break;
            }
        }
        return list;
    }

    private static Vertex NextRiverDirectionGuided(MHRandom random, Vertex riverPoint, Vertex previousRiverPoint, ref Vector3 prefDirection)
    {
        Vertex vertex = RiverFactory.NextRiverDirection(random, riverPoint, previousRiverPoint);
        Vertex vertex2 = RiverFactory.NextRiverDirection(random, riverPoint, previousRiverPoint);
        Vector3 vector = vertex.basePosition - riverPoint.basePosition;
        Vector3 vector2 = vertex2.basePosition - riverPoint.basePosition;
        if (previousRiverPoint == null && random.GetInt(0, 2) == 0)
        {
            if (previousRiverPoint == null)
            {
                prefDirection = vector2;
            }
            return vertex2;
        }
        if (Vector3.Dot(vector, prefDirection) > Vector3.Dot(vector2, prefDirection))
        {
            if (previousRiverPoint == null)
            {
                prefDirection = vector;
            }
            return vertex;
        }
        if (previousRiverPoint == null)
        {
            prefDirection = vector2;
        }
        return vertex2;
    }

    private static Vertex NextRiverDirection(MHRandom random, Vertex riverPoint, Vertex previousRiverPoint)
    {
        int num;
        if (previousRiverPoint == null)
        {
            num = random.GetInt(0, riverPoint.neighbourCorners.Length);
        }
        else
        {
            num = random.GetInt(0, riverPoint.neighbourCorners.Length);
            if (riverPoint.neighbourCorners[num] == previousRiverPoint)
            {
                num = (num + random.GetInt(1, 3)) % 3;
            }
        }
        return riverPoint.neighbourCorners[num];
    }
}
