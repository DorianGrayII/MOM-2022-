namespace MHUtils
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public class ScriptParameters : Attribute
    {
        public System.Type dropdownType;
        public System.Type variableType;
        public object dropdownListType;
        public string displayName;

        public ScriptParameters(string displayName)
        {
            this.displayName = displayName;
        }

        public ScriptParameters(object dropdownListType, System.Type variableType)
        {
            this.dropdownListType = dropdownListType;
            this.variableType = variableType;
        }

        public ScriptParameters(System.Type dropdownType, System.Type variableType)
        {
            this.dropdownType = dropdownType;
            this.variableType = variableType;
        }
    }
}

