using MHUtils;
using UnityEngine;

namespace DBDef
{
    public static class DescriptionInfoExtension
    {
        public static Texture2D GetTexture(this DescriptionInfo di)
        {
            return AssetManager.Get<Texture2D>(di.graphic);
        }

        public static Texture2D GetTexture(this IDescriptionInfoType t)
        {
            return t.GetDescriptionInfo()?.GetTexture();
        }

        public static Texture2D GetTextureLarge(this DescriptionInfo di)
        {
            return AssetManager.Get<Texture2D>(di.graphic + "_FULL");
        }

        public static Texture2D GetTextureLarge(this IDescriptionInfoType t)
        {
            return t.GetDescriptionInfo()?.GetTextureLarge();
        }

        public static string GetDILocalizedName(this IDescriptionInfoType t)
        {
            DescriptionInfo descriptionInfo = t.GetDescriptionInfo();
            if (descriptionInfo == null)
            {
                return "DB Missing DI";
            }
            return descriptionInfo.GetLocalizedName();
        }

        public static string GetDILocalizedDescription(this IDescriptionInfoType t)
        {
            DescriptionInfo descriptionInfo = t.GetDescriptionInfo();
            if (descriptionInfo == null)
            {
                return "DB Missing DI";
            }
            return descriptionInfo.GetLocalizedDescription();
        }
    }
}
