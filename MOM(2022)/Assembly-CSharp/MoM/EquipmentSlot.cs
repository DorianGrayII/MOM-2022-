namespace MOM
{
    using DBDef;
    using MHUtils;
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class EquipmentSlot
    {
        [ProtoMember(1)]
        public DBReference<ArtefactSlot> slotType;
        [ProtoMember(2)]
        private MOM.Artefact _item;
        [ProtoMember(3)]
        public Reference<BaseUnit> owner;

        public EquipmentSlot()
        {
        }

        public EquipmentSlot(BaseUnit owner)
        {
            this.owner = owner;
        }

        public bool IsCompatible(MOM.Artefact a)
        {
            return (Array.FindIndex<EEquipmentType>(this.slotType.Get().eTypes, o => o == a.equipmentType) > -1);
        }

        public MOM.Artefact item
        {
            get
            {
                return this._item;
            }
            set
            {
                if (!ReferenceEquals(this._item, value))
                {
                    if (this._item != null)
                    {
                        foreach (DBReference<ArtefactPower> reference in this._item.artefactPowers)
                        {
                            ISkillableExtension.RemoveSkill(this.owner.Get(), reference.Get().skill);
                        }
                    }
                    this._item = value;
                    this.owner.Get().artefactManager.UpdateSkills(this);
                    if (this.owner.Get() is MOM.Unit)
                    {
                        MOM.Group local2;
                        if ((this.owner.Get() as MOM.Unit).group != null)
                        {
                            local2 = (this.owner.Get() as MOM.Unit).group.Get();
                        }
                        else
                        {
                            Reference<MOM.Group> group = (this.owner.Get() as MOM.Unit).group;
                            local2 = null;
                        }
                        MOM.Group objB = local2;
                        if (objB != null)
                        {
                            objB.UpdateMovementFlags();
                            if (ReferenceEquals(FSMSelectionManager.Get().GetSelectedGroup(), objB))
                            {
                                MHEventSystem.TriggerEvent<FSMSelectionManager>(objB, null);
                            }
                        }
                    }
                    MHEventSystem.TriggerEvent<EquipmentSlot>(this, null);
                }
            }
        }
    }
}

