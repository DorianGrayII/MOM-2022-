using System;
using DBDef;
using MHUtils;
using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class EquipmentSlot
    {
        [ProtoMember(1)]
        public DBReference<ArtefactSlot> slotType;

        [ProtoMember(2)]
        private Artefact _item;

        [ProtoMember(3)]
        public Reference<BaseUnit> owner;

        public Artefact item
        {
            get
            {
                return this._item;
            }
            set
            {
                if (this._item == value)
                {
                    return;
                }
                if (this._item != null)
                {
                    foreach (DBReference<ArtefactPower> artefactPower in this._item.artefactPowers)
                    {
                        this.owner.Get().RemoveSkill(artefactPower.Get().skill);
                    }
                }
                this._item = value;
                this.owner.Get().artefactManager.UpdateSkills(this);
                if (this.owner.Get() is Unit)
                {
                    Group group = (this.owner.Get() as Unit).group?.Get();
                    if (group != null)
                    {
                        group.UpdateMovementFlags();
                        if (FSMSelectionManager.Get().GetSelectedGroup() == group)
                        {
                            MHEventSystem.TriggerEvent<FSMSelectionManager>(group, null);
                        }
                    }
                }
                MHEventSystem.TriggerEvent<EquipmentSlot>(this, null);
            }
        }

        public EquipmentSlot()
        {
        }

        public EquipmentSlot(BaseUnit owner)
        {
            this.owner = owner;
        }

        public bool IsCompatible(Artefact a)
        {
            return Array.FindIndex(this.slotType.Get().eTypes, (EEquipmentType o) => o == a.equipmentType) > -1;
        }
    }
}
