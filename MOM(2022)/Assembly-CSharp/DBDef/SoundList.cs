using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
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
            return DataBase.Get<SoundList>(e);
        }

        public static explicit operator SoundList(string e)
        {
            return DataBase.Get<SoundList>(e, reportMissing: true);
        }

        public void Set_sound(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.sound = new Sound[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Sound))
                {
                    Debug.LogError("sound of type Sound received invalid type from array! " + list[i]);
                }
                this.sound[i] = list[i] as Sound;
            }
        }
    }
}
