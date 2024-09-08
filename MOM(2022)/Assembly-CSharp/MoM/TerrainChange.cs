using System.Collections.Generic;
using DBDef;
using MHUtils;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
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

        public TerrainChange(Hex h, global::WorldCode.Plane p)
        {
            this.isArcanus = p.arcanusType;
            this.position = h.Position;
            this.terrainChange = h.GetTerrain();
        }

        public TerrainChange(Hex h, DBReference<Resource> r, bool updateResource)
        {
            this.isArcanus = World.GetArcanus().GetHexes().ContainsValue(h);
            this.position = h.Position;
            if (r != null)
            {
                this.resourceChange = r.Get();
            }
            else
            {
                this.resourceChange = null;
            }
            this.updateResource = updateResource;
        }

        public void ReApply()
        {
            if (this.terrainChange != null)
            {
                World.GetPlanes().Find((global::WorldCode.Plane o) => o.arcanusType == this.isArcanus && !o.battlePlane)?.UpdateTerrainAt(this.position, this.terrainChange.Get());
            }
            if (this.updateResource)
            {
                this.ChangeResource();
            }
        }

        public void ChangeResource()
        {
            global::WorldCode.Plane plane = World.GetPlanes().Find((global::WorldCode.Plane o) => o.arcanusType == this.isArcanus && !o.battlePlane);
            Hex hexAt = plane.GetHexAt(this.position);
            if (hexAt.resourceInstance != null)
            {
                Object.Destroy(hexAt.resourceInstance);
                hexAt.resourceInstance = null;
            }
            Resource resource = this.resourceChange?.Get();
            hexAt.Resource = resource;
            GameObject gameObject = null;
            if (resource != null)
            {
                gameObject = AssetManager.Get<GameObject>(resource.GetModel3dName());
            }
            if (gameObject == null)
            {
                return;
            }
            Vector3 vector = HexCoordinates.HexToWorld3D(hexAt.Position);
            Chunk chunkFor = plane.GetChunkFor(hexAt.Position);
            GameObject gameObject2 = GameObjectUtils.Instantiate(gameObject, chunkFor.go.transform);
            gameObject2.transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
            gameObject2.transform.position = vector;
            hexAt.resourceInstance = gameObject2;
            List<GameObject> list = null;
            foreach (Transform item in gameObject2.transform)
            {
                Vector3 pos = item.position;
                pos.y = plane.GetHeightAt(pos, allowUnderwater: true);
                GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
                if (component != null)
                {
                    if (pos.y < 0f)
                    {
                        if (list == null)
                        {
                            list = new List<GameObject>();
                        }
                        list.Add(item.gameObject);
                        continue;
                    }
                    pos.y += component.heightOffset;
                }
                item.position = pos;
            }
            if (list == null)
            {
                return;
            }
            foreach (GameObject item2 in list)
            {
                Object.Destroy(item2);
            }
        }
    }
}
