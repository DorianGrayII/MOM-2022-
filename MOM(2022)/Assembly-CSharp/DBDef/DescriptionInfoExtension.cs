namespace DBDef
{
    using MHUtils;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Extension]
    public static class DescriptionInfoExtension
    {
        [Extension]
        public static string GetDILocalizedDescription(IDescriptionInfoType t)
        {
            DescriptionInfo descriptionInfo = t.GetDescriptionInfo();
            return ((descriptionInfo != null) ? descriptionInfo.GetLocalizedDescription() : "DB Missing DI");
        }

        [Extension]
        public static string GetDILocalizedName(IDescriptionInfoType t)
        {
            DescriptionInfo descriptionInfo = t.GetDescriptionInfo();
            return ((descriptionInfo != null) ? descriptionInfo.GetLocalizedName() : "DB Missing DI");
        }

        [Extension]
        public static Texture2D GetTexture(DescriptionInfo di)
        {
            return AssetManager.Get<Texture2D>(di.graphic, true);
        }

        [Extension]
        public static Texture2D GetTexture(IDescriptionInfoType t)
        {
            DescriptionInfo descriptionInfo = t.GetDescriptionInfo();
            return ((descriptionInfo != null) ? GetTexture(descriptionInfo) : null);
        }

        [Extension]
        public static Texture2D GetTextureLarge(DescriptionInfo di)
        {
            return AssetManager.Get<Texture2D>(di.graphic + "_FULL", true);
        }

        [Extension]
        public static Texture2D GetTextureLarge(IDescriptionInfoType t)
        {
            DescriptionInfo descriptionInfo = t.GetDescriptionInfo();
            return ((descriptionInfo != null) ? GetTextureLarge(descriptionInfo) : null);
        }
    }
}

