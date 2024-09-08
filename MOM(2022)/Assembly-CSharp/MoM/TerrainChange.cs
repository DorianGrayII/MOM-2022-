namespace MOM
{
    using DBDef;
    using MHUtils;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using WorldCode;

    [ProtoContract]
    public class TerrainChange
    {
        [ProtoMember(1)]
        public bool isArcanus;
        [ProtoMember(2)]
        public Vector3i position;
        [ProtoMember(3)]
        public DBReference<Terrain> terrainChange;
        [ProtoMember(4)]
        public DBReference<Resource> resourceChange;
        [ProtoMember(5)]
        public bool updateResource;

        public TerrainChange()
        {
        }

        public TerrainChange(Hex h, WorldCode.Plane p)
        {
            this.isArcanus = p.arcanusType;
            this.position = h.Position;
            this.terrainChange = h.GetTerrain();
        }

        public TerrainChange(Hex h, DBReference<Resource> r, bool updateResource)
        {
            this.isArcanus = World.GetArcanus().GetHexes().ContainsValue(h);
            this.position = h.Position;
            this.resourceChange = (r == null) ? null : ((DBReference<Resource>) r.Get());
            this.updateResource = updateResource;
        }

        public unsafe void ChangeResource()
        {
            Resource local2;
            WorldCode.Plane plane = World.GetPlanes().Find(o => (o.arcanusType == this.isArcanus) && !o.battlePlane);
            Hex hexAt = plane.GetHexAt(this.position);
            if (hexAt.resourceInstance != null)
            {
                UnityEngine.Object.Destroy(hexAt.resourceInstance);
                hexAt.resourceInstance = null;
            }
            if (this.resourceChange != null)
            {
                local2 = this.resourceChange.Get();
            }
            else
            {
                DBReference<Resource> resourceChange = this.resourceChange;
                local2 = null;
            }
            Resource r = local2;
            hexAt.Resource = r;
            GameObject source = null;
            if (r != null)
            {
                source = AssetManager.Get<GameObject>(ResourceExtension.GetModel3dName(r), true);
            }
            if (source != null)
            {
                Vector3 vector = HexCoordinates.HexToWorld3D(hexAt.Position);
                GameObject obj3 = GameObjectUtils.Instantiate(source, plane.GetChunkFor(hexAt.Position).go.transform);
                obj3.transform.localRotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range((float) 0f, (float) 360f));
                obj3.transform.position = vector;
                hexAt.resourceInstance = obj3;
                List<GameObject> list = null;
                foreach (Transform transform in obj3.transform)
                {
                    Vector3 position = transform.position;
                    position.y = plane.GetHeightAt(position, true);
                    GroundOffset component = transform.gameObject.GetComponent<GroundOffset>();
                    if (component != null)
                    {
                        if (position.y < 0f)
                        {
                            list = new List<GameObject> {
                                transform.gameObject
                            };
                            continue;
                        }
                        float* singlePtr1 = &position.y;
                        singlePtr1[0] += component.heightOffset;
                    }
                    transform.position = position;
                }
                if (list != null)
                {
                    using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            UnityEngine.Object.Destroy(enumerator2.Current);
                        }
                    }
                }
            }
        }

        public void ReApply()
        {
            if (this.terrainChange != null)
            {
                WorldCode.Plane plane = World.GetPlanes().Find(o => (o.arcanusType == this.isArcanus) && !o.battlePlane);
                if (plane != null)
                {
                    plane.UpdateTerrainAt(this.position, this.terrainChange.Get());
                }
            }
            if (this.updateResource)
            {
                this.ChangeResource();
            }
        }
    }
}

