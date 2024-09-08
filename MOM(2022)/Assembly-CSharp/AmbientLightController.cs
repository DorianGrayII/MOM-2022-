// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AmbientLightController
using UnityEngine;

public class AmbientLightController : MonoBehaviour
{
    private static AmbientLightController instance;

    public Color ambientColorWorld;

    public Color ambientCutscene;

    private Color prevAmbientColor;

    private void Start()
    {
        AmbientLightController.instance = this;
        RenderSettings.ambientLight = this.ambientColorWorld;
    }

    public static void ChangeAmbientColor()
    {
        if (RenderSettings.ambientLight == AmbientLightController.instance.ambientColorWorld)
        {
            RenderSettings.ambientLight = AmbientLightController.instance.ambientCutscene;
        }
        else
        {
            RenderSettings.ambientLight = AmbientLightController.instance.ambientColorWorld;
        }
    }
}
