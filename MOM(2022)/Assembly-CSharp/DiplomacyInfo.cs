using DBDef;
using MOM;
using TMPro;
using UnityEngine;

public class DiplomacyInfo : MonoBehaviour
{
    public TextMeshProUGUI labelRelations;

    public TextMeshProUGUI labelTreaties;

    public TextMeshProUGUI labelPersonality;

    public TextMeshProUGUI labelObjective;

    public TextMeshProUGUI labelWizardName;

    public GameObject eyesGreen1;

    public GameObject eyesGreen2;

    public GameObject eyesYellow1;

    public GameObject eyesYellow2;

    public GameObject eyesRed1;

    public GameObject eyesRed2;

    public Color32 relationColourBad = new Color32(byte.MaxValue, 24, 0, byte.MaxValue);

    public Color32 relationColourNeutral = new Color32(byte.MaxValue, 208, 0, byte.MaxValue);

    public Color32 relationColourGood = new Color32(13, 215, 0, byte.MaxValue);

    public void Set(PlayerWizard w)
    {
        if (w == null)
        {
            this.labelTreaties.text = "";
            this.labelRelations.text = "";
            this.labelRelations.color = Color.white;
            this.labelPersonality.text = "";
            this.labelObjective.text = "";
            this.labelWizardName.text = "";
            this.eyesGreen1.SetActive(value: false);
            this.eyesGreen2.SetActive(value: false);
            this.eyesYellow1.SetActive(value: false);
            this.eyesYellow2.SetActive(value: false);
            this.eyesRed1.SetActive(value: false);
            this.eyesRed2.SetActive(value: false);
            return;
        }
        DiplomaticStatus statusToward = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard());
        if ((bool)this.labelWizardName)
        {
            this.labelWizardName.text = w.GetName();
        }
        this.labelTreaties.text = statusToward.GetTreatiesNamesAsString();
        this.labelRelations.text = statusToward.GetDBRelationship().GetDILocalizedName();
        this.labelPersonality.text = w.GetPersonality().GetDILocalizedDescription();
        this.labelObjective.text = "todo";
        int relationship = statusToward.GetRelationship();
        if ((bool)this.eyesGreen1)
        {
            this.eyesGreen1.SetActive(relationship >= 40);
        }
        if ((bool)this.eyesGreen2)
        {
            this.eyesGreen2.SetActive(relationship >= 40);
        }
        if ((bool)this.eyesYellow1)
        {
            this.eyesYellow1.SetActive(relationship > -40 && relationship < 40);
        }
        if ((bool)this.eyesYellow2)
        {
            this.eyesYellow2.SetActive(relationship > -40 && relationship < 40);
        }
        if ((bool)this.eyesRed1)
        {
            this.eyesRed1.SetActive(relationship <= -40);
        }
        if ((bool)this.eyesRed2)
        {
            this.eyesRed2.SetActive(relationship <= -40);
        }
        if (relationship >= 40)
        {
            this.labelRelations.color = this.relationColourGood;
        }
        if (relationship > -40 && relationship < 40)
        {
            this.labelRelations.color = this.relationColourNeutral;
        }
        if (relationship <= -40)
        {
            this.labelRelations.color = this.relationColourBad;
        }
    }
}
