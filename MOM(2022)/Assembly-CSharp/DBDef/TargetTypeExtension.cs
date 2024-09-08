namespace DBDef
{
    using DBUtils;
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class TargetTypeExtension
    {
        [Extension]
        public static string GetLocalizedTargetTypeDescription(string s)
        {
            return DBUtils.Localization.Get(s, true, Array.Empty<object>());
        }
    }
}

