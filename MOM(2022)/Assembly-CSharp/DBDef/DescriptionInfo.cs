using System;
using DBUtils;
using MHUtils;

namespace DBDef
{
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

        public string GetLocalizedDescription()
        {
            if (this.param != null && this.param.Length > 0)
            {
                return global::DBUtils.Localization.Get(this.description, true, this.param);
            }
            return global::DBUtils.Localization.Get(this.description, true);
        }

        public string GetLocalizedName()
        {
            if (this.param != null && this.param.Length > 0)
            {
                return global::DBUtils.Localization.Get(this.name, true, this.param);
            }
            return global::DBUtils.Localization.Get(this.name, true);
        }

        public string GetDescriptionKey()
        {
            return this.description;
        }

        public string GetName()
        {
            return this.name;
        }

        public static explicit operator DescriptionInfo(Enum e)
        {
            return DataBase.Get<DescriptionInfo>(e);
        }

        public static explicit operator DescriptionInfo(string e)
        {
            return DataBase.Get<DescriptionInfo>(e, reportMissing: true);
        }
    }
}
