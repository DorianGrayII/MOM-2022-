using DBDef;
using DBUtils;
using MHUtils;
using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RaceInfo : MonoBehaviour
{
    [FormerlySerializedAs("labelRaceName")]
    public TextMeshProUGUI labelName;
    [FormerlySerializedAs("labelRaceDescription")]
    public TextMeshProUGUI labelDescription;
    public TextMeshProUGUI labelProductionDescription;
    public TextMeshProUGUI labelFoodPerFarmer;
    public TextMeshProUGUI labelProductionPerFarmer;
    public TextMeshProUGUI labelPowerPerFarmer;
    public TextMeshProUGUI labelFoodPerWorker;
    public TextMeshProUGUI labelProductionPerWorker;
    public TextMeshProUGUI labelPowerPerWorker;
    public TextMeshProUGUI labelFoodPerRebel;
    public TextMeshProUGUI labelProductionPerRebel;
    public TextMeshProUGUI labelPowerPerRebel;
    public Color32 defaultColor = new Color32(0x2c, 0x22, 0x1a, 0xff);
    public Color32 highlightColor = new Color32(0xff, 240, 0, 0xff);
    public Color32 penaltyColor = new Color32(0xcb, 30, 0, 0xff);

    private void FillAndColorize(TextMeshProUGUI tf, float thresholdValue, FInt actualValue)
    {
        tf.text = actualValue.ToString(1);
        if (actualValue > thresholdValue)
        {
            tf.color = (Color) this.highlightColor;
        }
        else if (actualValue < thresholdValue)
        {
            tf.color = (Color) this.penaltyColor;
        }
        else
        {
            tf.color = (Color) this.defaultColor;
        }
    }

    public void Set(Race race, Town town)
    {
        if (town == null)
        {
            town = DataBase.GetType<Town>().Find(o => ReferenceEquals(o.race, race));
        }
        else
        {
            race = town.race;
        }
        if (this.labelName)
        {
            this.labelName.text = race.descriptionInfo.GetLocalizedName();
        }
        if (this.labelDescription)
        {
            this.labelDescription.text = race.descriptionInfo.GetLocalizedDescription();
        }
        if (this.labelProductionDescription)
        {
            this.labelProductionDescription.text = DBUtils.Localization.Get(race.productionDescription, true, Array.Empty<object>());
        }
        if (town != null)
        {
            this.FillAndColorize(this.labelFoodPerFarmer, 2f, town.farmer.farmer);
            this.FillAndColorize(this.labelProductionPerFarmer, 0.5f, town.farmer.production);
            this.FillAndColorize(this.labelPowerPerFarmer, 0f, town.farmer.powerProduction);
            this.FillAndColorize(this.labelFoodPerWorker, 0f, town.worker.farmer);
            this.FillAndColorize(this.labelProductionPerWorker, 2f, town.worker.production);
            this.FillAndColorize(this.labelPowerPerWorker, 0f, town.worker.powerProduction);
            this.FillAndColorize(this.labelFoodPerRebel, 0f, town.rebel.farmer);
            this.FillAndColorize(this.labelProductionPerRebel, 0f, town.rebel.production);
            this.FillAndColorize(this.labelPowerPerRebel, 0f, town.rebel.powerProduction);
        }
    }
}

