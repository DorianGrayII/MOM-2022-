namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("SOUND_LIST", "")]
    public class SoundList : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Sfx", false)]
        public bool IsSFX;
        [Prototype("Sound", true)]
        public Sound[] sound;
        [Prototype("Identifier", false)]
        public string identifier;

        public static explicit operator SoundList(Enum e)
        {
            return DataBase.Get<SoundList>(e, false);
        }

        public static explicit operator SoundList(string e)
        {
            return DataBase.Get<SoundList>(e, true);
        }

        public void Set_sound(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.sound = new Sound[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Sound))
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
                        Debug.LogError("sound of type Sound received invalid type from array! " + text1);
                    }
                    this.sound[i] = list[i] as Sound;
                }
            }
        }
    }
}

