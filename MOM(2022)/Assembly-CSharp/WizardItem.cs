using MOM;
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
        PlayerWizard.Color color = PlayerWizard.Color.None;
        if (w != null)
        {
            color = w.color;
        }
        if ((bool)this.none)
        {
            this.none.SetActive(color == PlayerWizard.Color.None);
        }
        if ((bool)this.blue)
        {
            this.blue.SetActive(color == PlayerWizard.Color.Blue);
        }
        if ((bool)this.red)
        {
            this.red.SetActive(color == PlayerWizard.Color.Red);
        }
        if ((bool)this.green)
        {
            this.green.SetActive(color == PlayerWizard.Color.Green);
        }
        if ((bool)this.yellow)
        {
            this.yellow.SetActive(color == PlayerWizard.Color.Yellow);
        }
        if ((bool)this.purple)
        {
            this.purple.SetActive(color == PlayerWizard.Color.Purple);
        }
        if ((bool)this.labelName)
        {
            this.labelName.gameObject.SetActive(w != null);
            this.labelName.text = w?.name;
        }
        if ((bool)this.image)
        {
            this.image.gameObject.SetActive(w != null);
            this.image.texture = w?.Graphic;
        }
    }
}
