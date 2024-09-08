using System;

namespace DBDef
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassPrototype : Attribute
    {
        public string dbName;

        public string dbAbbreviation;

        public ClassPrototype(string name)
        {
            this.dbName = name;
        }

        public ClassPrototype(string name, string abbreviation)
        {
            this.dbName = name;
            this.dbAbbreviation = abbreviation;
        }
    }
}
