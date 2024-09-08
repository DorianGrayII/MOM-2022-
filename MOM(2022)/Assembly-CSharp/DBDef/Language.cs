namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("LANGUAGE", "")]
    public class Language : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";
        [Prototype("DescriptionInfo", true)]
        public DescriptionInfo descriptionInfo;
        [Prototype("NameSuffix", true)]
        public string nameSuffix;
        [Prototype("LanguageID", true)]
        public string languageID;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Language(Enum e)
        {
            return DataBase.Get<Language>(e, false);
        }

        public static explicit operator Language(string e)
        {
            return DataBase.Get<Language>(e, true);
        }
    }
}

