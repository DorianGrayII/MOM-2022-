using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    [Serializable]
    public class TerrainTextures : MonoBehaviour
    {
        private static TerrainTextures instance;

        private static bool terrainErrorLogged;

        public int texturesSize;

        public List<string> diffuseNames;

        public List<string> normalNames;

        public List<string> specularNames;

        public List<string> heightNames;

        public List<string> mixerNames;

        public Texture2DArray diffuse;

        public Texture2DArray normal;

        public Texture2DArray specular;

        public Texture2DArray mixer;

        public Texture2D[] height;

        public Texture2D[] mixers;

        public Texture2DCache[] heightCache;

        public Texture2DCache[] mixersCache;

        public void Initialize(int texturesSize, List<string> diffuseNames, List<string> normalNames, List<string> specularNames, List<string> heightNames, List<string> mixerNames)
        {
            this.texturesSize = texturesSize;
            this.diffuseNames = diffuseNames;
            this.normalNames = normalNames;
            this.specularNames = specularNames;
            this.heightNames = heightNames;
            this.mixerNames = mixerNames;
            this.diffuse = null;
            this.normal = null;
            this.specular = null;
            this.mixer = null;
        }

        public void AddDiffuseTexture(Texture2D t)
        {
            int width = this.texturesSize;
            if (this.diffuse == null)
            {
                this.diffuse = new Texture2DArray(width, width, this.diffuseNames.Count, TextureFormat.RGB24, mipChain: false, linear: true);
            }
            this.AddTexture(t, this.diffuse, this.diffuseNames);
        }

        public void AddNormalTexture(Texture2D t)
        {
            int width = this.texturesSize;
            if (this.normal == null)
            {
                this.normal = new Texture2DArray(width, width, this.normalNames.Count, TextureFormat.RGB24, mipChain: false, linear: true);
            }
            this.AddTexture(t, this.normal, this.normalNames);
        }

        public void AddSpecularTexture(Texture2D t)
        {
            int width = this.texturesSize;
            if (this.specular == null)
            {
                this.specular = new Texture2DArray(width, width, this.specularNames.Count, TextureFormat.R8, mipChain: false, linear: true);
            }
            this.AddTexture(t, this.specular, this.specularNames);
        }

        private void AddAtlasMixerTexture(Texture2D t)
        {
            int width = this.texturesSize;
            if (this.mixer == null)
            {
                this.mixer = new Texture2DArray(width, width, this.mixerNames.Count, TextureFormat.R8, mipChain: false, linear: true);
            }
            this.AddTexture(t, this.mixer, this.mixerNames);
        }

        public void AddHeightTexture(Texture2D t)
        {
            if (this.height == null || this.height.Length != this.heightNames.Count)
            {
                this.height = new Texture2D[this.heightNames.Count];
            }
            int num = this.heightNames.IndexOf(t.name);
            if (num == -1)
            {
                Debug.LogError("Texture " + t.name + " not found in an array");
            }
            this.height[num] = t;
        }

        public void AddMixerTexture(Texture2D t)
        {
            if (this.mixers == null || this.mixers.Length != this.mixerNames.Count)
            {
                this.mixers = new Texture2D[this.mixerNames.Count];
            }
            int num = this.mixerNames.IndexOf(t.name);
            if (num == -1)
            {
                Debug.LogError("Texture " + t.name + " not found in an array");
            }
            this.mixers[num] = t;
            this.AddAtlasMixerTexture(t);
        }

        public void AddTexture(Texture2D t, Texture2DArray target, List<string> names)
        {
            if (t.width != t.height)
            {
                Debug.LogError("Texture " + t.name + " have irregular shape! it should be square " + t.width + " vs " + t.height);
            }
            if ((t.width >> 1) * 2 != t.width)
            {
                Debug.LogError("Texture " + t.name + " is not power of 2: " + t.width);
            }
            int num = names.IndexOf(t.name);
            if (num == -1)
            {
                Debug.LogError("Texture " + t.name + " not found in an array");
            }
            try
            {
                Color[] array = new Color[this.texturesSize * this.texturesSize];
                for (int i = 0; i < this.texturesSize; i++)
                {
                    for (int j = 0; j < this.texturesSize; j++)
                    {
                        Color pixelBilinear = t.GetPixelBilinear(((float)i + 0.5f) / (float)this.texturesSize, ((float)j + 0.5f) / (float)this.texturesSize);
                        array[i + j * this.texturesSize] = pixelBilinear;
                    }
                }
                target.SetPixels(array, num);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error " + ex);
            }
        }

        public int IndexOfDiffuse(string s)
        {
            if (s != null && this.diffuseNames != null)
            {
                return this.diffuseNames.IndexOf(s);
            }
            return -1;
        }

        public int IndexOfNormal(string s)
        {
            if (s != null && this.normalNames != null)
            {
                return this.normalNames.IndexOf(s);
            }
            return -1;
        }

        public int IndexOfSpecular(string s)
        {
            if (s != null && this.specularNames != null)
            {
                return this.specularNames.IndexOf(s);
            }
            return -1;
        }

        public Texture2D GetHeight(string s)
        {
            if (this.height != null && this.heightNames != null && !string.IsNullOrEmpty(s))
            {
                int num = this.heightNames.IndexOf(s);
                if (num < 0)
                {
                    Debug.LogError(s + " not found in height");
                }
                return this.height[num];
            }
            return null;
        }

        public Texture2D GetMixer(string s)
        {
            if (this.mixers != null && this.mixerNames != null && !string.IsNullOrEmpty(s))
            {
                int num = this.mixerNames.IndexOf(s);
                if (num < 0)
                {
                    Debug.LogError(s + " not found in mixers");
                }
                return this.mixers[num];
            }
            return null;
        }

        public void InitializeCache()
        {
            if (this.heightCache == null && this.height != null)
            {
                this.heightCache = new Texture2DCache[this.height.Length];
                for (int i = 0; i < this.height.Length; i++)
                {
                    this.heightCache[i] = new Texture2DCache(this.height[i]);
                }
            }
            if (this.mixersCache == null && this.mixers != null)
            {
                this.mixersCache = new Texture2DCache[this.mixers.Length];
                for (int j = 0; j < this.mixers.Length; j++)
                {
                    this.mixersCache[j] = new Texture2DCache(this.mixers[j]);
                }
            }
        }

        public Texture2DCache GetHeightC(string s)
        {
            if (this.heightCache != null && this.heightNames != null && !string.IsNullOrEmpty(s))
            {
                int num = this.heightNames.IndexOf(s);
                if (num < 0)
                {
                    Debug.LogError(s + " not found in height cached");
                }
                return this.heightCache[num];
            }
            return null;
        }

        public Texture2DCache GetMixerC(string s)
        {
            if (this.mixersCache != null && this.mixerNames != null && !string.IsNullOrEmpty(s))
            {
                int num = this.mixerNames.IndexOf(s);
                if (num < 0)
                {
                    Debug.LogError(s + " not found in mixer cached");
                }
                return this.mixersCache[num];
            }
            return null;
        }

        public void Apply()
        {
            if (this.diffuse != null)
            {
                this.diffuse.Apply();
            }
            if (this.normal != null)
            {
                this.normal.Apply();
            }
            if (this.specular != null)
            {
                this.specular.Apply();
            }
            if (this.mixer != null)
            {
                this.mixer.Apply();
            }
        }

        public static TerrainTextures GetInstance(bool forceLoad)
        {
            if (TerrainTextures.instance == null || forceLoad)
            {
                TerrainTextures.instance = GameObject.Find("terrainAssetFull").GetComponent<TerrainTextures>();
            }
            return TerrainTextures.instance;
        }

        public static TerrainTextures Get()
        {
            if (TerrainTextures.instance == null)
            {
                if (TerrainTextures.terrainErrorLogged)
                {
                    return null;
                }
                TerrainTextures.terrainErrorLogged = true;
                Debug.LogError("Use GetInstance if you expect TerrainTextures to be not ready. Unless you are in non-main thread.");
                throw new NullReferenceException();
            }
            return TerrainTextures.instance;
        }

        public static void Unload()
        {
            if (TerrainTextures.instance != null)
            {
                Resources.UnloadAsset(TerrainTextures.instance);
            }
            TerrainTextures.instance = null;
        }

        public void DebugImages()
        {
            Image component = GameObject.Find("TerrainTextureTest").GetComponent<Image>();
            component.material.SetTexture("_MainTex2", this.diffuse);
            component.material.SetFloat("_ArraySize", this.diffuseNames.Count);
        }
    }
}
