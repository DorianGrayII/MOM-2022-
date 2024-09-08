using MOM;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PosProcessingLibrary : MonoBehaviour
{
    public PostProcessingProfile arcanusPosProcess;

    public PostProcessingProfile myrrorPosProcess;

    private PostProcessingBehaviour ppBehaviour;

    private static PosProcessingLibrary instance;

    private static bool initialized;

    private void Awake()
    {
        PosProcessingLibrary.instance = this;
        this.ppBehaviour = base.gameObject.GetComponent<PostProcessingBehaviour>();
    }

    public static void SetArcanusMode()
    {
        PosProcessingLibrary.EnsureInitialization();
        PosProcessingLibrary.instance.ppBehaviour.profile = PosProcessingLibrary.instance.arcanusPosProcess;
    }

    public static void SetMyrrorMode()
    {
        PosProcessingLibrary.EnsureInitialization();
        PosProcessingLibrary.instance.ppBehaviour.profile = PosProcessingLibrary.instance.myrrorPosProcess;
    }

    private static void EnsureInitialization()
    {
        if (!PosProcessingLibrary.initialized)
        {
            PosProcessingLibrary.initialized = true;
            PosProcessingLibrary.ArcanusGamma(Settings.GetData().Get<int>(Settings.Name.arcanusGamma));
            PosProcessingLibrary.MyrrorGamma(Settings.GetData().Get<int>(Settings.Name.myrrorGamma));
        }
    }

    public static void ArcanusGamma(int value)
    {
        ColorGradingModel.Settings settings = PosProcessingLibrary.instance.arcanusPosProcess.colorGrading.settings;
        settings.basic.postExposure = (float)value * 0.01f;
        PosProcessingLibrary.instance.arcanusPosProcess.colorGrading.settings = settings;
    }

    public static void MyrrorGamma(int value)
    {
        ColorGradingModel.Settings settings = PosProcessingLibrary.instance.myrrorPosProcess.colorGrading.settings;
        settings.basic.postExposure = (float)value * 0.01f;
        PosProcessingLibrary.instance.myrrorPosProcess.colorGrading.settings = settings;
    }
}
