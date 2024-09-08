using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("SOUND", "")]
    public class Sound : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Name", true)]
        public string name;

        [Prototype("File", true)]
        public string file;

        public static explicit operator Sound(Enum e)
        {
            return DataBase.Get<Sound>(e);
        }

        public static explicit operator Sound(string e)
        {
            return DataBase.Get<Sound>(e, reportMissing: true);
        }
    }
}
