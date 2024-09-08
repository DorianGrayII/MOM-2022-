namespace DBDef
{
    using DBUtils;
    using MHUtils;
    using System;

    [ClassPrototype("DESCRIPTION_INFO", "")]
    public class DescriptionInfo : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Name", true)]
        private string name;
        [Prototype("Description", true)]
        private string description;
        [Prototype("Graphic", false)]
        public string graphic;
        [Prototype("Param", false)]
        private string param;

        public string GetDescriptionKey()
        {
            return this.description;
        }

        public string GetLocalizedDescription()
        {
            if ((this.param == null) || (this.param.Length <= 0))
            {
                return DBUtils.Localization.Get(this.description, true, Array.Empty<object>());
            }
            object[] parameters = new object[] { this.param };
            return DBUtils.Localization.Get(this.description, true, parameters);
        }

        public string GetLocalizedName()
        {
            if ((this.param == null) || (this.param.Length <= 0))
            {
                return DBUtils.Localization.Get(this.name, true, Array.Empty<object>());
            }
            object[] parameters = new object[] { this.param };
            return DBUtils.Localization.Get(this.name, true, parameters);
        }

        public string GetName()
        {
            return this.name;
        }

        public static explicit operator DescriptionInfo(Enum e)
        {
            return DataBase.Get<DescriptionInfo>(e, false);
        }

        public static explicit operator DescriptionInfo(string e)
        {
            return DataBase.Get<DescriptionInfo>(e, true);
        }
    }
}

