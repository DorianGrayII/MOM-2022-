using DBDef;
using MOM;
using System;
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
    public Color32 relationColourBad = new Color32(0xff, 0x18, 0, 0xff);
    public Color32 relationColourNeutral = new Color32(0xff, 0xd0, 0, 0xff);
    public Color32 relationColourGood = new Color32(13, 0xd7, 0, 0xff);

    public void Set(PlayerWizard w)
    {
        if (w == null)
        {
            this.labelTreaties.text = "";
            this.labelRelations.text = "";
            this.labelRelations.color = UnityEngine.Color.white;
            this.labelPersonality.text = "";
            this.labelObjective.text = "";
            this.labelWizardName.text = "";
            this.eyesGreen1.SetActive(false);
            this.eyesGreen2.SetActive(false);
            this.eyesYellow1.SetActive(false);
            this.eyesYellow2.SetActive(false);
            this.eyesRed1.SetActive(false);
            this.eyesRed2.SetActive(false);
        }
        else
        {
            DiplomaticStatus statusToward = w.GetDiplomacy().GetStatusToward(GameManager.GetHumanWizard());
            if (this.labelWizardName)
            {
                this.labelWizardName.text = w.GetName();
            }
            this.labelTreaties.text = statusToward.GetTreatiesNamesAsString();
            this.labelRelations.text = DescriptionInfoExtension.GetDILocalizedName(statusToward.GetDBRelationship());
            this.labelPersonality.text = DescriptionInfoExtension.GetDILocalizedDescription(w.GetPersonality());
            this.labelObjective.text = "todo";
            int relationship = statusToward.GetRelationship();
            if (this.eyesGreen1)
            {
                this.eyesGreen1.SetActive(relationship >= 40);
            }
            if (this.eyesGreen2)
            {
                this.eyesGreen2.SetActive(relationship >= 40);
            }
            if (this.eyesYellow1)
            {
                this.eyesYellow1.SetActive((relationship > -40) && (relationship < 40));
            }
            if (this.eyesYellow2)
            {
                this.eyesYellow2.SetActive((relationship > -40) && (relationship < 40));
            }
            if (this.eyesRed1)
            {
                this.eyesRed1.SetActive(relationship <= -40);
            }
            if (this.eyesRed2)
            {
                this.eyesRed2.SetActive(relationship <= -40);
            }
            if (relationship >= 40)
            {
                this.labelRelations.color = (UnityEngine.Color) this.relationColourGood;
            }
            if ((relationship > -40) && (relationship < 40))
            {
                this.labelRelations.color = (UnityEngine.Color) this.relationColourNeutral;
            }
            if (relationship <= -40)
            {
                this.labelRelations.color = (UnityEngine.Color) this.relationColourBad;
            }
        }
    }
}

