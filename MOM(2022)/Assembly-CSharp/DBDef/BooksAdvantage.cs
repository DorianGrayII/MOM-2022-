using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("BOOKS_ADVANTAGE", "")]
    public class BooksAdvantage : DBClass
    {
        public static string abbreviation = "";

        [Prototype("BookCount", true)]
        public int bookCount;

        [Prototype("ResearchLimit", true)]
        public SpellsSection[] researchLimit;

        [Prototype("StartingSpells", false)]
        public SpellsSection[] startingSpells;

        [Prototype("GuaranteedSpells", false)]
        public SpellsSection[] guaranteedSpells;

        [Prototype("Bonus", false)]
        public SpellResearchBonus bonus;

        [Prototype("RewardLimit", true)]
        public ERarity rewardLimit;

        public static explicit operator BooksAdvantage(Enum e)
        {
            return DataBase.Get<BooksAdvantage>(e);
        }

        public static explicit operator BooksAdvantage(string e)
        {
            return DataBase.Get<BooksAdvantage>(e, reportMissing: true);
        }

        public void Set_researchLimit(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.researchLimit = new SpellsSection[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is SpellsSection))
                {
                    Debug.LogError("researchLimit of type SpellsSection received invalid type from array! " + list[i]);
                }
                this.researchLimit[i] = list[i] as SpellsSection;
            }
        }

        public void Set_startingSpells(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.startingSpells = new SpellsSection[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is SpellsSection))
                {
                    Debug.LogError("startingSpells of type SpellsSection received invalid type from array! " + list[i]);
                }
                this.startingSpells[i] = list[i] as SpellsSection;
            }
        }

        public void Set_guaranteedSpells(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.guaranteedSpells = new SpellsSection[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is SpellsSection))
                {
                    Debug.LogError("guaranteedSpells of type SpellsSection received invalid type from array! " + list[i]);
                }
                this.guaranteedSpells[i] = list[i] as SpellsSection;
            }
        }
    }
}
