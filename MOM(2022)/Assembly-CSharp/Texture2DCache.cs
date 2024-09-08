using System;
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
        int num = 0;
        while (num < this.height)
        {
            int num2 = 0;
            while (true)
            {
                if (num2 >= this.width)
                {
                    num++;
                    break;
                }
                this.depth[num2 + (num * this.width)] = pixels[num2 + (num * this.width)].r;
                num2++;
            }
        }
    }

    public float GetBilinear(float u, float v)
    {
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        if (this.samplerX == 0f)
        {
            this.samplerX = 1f / ((float) this.width);
            this.samplerY = 1f / ((float) this.height);
            this.hSamplerX = this.samplerX / 2f;
            this.hSamplerY = this.samplerY / 2f;
        }
        int x = Mathf.Clamp((int) ((u - this.hSamplerX) / this.samplerX), 0, this.width - 1);
        int num2 = Mathf.Clamp((int) ((u + this.hSamplerX) / this.samplerX), 0, this.width - 1);
        int y = Mathf.Clamp((int) ((v - this.hSamplerY) / this.samplerY), 0, this.height - 1);
        int num4 = Mathf.Clamp((int) ((v + this.hSamplerY) / this.samplerY), 0, this.height - 1);
        int num5 = (int) (u / this.samplerX);
        int num6 = (int) (v / this.samplerY);
        float num7 = (u - (num5 * this.samplerX)) / this.samplerX;
        float num8 = (v - (num6 * this.samplerY)) / this.samplerY;
        float num9 = 1f - num7;
        float num10 = num7;
        float num11 = 1f - num8;
        float num12 = num8;
        float num13 = 0f;
        return ((num5 != x) ? ((num6 != y) ? ((((num13 + ((this.GetPixel(x, y) * num10) * num12)) + ((this.GetPixel(x, num4) * num10) * num11)) + ((this.GetPixel(num2, y) * num9) * num12)) + ((this.GetPixel(num2, num4) * num9) * num11)) : ((((num13 + ((this.GetPixel(x, y) * num10) * num11)) + ((this.GetPixel(x, num4) * num10) * num12)) + ((this.GetPixel(num2, y) * num9) * num11)) + ((this.GetPixel(num2, num4) * num9) * num12))) : ((num6 != y) ? ((((num13 + ((this.GetPixel(x, y) * num9) * num12)) + ((this.GetPixel(x, num4) * num9) * num11)) + ((this.GetPixel(num2, y) * num10) * num12)) + ((this.GetPixel(num2, num4) * num10) * num11)) : ((((num13 + ((this.GetPixel(x, y) * num9) * num11)) + ((this.GetPixel(x, num4) * num9) * num12)) + ((this.GetPixel(num2, y) * num10) * num11)) + ((this.GetPixel(num2, num4) * num10) * num12))));
    }

    public float GetPixel(int x, int y)
    {
        return this.depth[x + (y * this.width)];
    }
}

