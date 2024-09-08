// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.AdventureOutcome
using System;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureOutcome : MonoBehaviour
{
    [Serializable]
    public class TypeData
    {
        public Types outcomeType;

        public Sprite sprite;

        public string gainTitle;

        public string loseTitle;
    }

    public enum Types
    {
        None = 0,
        Building = 1,
        Town = 2,
        Fame = 3,
        Food = 4,
        Gold = 5,
        Hero = 6,
        HeroStat = 7,
        Item = 8,
        Mana = 9,
        Population = 10,
        Power = 11,
        Production = 12,
        ResearchPoints = 13,
        Resource = 14,
        Spell = 15,
        Spellbook = 16,
        Trait = 17,
        Unit = 18,
        UnitStat = 19,
        Unrest = 20,
        Enchantment = 21,
        HeroDied = 22,
        UnitDied = 23,
        Skill = 24,
        CastingSkillDevelopment = 25
    }

    public enum StatType
    {
        None = 0,
        Melee = 1,
        MeleeHitChance = 2,
        Ranged = 3,
        RangedHitChance = 4,
        Armour = 5,
        Resist = 6,
        Hits = 7,
        MovementPoints = 8,
        Experience = 9,
        DefenceChance = 10
    }

    public TextMeshProUGUI heading;

    public TextMeshProUGUI label;

    public TextMeshProUGUI delta;

    public TextMeshProUGUI duration;

    public MaskableGraphic icon;

    [Tooltip("If the outcome object should have an automated simple tooltip")]
    public RolloverObject rollover;

    [Tooltip("List of game objects to display when delta is positive (Give)")]
    public List<GameObject> positive;

    [Tooltip("List of game objects to display when delta is negative (Take)")]
    public List<GameObject> negative;

    [Tooltip("List of game objects to display when showing a duration / number of turns")]
    public List<GameObject> hasDuration;

    public List<TypeData> typeData;

    public virtual void Set(AdventureOutcomeDelta.Outcome o)
    {
        TypeData typeData = this.typeData.Find((TypeData t) => t.outcomeType == o.outcomeType);
        global::MOM.Artefact artefact = o.thing as global::MOM.Artefact;
        DescriptionInfo descriptionInfo = o.thing as DescriptionInfo;
        if (descriptionInfo == null && artefact == null)
        {
            if (o.thing is IDescriptionInfoType descriptionInfoType)
            {
                descriptionInfo = descriptionInfoType.GetDescriptionInfo();
            }
            else if (o.thing is BaseUnit baseUnit)
            {
                descriptionInfo = baseUnit.GetDescriptionInfo();
            }
        }
        if ((bool)this.rollover)
        {
            this.rollover.source = o.thing;
        }
        if ((bool)this.label)
        {
            this.label.gameObject.SetActive(descriptionInfo != null || artefact != null);
            if (descriptionInfo != null)
            {
                this.label.text = descriptionInfo.GetLocalizedName();
            }
            else if (artefact != null)
            {
                this.label.text = artefact.name;
            }
        }
        if ((bool)this.delta)
        {
            this.delta.gameObject.SetActive(o.delta != 0);
            this.delta.text = o.delta.ToString("+#;-#");
        }
        if ((descriptionInfo != null || artefact != null) && this.icon is RawImage rawImage)
        {
            if (descriptionInfo != null)
            {
                rawImage.texture = descriptionInfo.GetTexture();
            }
            else
            {
                rawImage.texture = AssetManager.Get<Texture2D>(artefact.graphic);
            }
        }
        else if (this.icon is Image image)
        {
            image.sprite = typeData.sprite;
        }
        if ((bool)this.heading && typeData != null)
        {
            this.heading.text = global::DBUtils.Localization.Get((o.delta > 0) ? typeData.gainTitle : typeData.loseTitle, o.location != null, o.location?.GetName());
        }
        foreach (GameObject item in this.hasDuration)
        {
            item.SetActive(o.duration > 0);
        }
        if (this.hasDuration.Count > 0 && o.duration > 0 && (bool)this.delta)
        {
            this.delta.text = this.delta.text + " /";
            this.duration.text = global::DBUtils.Localization.Get("UI_TURNS", true, o.duration);
        }
        foreach (GameObject item2 in this.positive)
        {
            item2.SetActive(o.delta > 0);
        }
        foreach (GameObject item3 in this.negative)
        {
            item3.SetActive(o.delta < 0);
        }
    }
}
