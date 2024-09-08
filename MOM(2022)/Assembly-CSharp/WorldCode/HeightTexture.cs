using System;
using System.IO;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public class HeightTexture
    {
        [ProtoMember(1)]
        public float[] data;

        [ProtoMember(2)]
        public int width;

        [ProtoMember(3)]
        public int height;

        public byte[] GetAsByteSource()
        {
            byte[] array = new byte[this.data.Length * 4 + 8];
            array[0] = (byte)((uint)(this.width >> 24) & 0xFFu);
            array[1] = (byte)((uint)(this.width >> 16) & 0xFFu);
            array[2] = (byte)((uint)(this.width >> 8) & 0xFFu);
            array[3] = (byte)((uint)this.width & 0xFFu);
            array[4] = (byte)((uint)(this.height >> 24) & 0xFFu);
            array[5] = (byte)((uint)(this.height >> 16) & 0xFFu);
            array[6] = (byte)((uint)(this.height >> 8) & 0xFFu);
            array[7] = (byte)((uint)this.height & 0xFFu);
            Buffer.BlockCopy(this.data, 0, array, 8, array.Length - 8);
            return array;
        }

        public void SetFromByteSource(byte[] source)
        {
            this.width = (source[0] << 24) | (source[1] << 16) | (source[2] << 8) | source[3];
            this.height = (source[4] << 24) | (source[5] << 16) | (source[6] << 8) | source[7];
            int num = (source.Length - 8) / 4;
            this.data = new float[num];
            Buffer.BlockCopy(source, 8, this.data, 0, source.Length - 8);
        }

        public static HeightTexture Create(Texture2D source, int channelIndex = 0)
        {
            HeightTexture heightTexture = new HeightTexture();
            Color[] pixels = source.GetPixels();
            heightTexture.data = new float[pixels.Length];
            heightTexture.width = source.width;
            heightTexture.height = source.height;
            for (int i = 0; i < pixels.Length; i++)
            {
                heightTexture.data[i] = pixels[i][channelIndex];
            }
            return heightTexture;
        }

        public float SampleTexture(Vector2 uv)
        {
            if (uv.x < 0f)
            {
                float num = uv.x % 1f;
                uv.x = 1f - num;
            }
            else
            {
                uv.x %= 1f;
            }
            if (uv.y < 0f)
            {
                float num2 = uv.y % 1f;
                uv.y = 1f - num2;
            }
            else
            {
                uv.y %= 1f;
            }
            int num3 = Mathf.Clamp((int)(uv.x * (float)this.width), 0, this.width - 1);
            int num4 = Mathf.Clamp((int)(uv.y * (float)this.height), 0, this.height - 1);
            int num5 = num3 + num4 * this.width;
            return this.data[num5];
        }

        public void ExportToFile(string prefix)
        {
            Texture2D texture2D = new Texture2D(this.width, this.height, TextureFormat.ARGB32, mipChain: false);
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    Color color = new Color(this.data[i + j * this.width], 0.5f, 0.5f, 1f);
                    texture2D.SetPixel(i, j, color);
                }
            }
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            string text = Application.streamingAssetsPath + "/../../TerrainDebug/";
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            File.WriteAllBytes(text + prefix + ".png", bytes);
        }
    }
}
