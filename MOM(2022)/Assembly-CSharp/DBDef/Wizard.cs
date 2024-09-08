using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("WIZARD", "")]
    public class Wizard : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";

        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;

        [Prototype("Quote", false)]
        public string quote;

        [Prototype("PersonalGreeting", false)]
        public string personalGreeting;

        [Prototype("Icon", false)]
        public string icon;

        [Prototype("Background", false)]
        public string background;

        [Prototype("Tags", false)]
        public CountedTag[] tags;

        [Prototype("Trait", false)]
        public Trait[] traits;

        [Prototype("Dlc", false)]
        public string dlc;

        [Prototype("AssociateInitialGreeting", false)]
        public Associate_Greeting[] associateInitialGreeting;

        [Prototype("AssociateGreeting", false)]
        public Associate_Greeting[] associateGreeting;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Wizard(Enum e)
        {
            return DataBase.Get<Wizard>(e);
        }

        public static explicit operator Wizard(string e)
        {
            return DataBase.Get<Wizard>(e, reportMissing: true);
        }

        public void Set_tags(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.tags = new CountedTag[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is CountedTag))
                {
                    Debug.LogError("tags of type CountedTag received invalid type from array! " + list[i]);
                }
                this.tags[i] = list[i] as CountedTag;
            }
        }

        public void Set_traits(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.traits = new Trait[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Trait))
                {
                    Debug.LogError("traits of type Trait received invalid type from array! " + list[i]);
                }
                this.traits[i] = list[i] as Trait;
            }
        }

        public void Set_associateInitialGreeting(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.associateInitialGreeting = new Associate_Greeting[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Associate_Greeting))
                {
                    Debug.LogError("associateInitialGreeting of type Associate_Greeting received invalid type from array! " + list[i]);
                }
                this.associateInitialGreeting[i] = list[i] as Associate_Greeting;
            }
        }

        public void Set_associateGreeting(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.associateGreeting = new Associate_Greeting[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Associate_Greeting))
                {
                    Debug.LogError("associateGreeting of type Associate_Greeting received invalid type from array! " + list[i]);
                }
                this.associateGreeting[i] = list[i] as Associate_Greeting;
            }
        }
    }
}
