namespace MOM
{
    using DBDef;
    using DBEnum;
    using HutongGames.PlayMaker;
    using MHUtils;
    using MHUtils.UI;
    using MoM.Scripts;
    using ProtoBuf;
    using System;
    using UnityEngine;
    using WorldCode;

    [ProtoContract]
    public class SummaryInfo
    {
        [ProtoMember(1)]
        public SummaryType summaryType;
        [ProtoMember(2)]
        public Reference<MOM.Location> location;
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
        public Reference<MOM.Group> group;
        [ProtoMember(10)]
        public MOM.Artefact artefact;
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

        public void Activate()
        {
            FSMSelectionManager.Get().Select(null, false);
            switch (this.summaryType)
            {
                case SummaryType.eBuildingProgress:
                case SummaryType.eOutpostToTown:
                case SummaryType.eStarvation:
                case SummaryType.eConstructionQueueComplete:
                    MOM.Location local3;
                    if (this.location != null)
                    {
                        local3 = this.location.Get();
                    }
                    else
                    {
                        Reference<MOM.Location> location = this.location;
                        local3 = null;
                    }
                    if ((local3 != null) && ((this.location.Get().GetOwnerID() == PlayerWizard.HumanID()) && (EntityManager.GetEntity(this.location.ID) != null)))
                    {
                        FSMSelectionManager.Get().Select(this.location.Get(), true);
                        new FsmEventTarget().target = FsmEventTarget.EventTarget.Self;
                        MHEventSystem.TriggerEvent<SummaryInfo>(this.location.Get(), "TownScreen");
                    }
                    this.Remove();
                    return;

                case SummaryType.eCastingSpell:
                    Debug.Log("start casting spell");
                    MHEventSystem.TriggerEvent<SummaryInfo>(this, "CastingSpell");
                    return;

                case SummaryType.eCraftingArtefact:
                    UIManager.Open<HeroEquip>(UIManager.Layer.Popup, null);
                    this.Remove();
                    return;

                case SummaryType.eEnemySpellBlocked:
                    this.Remove();
                    return;

                case SummaryType.eTownLost:
                case SummaryType.eResourceDiscovered:
                case SummaryType.eSummoningCircleMoved:
                case SummaryType.eGroupDrown:
                {
                    WorldCode.Plane activePlane = World.GetActivePlane();
                    if (this.location != null)
                    {
                        if (!ReferenceEquals(activePlane, this.location.Get().plane))
                        {
                            World.ActivatePlane(World.GetOtherPlane(activePlane), false);
                        }
                        CameraController.CenterAt(this.location.Get().Position, false, 0f);
                    }
                    else if (this.group != null)
                    {
                        FSMSelectionManager.Get().Select(this.group.Get(), true);
                    }
                    else
                    {
                        Vector3i position = this.position;
                        if (activePlane.arcanusType != this.isArcanus)
                        {
                            World.ActivatePlane(World.GetOtherPlane(activePlane), false);
                        }
                        CameraController.CenterAt(this.position, false, 0f);
                    }
                    this.Remove();
                    return;
                }
                case SummaryType.eEnemySpellSuccessful:
                {
                    WorldCode.Plane activePlane = World.GetActivePlane();
                    if (this.location != null)
                    {
                        if (!ReferenceEquals(activePlane, this.location.Get().plane))
                        {
                            World.ActivatePlane(World.GetOtherPlane(activePlane), false);
                        }
                        CameraController.CenterAt(this.location.Get().Position, false, 0f);
                    }
                    else if (this.group != null)
                    {
                        FSMSelectionManager.Get().Select(this.group.Get(), true);
                    }
                    else if ((this.spell != null) && ReferenceEquals(this.spell.Get().targetType, (TargetType) TARGET_TYPE.WIZARD_ENEMY))
                    {
                        PlayerWizard caster = null;
                        if ((this.casterAI != null) && GameManager.GetHumanWizard().GetDiscoveredWizards().Contains(this.casterAI))
                        {
                            caster = (PlayerWizard) this.casterAI;
                        }
                        UIManager.Open<PopupGlobalCast>(UIManager.Layer.Popup, null).Set((Spell) this.spell, caster, GameManager.GetHumanWizard());
                    }
                    else if ((this.spell != null) && ReferenceEquals(this.spell.Get().targetType, (TargetType) TARGET_TYPE.GLOBAL))
                    {
                        PlayerWizard caster = null;
                        if ((this.casterAI != null) && GameManager.GetHumanWizard().GetDiscoveredWizards().Contains(this.casterAI))
                        {
                            caster = (PlayerWizard) this.casterAI;
                        }
                        UIManager.Open<PopupGlobalCast>(UIManager.Layer.Popup, null).Set((Spell) this.spell, caster);
                    }
                    else
                    {
                        Vector3i position = this.position;
                        if (activePlane.arcanusType != this.isArcanus)
                        {
                            World.ActivatePlane(World.GetOtherPlane(activePlane), false);
                        }
                        CameraController.CenterAt(this.position, false, 0f);
                    }
                    this.Remove();
                    return;
                }
                case SummaryType.eResearchAvailiable:
                    HUD.Get().OpenResearch();
                    this.Remove();
                    return;

                case SummaryType.eUnitLeveledUp:
                {
                    Reference<MOM.Group> group;
                    MOM.Unit unit1 = this.unit.Get() as MOM.Unit;
                    if (unit1 != null)
                    {
                        group = unit1.group;
                    }
                    else
                    {
                        MOM.Unit local2 = unit1;
                        group = null;
                    }
                    if (group != null)
                    {
                        LevelUp.PopUp((BaseUnit) this.unit, this.attributeDelta);
                    }
                    this.Remove();
                    return;
                }
                case SummaryType.eUnitSummoned:
                case SummaryType.eRaisedUndead:
                {
                    MOM.Unit unit = this.unit.Get() as MOM.Unit;
                    if (((unit != null) ? unit.group : null) != null)
                    {
                        FSMSelectionManager.Get().Select(unit.group.Get(), true);
                    }
                    this.Remove();
                    return;
                }
            }
        }

        public string GetEndTurnReason()
        {
            return ((this.summaryType != SummaryType.eCastingSpell) ? null : "UI_SPELL_CAST_PROMPT");
        }

        public void Remove()
        {
            GameManager.GetHumanWizard().RemoveSummaryInfo(this);
        }

        public bool RequiresAction()
        {
            return (this.GetEndTurnReason() != null);
        }

        public enum SummaryType
        {
            eUnknown,
            eBuildingProgress,
            eCastingSpell,
            eCraftingArtefact,
            eEnemySpellBlocked,
            eTownLost,
            eEnemySpellSuccessful,
            eResearchAvailiable,
            eOutpostToTown,
            eUnitLeveledUp,
            eUnitSummoned,
            eStarvation,
            eRaisedUndead,
            eResourceDiscovered,
            eSummoningCircleMoved,
            eConstructionQueueComplete,
            eGroupDrown,
            eMAX
        }
    }
}

