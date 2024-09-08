using MOM;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PosProcessingLibrary : MonoBehaviour
{
    public PostProcessingProfile arcanusPosProcess;
    public PostProcessingProfile myrrorPosProcess;
    private PostProcessingBehaviour ppBehaviour;
    private static PosProcessingLibrary instance;
    private static bool initialized;

    public static void ArcanusGamma(int value)
    {
        ColorGradingModel.Settings settings = instance.arcanusPosProcess.colorGrading.settings;
        settings.basic.postExposure = value * 0.01f;
        instance.arcanusPosProcess.colorGrading.settings = settings;
    }

    private void Awake()
    {
        instance = this;
        this.ppBehaviour = base.gameObject.GetComponent<PostProcessingBehaviour>();
    }

    private static void EnsureInitialization()
    {
        if (!initialized)
        {
            initialized = true;
            ArcanusGamma(MOM.Settings.GetData().Get<int>(MOM.Settings.Name.arcanusGamma));
            MyrrorGamma(MOM.Settings.GetData().Get<int>(MOM.Settings.Name.myrrorGamma));
        }
    }

    public static void MyrrorGamma(int value)
    {
        ColorGradingModel.Settings settings = instance.myrrorPosProcess.colorGrading.settings;
        settings.basic.postExposure = value * 0.01f;
        instance.myrrorPosProcess.colorGrading.settings = settings;
    }

    public static void SetArcanusMode()
    {
        EnsureInitialization();
        instance.ppBehaviour.profile = instance.arcanusPosProcess;
    }

    public static void SetMyrrorMode()
    {
        EnsureInitialization();
        instance.ppBehaviour.profile = instance.myrrorPosProcess;
    }
}

