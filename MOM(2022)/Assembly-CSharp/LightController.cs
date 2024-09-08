using UnityEngine;

public class LightController : MonoBehaviour
{
    public static LightController instance;

    public Light pLight;

    private float defaultValue;

    private void Start()
    {
        LightController.instance = this;
        this.defaultValue = LightController.instance.pLight.intensity;
    }

    public static void SetInstensity(float str = 0f)
    {
        if (str == 0f)
        {
            LightController.instance.pLight.intensity = LightController.instance.defaultValue;
        }
        else
        {
            LightController.instance.pLight.intensity = str;
        }
    }
}
