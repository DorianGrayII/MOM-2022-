namespace MHUtils
{
    using System;
    using System.Runtime.InteropServices;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public class ScriptAttribute : Attribute
    {
        public System.Type parameterExpected;
        public System.Type requiredTextParameter;

        public ScriptAttribute(System.Type parameterExpected, System.Type requiredTextParameter)
        {
            this.parameterExpected = parameterExpected;
            this.requiredTextParameter = requiredTextParameter;
        }
    }
}

