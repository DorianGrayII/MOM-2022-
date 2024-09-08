namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

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
            return DataBase.Get<BooksAdvantage>(e, false);
        }

        public static explicit operator BooksAdvantage(string e)
        {
            return DataBase.Get<BooksAdvantage>(e, true);
        }

        public void Set_guaranteedSpells(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.guaranteedSpells = new SpellsSection[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is SpellsSection))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("guaranteedSpells of type SpellsSection received invalid type from array! " + text1);
                    }
                    this.guaranteedSpells[i] = list[i] as SpellsSection;
                }
            }
        }

        public void Set_researchLimit(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.researchLimit = new SpellsSection[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is SpellsSection))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("researchLimit of type SpellsSection received invalid type from array! " + text1);
                    }
                    this.researchLimit[i] = list[i] as SpellsSection;
                }
            }
        }

        public void Set_startingSpells(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.startingSpells = new SpellsSection[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is SpellsSection))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("startingSpells of type SpellsSection received invalid type from array! " + text1);
                    }
                    this.startingSpells[i] = list[i] as SpellsSection;
                }
            }
        }
    }
}

