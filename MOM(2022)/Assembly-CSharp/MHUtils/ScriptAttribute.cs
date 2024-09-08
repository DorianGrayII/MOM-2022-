using System;

namespace MHUtils
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ScriptAttribute : Attribute
    {
        public Type parameterExpected;

        public Type requiredTextParameter;

        public ScriptAttribute(Type parameterExpected, Type requiredTextParameter = null)
        {
            this.parameterExpected = parameterExpected;
            this.requiredTextParameter = requiredTextParameter;
        }
    }
}
