using DBDef;
using DBEnum;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using MoM.Scripts;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class SummaryInfo
    {
        public enum SummaryType
        {
            eUnknown = 0,
            eBuildingProgress = 1,
            eCastingSpell = 2,
            eCraftingArtefact = 3,
            eEnemySpellBlocked = 4,
            eTownLost = 5,
            eEnemySpellSuccessful = 6,
            eResearchAvailiable = 7,
            eOutpostToTown = 8,
            eUnitLeveledUp = 9,
            eUnitSummoned = 10,
            eStarvation = 11,
            eRaisedUndead = 12,
            eResourceDiscovered = 13,
            eSummoningCircleMoved = 14,
            eConstructionQueueComplete = 15,
            eGroupDrown = 16,
            eMAX = 17
        }

        [ProtoMember(1)]
        public SummaryType summaryType;

        [ProtoMember(2)]
        public Reference<Location> location;

        [ProtoMember(3)]
        public FInt dataInfo;

        [ProtoMember(4)]
        public string graphic;

        [ProtoMember(5)]
        public string title;

        [ProtoMember(6)]
        public DBReference<Spell> spell;

        [ProtoMember(7)]
        public Reference<BaseUnit> unit;

        [ProtoMember(8)]
        public AttributeDelta attributeDelta;

        [ProtoMember(9)]
        public Reference<Group> group;

        [ProtoMember(10)]
        public Artefact artefact;

        [ProtoMember(11)]
        public DBReference<Building> building;

        [ProtoMember(12)]
        public string name;

        [ProtoMember(13)]
        public bool isArcanus;

        [ProtoMember(14)]
        public Vector3i position;

        [ProtoMember(15)]
        public Reference<PlayerWizard> casterAI;

        public bool RequiresAction()
        {
            return this.GetEndTurnReason() != null;
        }

        public string GetEndTurnReason()
        {
            if (this.summaryType == SummaryType.eCastingSpell)
            {
                return "UI_SPELL_CAST_PROMPT";
            }
            return null;
        }

        public void Activate()
        {
            FSMSelectionManager.Get().Select(null, focus: false);
            switch (this.summaryType)
            {
            case SummaryType.eBuildingProgress:
            case SummaryType.eOutpostToTown:
            case SummaryType.eStarvation:
            case SummaryType.eConstructionQueueComplete:
                if (this.location?.Get() != null && this.location.Get().GetOwnerID() == PlayerWizard.HumanID() && EntityManager.GetEntity(this.location.ID) != null)
                {
                    FSMSelectionManager.Get().Select(this.location.Get(), focus: true);
                    new FsmEventTarget().target = FsmEventTarget.EventTarget.Self;
                    MHEventSystem.TriggerEvent<SummaryInfo>(this.location.Get(), "TownScreen");
                }
                this.Remove();
                break;
            case SummaryType.eCastingSpell:
                Debug.Log("start casting spell");
                MHEventSystem.TriggerEvent<SummaryInfo>(this, "CastingSpell");
                break;
            case SummaryType.eCraftingArtefact:
                UIManager.Open<HeroEquip>(UIManager.Layer.Popup);
                this.Remove();
                break;
            case SummaryType.eEnemySpellBlocked:
                this.Remove();
                break;
            case SummaryType.eEnemySpellSuccessful:
            {
                global::WorldCode.Plane activePlane2 = World.GetActivePlane();
                if (this.location != null)
                {
                    if (activePlane2 != this.location.Get().plane)
                    {
                        World.ActivatePlane(World.GetOtherPlane(activePlane2));
                    }
                    CameraController.CenterAt(this.location.Get().Position);
                }
                else if (this.group != null)
                {
                    FSMSelectionManager.Get().Select(this.group.Get(), focus: true);
                }
                else if (this.spell != null && this.spell.Get().targetType == (TargetType)TARGET_TYPE.WIZARD_ENEMY)
                {
                    PlayerWizard caster = null;
                    if (this.casterAI != null && GameManager.GetHumanWizard().GetDiscoveredWizards().Contains(this.casterAI))
                    {
                        caster = this.casterAI;
                    }
                    UIManager.Open<PopupGlobalCast>(UIManager.Layer.Popup).Set(this.spell, caster, GameManager.GetHumanWizard());
                }
                else if (this.spell != null && this.spell.Get().targetType == (TargetType)TARGET_TYPE.GLOBAL)
                {
                    PlayerWizard caster2 = null;
                    if (this.casterAI != null && GameManager.GetHumanWizard().GetDiscoveredWizards().Contains(this.casterAI))
                    {
                        caster2 = this.casterAI;
                    }
                    UIManager.Open<PopupGlobalCast>(UIManager.Layer.Popup).Set(this.spell, caster2);
                }
                else
                {
                    _ = this.position;
                    if (activePlane2.arcanusType != this.isArcanus)
                    {
                        World.ActivatePlane(World.GetOtherPlane(activePlane2));
                    }
                    CameraController.CenterAt(this.position);
                }
                this.Remove();
                break;
            }
            case SummaryType.eTownLost:
            case SummaryType.eResourceDiscovered:
            case SummaryType.eSummoningCircleMoved:
            case SummaryType.eGroupDrown:
            {
                global::WorldCode.Plane activePlane = World.GetActivePlane();
                if (this.location != null)
                {
                    if (activePlane != this.location.Get().plane)
                    {
                        World.ActivatePlane(World.GetOtherPlane(activePlane));
                    }
                    CameraController.CenterAt(this.location.Get().Position);
                }
                else if (this.group != null)
                {
                    FSMSelectionManager.Get().Select(this.group.Get(), focus: true);
                }
                else
                {
                    _ = this.position;
                    if (activePlane.arcanusType != this.isArcanus)
                    {
                        World.ActivatePlane(World.GetOtherPlane(activePlane));
                    }
                    CameraController.CenterAt(this.position);
                }
                this.Remove();
                break;
            }
            case SummaryType.eResearchAvailiable:
                HUD.Get().OpenResearch();
                this.Remove();
                break;
            case SummaryType.eUnitLeveledUp:
                if ((this.unit.Get() as Unit)?.group != null)
                {
                    LevelUp.PopUp(this.unit, this.attributeDelta);
                }
                this.Remove();
                break;
            case SummaryType.eUnitSummoned:
            case SummaryType.eRaisedUndead:
            {
                Unit unit = this.unit.Get() as Unit;
                if (unit?.group != null)
                {
                    FSMSelectionManager.Get().Select(unit.group.Get(), focus: true);
                }
                this.Remove();
                break;
            }
            }
        }

        public void Remove()
        {
            GameManager.GetHumanWizard().RemoveSummaryInfo(this);
        }
    }
}
