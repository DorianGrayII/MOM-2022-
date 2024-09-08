using System;
using System.Collections.Generic;
using System.IO;
using DBDef;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    public class ForegroundFactory : MonoBehaviour
    {
        public delegate void ExportProgress(float f);

        public static int textureSize = 8192;

        public static int atlasDimension = 16;

        public static float waterLevel = 0f;

        private Texture2D foregroundAtlass;

        private static Dictionary<string, ForegroundInstance> data = new Dictionary<string, ForegroundInstance>();

        public static void PlanForegroundForHex(Hex hex, Plane p)
        {
            if (hex.foregroundMesh != null)
            {
                global::UnityEngine.Object.Destroy(hex.foregroundMesh);
                hex.foregroundMesh = null;
            }
            if (hex.foregroundPlan != null)
            {
                hex.foregroundPlan = null;
            }
            Vector2 vector = HexCoordinates.HexToWorld(hex.Position);
            Terrain terrain = hex.GetTerrain();
            MHRandom mHRandom = MHRandom.Get();
            if (terrain.foliage == null)
            {
                return;
            }
            FoliageSet[] foliage = terrain.foliage;
            for (int i = 0; i < foliage.Length; i++)
            {
                Foliage[] foliage2 = foliage[i].foliage;
                foreach (Foliage foliage3 in foliage2)
                {
                    if ((foliage3.battleOnly && !p.battlePlane) || (foliage3.worldOnly && p.battlePlane) || (foliage3.forest && !hex.HaveFlag(ETerrainType.Forest)) || (foliage3.chance > FInt.ZERO && mHRandom.GetFloat(0f, 1f) > foliage3.chance.ToFloat()))
                    {
                        continue;
                    }
                    for (int k = 0; k < foliage3.count; k++)
                    {
                        float num = mHRandom.GetFloat(0f, 0.6f) + mHRandom.GetFloat(0f, 0.6f);
                        float @float = mHRandom.GetFloat(0f, (float)Math.PI * 2f);
                        Vector2 vector2 = new Vector2(Mathf.Cos(@float), Mathf.Sin(@float)) * num;
                        Vector2 vector3 = vector + vector2;
                        Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.y);
                        float heightAt = p.GetHeightAt(vector4, allowUnderwater: true);
                        if (!(heightAt < 0f))
                        {
                            vector4.y = heightAt;
                            if (hex.foregroundPlan == null)
                            {
                                hex.foregroundPlan = new ForegroundMeshPlan();
                            }
                            hex.foregroundPlan.Add(vector4, foliage3, mHRandom, foliage3.allowRotation);
                        }
                    }
                }
            }
        }

        public static void ProduceForegroundForHex(Hex hex, Plane p)
        {
            Chunk chunkFor = p.GetChunkFor(hex.Position);
            hex.ProduceForegroundObjects(p.GetForegroundManterial(), chunkFor);
        }

        public void PrepareFromCache()
        {
            byte[] array = File.ReadAllBytes(Application.streamingAssetsPath + "/Foregrounds/cache.data");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(array, 0, array.Length);
                memoryStream.Position = 0L;
                ForegroundInstance[] fGDef = Serializer.Deserialize<ForegroundInstance[]>(memoryStream);
                this.SetFGDef(fGDef);
            }
            byte[] array2 = File.ReadAllBytes(Application.streamingAssetsPath + "/Foregrounds/cacheTexture.png");
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(array2);
            Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGBA32, mipChain: true, linear: true);
            texture2D2.SetPixels32(texture2D.GetPixels32());
            texture2D2.filterMode = FilterMode.Bilinear;
            texture2D2.Apply(updateMipmaps: true, makeNoLongerReadable: false);
            texture2D2.Compress(highQuality: true);
            texture2D2.Apply(updateMipmaps: true, makeNoLongerReadable: true);
            AssetManager.Get().SetForegroundTexture(texture2D2);
            MemoryManager.RegisterPermanent(texture2D2);
            global::UnityEngine.Object.Destroy(texture2D);
        }

        public static void ExportAssets(List<GameObject> objs, ExportProgress callback)
        {
            Texture2D texture2D = new Texture2D(ForegroundFactory.textureSize, ForegroundFactory.textureSize, TextureFormat.RGBA32, mipChain: true, linear: true);
            List<ForegroundInstance> list = new List<ForegroundInstance>();
            List<Texture2D> textures = new List<Texture2D>();
            Color color = new Color(0.2f, 0.22f, 0.08f, 0f);
            for (int i = 0; i < ForegroundFactory.textureSize; i++)
            {
                for (int j = 0; j < ForegroundFactory.textureSize; j++)
                {
                    texture2D.SetPixel(i, j, color);
                }
            }
            for (int k = 0; k < objs.Count; k++)
            {
                ForegroundFactory.AddAsset(objs[k], list, textures, texture2D);
                callback?.Invoke((float)k / (float)objs.Count);
            }
            texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: false);
            ForegroundInstance[] instance = list.ToArray();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, instance);
                memoryStream.Position = 0L;
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Read(array, 0, (int)memoryStream.Length);
                File.WriteAllBytes(Application.streamingAssetsPath + "/Foregrounds/cache.data", array);
            }
            texture2D.EncodeToPNG();
            Debug.Log("Foreground export cached!");
        }

        public void SetFGDef(ForegroundInstance[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                ForegroundInstance value = d[i];
                ForegroundFactory.data[value.name] = value;
            }
        }

        public static ForegroundInstance GetTree(Foliage foliage)
        {
            if (ForegroundFactory.data == null || !ForegroundFactory.data.ContainsKey(foliage.treeName))
            {
                return default(ForegroundInstance);
            }
            return ForegroundFactory.data[foliage.treeName];
        }

        private static void AddAsset(GameObject source, List<ForegroundInstance> foregroundInstnceData, List<Texture2D> textures, Texture2D targetAtlas)
        {
            MeshFilter component = source.GetComponent<MeshFilter>();
            MeshRenderer component2 = source.GetComponent<MeshRenderer>();
            if (component2.sharedMaterial == null)
            {
                Debug.LogError("[ERROR]Missing shared material for " + source.name);
            }
            ForegroundInstance item = new ForegroundInstance
            {
                name = source.name
            };
            Quaternion localRotation = source.transform.localRotation;
            Vector3 localScale = source.transform.localScale;
            Mesh sharedMesh = component.sharedMesh;
            Vector3[] vertices = sharedMesh.vertices;
            item.vertices = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 a = localRotation * vertices[i];
                item.vertices[i] = Vector3.Scale(a, localScale);
            }
            List<int> list = new List<int>();
            List<Vector2> list2 = new List<Vector2>();
            int[] array = new int[vertices.Length];
            for (int j = 0; j < component2.sharedMaterials.Length; j++)
            {
                Texture2D texture2D = component2.sharedMaterials[j].mainTexture as Texture2D;
                if (texture2D.format == TextureFormat.DXT5Crunched)
                {
                    Debug.LogError("[ERROR]Crunched texture " + texture2D.name + " in " + source.name);
                    continue;
                }
                float num3;
                float num4;
                if (!textures.Contains(texture2D))
                {
                    int count = textures.Count;
                    int num = count % ForegroundFactory.atlasDimension;
                    int num2 = count / ForegroundFactory.atlasDimension;
                    num3 = (float)num / (float)ForegroundFactory.atlasDimension;
                    num4 = (float)num2 / (float)ForegroundFactory.atlasDimension;
                    int num5 = ForegroundFactory.textureSize / ForegroundFactory.atlasDimension;
                    int num6 = Mathf.RoundToInt(num3 * (float)ForegroundFactory.textureSize);
                    int num7 = Mathf.RoundToInt(num4 * (float)ForegroundFactory.textureSize);
                    float num8 = 1f / (float)num5;
                    for (int k = 0; k < num5; k++)
                    {
                        for (int l = 0; l < num5; l++)
                        {
                            float u = ((float)k + 0.5f) * num8;
                            float v = ((float)l + 0.5f) * num8;
                            Color pixelBilinear = texture2D.GetPixelBilinear(u, v);
                            targetAtlas.SetPixel(num6 + k, num7 + l, pixelBilinear);
                        }
                    }
                    textures.Add(texture2D);
                }
                else
                {
                    int num9 = textures.IndexOf(texture2D);
                    int num10 = num9 % ForegroundFactory.atlasDimension;
                    int num11 = num9 / ForegroundFactory.atlasDimension;
                    num3 = (float)num10 / (float)ForegroundFactory.atlasDimension;
                    num4 = (float)num11 / (float)ForegroundFactory.atlasDimension;
                }
                list2.Add(new Vector2(num3, num4));
                int[] indices = sharedMesh.GetIndices(j);
                list.AddRange(indices);
                int[] array2 = indices;
                foreach (int num12 in array2)
                {
                    array[num12] = j;
                }
            }
            float num13 = 1f / (float)ForegroundFactory.atlasDimension;
            item.uv = new Vector2[vertices.Length];
            for (int n = 0; n < sharedMesh.uv.Length; n++)
            {
                Vector2 vector = sharedMesh.uv[n] * num13;
                Vector2 vector2 = list2[array[n]];
                vector.x += vector2.x;
                vector.y += vector2.y;
                item.uv[n] = vector;
            }
            item.uv2 = new Vector2[vertices.Length];
            for (int num14 = 0; num14 < vertices.Length; num14++)
            {
                float num15 = item.vertices[num14].y * localScale.y;
                item.uv2[num14].x = Mathf.Sin(num15 * (float)Math.PI * 0.4f);
            }
            item.indices = list.ToArray();
            foregroundInstnceData.Add(item);
        }
    }
}
