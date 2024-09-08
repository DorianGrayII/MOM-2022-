namespace DBDef
{
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class ResourceExtension
    {
        [Extension]
        public static string GetModel3dName(Resource r)
        {
            return ((r.model3d == null) ? r.GetDescriptionInfo().graphic : r.model3d);
        }
    }
}

