namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("DIFFICULTY_OPTION", "")]
    public class DifficultyOption : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Title", true)]
        public string title;
        [Prototype("Value", true)]
        public string value;
        [Prototype("Difficulty", true)]
        public float difficulty;
        [Prototype("Collection", true)]
        public int collection;
        [Prototype("QuickStart", false)]
        public bool quickStart;

        public static explicit operator DifficultyOption(Enum e)
        {
            return DataBase.Get<DifficultyOption>(e, false);
        }

        public static explicit operator DifficultyOption(string e)
        {
            return DataBase.Get<DifficultyOption>(e, true);
        }
    }
}

