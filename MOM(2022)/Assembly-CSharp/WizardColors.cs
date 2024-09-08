using MOM;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WizardColors : MonoBehaviour
{
    public UnityEngine.Color none;
    public UnityEngine.Color green;
    public UnityEngine.Color blue;
    public UnityEngine.Color red;
    public UnityEngine.Color purple;
    public UnityEngine.Color yellow;
    private static Dictionary<PlayerWizard.Color, UnityEngine.Color> cols = new Dictionary<PlayerWizard.Color, UnityEngine.Color>();

    private void Awake()
    {
        cols[PlayerWizard.Color.None] = this.none;
        cols[PlayerWizard.Color.Green] = this.green;
        cols[PlayerWizard.Color.Blue] = this.blue;
        cols[PlayerWizard.Color.Red] = this.red;
        cols[PlayerWizard.Color.Purple] = this.purple;
        cols[PlayerWizard.Color.Yellow] = this.yellow;
        cols[PlayerWizard.Color.MAX] = UnityEngine.Color.white;
    }

    public static UnityEngine.Color GetColor(PlayerWizard w)
    {
        return ((w != null) ? cols[w.color] : cols[PlayerWizard.Color.None]);
    }

    public static UnityEngine.Color GetColor(PlayerWizard.Color c)
    {
        return cols[c];
    }

    public static UnityEngine.Color GetColor(int id)
    {
        return ((id != 0) ? GetColor(GameManager.GetWizard(id)) : cols[PlayerWizard.Color.None]);
    }

    public static string GetHex(PlayerWizard w)
    {
        return ColorUtility.ToHtmlStringRGB(GetColor(w));
    }
}

