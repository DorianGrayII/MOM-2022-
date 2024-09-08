using System.Collections.Generic;
using MOM.Adventures;
using ProtoBuf;

namespace MOM
{
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
            Module module = modules.Find((Module o) => o.name == this.module);
            if (module == null || module.adventures == null)
            {
                return null;
            }
            return module.adventures.Find((Adventure o) => o.uniqueID == this.adventure);
        }

        public void Set(Adventure a)
        {
            a.ReAcquireModule();
            this.module = a.module.name;
            this.adventure = a.uniqueID;
        }
    }
}
