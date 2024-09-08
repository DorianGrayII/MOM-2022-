using System.Collections.Generic;
using DBUtils;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Owner : MonoBehaviour
{
    public List<GameObject> neutral;

    public List<GameObject> green;

    public List<GameObject> blue;

    public List<GameObject> red;

    public List<GameObject> purple;

    public List<GameObject> yellow;

    public RawImage riWizardPortrait;

    public TextMeshProUGUI labelOwner;

    private Dictionary<PlayerWizard.Color, List<GameObject>> listsByColor = new Dictionary<PlayerWizard.Color, List<GameObject>>();

    private void Awake()
    {
        this.listsByColor.Add(PlayerWizard.Color.None, this.neutral);
        this.listsByColor.Add(PlayerWizard.Color.Green, this.green);
        this.listsByColor.Add(PlayerWizard.Color.Blue, this.blue);
        this.listsByColor.Add(PlayerWizard.Color.Red, this.red);
        this.listsByColor.Add(PlayerWizard.Color.Purple, this.purple);
        this.listsByColor.Add(PlayerWizard.Color.Yellow, this.yellow);
    }

    public void SetWizard(PlayerWizard w)
    {
        PlayerWizard.Color color = PlayerWizard.Color.None;
        if (w != null)
        {
            color = w.color;
            this.riWizardPortrait.gameObject.SetActive(value: true);
            this.riWizardPortrait.texture = w.Graphic;
        }
        foreach (KeyValuePair<PlayerWizard.Color, List<GameObject>> item in this.listsByColor)
        {
            bool active = item.Key == color;
            foreach (GameObject item2 in item.Value)
            {
                item2.SetActive(active);
            }
        }
        this.labelOwner.text = ((w != null) ? w.name : Localization.Get("UI_INDEPENDENT", true));
    }
}
