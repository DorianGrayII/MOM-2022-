namespace MHUtils
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Extension]
    public static class ConvertUtils
    {
        [Extension]
        public static string ColorSInt(int source)
        {
            return ((source >= 0) ? ("+" + source.ToString()) : ("<#FF8000>" + source.ToString() + "</color>"));
        }

        [Extension]
        public static string Percent(float source)
        {
            return (Mathf.RoundToInt(source * 100f).ToString() + "%");
        }

        [Extension]
        public static string SInt(int source)
        {
            return ((source >= 0) ? ("+" + source.ToString()) : source.ToString());
        }
    }
}

