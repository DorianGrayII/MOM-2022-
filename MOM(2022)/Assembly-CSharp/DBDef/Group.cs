namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("GROUP", "")]
    public class Group : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Race", false)]
        public Race[] race;
        [Prototype("Units", false)]
        public Unit[] Units;
        [Prototype("Heroes", false)]
        public Hero[] heroes;
        [Prototype("MaxHeroesCount", false)]
        public int maxHeroesCount;
        [Prototype("RequiredTags", false)]
        public CountedTag[] requiredTags;
        [Prototype("OptionalTags", false)]
        public CountedTag[] optionalTags;
        [Prototype("ForbiddenTags", false)]
        public CountedTag[] forbiddenTags;
        [Prototype("CreationScript", false)]
        public string creationScript;
        [Prototype("Dlc", false)]
        public string dlc;

        public static explicit operator Group(Enum e)
        {
            return DataBase.Get<Group>(e, false);
        }

        public static explicit operator Group(string e)
        {
            return DataBase.Get<Group>(e, true);
        }

        public void Set_forbiddenTags(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.forbiddenTags = new CountedTag[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is CountedTag))
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
                        Debug.LogError("forbiddenTags of type CountedTag received invalid type from array! " + text1);
                    }
                    this.forbiddenTags[i] = list[i] as CountedTag;
                }
            }
        }

        public void Set_heroes(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.heroes = new Hero[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Hero))
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
                        Debug.LogError("heroes of type Hero received invalid type from array! " + text1);
                    }
                    this.heroes[i] = list[i] as Hero;
                }
            }
        }

        public void Set_optionalTags(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.optionalTags = new CountedTag[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is CountedTag))
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
                        Debug.LogError("optionalTags of type CountedTag received invalid type from array! " + text1);
                    }
                    this.optionalTags[i] = list[i] as CountedTag;
                }
            }
        }

        public void Set_race(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.race = new Race[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Race))
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
                        Debug.LogError("race of type Race received invalid type from array! " + text1);
                    }
                    this.race[i] = list[i] as Race;
                }
            }
        }

        public void Set_requiredTags(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.requiredTags = new CountedTag[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is CountedTag))
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
                        Debug.LogError("requiredTags of type CountedTag received invalid type from array! " + text1);
                    }
                    this.requiredTags[i] = list[i] as CountedTag;
                }
            }
        }

        public void Set_Units(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.Units = new Unit[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Unit))
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
                        Debug.LogError("Units of type Unit received invalid type from array! " + text1);
                    }
                    this.Units[i] = list[i] as Unit;
                }
            }
        }
    }
}

