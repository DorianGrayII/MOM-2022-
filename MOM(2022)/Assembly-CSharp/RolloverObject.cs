using System;
using UnityEngine;

public class RolloverObject : RolloverBase
{
    public object source;
    [Tooltip("Show the detailed tooltip if available")]
    public bool detail;
    [Tooltip("Hide the description if supported")]
    public bool hideDescription;
    public GameObject overrideTooltipPrefab;
    public string optionalMessage;
    public string overrideDescription;
    public string overrideTitle;
    public Texture2D overrideTexture;
    private bool rightClicked;

    public void Clear()
    {
        this.source = null;
        this.optionalMessage = null;
        this.overrideDescription = null;
        this.overrideTitle = null;
    }
}

