using DBUtils;
using MOM;
using System;
using System.Collections.Generic;
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
        PlayerWizard.Color none = PlayerWizard.Color.None;
        if (w != null)
        {
            none = w.color;
            this.riWizardPortrait.gameObject.SetActive(true);
            this.riWizardPortrait.texture = w.Graphic;
        }
        foreach (KeyValuePair<PlayerWizard.Color, List<GameObject>> pair in this.listsByColor)
        {
            bool flag = ((PlayerWizard.Color) pair.Key) == none;
            using (List<GameObject>.Enumerator enumerator2 = pair.Value.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.SetActive(flag);
                }
            }
        }
        this.labelOwner.text = (w != null) ? w.name : Localization.Get("UI_INDEPENDENT", true, Array.Empty<object>());
    }
}

