using DBUtils;

namespace DBDef
{
    public static class TargetTypeExtension
    {
        public static string GetLocalizedTargetTypeDescription(this string s)
        {
            return global::DBUtils.Localization.Get(s, true);
        }
    }
}
