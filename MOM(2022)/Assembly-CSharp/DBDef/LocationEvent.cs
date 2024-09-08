using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("LOCATION_EVENT", "")]
    public class LocationEvent : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Module", true)]
        public string module;

        [Prototype("Adventure", true)]
        public int adventure;

        public static explicit operator LocationEvent(Enum e)
        {
            return DataBase.Get<LocationEvent>(e);
        }

        public static explicit operator LocationEvent(string e)
        {
            return DataBase.Get<LocationEvent>(e, reportMissing: true);
        }
    }
}
