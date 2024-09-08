using MHUtils;
using System;

public class HeatMap
{
    public int[,] data;
    public V3iRect rect;
    public Vector2i size;
    public bool valid;

    public HeatMap()
    {
    }

    public HeatMap(V3iRect v)
    {
        this.rect = v;
        this.size = new Vector2i(v.AreaWidth, v.AreaHeight);
        this.data = new int[this.size.x, this.size.y];
    }

    public int GetValue(Vector3i p)
    {
        if (!this.valid)
        {
            return -1;
        }
        Vector2i vectori = this.HexToArray(p);
        vectori.x = (vectori.x + this.size.x) % this.size.x;
        vectori.y = (vectori.y + this.size.y) % this.size.y;
        return this.data[vectori.x, vectori.y];
    }

    public Vector2i HexToArray(Vector3i p)
    {
        return this.rect.ConvertHexTo2DCenteredLoation(p);
    }

    public void SetValue(Vector3i p, int value)
    {
        Vector2i vectori = this.HexToArray(p);
        vectori.x = (vectori.x + this.size.x) % this.size.x;
        vectori.y = (vectori.y + this.size.y) % this.size.y;
        this.data[vectori.x, vectori.y] = value;
    }
}

