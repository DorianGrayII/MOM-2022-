namespace MOM
{
    using System;

    internal class DBRef
    {
        public static bool Valid<T>(DBReference<T> inst) where T: DBClass
        {
            return ((inst != null) ? (inst.Get() != null) : false);
        }
    }
}

