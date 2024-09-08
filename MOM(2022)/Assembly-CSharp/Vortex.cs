// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Vortex
using System.Collections;
using System.Collections.Generic;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;

public class Vortex
{
    private Vector3i position;

    private GameObject model;

    private int lifeTime = 10;

    public bool attacker;

    private readonly int[] map = new int[6] { 2, 5, 3, 1, 4, 0 };

    public static void CreateVortex(Battle b, Vector3i position, bool attacker)
    {
        Vortex vortex = new Vortex();
        if (b.vortexList == null)
        {
            b.vortexList = new List<Vortex>();
        }
        b.vortexList.Add(vortex);
        vortex.position = position;
        GameObject original = AssetManager.Get<GameObject>("Effect_MagicVortexLoop");
        vortex.model = Object.Instantiate(original, b.plane.GetChunkFor(Vector3i.zero).go.transform);
        MHZombieMemoryDetector.Track(vortex.model);
        Vector3 pos = HexCoordinates.HexToWorld3D(position);
        pos.y = b.plane.GetHeightAt(pos);
        vortex.model.transform.position = pos;
        vortex.attacker = attacker;
    }

    public IEnumerator Update(Battle b)
    {
        int dir = this.ChooseDirection(this.attacker ? 5 : 2, b);
        this.lifeTime--;
        bool destroy = this.lifeTime <= 0;
        float maxT = 0.6f;
        for (int i = 0; i < 4; i++)
        {
            if (dir == -1)
            {
                destroy = true;
                break;
            }
            Vector3i p = this.position + HexNeighbors.neighbours[dir];
            if (!b.plane.area.IsInside(p))
            {
                destroy = true;
                break;
            }
            Vector3 posA = HexCoordinates.HexToWorld3D(this.position);
            Vector3 posB = HexCoordinates.HexToWorld3D(p);
            float t = 0f;
            while (t < maxT)
            {
                t = Mathf.Min(maxT, t + Time.deltaTime);
                Vector3 pos = Vector3.Lerp(posA, posB, t / maxT);
                pos.y = b.plane.GetHeightAt(pos);
                this.model.transform.position = pos;
                if (Settings.GetData().GetBattleCameraFollow())
                {
                    CameraController.CenterAt(pos);
                }
                yield return null;
            }
            this.position = p;
            BattleUnit unitAt = b.GetUnitAt(this.position);
            if (unitAt != null)
            {
                this.ApplyDoomDmg(unitAt);
            }
            Vector3i[] neighbours = HexNeighbors.neighbours;
            foreach (Vector3i vector3i in neighbours)
            {
                if (Random.Range(0f, 1f) < 0.33f)
                {
                    Vector3i pos2 = this.position + vector3i;
                    unitAt = b.GetUnitAt(pos2);
                    if (unitAt != null)
                    {
                        this.ApplyPiercingDmg(unitAt);
                    }
                }
            }
            dir = this.ChooseDirection(dir, b);
        }
        if (destroy)
        {
            if (b.vortexList != null && b.vortexList.Contains(this))
            {
                b.vortexList.Remove(this);
            }
            if (this.model != null)
            {
                Object.Destroy(this.model);
            }
        }
    }

    public int ChooseDirection(int prevDir, Battle b)
    {
        int @int = new MHRandom().GetInt(0, 6);
        for (int i = 0; i < 12; i++)
        {
            int num = (i + @int + 6) % this.map.Length;
            int num2 = prevDir + this.map[num];
            num2 %= 6;
            int num3 = this.map[num];
            if (i >= 6 || num3 == 5 || num3 == 0 || num3 == 1)
            {
                Vector3i item = this.position + HexNeighbors.neighbours[num2];
                HashSet<Vector3i> exclusionPoints = b.plane.exclusionPoints;
                if (exclusionPoints == null || !exclusionPoints.Contains(item))
                {
                    return num2;
                }
            }
        }
        return -1;
    }

    private void ApplyDoomDmg(BattleUnit bu)
    {
        int[] damage = new int[1] { 5 };
        bool canDefend = bu.canDefend;
        bu.canDefend = false;
        bu.ApplyDamage(damage, new MHRandom(), null, 0);
        bu.canDefend = canDefend;
        VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
        bu.GetOrCreateFormation().UpdateFigureCount();
    }

    private void ApplyPiercingDmg(BattleUnit bu)
    {
        int[] damage = new int[1] { 5 };
        FInt attFinal = bu.GetAttFinal(TAG.DEFENCE);
        bu.ApplyDamage(damage, new MHRandom(), null, -(attFinal.ToInt() / 2));
        VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
        bu.GetOrCreateFormation().UpdateFigureCount();
    }
}
