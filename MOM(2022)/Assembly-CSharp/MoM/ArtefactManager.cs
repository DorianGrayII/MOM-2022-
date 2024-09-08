namespace MOM
{
    using DBDef;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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
                foreach (ArtefactSlot slot in (owner.dbSource.Get() as Hero).equipmentSlot)
                {
                    EquipmentSlot item = new EquipmentSlot(owner) {
                        slotType = slot
                    };
                    this.equipmentSlots.Add(item);
                }
            }
        }

        public void ReturnEquipment(PlayerWizard w)
        {
            foreach (EquipmentSlot slot in this.equipmentSlots)
            {
                if (slot.item != null)
                {
                    w.artefacts.Add(slot.item);
                    slot.item = null;
                }
            }
        }

        public void UpdateSkills(EquipmentSlot slot)
        {
            if (slot == null)
            {
                Debug.LogWarning("Data in Artefact Manager UpdateSkills should be EquipmentSlot type");
            }
            else
            {
                List<DBReference<ArtefactPower>>.Enumerator enumerator;
                if (slot.item == null)
                {
                    List<DBReference<Skill>> skills = ISkillableExtension.GetSkills(this.owner.Get());
                    foreach (EquipmentSlot slot2 in this.equipmentSlots)
                    {
                        if (!ReferenceEquals(slot2, slot) && (slot2.item != null))
                        {
                            using (enumerator = slot2.item.artefactPowers.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    Skill skill = enumerator.Current.Get().skill;
                                    if (!skills.Contains(skill))
                                    {
                                        ISkillableExtension.AddSkill(this.owner.Get(), skill);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<DBReference<Skill>> skills = ISkillableExtension.GetSkills(this.owner.Get());
                    List<EnchantmentInstance> enchantments = IEnchantableExtension.GetEnchantments(this.owner.Get());
                    using (enumerator = slot.item.artefactPowers.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Skill item = enumerator.Current.Get().skill;
                            if (!skills.Contains(item) || item.stackable)
                            {
                                Enchantment[] relatedEnchantment = item.relatedEnchantment;
                                if ((relatedEnchantment != null) && (relatedEnchantment.Length != 0))
                                {
                                    foreach (Enchantment en in relatedEnchantment)
                                    {
                                        EnchantmentInstance e = enchantments.Find(o => o.source == en);
                                        if (e != null)
                                        {
                                            IEnchantableExtension.RemoveEnchantment(this.owner.Get(), e);
                                        }
                                    }
                                }
                                ISkillableExtension.AddSkill(this.owner.Get(), item);
                            }
                        }
                    }
                }
            }
        }
    }
}

