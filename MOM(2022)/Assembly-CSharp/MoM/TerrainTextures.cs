namespace MOM
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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

        private void AddAtlasMixerTexture(Texture2D t)
        {
            int texturesSize = this.texturesSize;
            if (this.mixer == null)
            {
                this.mixer = new Texture2DArray(texturesSize, texturesSize, this.mixerNames.Count, TextureFormat.R8, false, true);
            }
            this.AddTexture(t, this.mixer, this.mixerNames);
        }

        public void AddDiffuseTexture(Texture2D t)
        {
            int texturesSize = this.texturesSize;
            if (this.diffuse == null)
            {
                this.diffuse = new Texture2DArray(texturesSize, texturesSize, this.diffuseNames.Count, TextureFormat.RGB24, false, true);
            }
            this.AddTexture(t, this.diffuse, this.diffuseNames);
        }

        public void AddHeightTexture(Texture2D t)
        {
            if ((this.height == null) || (this.height.Length != this.heightNames.Count))
            {
                this.height = new Texture2D[this.heightNames.Count];
            }
            int index = this.heightNames.IndexOf(t.name);
            if (index == -1)
            {
                Debug.LogError("Texture " + t.name + " not found in an array");
            }
            this.height[index] = t;
        }

        public void AddMixerTexture(Texture2D t)
        {
            if ((this.mixers == null) || (this.mixers.Length != this.mixerNames.Count))
            {
                this.mixers = new Texture2D[this.mixerNames.Count];
            }
            int index = this.mixerNames.IndexOf(t.name);
            if (index == -1)
            {
                Debug.LogError("Texture " + t.name + " not found in an array");
            }
            this.mixers[index] = t;
            this.AddAtlasMixerTexture(t);
        }

        public void AddNormalTexture(Texture2D t)
        {
            int texturesSize = this.texturesSize;
            if (this.normal == null)
            {
                this.normal = new Texture2DArray(texturesSize, texturesSize, this.normalNames.Count, TextureFormat.RGB24, false, true);
            }
            this.AddTexture(t, this.normal, this.normalNames);
        }

        public void AddSpecularTexture(Texture2D t)
        {
            int texturesSize = this.texturesSize;
            if (this.specular == null)
            {
                this.specular = new Texture2DArray(texturesSize, texturesSize, this.specularNames.Count, TextureFormat.R8, false, true);
            }
            this.AddTexture(t, this.specular, this.specularNames);
        }

        public void AddTexture(Texture2D t, Texture2DArray target, List<string> names)
        {
            if (t.width != t.height)
            {
                string[] textArray1 = new string[] { "Texture ", t.name, " have irregular shape! it should be square ", t.width.ToString(), " vs ", t.height.ToString() };
                Debug.LogError(string.Concat(textArray1));
            }
            if (((t.width >> 1) * 2) != t.width)
            {
                Debug.LogError("Texture " + t.name + " is not power of 2: " + t.width.ToString());
            }
            int index = names.IndexOf(t.name);
            if (index == -1)
            {
                Debug.LogError("Texture " + t.name + " not found in an array");
            }
            try
            {
                UnityEngine.Color[] colors = new UnityEngine.Color[this.texturesSize * this.texturesSize];
                int num3 = 0;
                while (true)
                {
                    if (num3 >= this.texturesSize)
                    {
                        target.SetPixels(colors, index);
                        break;
                    }
                    int num4 = 0;
                    while (true)
                    {
                        if (num4 >= this.texturesSize)
                        {
                            num3++;
                            break;
                        }
                        UnityEngine.Color pixelBilinear = t.GetPixelBilinear((num3 + 0.5f) / ((float) this.texturesSize), (num4 + 0.5f) / ((float) this.texturesSize));
                        colors[num3 + (num4 * this.texturesSize)] = pixelBilinear;
                        num4++;
                    }
                }
            }
            catch (Exception exception)
            {
                string text1;
                if (exception != null)
                {
                    text1 = exception.ToString();
                }
                else
                {
                    Exception local1 = exception;
                    text1 = null;
                }
                Debug.LogError("Error " + text1);
            }
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

        public void DebugImages()
        {
            Image component = GameObject.Find("TerrainTextureTest").GetComponent<Image>();
            component.material.SetTexture("_MainTex2", this.diffuse);
            component.material.SetFloat("_ArraySize", (float) this.diffuseNames.Count);
        }

        public static TerrainTextures Get()
        {
            if (instance != null)
            {
                return instance;
            }
            if (terrainErrorLogged)
            {
                return null;
            }
            terrainErrorLogged = true;
            Debug.LogError("Use GetInstance if you expect TerrainTextures to be not ready. Unless you are in non-main thread.");
            throw new NullReferenceException();
        }

        public Texture2D GetHeight(string s)
        {
            if ((this.height == null) || ((this.heightNames == null) || string.IsNullOrEmpty(s)))
            {
                return null;
            }
            int index = this.heightNames.IndexOf(s);
            if (index < 0)
            {
                Debug.LogError(s + " not found in height");
            }
            return this.height[index];
        }

        public Texture2DCache GetHeightC(string s)
        {
            if ((this.heightCache == null) || ((this.heightNames == null) || string.IsNullOrEmpty(s)))
            {
                return null;
            }
            int index = this.heightNames.IndexOf(s);
            if (index < 0)
            {
                Debug.LogError(s + " not found in height cached");
            }
            return this.heightCache[index];
        }

        public static TerrainTextures GetInstance(bool forceLoad)
        {
            if ((instance == null) | forceLoad)
            {
                instance = GameObject.Find("terrainAssetFull").GetComponent<TerrainTextures>();
            }
            return instance;
        }

        public Texture2D GetMixer(string s)
        {
            if ((this.mixers == null) || ((this.mixerNames == null) || string.IsNullOrEmpty(s)))
            {
                return null;
            }
            int index = this.mixerNames.IndexOf(s);
            if (index < 0)
            {
                Debug.LogError(s + " not found in mixers");
            }
            return this.mixers[index];
        }

        public Texture2DCache GetMixerC(string s)
        {
            if ((this.mixersCache == null) || ((this.mixerNames == null) || string.IsNullOrEmpty(s)))
            {
                return null;
            }
            int index = this.mixerNames.IndexOf(s);
            if (index < 0)
            {
                Debug.LogError(s + " not found in mixer cached");
            }
            return this.mixersCache[index];
        }

        public int IndexOfDiffuse(string s)
        {
            return (((s == null) || (this.diffuseNames == null)) ? -1 : this.diffuseNames.IndexOf(s));
        }

        public int IndexOfNormal(string s)
        {
            return (((s == null) || (this.normalNames == null)) ? -1 : this.normalNames.IndexOf(s));
        }

        public int IndexOfSpecular(string s)
        {
            return (((s == null) || (this.specularNames == null)) ? -1 : this.specularNames.IndexOf(s));
        }

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

        public void InitializeCache()
        {
            if ((this.heightCache == null) && (this.height != null))
            {
                this.heightCache = new Texture2DCache[this.height.Length];
                for (int i = 0; i < this.height.Length; i++)
                {
                    this.heightCache[i] = new Texture2DCache(this.height[i]);
                }
            }
            if ((this.mixersCache == null) && (this.mixers != null))
            {
                this.mixersCache = new Texture2DCache[this.mixers.Length];
                for (int i = 0; i < this.mixers.Length; i++)
                {
                    this.mixersCache[i] = new Texture2DCache(this.mixers[i]);
                }
            }
        }

        public static void Unload()
        {
            if (instance != null)
            {
                Resources.UnloadAsset(instance);
            }
            instance = null;
        }
    }
}

