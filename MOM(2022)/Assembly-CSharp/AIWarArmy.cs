using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class AIWarArmy : IPlanePosition
{
    public enum Mobility
    {
        UseWalk = 0,
        ForceWaterWalk = 1,
        UseShip = 2,
        ForceFly = 3,
        UseSuperTransport = 4,
        Max = 5
    }

    [ProtoMember(1)]
    public Reference<PlayerWizard> owner;

    [ProtoMember(2)]
    public AIWarTarget target;

    [ProtoMember(3)]
    public Reference<global::MOM.Group> group;

    [ProtoMember(4)]
    public Mobility armyMobilityDesignation;

    [ProtoMember(5)]
    public int expectedMinimumPower;

    [ProtoMember(6)]
    public Vector3i placeOfCreation;

    [ProtoMember(7)]
    public bool placeOfCreationPlaneArcane;

    [ProtoMember(8)]
    public DBReference<Spell> needOfSpell;

    public AIWarArmy()
    {
    }

    public AIWarArmy(Vector3i placeOfCreation, bool arcane)
    {
        this.placeOfCreation = placeOfCreation;
        this.placeOfCreationPlaneArcane = arcane;
    }

    public void Validate()
    {
        if (this.group?.Get() == null || !this.group.Get().alive || this.group.Get().GetUnits().Count < 4)
        {
            this.group = null;
        }
        if (this.target != null && !this.target.Valid())
        {
            this.target = null;
        }
    }

    public void SetTarget(AIWarTarget wt)
    {
        if (this.target != null)
        {
            this.target.haveAssignee = false;
        }
        this.target = wt;
        wt.haveAssignee = true;
    }

    public Vector3i GetPosition()
    {
        if (this.group?.Get() != null)
        {
            return this.group.Get().GetPosition();
        }
        return this.placeOfCreation;
    }

    public Vector3 GetPhysicalPosition()
    {
        if (this.GetPlane() != null)
        {
            Chunk chunkFor = this.GetPlane().GetChunkFor(this.GetPosition());
            Vector3 vector = HexCoordinates.HexToWorld3D(this.GetPosition());
            return chunkFor.go.transform.position + vector;
        }
        return Vector3.zero;
    }

    public global::WorldCode.Plane GetPlane()
    {
        if (this.group?.Get() != null)
        {
            return this.group.Get().GetPlane();
        }
        if (!this.placeOfCreationPlaneArcane)
        {
            return World.GetMyrror();
        }
        return World.GetArcanus();
    }

    public int GetWizardID()
    {
        return this.owner.Get().ID;
    }

    public PlayerWizardAI GetWizard()
    {
        return this.owner.Get() as PlayerWizardAI;
    }

    public bool SpellRequest(DBReference<Spell> spell)
    {
        if (this.owner.Get().GetMagicAndResearch().GetCastingTimeLeft() > 15)
        {
            return false;
        }
        this.needOfSpell = spell;
        return true;
    }
}
