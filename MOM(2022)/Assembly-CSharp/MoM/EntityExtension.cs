namespace MOM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Extension]
    public static class EntityExtension
    {
        [Extension]
        public static void RegisterEntity(Entity obj, int forcedID)
        {
            EntityManager.RegisterEntity(obj, forcedID);
        }

        [Extension]
        public static void UnregisterEntity(Entity obj)
        {
            EntityManager.UnregisterEntity(obj);
        }
    }
}

