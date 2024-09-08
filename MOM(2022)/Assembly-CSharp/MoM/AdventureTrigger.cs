namespace MOM
{
    using MOM.Adventures;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

    [ProtoContract]
    public class AdventureTrigger
    {
        [ProtoMember(1)]
        public string module;
        [ProtoMember(2)]
        public int adventure;

        public Adventure Get()
        {
            List<Module> modules = AdventureLibrary.currentLibrary.GetModules();
            if (modules == null)
            {
                return null;
            }
            Module module = modules.Find(o => o.name == this.module);
            return (((module == null) || (module.adventures == null)) ? null : module.adventures.Find(o => o.uniqueID == this.adventure));
        }

        public void Set(Adventure a)
        {
            a.ReAcquireModule();
            this.module = a.module.name;
            this.adventure = a.uniqueID;
        }
    }
}

