namespace MOM
{
    using DBDef;
    using MHUtils;
    using System;
    using TMPro;
    using UnityEngine;

    public class AdventureOutcomeSkill : AdventureOutcome
    {
        public GameObject skill;
        public GameObject skillTarget;
        public TextMeshProUGUI skillName;
        public TextMeshProUGUI targetName;

        public override void Set(AdventureOutcomeDelta.Outcome o)
        {
            base.Set(o);
            Skill thing = o.thing as Skill;
            MOM.Unit additionalData = o.additionalData as MOM.Unit;
            if (thing != null)
            {
                DescriptionInfo descriptionInfo = thing.GetDescriptionInfo();
                this.skill.GetComponentInChildren<RawImage>().texture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                if (string.IsNullOrEmpty(thing.descriptionScript))
                {
                    this.skillName.text = descriptionInfo.GetLocalizedName();
                }
                else
                {
                    object[] parameters = new object[3];
                    parameters[0] = additionalData;
                    parameters[1] = thing;
                    this.skillName.text = (string) ScriptLibrary.Call(thing.descriptionScript, parameters);
                }
                RolloverSimpleTooltip orAddComponent = GameObjectUtils.GetOrAddComponent<RolloverSimpleTooltip>(this.skill);
                orAddComponent.image = DescriptionInfoExtension.GetTexture(descriptionInfo);
                orAddComponent.description = descriptionInfo.GetLocalizedDescription();
                orAddComponent.title = this.skillName.text;
                orAddComponent.sourceAsDbName = null;
            }
            if (additionalData != null)
            {
                DescriptionInfo descriptionInfo = additionalData.GetDescriptionInfo();
                this.skillTarget.GetComponentInChildren<RawImage>((bool) this.skillTarget).texture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                this.targetName.text = descriptionInfo.GetLocalizedName();
                GameObjectUtils.GetOrAddComponent<RolloverObject>(this.skillTarget).source = additionalData;
            }
        }
    }
}

