// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// EnchantmentGrid
using System.Collections.Generic;
using MHUtils.UI;
using MOM;
using UnityEngine;

[RequireComponent(typeof(GridItemManager))]
public class EnchantmentGrid : MonoBehaviour
{
    private List<object> enchantments = new List<object>();

    private GridItemManager gridManager;

    public void SetEnchantments<T>(List<T> enchantments)
    {
        this.enchantments.Clear();
        foreach (T enchantment in enchantments)
        {
            if (!(enchantment is EnchantmentInstance enchantmentInstance) || !enchantmentInstance.buildingEnchantment)
            {
                this.enchantments.Add(enchantment);
            }
        }
        this.gridManager.UpdateGrid(this.enchantments);
    }

    public void Awake()
    {
        this.gridManager = base.GetComponent<GridItemManager>();
        this.gridManager.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            itemSource.GetComponent<EnchantmentListItem>().Set(source);
        }, delegate
        {
            this.gridManager.UpdateGrid(this.enchantments);
        });
    }
}
