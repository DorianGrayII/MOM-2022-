using System.Collections.Generic;
using DBDef;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class ArtefactManager
    {
        [ProtoMember(1)]
        public Reference<BaseUnit> owner;

        [ProtoMember(2)]
        public List<EquipmentSlot> equipmentSlots;

        public ArtefactManager()
        {
        }

        public ArtefactManager(BaseUnit owner)
        {
            this.owner = owner;
            this.equipmentSlots = new List<EquipmentSlot>();
            if (owner.dbSource.Get() is Hero)
            {
                ArtefactSlot[] equipmentSlot = (owner.dbSource.Get() as Hero).equipmentSlot;
                foreach (ArtefactSlot artefactSlot in equipmentSlot)
                {
                    EquipmentSlot item = new EquipmentSlot(owner)
                    {
                        slotType = artefactSlot
                    };
                    this.equipmentSlots.Add(item);
                }
            }
        }

        public void ReturnEquipment(PlayerWizard w)
        {
            foreach (EquipmentSlot equipmentSlot in this.equipmentSlots)
            {
                if (equipmentSlot.item != null)
                {
                    w.artefacts.Add(equipmentSlot.item);
                    equipmentSlot.item = null;
                }
            }
        }

        public void UpdateSkills(EquipmentSlot slot)
        {
            if (slot == null)
            {
                Debug.LogWarning("Data in Artefact Manager UpdateSkills should be EquipmentSlot type");
                return;
            }
            if (slot.item != null)
            {
                List<DBReference<Skill>> skills = this.owner.Get().GetSkills();
                List<EnchantmentInstance> enchantments = this.owner.Get().GetEnchantments();
                {
                    foreach (DBReference<ArtefactPower> artefactPower in slot.item.artefactPowers)
                    {
                        Skill skill = artefactPower.Get().skill;
                        if (skills.Contains(skill) && !skill.stackable)
                        {
                            continue;
                        }
                        Enchantment[] relatedEnchantment = skill.relatedEnchantment;
                        if (relatedEnchantment != null && relatedEnchantment.Length != 0)
                        {
                            Enchantment[] array = relatedEnchantment;
                            foreach (Enchantment en in array)
                            {
                                EnchantmentInstance enchantmentInstance = enchantments.Find((EnchantmentInstance o) => o.source == en);
                                if (enchantmentInstance != null)
                                {
                                    this.owner.Get().RemoveEnchantment(enchantmentInstance);
                                }
                            }
                        }
                        this.owner.Get().AddSkill(skill);
                    }
                    return;
                }
            }
            List<DBReference<Skill>> skills2 = this.owner.Get().GetSkills();
            foreach (EquipmentSlot equipmentSlot in this.equipmentSlots)
            {
                if (equipmentSlot == slot || equipmentSlot.item == null)
                {
                    continue;
                }
                foreach (DBReference<ArtefactPower> artefactPower2 in equipmentSlot.item.artefactPowers)
                {
                    Skill skill2 = artefactPower2.Get().skill;
                    if (!skills2.Contains(skill2))
                    {
                        this.owner.Get().AddSkill(skill2);
                    }
                }
            }
        }
    }
}
