using System.Collections.Generic;
using System.IO;
using DBDef;
using UnityEngine;

namespace WorldCode
{
    public class TerrainTheme
    {
        public string name;

        public Texture2DArray diffuse;

        public Texture2DArray normal;

        public Texture2DArray diffuseW;

        public Texture2DArray diffuseA;

        public Texture2DArray normalW;

        public Texture2DArray normalA;

        public Texture2D height;

        public HeightTexture mixerMap;

        public HeightTexture heightMap;

        public List<Terrain> terrains;

        public int atlasWidthSize;

        public bool IsLoaded()
        {
            if (this.diffuse != null)
            {
                return this.normal != null;
            }
            return false;
        }

        public void ProduceDataMaps()
        {
            byte[] fromByteSource = File.ReadAllBytes(Application.streamingAssetsPath + "/Terrain/" + this.name + "_m.data");
            this.mixerMap = new HeightTexture();
            this.mixerMap.SetFromByteSource(fromByteSource);
            fromByteSource = File.ReadAllBytes(Application.streamingAssetsPath + "/Terrain/" + this.name + "_h.data");
            this.heightMap = new HeightTexture();
            this.heightMap.SetFromByteSource(fromByteSource);
        }
    }
}
