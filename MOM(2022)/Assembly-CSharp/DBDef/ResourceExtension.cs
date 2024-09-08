namespace DBDef
{
    public static class ResourceExtension
    {
        public static string GetModel3dName(this Resource r)
        {
            if (r.model3d != null)
            {
                return r.model3d;
            }
            return r.GetDescriptionInfo().graphic;
        }
    }
}
