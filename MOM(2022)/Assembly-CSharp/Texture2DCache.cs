using UnityEngine;

public class Texture2DCache
{
    public string name;

    public int width;

    public int height;

    public float[] depth;

    public float samplerX;

    public float samplerY;

    public float hSamplerX;

    public float hSamplerY;

    public Texture2DCache(Texture2D t)
    {
        this.name = t.name;
        this.width = t.width;
        this.height = t.height;
        this.depth = new float[this.width * this.height];
        Color[] pixels = t.GetPixels();
        for (int i = 0; i < this.height; i++)
        {
            for (int j = 0; j < this.width; j++)
            {
                this.depth[j + i * this.width] = pixels[j + i * this.width].r;
            }
        }
    }

    public float GetBilinear(float u, float v)
    {
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        if (this.samplerX == 0f)
        {
            this.samplerX = 1f / (float)this.width;
            this.samplerY = 1f / (float)this.height;
            this.hSamplerX = this.samplerX / 2f;
            this.hSamplerY = this.samplerY / 2f;
        }
        int num = Mathf.Clamp((int)((u - this.hSamplerX) / this.samplerX), 0, this.width - 1);
        int x = Mathf.Clamp((int)((u + this.hSamplerX) / this.samplerX), 0, this.width - 1);
        int num2 = Mathf.Clamp((int)((v - this.hSamplerY) / this.samplerY), 0, this.height - 1);
        int y = Mathf.Clamp((int)((v + this.hSamplerY) / this.samplerY), 0, this.height - 1);
        int num3 = (int)(u / this.samplerX);
        int num4 = (int)(v / this.samplerY);
        float num5 = (u - (float)num3 * this.samplerX) / this.samplerX;
        float num6 = (v - (float)num4 * this.samplerY) / this.samplerY;
        float num7 = 1f - num5;
        float num8 = num5;
        float num9 = 1f - num6;
        float num10 = num6;
        float num11 = 0f;
        if (num3 == num)
        {
            if (num4 == num2)
            {
                num11 += this.GetPixel(num, num2) * num7 * num9;
                num11 += this.GetPixel(num, y) * num7 * num10;
                num11 += this.GetPixel(x, num2) * num8 * num9;
                return num11 + this.GetPixel(x, y) * num8 * num10;
            }
            num11 += this.GetPixel(num, num2) * num7 * num10;
            num11 += this.GetPixel(num, y) * num7 * num9;
            num11 += this.GetPixel(x, num2) * num8 * num10;
            return num11 + this.GetPixel(x, y) * num8 * num9;
        }
        if (num4 == num2)
        {
            num11 += this.GetPixel(num, num2) * num8 * num9;
            num11 += this.GetPixel(num, y) * num8 * num10;
            num11 += this.GetPixel(x, num2) * num7 * num9;
            return num11 + this.GetPixel(x, y) * num7 * num10;
        }
        num11 += this.GetPixel(num, num2) * num8 * num10;
        num11 += this.GetPixel(num, y) * num8 * num9;
        num11 += this.GetPixel(x, num2) * num7 * num10;
        return num11 + this.GetPixel(x, y) * num7 * num9;
    }

    public float GetPixel(int x, int y)
    {
        return this.depth[x + y * this.width];
    }
}
