using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController instance;
    public Light pLight;
    private float defaultValue;

    public static void SetInstensity(float str)
    {
        if (str == 0f)
        {
            instance.pLight.intensity = instance.defaultValue;
        }
        else
        {
            instance.pLight.intensity = str;
        }
    }

    private void Start()
    {
        instance = this;
        this.defaultValue = instance.pLight.intensity;
    }
}

