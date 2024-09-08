namespace WorldCode
{
    using DBDef;
    using MHUtils;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ForegroundFactory : MonoBehaviour
    {
        public static int textureSize = 0x2000;
        public static int atlasDimension = 0x10;
        public static float waterLevel = 0f;
        private Texture2D foregroundAtlass;
        private static Dictionary<string, ForegroundInstance> data = new Dictionary<string, ForegroundInstance>();

        private static unsafe void AddAsset(GameObject source, List<ForegroundInstance> foregroundInstnceData, List<Texture2D> textures, Texture2D targetAtlas)
        {
            MeshRenderer component = source.GetComponent<MeshRenderer>();
            if (component.sharedMaterial == null)
            {
                Debug.LogError("[ERROR]Missing shared material for " + source.name);
            }
            ForegroundInstance item = new ForegroundInstance {
                name = source.name
            };
            Quaternion localRotation = source.transform.localRotation;
            Vector3 localScale = source.transform.localScale;
            Mesh sharedMesh = source.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = sharedMesh.vertices;
            item.vertices = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 a = (Vector3) (localRotation * vertices[i]);
                item.vertices[i] = Vector3.Scale(a, localScale);
            }
            List<int> list = new List<int>();
            List<Vector2> list2 = new List<Vector2>();
            int[] numArray = new int[vertices.Length];
            for (int j = 0; j < component.sharedMaterials.Length; j++)
            {
                Texture2D mainTexture = component.sharedMaterials[j].mainTexture as Texture2D;
                if (mainTexture.format == TextureFormat.DXT5Crunched)
                {
                    Debug.LogError("[ERROR]Crunched texture " + mainTexture.name + " in " + source.name);
                }
                else
                {
                    float num4;
                    float num5;
                    if (textures.Contains(mainTexture))
                    {
                        int index = textures.IndexOf(mainTexture);
                        int num15 = index % atlasDimension;
                        num4 = ((float) num15) / ((float) atlasDimension);
                        num5 = ((float) (index / atlasDimension)) / ((float) atlasDimension);
                    }
                    else
                    {
                        int count = textures.Count;
                        int num6 = count % atlasDimension;
                        num4 = ((float) num6) / ((float) atlasDimension);
                        num5 = ((float) (count / atlasDimension)) / ((float) atlasDimension);
                        int num7 = textureSize / atlasDimension;
                        int num8 = Mathf.RoundToInt(num4 * textureSize);
                        int num9 = Mathf.RoundToInt(num5 * textureSize);
                        float num10 = 1f / ((float) num7);
                        int num11 = 0;
                        while (true)
                        {
                            if (num11 >= num7)
                            {
                                textures.Add(mainTexture);
                                break;
                            }
                            int num12 = 0;
                            while (true)
                            {
                                if (num12 >= num7)
                                {
                                    num11++;
                                    break;
                                }
                                float u = (num11 + 0.5f) * num10;
                                float v = (num12 + 0.5f) * num10;
                                Color pixelBilinear = mainTexture.GetPixelBilinear(u, v);
                                targetAtlas.SetPixel(num8 + num11, num9 + num12, pixelBilinear);
                                num12++;
                            }
                        }
                    }
                    list2.Add(new Vector2(num4, num5));
                    int[] indices = sharedMesh.GetIndices(j);
                    list.AddRange(indices);
                    foreach (int num17 in indices)
                    {
                        numArray[num17] = j;
                    }
                }
            }
            float num = 1f / ((float) atlasDimension);
            item.uv = new Vector2[vertices.Length];
            for (int k = 0; k < sharedMesh.uv.Length; k++)
            {
                Vector2 vector3 = sharedMesh.uv[k] * num;
                Vector2 vector4 = list2[numArray[k]];
                float* singlePtr1 = &vector3.x;
                singlePtr1[0] += vector4.x;
                float* singlePtr2 = &vector3.y;
                singlePtr2[0] += vector4.y;
                item.uv[k] = vector3;
            }
            item.uv2 = new Vector2[vertices.Length];
            for (int m = 0; m < vertices.Length; m++)
            {
                float num20 = item.vertices[m].y * localScale.y;
                item.uv2[m].x = Mathf.Sin((num20 * 3.141593f) * 0.4f);
            }
            item.indices = list.ToArray();
            foregroundInstnceData.Add(item);
        }

        public static void ExportAssets(List<GameObject> objs, ExportProgress callback)
        {
            Texture2D targetAtlas = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, true, true);
            List<ForegroundInstance> foregroundInstnceData = new List<ForegroundInstance>();
            List<Texture2D> textures = new List<Texture2D>();
            Color color = new Color(0.2f, 0.22f, 0.08f, 0f);
            int x = 0;
            while (x < textureSize)
            {
                int y = 0;
                while (true)
                {
                    if (y >= textureSize)
                    {
                        x++;
                        break;
                    }
                    targetAtlas.SetPixel(x, y, color);
                    y++;
                }
            }
            for (int i = 0; i < objs.Count; i++)
            {
                AddAsset(objs[i], foregroundInstnceData, textures, targetAtlas);
                if (callback != null)
                {
                    callback(((float) i) / ((float) objs.Count));
                }
            }
            targetAtlas.Apply(true, false);
            ForegroundInstance[] instance = foregroundInstnceData.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize<ForegroundInstance[]>(stream, instance);
                stream.Position = 0L;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int) stream.Length);
                File.WriteAllBytes(Application.streamingAssetsPath + "/Foregrounds/cache.data", buffer);
            }
            ImageConversion.EncodeToPNG(targetAtlas);
            Debug.Log("Foreground export cached!");
        }

        public static ForegroundInstance GetTree(Foliage foliage)
        {
            if ((data != null) && data.ContainsKey(foliage.treeName))
            {
                return data[foliage.treeName];
            }
            return new ForegroundInstance();
        }

        public static void PlanForegroundForHex(Hex hex, WorldCode.Plane p)
        {
            if (hex.foregroundMesh != null)
            {
                Destroy(hex.foregroundMesh);
                hex.foregroundMesh = null;
            }
            if (hex.foregroundPlan != null)
            {
                hex.foregroundPlan = null;
            }
            Vector2 vector = HexCoordinates.HexToWorld(hex.Position);
            Terrain terrain = hex.GetTerrain();
            MHRandom random = MHRandom.Get();
            if (terrain.foliage != null)
            {
                FoliageSet[] setArray = terrain.foliage;
                int index = 0;
                while (index < setArray.Length)
                {
                    Foliage[] foliageArray = setArray[index].foliage;
                    int num2 = 0;
                    while (true)
                    {
                        if (num2 >= foliageArray.Length)
                        {
                            index++;
                            break;
                        }
                        Foliage foliage = foliageArray[num2];
                        if (((!foliage.battleOnly || p.battlePlane) && ((!foliage.worldOnly || !p.battlePlane) && (!foliage.forest || hex.HaveFlag(ETerrainType.Forest)))) && ((foliage.chance <= FInt.ZERO) || (random.GetFloat(0f, 1f) <= foliage.chance.ToFloat())))
                        {
                            for (int i = 0; i < foliage.count; i++)
                            {
                                float num4 = random.GetFloat(0f, 0.6f) + random.GetFloat(0f, 0.6f);
                                float @float = random.GetFloat(0f, 6.283185f);
                                Vector2 vector2 = new Vector2(Mathf.Cos(@float), Mathf.Sin(@float)) * num4;
                                Vector2 vector3 = vector + vector2;
                                Vector3 pos = new Vector3(vector3.x, 0f, vector3.y);
                                float heightAt = p.GetHeightAt(pos, true);
                                if (heightAt >= 0f)
                                {
                                    pos.y = heightAt;
                                    if (hex.foregroundPlan == null)
                                        hex.foregroundPlan = new ForegroundMeshPlan();
                                    hex.foregroundPlan.Add(pos, foliage, random, foliage.allowRotation);
                                }
                            }
                        }
                        num2++;
                    }
                }
            }
        }

        public void PrepareFromCache()
        {
            byte[] buffer = File.ReadAllBytes(Application.streamingAssetsPath + "/Foregrounds/cache.data");
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Position = 0L;
                ForegroundInstance[] d = Serializer.Deserialize<ForegroundInstance[]>(stream);
                this.SetFGDef(d);
            }
            Texture2D tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, File.ReadAllBytes(Application.streamingAssetsPath + "/Foregrounds/cacheTexture.png"));
            Texture2D textured2 = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, true, true);
            textured2.SetPixels32(tex.GetPixels32());
            textured2.filterMode = FilterMode.Bilinear;
            textured2.Apply(true, false);
            textured2.Compress(true);
            textured2.Apply(true, true);
            AssetManager.Get().SetForegroundTexture(textured2);
            MemoryManager.RegisterPermanent(textured2);
            Destroy(tex);
        }

        public static void ProduceForegroundForHex(Hex hex, WorldCode.Plane p)
        {
            Chunk chunkFor = p.GetChunkFor(hex.Position);
            hex.ProduceForegroundObjects(p.GetForegroundManterial(), chunkFor);
        }

        public void SetFGDef(ForegroundInstance[] d)
        {
            foreach (ForegroundInstance instance in d)
            {
                data[instance.name] = instance;
            }
        }

        public delegate void ExportProgress(float f);
    }
}

