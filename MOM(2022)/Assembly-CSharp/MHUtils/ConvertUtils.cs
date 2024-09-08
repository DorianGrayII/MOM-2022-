using UnityEngine;

namespace MHUtils
{
    public static class ConvertUtils
    {
        public static string SInt(this int source)
        {
            if (source < 0)
            {
                return source.ToString();
            }
            return "+" + source;
        }

        public static string ColorSInt(this int source)
        {
            if (source < 0)
            {
                return "<#FF8000>" + source + "</color>";
            }
            return "+" + source;
        }

        public static string Percent(this float source)
        {
            return Mathf.RoundToInt(source * 100f) + "%";
        }
    }
}
