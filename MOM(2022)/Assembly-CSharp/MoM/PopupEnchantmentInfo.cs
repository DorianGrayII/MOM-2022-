// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupEnchantmentInfo
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnchantmentInfo))]
public class PopupEnchantmentInfo : ScreenBase
{
    public Button btClose;

    private EnchantmentInfo info;

    private new void Awake()
    {
        this.info = base.GetComponent<EnchantmentInfo>();
    }

    public void Set(object enchantment)
    {
        this.info.Set(enchantment);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btClose)
        {
            UIManager.Close(this);
        }
    }
}
