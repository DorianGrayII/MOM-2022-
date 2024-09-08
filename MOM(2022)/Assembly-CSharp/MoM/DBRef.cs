using DBDef;

namespace MOM
{
    internal class DBRef
    {
        public static bool Valid<T>(DBReference<T> inst) where T : DBClass
        {
            if (inst == null)
            {
                return false;
            }
            if (inst.Get() != null)
            {
                return true;
            }
            return false;
        }
    }
}
