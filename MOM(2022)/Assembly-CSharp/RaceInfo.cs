using DBDef;
using DBUtils;
using MHUtils;
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

    public Color32 defaultColor = new Color32(44, 34, 26, byte.MaxValue);

    public Color32 highlightColor = new Color32(byte.MaxValue, 240, 0, byte.MaxValue);

    public Color32 penaltyColor = new Color32(203, 30, 0, byte.MaxValue);

    public void Set(Race race, Town town = null)
    {
        if (town == null)
        {
            town = DataBase.GetType<Town>().Find((Town o) => o.race == race);
        }
        else
        {
            race = town.race;
        }
        if ((bool)this.labelName)
        {
            this.labelName.text = race.descriptionInfo.GetLocalizedName();
        }
        if ((bool)this.labelDescription)
        {
            this.labelDescription.text = race.descriptionInfo.GetLocalizedDescription();
        }
        if ((bool)this.labelProductionDescription)
        {
            this.labelProductionDescription.text = global::DBUtils.Localization.Get(race.productionDescription, true);
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

    private void FillAndColorize(TextMeshProUGUI tf, float thresholdValue, FInt actualValue)
    {
        tf.text = actualValue.ToString(1);
        if (actualValue > thresholdValue)
        {
            tf.color = this.highlightColor;
        }
        else if (actualValue < thresholdValue)
        {
            tf.color = this.penaltyColor;
        }
        else
        {
            tf.color = this.defaultColor;
        }
    }
}
