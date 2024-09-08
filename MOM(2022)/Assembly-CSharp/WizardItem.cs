using MOM;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WizardItem : MonoBehaviour
{
    public GameObject none;
    public GameObject blue;
    public GameObject red;
    public GameObject green;
    public GameObject yellow;
    public GameObject purple;
    public TextMeshProUGUI labelName;
    public RawImage image;

    public void Set(PlayerWizard w)
    {
        PlayerWizard.Color none = PlayerWizard.Color.None;
        if (w != null)
        {
            none = w.color;
        }
        if (this.none)
        {
            this.none.SetActive(none == PlayerWizard.Color.None);
        }
        if (this.blue)
        {
            this.blue.SetActive(none == PlayerWizard.Color.Blue);
        }
        if (this.red)
        {
            this.red.SetActive(none == PlayerWizard.Color.Red);
        }
        if (this.green)
        {
            this.green.SetActive(none == PlayerWizard.Color.Green);
        }
        if (this.yellow)
        {
            this.yellow.SetActive(none == PlayerWizard.Color.Yellow);
        }
        if (this.purple)
        {
            this.purple.SetActive(none == PlayerWizard.Color.Purple);
        }
        if (this.labelName)
        {
            this.labelName.gameObject.SetActive(w != null);
            this.labelName.text = (w != null) ? w.name : null;
        }
        if (this.image)
        {
            this.image.gameObject.SetActive(w != null);
            this.image.texture = w?.Graphic;
        }
    }
}

