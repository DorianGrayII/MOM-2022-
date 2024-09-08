namespace MOM
{
    public static class EntityExtension
    {
        public static void RegisterEntity(this Entity obj, int forcedID = 0)
        {
            EntityManager.RegisterEntity(obj, forcedID);
        }

        public static void UnregisterEntity(this Entity obj)
        {
            EntityManager.UnregisterEntity(obj);
        }
    }
}
