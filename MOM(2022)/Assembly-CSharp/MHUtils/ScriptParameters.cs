using System;

namespace MHUtils
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScriptParameters : Attribute
    {
        public Type dropdownType;

        public Type variableType;

        public object dropdownListType;

        public string displayName;

        public ScriptParameters(Type dropdownType, Type variableType = null)
        {
            this.dropdownType = dropdownType;
            this.variableType = variableType;
        }

        public ScriptParameters(object dropdownListType, Type variableType = null)
        {
            this.dropdownListType = dropdownListType;
            this.variableType = variableType;
        }

        public ScriptParameters(string displayName)
        {
            this.displayName = displayName;
        }
    }
}
