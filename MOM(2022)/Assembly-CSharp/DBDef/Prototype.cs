using System;

namespace DBDef
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Prototype : Attribute
    {
        public string name;

        public bool isEnum;

        public bool required;

        public Prototype(string name, bool required)
        {
            this.name = name;
            this.isEnum = false;
            this.required = required;
        }
    }
}
