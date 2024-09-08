using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("HERO", "")]
    public class Hero : Subrace
    {
        public new static string abbreviation = "";

        [Prototype("AlterName", false)]
        public string[] alterName;

        [Prototype("Champion", false)]
        public bool champion;

        [Prototype("RecruitmentCost", true)]
        public int recruitmentCost;

        [Prototype("RecruitmentMinFame", true)]
        public int recruitmentMinFame;

        [Prototype("RecruitmentMinBooks", false)]
        public CountedTag[] recruitmentMinBooks;

        [Prototype("SkillPacks", false)]
        public SkillPack[] skillPacks;

        [Prototype("EquipmentSlot", true)]
        public ArtefactSlot[] equipmentSlot;

        public static explicit operator Hero(Enum e)
        {
            return DataBase.Get<Hero>(e);
        }

        public static explicit operator Hero(string e)
        {
            return DataBase.Get<Hero>(e, reportMissing: true);
        }

        public void Set_alterName(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.alterName = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is string))
                {
                    Debug.LogError("alterName of type string received invalid type from array! " + list[i]);
                }
                this.alterName[i] = list[i] as string;
            }
        }

        public void Set_recruitmentMinBooks(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.recruitmentMinBooks = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("recruitmentMinBooks of type CountedTag received invalid type from array! " + list[i]);
                }
                this.recruitmentMinBooks[i] = list[i] as CountedTag;
            }
        }

        public void Set_skillPacks(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.skillPacks = new SkillPack[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is SkillPack))
                {
                    Debug.LogError("skillPacks of type SkillPack received invalid type from array! " + list[i]);
                }
                this.skillPacks[i] = list[i] as SkillPack;
            }
        }

        public void Set_equipmentSlot(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.equipmentSlot = new ArtefactSlot[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is ArtefactSlot))
                {
                    Debug.LogError("equipmentSlot of type ArtefactSlot received invalid type from array! " + list[i]);
                }
                this.equipmentSlot[i] = list[i] as ArtefactSlot;
            }
        }
    }
}
