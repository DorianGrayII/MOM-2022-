using DBDef;
using DBUtils;
using MHUtils;
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

    public string GetTitle()
    {
        if (!string.IsNullOrEmpty(this.sourceAsDbName) && string.IsNullOrEmpty(this.title))
        {
            DBClass dBClass = DataBase.Get(this.sourceAsDbName, reportMissing: true);
            if (dBClass == null)
            {
                return null;
            }
            DescriptionInfo descriptionInfo = dBClass as DescriptionInfo;
            if (descriptionInfo == null && dBClass is IDescriptionInfoType)
            {
                descriptionInfo = (dBClass as IDescriptionInfoType).GetDescriptionInfo();
            }
            if (descriptionInfo != null)
            {
                return descriptionInfo.GetLocalizedName();
            }
        }
        return this.title;
    }

    public string GetDescription()
    {
        if (!string.IsNullOrEmpty(this.sourceAsDbName))
        {
            DBClass dBClass = DataBase.Get(this.sourceAsDbName, reportMissing: true);
            if (dBClass == null)
            {
                return null;
            }
            DescriptionInfo descriptionInfo = dBClass as DescriptionInfo;
            if (descriptionInfo == null && dBClass is IDescriptionInfoType)
            {
                descriptionInfo = (dBClass as IDescriptionInfoType).GetDescriptionInfo();
            }
            if (descriptionInfo != null)
            {
                return descriptionInfo.GetLocalizedDescription();
            }
        }
        if (this.descriptionParams != null)
        {
            return global::DBUtils.Localization.Get(this.description, symbolParse: true, this.descriptionParams);
        }
        return this.description;
    }

    public string GetIconName()
    {
        if (!string.IsNullOrEmpty(this.sourceAsDbName))
        {
            DBClass dBClass = DataBase.Get(this.sourceAsDbName, reportMissing: true);
            if (dBClass == null)
            {
                return null;
            }
            DescriptionInfo descriptionInfo = dBClass as DescriptionInfo;
            if (descriptionInfo == null && dBClass is IDescriptionInfoType)
            {
                descriptionInfo = (dBClass as IDescriptionInfoType).GetDescriptionInfo();
            }
            if (descriptionInfo != null)
            {
                return descriptionInfo.graphic;
            }
        }
        return this.imageName;
    }

    public Texture2D GetIcon()
    {
        if (this.image != null)
        {
            return this.image;
        }
        return AssetManager.Get<Texture2D>(this.GetIconName());
    }
}
