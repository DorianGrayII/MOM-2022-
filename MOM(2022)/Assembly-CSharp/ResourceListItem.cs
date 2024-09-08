// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ResourceListItem
using DBDef;
using MHUtils;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceListItem : ListItem
{
    [Tooltip("The image that represents the resource")]
    public RawImage image;

    public new TextMeshProUGUI name;

    public TextMeshProUGUI description;

    [Tooltip("If true a roll over tooltip will be displayed when hovering")]
    public bool supportTooltip = true;

    private RolloverObject tooltip;

    private void Awake()
    {
        if (this.supportTooltip)
        {
            this.tooltip = base.gameObject.GetOrAddComponent<RolloverObject>();
        }
    }

    public override void Set(object o, object data = null, int index = -1)
    {
        Resource resource = o as Resource;
        if (resource == null)
        {
            Multitype<Resource, bool> multitype = o as Multitype<Resource, bool>;
            resource = multitype.t0;
            if ((bool)this.image)
            {
                this.image.material = (multitype.t1 ? null : UIReferences.GetGrayscale());
            }
        }
        DescriptionInfo descriptionInfo = resource.GetDescriptionInfo();
        if ((bool)this.image)
        {
            this.image.texture = AssetManager.Get<Texture2D>(descriptionInfo.graphic);
        }
        if ((bool)this.name)
        {
            this.name.text = descriptionInfo.GetLocalizedName();
        }
        if ((bool)this.description)
        {
            this.description.text = descriptionInfo.GetLocalizedDescription();
        }
        if ((bool)this.tooltip)
        {
            this.tooltip.source = resource;
        }
    }
}
