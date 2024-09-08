namespace MHUtils
{
    using MOM.Adventures;
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class ScriptRetType : Attribute
    {
        public LogicEntry.LEntry type;

        public ScriptRetType(LogicEntry.LEntry type)
        {
            this.type = type;
        }
    }
}

