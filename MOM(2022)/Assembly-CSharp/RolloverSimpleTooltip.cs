using DBDef;
using DBUtils;
using MHUtils;
using System;
using UnityEngine;

public class RolloverSimpleTooltip : RolloverBase
{
    public string sourceAsDbName;
    public string title;
    public string description;
    public string imageName;
    public string data;
    public Texture2D image;
    public object[] descriptionParams;

    public string GetDescription()
    {
        if (!string.IsNullOrEmpty(this.sourceAsDbName))
        {
            DBClass class2 = DataBase.Get(this.sourceAsDbName, true);
            if (class2 == null)
            {
                return null;
            }
            DescriptionInfo descriptionInfo = class2 as DescriptionInfo;
            if ((descriptionInfo == null) && (class2 is IDescriptionInfoType))
            {
                descriptionInfo = (class2 as IDescriptionInfoType).GetDescriptionInfo();
            }
            if (descriptionInfo != null)
            {
                return descriptionInfo.GetLocalizedDescription();
            }
        }
        return ((this.descriptionParams == null) ? this.description : DBUtils.Localization.Get(this.description, true, this.descriptionParams));
    }

    public Texture2D GetIcon()
    {
        return ((this.image == null) ? AssetManager.Get<Texture2D>(this.GetIconName(), true) : this.image);
    }

    public string GetIconName()
    {
        if (!string.IsNullOrEmpty(this.sourceAsDbName))
        {
            DBClass class2 = DataBase.Get(this.sourceAsDbName, true);
            if (class2 == null)
            {
                return null;
            }
            DescriptionInfo descriptionInfo = class2 as DescriptionInfo;
            if ((descriptionInfo == null) && (class2 is IDescriptionInfoType))
            {
                descriptionInfo = (class2 as IDescriptionInfoType).GetDescriptionInfo();
            }
            if (descriptionInfo != null)
            {
                return descriptionInfo.graphic;
            }
        }
        return this.imageName;
    }

    public string GetTitle()
    {
        if (!string.IsNullOrEmpty(this.sourceAsDbName) && string.IsNullOrEmpty(this.title))
        {
            DBClass class2 = DataBase.Get(this.sourceAsDbName, true);
            if (class2 == null)
            {
                return null;
            }
            DescriptionInfo descriptionInfo = class2 as DescriptionInfo;
            if ((descriptionInfo == null) && (class2 is IDescriptionInfoType))
            {
                descriptionInfo = (class2 as IDescriptionInfoType).GetDescriptionInfo();
            }
            if (descriptionInfo != null)
            {
                return descriptionInfo.GetLocalizedName();
            }
        }
        return this.title;
    }
}

