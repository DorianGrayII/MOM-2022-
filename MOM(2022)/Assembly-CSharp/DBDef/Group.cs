using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
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
            return DataBase.Get<Group>(e);
        }

        public static explicit operator Group(string e)
        {
            return DataBase.Get<Group>(e, reportMissing: true);
        }

        public void Set_race(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.race = new Race[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Race))
                {
                    Debug.LogError("race of type Race received invalid type from array! " + list[i]);
                }
                this.race[i] = list[i] as Race;
            }
        }

        public void Set_Units(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.Units = new Unit[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Unit))
                {
                    Debug.LogError("Units of type Unit received invalid type from array! " + list[i]);
                }
                this.Units[i] = list[i] as Unit;
            }
        }

        public void Set_heroes(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.heroes = new Hero[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Hero))
                {
                    Debug.LogError("heroes of type Hero received invalid type from array! " + list[i]);
                }
                this.heroes[i] = list[i] as Hero;
            }
        }

        public void Set_requiredTags(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.requiredTags = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("requiredTags of type CountedTag received invalid type from array! " + list[i]);
                }
                this.requiredTags[i] = list[i] as CountedTag;
            }
        }

        public void Set_optionalTags(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.optionalTags = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("optionalTags of type CountedTag received invalid type from array! " + list[i]);
                }
                this.optionalTags[i] = list[i] as CountedTag;
            }
        }

        public void Set_forbiddenTags(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.forbiddenTags = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("forbiddenTags of type CountedTag received invalid type from array! " + list[i]);
                }
                this.forbiddenTags[i] = list[i] as CountedTag;
            }
        }
    }
}
