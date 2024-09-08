namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("TARGET_TYPE", "")]
    public class TargetType : DBClass
    {
        public static string abbreviation = "";
        [Prototype("EnumType", false)]
        public ETargetType enumType;
        [Prototype("DesType", false)]
        public string desType;

        public static explicit operator TargetType(Enum e)
        {
            return DataBase.Get<TargetType>(e, false);
        }

        public static explicit operator TargetType(string e)
        {
            return DataBase.Get<TargetType>(e, true);
        }
    }
}

