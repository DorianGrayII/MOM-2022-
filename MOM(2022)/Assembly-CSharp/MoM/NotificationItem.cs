namespace MOM
{
    using DBDef;
    using DBEnum;
    using DBUtils;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class NotificationItem : ListItem, IPointerClickHandler, IEventSystemHandler
    {
        public RawImage riIcon;
        public GameObject goTownLost;
        public GameObject goEnemySpellBlocked;
        public GameObject goEnemySpellSuccessfulOnTown;
        public GameObject goEnemySpellSuccessfulOnUnit;
        public GameObject goEnemySpellSuccessfulOnWizard;
        public GameObject goEnemyCastsGlobalSpell;
        public GameObject goResearchAvailable;
        public GameObject goOutpostToTown;
        public GameObject goStarvation;
        public GameObject goRaisedUndead;
        public GameObject goConstructionQueueComplete;
        public GameObject goGroupDestroyed;
        public UnitExperience rank;
        private SummaryInfo summaryInfo;
        private RolloverObject rollover;
        private ButtonWithPulse bt;

        public void Activate()
        {
            this.summaryInfo.Activate();
        }

        private void Awake()
        {
            this.bt = base.GetComponent<ButtonWithPulse>();
            if (this.bt != null)
            {
                this.bt.onClick.AddListener(() => this.Activate());
            }
            this.rollover = GameObjectUtils.GetOrAddComponent<RolloverObject>(base.gameObject);
            this.rollover.enabled = false;
            this.rollover.useMouseLocation = false;
            this.rollover.anchor = new Vector2(1f, 0.5f);
            this.rollover.position = new Vector2(0f, 0.5f);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!this.summaryInfo.RequiresAction())
                {
                    if (UIManager.GetLayer(UIManager.Layer.TopLayer).gameObject.GetComponentInChildren<TooltipBase>() != null)
                    {
                        TooltipBase.Close();
                    }
                    this.summaryInfo.Remove();
                }
                else if (this.summaryInfo.summaryType == SummaryInfo.SummaryType.eCastingSpell)
                {
                    PopupGeneral.OpenPopup(HUD.Get(), this.summaryInfo.spell.Get().GetDescriptionInfo().GetLocalizedName(), "UI_DISCARD_SPELL_SURE", "UI_DISCARD", delegate (object o) {
                        this.summaryInfo.Remove();
                        FSMMapGame.Get().CancelSpellTargetSelection(this);
                        HUD.Get().UpdateEndTurnButtons();
                        HUD.Get().btCast.interactable = true;
                        RolloverSimpleTooltip orAddComponent = GameObjectUtils.GetOrAddComponent<RolloverSimpleTooltip>(HUD.Get().btCast.gameObject);
                        orAddComponent.title = "UI_CAST_SPELLS2";
                        orAddComponent.description = null;
                        orAddComponent.anchor.x = 0.3f;
                    }, "UI_CANCEL", null, null, null, null);
                }
            }
        }

        public void Pulse()
        {
            this.bt.Pulse();
        }

        public override void Set(object o, object data, int index)
        {
            bool flag1;
            bool flag2;
            this.summaryInfo = o as SummaryInfo;
            this.goTownLost.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eTownLost);
            this.goResearchAvailable.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eResearchAvailiable);
            this.goOutpostToTown.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eOutpostToTown);
            this.goStarvation.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eStarvation);
            this.goRaisedUndead.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eRaisedUndead);
            this.goConstructionQueueComplete.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eConstructionQueueComplete);
            this.goGroupDestroyed.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eGroupDrown);
            this.goEnemySpellBlocked.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellBlocked);
            this.goEnemySpellSuccessfulOnTown.SetActive((this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellSuccessful) && (this.summaryInfo.location != null));
            this.goEnemySpellSuccessfulOnUnit.SetActive((this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellSuccessful) && ((this.summaryInfo.unit != null) || (this.summaryInfo.group != null)));
            if (this.summaryInfo.summaryType != SummaryInfo.SummaryType.eEnemySpellSuccessful)
            {
                flag1 = false;
            }
            else
            {
                TargetType targetType;
                if (this.summaryInfo.spell != null)
                {
                    targetType = this.summaryInfo.spell.Get().targetType;
                }
                else
                {
                    DBReference<Spell> spell = this.summaryInfo.spell;
                    targetType = null;
                }
                flag1 = targetType == ((TargetType) TARGET_TYPE.WIZARD_ENEMY);
            }
            this.goEnemySpellSuccessfulOnWizard.SetActive(flag1);
            if (this.summaryInfo.summaryType != SummaryInfo.SummaryType.eEnemySpellSuccessful)
            {
                flag2 = false;
            }
            else
            {
                TargetType targetType;
                if (this.summaryInfo.spell != null)
                {
                    targetType = this.summaryInfo.spell.Get().targetType;
                }
                else
                {
                    DBReference<Spell> spell = this.summaryInfo.spell;
                    targetType = null;
                }
                flag2 = targetType == ((TargetType) TARGET_TYPE.GLOBAL);
            }
            this.goEnemyCastsGlobalSpell.SetActive(flag2);
            this.riIcon.texture = AssetManager.Get<Texture2D>(this.summaryInfo.graphic, true);
            this.riIcon.gameObject.SetActive(this.summaryInfo.graphic != null);
            this.rollover.Clear();
            this.rank.gameObject.SetActive(false);
            bool flag = true;
            switch (this.summaryInfo.summaryType)
            {
                case SummaryInfo.SummaryType.eBuildingProgress:
                {
                    if (this.summaryInfo.building != null)
                    {
                        this.rollover.source = this.summaryInfo.building.Get();
                    }
                    else if (this.summaryInfo.unit != null)
                    {
                        this.rollover.source = this.summaryInfo.unit.Get();
                    }
                    object[] parameters = new object[] { this.summaryInfo.name };
                    this.rollover.optionalMessage = DBUtils.Localization.Get("UI_NOTIFICATION_CONSTRUCTION_COMPLETE_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eCastingSpell:
                    this.rollover.source = this.summaryInfo.spell.Get();
                    this.rollover.optionalMessage = "UI_NOTIFICATION_CASTING_COMPLETE_DES";
                    break;

                case SummaryInfo.SummaryType.eCraftingArtefact:
                    this.rollover.source = this.summaryInfo.artefact;
                    this.rollover.optionalMessage = "UI_NOTIFICATION_CRAFTING_COMPLETE_DES";
                    break;

                case SummaryInfo.SummaryType.eEnemySpellBlocked:
                {
                    DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                    this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                    this.rollover.overrideTexture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_BLOCKED_DES", true, Array.Empty<object>());
                    break;
                }
                case SummaryInfo.SummaryType.eTownLost:
                {
                    this.rollover.overrideTitle = "UI_NOTIFICATION_TOWN_LOST";
                    object[] parameters = new object[] { this.summaryInfo.name };
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_TOWN_LOST_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eEnemySpellSuccessful:
                    if (this.summaryInfo.location != null)
                    {
                        TownLocation location = this.summaryInfo.location.Get() as TownLocation;
                        if (location != null)
                        {
                            DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                            object[] parameters = new object[] { location.GetName() };
                            this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_TOWN_DES", true, parameters);
                            this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                            this.rollover.overrideTexture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                            break;
                        }
                    }
                    if (this.summaryInfo.name != null)
                    {
                        DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                        object[] parameters = new object[] { this.summaryInfo.name };
                        this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_TOWN_DES", true, parameters);
                        this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                        this.rollover.overrideTexture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                    }
                    else if (ReferenceEquals(this.summaryInfo.spell.Get().targetType, (TargetType) TARGET_TYPE.GLOBAL))
                    {
                        DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                        this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                        this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_AI_CASTS_GLOBAL_SPELL_DES", true, Array.Empty<object>());
                        this.rollover.overrideTexture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                    }
                    else if (ReferenceEquals(this.summaryInfo.spell.Get().targetType, (TargetType) TARGET_TYPE.WIZARD_ENEMY))
                    {
                        DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                        this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                        this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_WIZARD_DES", true, Array.Empty<object>());
                        this.rollover.overrideTexture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                    }
                    else if (this.summaryInfo.group == null)
                    {
                        this.rollover.source = this.summaryInfo.group;
                        this.rollover.overrideTitle = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_UNIT", true, Array.Empty<object>());
                        this.rollover.optionalMessage = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_UNIT_DES", true, Array.Empty<object>());
                    }
                    else
                    {
                        DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                        this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                        this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_ARMY_DES", true, Array.Empty<object>());
                        this.rollover.overrideTexture = DescriptionInfoExtension.GetTexture(descriptionInfo);
                    }
                    break;

                case SummaryInfo.SummaryType.eResearchAvailiable:
                    this.rollover.overrideTitle = "UI_NOTIFICATION_RESEARCH_AVAILABLE";
                    this.rollover.overrideDescription = "UI_NOTIFICATION_RESEARCH_AVAILABLE_DES";
                    break;

                case SummaryInfo.SummaryType.eOutpostToTown:
                {
                    this.rollover.overrideTitle = "UI_NOTIFICATION_OUTPOST_TO_TOWN";
                    object[] parameters = new object[] { this.summaryInfo.name };
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_OUTPOST_TO_TOWN_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eUnitLeveledUp:
                {
                    this.rollover.source = this.summaryInfo.unit.Get();
                    object[] parameters = new object[] { this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName() };
                    this.rollover.optionalMessage = DBUtils.Localization.Get("UI_NOTIFICATION_UNIT_LEVEL_UP_DES", true, parameters);
                    this.rank.gameObject.SetActive(true);
                    this.rank.Set((BaseUnit) this.summaryInfo.unit);
                    break;
                }
                case SummaryInfo.SummaryType.eUnitSummoned:
                {
                    this.rollover.source = this.summaryInfo.unit.Get();
                    object[] parameters = new object[] { this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName() };
                    this.rollover.optionalMessage = DBUtils.Localization.Get("UI_NOTIFICATION_SUMMON_COMPLETE_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eStarvation:
                {
                    this.rollover.overrideTitle = "UI_NOTIFICATION_STARVATION";
                    object[] parameters = new object[] { this.summaryInfo.name };
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_STARVATION_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eRaisedUndead:
                {
                    this.rollover.source = this.summaryInfo.unit.Get();
                    object[] parameters = new object[] { this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName() };
                    this.rollover.optionalMessage = DBUtils.Localization.Get("UI_NOTIFICATION_RAISED_UNDEAD_DES", true, parameters);
                    this.rollover.overrideTitle = "UI_NOTIFICATION_RAISED_UNDEAD";
                    object[] objArray11 = new object[] { this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName() };
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_RAISED_UNDEAD_DES", true, objArray11);
                    break;
                }
                case SummaryInfo.SummaryType.eSummoningCircleMoved:
                {
                    this.rollover.overrideTitle = "UI_SUMMONING_CIRCLE_LOCATION";
                    object[] parameters = new object[] { this.summaryInfo.location.Get().GetName(), this.summaryInfo.name };
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_SUMMONING_CIRCLE_LOCATION_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eConstructionQueueComplete:
                {
                    this.rollover.overrideTitle = "UI_NOTIFICATION_QUEUE_COMPLETE";
                    object[] parameters = new object[] { this.summaryInfo.name };
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_QUEUE_COMPLETE_DES", true, parameters);
                    break;
                }
                case SummaryInfo.SummaryType.eGroupDrown:
                    this.rollover.overrideTitle = "UI_NOTIFICATION_GROUP_DROWN";
                    this.rollover.overrideDescription = DBUtils.Localization.Get("UI_NOTIFICATION_GROUP_DROWN_DES", true, Array.Empty<object>());
                    break;

                default:
                    flag = false;
                    break;
            }
            this.rollover.enabled = flag;
            this.bt.interactable = !TurnManager.Get(false).endTurn;
            if (this.rollover.isActiveAndEnabled && UIManager.IsTopForInput(HUD.Get()))
            {
                this.rollover.Close();
                TooltipBase.Open(this.rollover, null);
            }
        }
    }
}

