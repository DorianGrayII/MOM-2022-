using DBDef;
using MHUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class AdventureOutcomeSkill : AdventureOutcome
    {
        public GameObject skill;

        public GameObject skillTarget;

        public TextMeshProUGUI skillName;

        public TextMeshProUGUI targetName;

        public override void Set(AdventureOutcomeDelta.Outcome o)
        {
            base.Set(o);
            Skill skill = o.thing as Skill;
            Unit unit = o.additionalData as Unit;
            if (skill != null)
            {
                DescriptionInfo descriptionInfo = skill.GetDescriptionInfo();
                this.skill.GetComponentInChildren<RawImage>().texture = descriptionInfo.GetTexture();
                if (!string.IsNullOrEmpty(skill.descriptionScript))
                {
                    this.skillName.text = (string)ScriptLibrary.Call(skill.descriptionScript, unit, skill, null);
                }
                else
                {
                    this.skillName.text = descriptionInfo.GetLocalizedName();
                }
                RolloverSimpleTooltip orAddComponent = this.skill.GetOrAddComponent<RolloverSimpleTooltip>();
                orAddComponent.image = descriptionInfo.GetTexture();
                orAddComponent.description = descriptionInfo.GetLocalizedDescription();
                orAddComponent.title = this.skillName.text;
                orAddComponent.sourceAsDbName = null;
            }
            if (unit != null)
            {
                DescriptionInfo descriptionInfo2 = unit.GetDescriptionInfo();
                this.skillTarget.GetComponentInChildren<RawImage>(this.skillTarget).texture = descriptionInfo2.GetTexture();
                this.targetName.text = descriptionInfo2.GetLocalizedName();
                this.skillTarget.GetOrAddComponent<RolloverObject>().source = unit;
            }
        }
    }
}
