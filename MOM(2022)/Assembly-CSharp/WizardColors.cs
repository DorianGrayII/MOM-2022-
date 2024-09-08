using System.Collections.Generic;
using MOM;
using UnityEngine;

public class WizardColors : MonoBehaviour
{
    public Color none;

    public Color green;

    public Color blue;

    public Color red;

    public Color purple;

    public Color yellow;

    private static Dictionary<PlayerWizard.Color, Color> cols = new Dictionary<PlayerWizard.Color, Color>();

    public static Color GetColor(PlayerWizard.Color c)
    {
        return WizardColors.cols[c];
    }

    public static Color GetColor(PlayerWizard w)
    {
        if (w == null)
        {
            return WizardColors.cols[PlayerWizard.Color.None];
        }
        return WizardColors.cols[w.color];
    }

    public static Color GetColor(int id)
    {
        if (id == 0)
        {
            return WizardColors.cols[PlayerWizard.Color.None];
        }
        return WizardColors.GetColor(GameManager.GetWizard(id));
    }

    public static string GetHex(PlayerWizard w)
    {
        return ColorUtility.ToHtmlStringRGB(WizardColors.GetColor(w));
    }

    private void Awake()
    {
        WizardColors.cols[PlayerWizard.Color.None] = this.none;
        WizardColors.cols[PlayerWizard.Color.Green] = this.green;
        WizardColors.cols[PlayerWizard.Color.Blue] = this.blue;
        WizardColors.cols[PlayerWizard.Color.Red] = this.red;
        WizardColors.cols[PlayerWizard.Color.Purple] = this.purple;
        WizardColors.cols[PlayerWizard.Color.Yellow] = this.yellow;
        WizardColors.cols[PlayerWizard.Color.MAX] = Color.white;
    }
}
