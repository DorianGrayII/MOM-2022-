using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MOM
{
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

        private void Awake()
        {
            this.bt = base.GetComponent<ButtonWithPulse>();
            if (this.bt != null)
            {
                this.bt.onClick.AddListener(delegate
                {
                    this.Activate();
                });
            }
            this.rollover = base.gameObject.GetOrAddComponent<RolloverObject>();
            this.rollover.enabled = false;
            this.rollover.useMouseLocation = false;
            this.rollover.anchor = new Vector2(1f, 0.5f);
            this.rollover.position = new Vector2(0f, 0.5f);
        }

        public void Pulse()
        {
            this.bt.Pulse();
        }

        public override void Set(object o, object data, int index)
        {
            this.summaryInfo = o as SummaryInfo;
            this.goTownLost.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eTownLost);
            this.goResearchAvailable.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eResearchAvailiable);
            this.goOutpostToTown.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eOutpostToTown);
            this.goStarvation.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eStarvation);
            this.goRaisedUndead.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eRaisedUndead);
            this.goConstructionQueueComplete.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eConstructionQueueComplete);
            this.goGroupDestroyed.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eGroupDrown);
            this.goEnemySpellBlocked.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellBlocked);
            this.goEnemySpellSuccessfulOnTown.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellSuccessful && this.summaryInfo.location != null);
            this.goEnemySpellSuccessfulOnUnit.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellSuccessful && (this.summaryInfo.unit != null || this.summaryInfo.group != null));
            this.goEnemySpellSuccessfulOnWizard.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellSuccessful && this.summaryInfo.spell?.Get().targetType == (TargetType)TARGET_TYPE.WIZARD_ENEMY);
            this.goEnemyCastsGlobalSpell.SetActive(this.summaryInfo.summaryType == SummaryInfo.SummaryType.eEnemySpellSuccessful && this.summaryInfo.spell?.Get().targetType == (TargetType)TARGET_TYPE.GLOBAL);
            this.riIcon.texture = AssetManager.Get<Texture2D>(this.summaryInfo.graphic);
            this.riIcon.gameObject.SetActive(this.summaryInfo.graphic != null);
            this.rollover.Clear();
            this.rank.gameObject.SetActive(value: false);
            bool flag = true;
            switch (this.summaryInfo.summaryType)
            {
            case SummaryInfo.SummaryType.eBuildingProgress:
                if (this.summaryInfo.building != null)
                {
                    this.rollover.source = this.summaryInfo.building.Get();
                }
                else if (this.summaryInfo.unit != null)
                {
                    this.rollover.source = this.summaryInfo.unit.Get();
                }
                this.rollover.optionalMessage = global::DBUtils.Localization.Get("UI_NOTIFICATION_CONSTRUCTION_COMPLETE_DES", true, this.summaryInfo.name);
                break;
            case SummaryInfo.SummaryType.eCastingSpell:
                this.rollover.source = this.summaryInfo.spell.Get();
                this.rollover.optionalMessage = "UI_NOTIFICATION_CASTING_COMPLETE_DES";
                break;
            case SummaryInfo.SummaryType.eCraftingArtefact:
                this.rollover.source = this.summaryInfo.artefact;
                this.rollover.optionalMessage = "UI_NOTIFICATION_CRAFTING_COMPLETE_DES";
                break;
            case SummaryInfo.SummaryType.eResearchAvailiable:
                this.rollover.overrideTitle = "UI_NOTIFICATION_RESEARCH_AVAILABLE";
                this.rollover.overrideDescription = "UI_NOTIFICATION_RESEARCH_AVAILABLE_DES";
                break;
            case SummaryInfo.SummaryType.eTownLost:
                this.rollover.overrideTitle = "UI_NOTIFICATION_TOWN_LOST";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_TOWN_LOST_DES", true, this.summaryInfo.name);
                break;
            case SummaryInfo.SummaryType.eSummoningCircleMoved:
                this.rollover.overrideTitle = "UI_SUMMONING_CIRCLE_LOCATION";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_SUMMONING_CIRCLE_LOCATION_DES", true, this.summaryInfo.location.Get().GetName(), this.summaryInfo.name);
                break;
            case SummaryInfo.SummaryType.eUnitSummoned:
                this.rollover.source = this.summaryInfo.unit.Get();
                this.rollover.optionalMessage = global::DBUtils.Localization.Get("UI_NOTIFICATION_SUMMON_COMPLETE_DES", true, this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName());
                break;
            case SummaryInfo.SummaryType.eEnemySpellBlocked:
            {
                DescriptionInfo descriptionInfo6 = this.summaryInfo.spell.Get().GetDescriptionInfo();
                this.rollover.overrideTitle = descriptionInfo6.GetLocalizedName();
                this.rollover.overrideTexture = descriptionInfo6.GetTexture();
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_BLOCKED_DES", true);
                break;
            }
            case SummaryInfo.SummaryType.eEnemySpellSuccessful:
                if (this.summaryInfo.location != null && this.summaryInfo.location.Get() is TownLocation townLocation)
                {
                    DescriptionInfo descriptionInfo = this.summaryInfo.spell.Get().GetDescriptionInfo();
                    this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_TOWN_DES", true, townLocation.GetName());
                    this.rollover.overrideTitle = descriptionInfo.GetLocalizedName();
                    this.rollover.overrideTexture = descriptionInfo.GetTexture();
                }
                else if (this.summaryInfo.name != null)
                {
                    DescriptionInfo descriptionInfo2 = this.summaryInfo.spell.Get().GetDescriptionInfo();
                    this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_TOWN_DES", true, this.summaryInfo.name);
                    this.rollover.overrideTitle = descriptionInfo2.GetLocalizedName();
                    this.rollover.overrideTexture = descriptionInfo2.GetTexture();
                }
                else if (this.summaryInfo.spell.Get().targetType == (TargetType)TARGET_TYPE.GLOBAL)
                {
                    DescriptionInfo descriptionInfo3 = this.summaryInfo.spell.Get().GetDescriptionInfo();
                    this.rollover.overrideTitle = descriptionInfo3.GetLocalizedName();
                    this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_AI_CASTS_GLOBAL_SPELL_DES", true);
                    this.rollover.overrideTexture = descriptionInfo3.GetTexture();
                }
                else if (this.summaryInfo.spell.Get().targetType == (TargetType)TARGET_TYPE.WIZARD_ENEMY)
                {
                    DescriptionInfo descriptionInfo4 = this.summaryInfo.spell.Get().GetDescriptionInfo();
                    this.rollover.overrideTitle = descriptionInfo4.GetLocalizedName();
                    this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_WIZARD_DES", true);
                    this.rollover.overrideTexture = descriptionInfo4.GetTexture();
                }
                else if (this.summaryInfo.group != null)
                {
                    DescriptionInfo descriptionInfo5 = this.summaryInfo.spell.Get().GetDescriptionInfo();
                    this.rollover.overrideTitle = descriptionInfo5.GetLocalizedName();
                    this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_ARMY_DES", true);
                    this.rollover.overrideTexture = descriptionInfo5.GetTexture();
                }
                else
                {
                    this.rollover.source = this.summaryInfo.group;
                    this.rollover.overrideTitle = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_UNIT", true);
                    this.rollover.optionalMessage = global::DBUtils.Localization.Get("UI_NOTIFICATION_SPELL_ON_UNIT_DES", true);
                }
                break;
            case SummaryInfo.SummaryType.eOutpostToTown:
                this.rollover.overrideTitle = "UI_NOTIFICATION_OUTPOST_TO_TOWN";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_OUTPOST_TO_TOWN_DES", true, this.summaryInfo.name);
                break;
            case SummaryInfo.SummaryType.eUnitLeveledUp:
                this.rollover.source = this.summaryInfo.unit.Get();
                this.rollover.optionalMessage = global::DBUtils.Localization.Get("UI_NOTIFICATION_UNIT_LEVEL_UP_DES", true, this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName());
                this.rank.gameObject.SetActive(value: true);
                this.rank.Set(this.summaryInfo.unit);
                break;
            case SummaryInfo.SummaryType.eStarvation:
                this.rollover.overrideTitle = "UI_NOTIFICATION_STARVATION";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_STARVATION_DES", true, this.summaryInfo.name);
                break;
            case SummaryInfo.SummaryType.eRaisedUndead:
                this.rollover.source = this.summaryInfo.unit.Get();
                this.rollover.optionalMessage = global::DBUtils.Localization.Get("UI_NOTIFICATION_RAISED_UNDEAD_DES", true, this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName());
                this.rollover.overrideTitle = "UI_NOTIFICATION_RAISED_UNDEAD";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_RAISED_UNDEAD_DES", true, this.summaryInfo.unit.Get().GetDescriptionInfo().GetLocalizedName());
                break;
            case SummaryInfo.SummaryType.eConstructionQueueComplete:
                this.rollover.overrideTitle = "UI_NOTIFICATION_QUEUE_COMPLETE";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_QUEUE_COMPLETE_DES", true, this.summaryInfo.name);
                break;
            case SummaryInfo.SummaryType.eGroupDrown:
                this.rollover.overrideTitle = "UI_NOTIFICATION_GROUP_DROWN";
                this.rollover.overrideDescription = global::DBUtils.Localization.Get("UI_NOTIFICATION_GROUP_DROWN_DES", true);
                break;
            default:
                flag = false;
                break;
            }
            this.rollover.enabled = flag;
            this.bt.interactable = !TurnManager.Get().endTurn;
            if (this.rollover.isActiveAndEnabled && UIManager.IsTopForInput(HUD.Get()))
            {
                this.rollover.Close();
                TooltipBase.Open(this.rollover);
            }
        }

        public void Activate()
        {
            this.summaryInfo.Activate();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }
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
                PopupGeneral.OpenPopup(HUD.Get(), this.summaryInfo.spell.Get().GetDescriptionInfo().GetLocalizedName(), "UI_DISCARD_SPELL_SURE", "UI_DISCARD", delegate
                {
                    this.summaryInfo.Remove();
                    FSMMapGame.Get().CancelSpellTargetSelection(this);
                    HUD.Get().UpdateEndTurnButtons();
                    HUD.Get().btCast.interactable = true;
                    RolloverSimpleTooltip orAddComponent = HUD.Get().btCast.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
                    orAddComponent.title = "UI_CAST_SPELLS2";
                    orAddComponent.description = null;
                    orAddComponent.anchor.x = 0.3f;
                }, "UI_CANCEL");
            }
        }
    }
}
