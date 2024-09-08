namespace WorldCode
{
    using ProtoBuf;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [ProtoContract]
    public class HeightTexture
    {
        [ProtoMember(1)]
        public float[] data;
        [ProtoMember(2)]
        public int width;
        [ProtoMember(3)]
        public int height;

        public static HeightTexture Create(Texture2D source, int channelIndex)
        {
            HeightTexture texture = new HeightTexture();
            Color[] pixels = source.GetPixels();
            texture.data = new float[pixels.Length];
            texture.width = source.width;
            texture.height = source.height;
            for (int i = 0; i < pixels.Length; i++)
            {
                texture.data[i] = pixels[i][channelIndex];
            }
            return texture;
        }

        public void ExportToFile(string prefix)
        {
            Texture2D tex = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false);
            int x = 0;
            while (x < this.width)
            {
                int y = 0;
                while (true)
                {
                    if (y >= this.height)
                    {
                        x++;
                        break;
                    }
                    Color color = new Color(this.data[x + (y * this.width)], 0.5f, 0.5f, 1f);
                    tex.SetPixel(x, y, color);
                    y++;
                }
            }
            tex.Apply();
            byte[] bytes = ImageConversion.EncodeToPNG(tex);
            string path = Application.streamingAssetsPath + "/../../TerrainDebug/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes(path + prefix + ".png", bytes);
        }

        public byte[] GetAsByteSource()
        {
            byte[] dst = new byte[] { (byte) ((this.width >> 0x18) & 0xff), (byte) ((this.width >> 0x10) & 0xff), (byte) ((this.width >> 8) & 0xff), (byte) (this.width & 0xff), (byte) ((this.height >> 0x18) & 0xff), (byte) ((this.height >> 0x10) & 0xff), (byte) ((this.height >> 8) & 0xff), (byte) (this.height & 0xff) };
            Buffer.BlockCopy(this.data, 0, dst, 8, dst.Length - 8);
            return dst;
        }

        public float SampleTexture(Vector2 uv)
        {
            if (uv.x >= 0f)
            {
                uv.x = uv.x % 1f;
            }
            else
            {
                float num3 = uv.x % 1f;
                uv.x = 1f - num3;
            }
            if (uv.y >= 0f)
            {
                uv.y = uv.y % 1f;
            }
            else
            {
                float num4 = uv.y % 1f;
                uv.y = 1f - num4;
            }
            int index = Mathf.Clamp((int) (uv.x * this.width), 0, this.width - 1) + (Mathf.Clamp((int) (uv.y * this.height), 0, this.height - 1) * this.width);
            return this.data[index];
        }

        public void SetFromByteSource(byte[] source)
        {
            this.width = (((source[0] << 0x18) | (source[1] << 0x10)) | (source[2] << 8)) | source[3];
            this.height = (((source[4] << 0x18) | (source[5] << 0x10)) | (source[6] << 8)) | source[7];
            int num = (source.Length - 8) / 4;
            this.data = new float[num];
            Buffer.BlockCopy(source, 8, this.data, 0, source.Length - 8);
        }
    }
}

