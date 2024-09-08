// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupUnits
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUnits : ScreenBase
{
    private static PopupUnits instance;

    public Button btCancel;

    public TextMeshProUGUI heading;

    public GridItemManager gridUnits;

    private Callback select;

    private Callback cancel;

    public static void OpenPopup(ScreenBase parent, object units, string title = null, Callback cancel = null, Callback select = null)
    {
        PopupUnits.instance = UIManager.Open<PopupUnits>(UIManager.Layer.Popup, parent);
        PopupUnits.instance.cancel = cancel;
        PopupUnits.instance.select = select;
        PopupUnits.instance.heading.text = ((title != null) ? title : "UI_UNITS");
        PopupUnits.instance.gridUnits.CustomDynamicItem(PopupUnits.instance.UnitItem);
        if (units is List<Reference<global::MOM.Unit>>)
        {
            PopupUnits.instance.gridUnits.UpdateGrid(units as List<Reference<global::MOM.Unit>>);
        }
        else
        {
            PopupUnits.instance.gridUnits.UpdateGrid(units as List<BattleUnit>);
        }
    }

    public static bool IsOpen()
    {
        return PopupUnits.instance != null;
    }

    public override IEnumerator Closing()
    {
        PopupUnits.instance = null;
        yield return base.Closing();
    }

    private void UnitItem(GameObject itemSource, object source, object data, int index)
    {
        BaseUnit u = null;
        if (source is Reference<global::MOM.Unit>)
        {
            u = (source as Reference<global::MOM.Unit>).Get();
        }
        if (source is BattleUnit)
        {
            u = source as BattleUnit;
        }
        CharacterListItem component = itemSource.GetComponent<CharacterListItem>();
        component.hp.value = u.GetTotalHpPercent();
        component.portrait.texture = u.dbSource.Get().GetDescriptionInfo().GetTexture();
        if (component.race != null)
        {
            if (u.GetAttFinal((Tag)TAG.REANIMATED) > 0)
            {
                component.race.gameObject.SetActive(value: true);
                component.race.texture = ((Race)RACE.REALM_DEATH).GetDescriptionInfo().GetTexture();
            }
            else if (u.GetAttFinal((Tag)TAG.FANTASTIC_CLASS) > 0)
            {
                component.race.gameObject.SetActive(value: true);
                component.race.texture = u.race.Get().GetDescriptionInfo().GetTexture();
            }
            else
            {
                component.race.gameObject.SetActive(value: false);
            }
        }
        if (component.goodEnchantment != null && component.badEnchantment != null && component.goodBadEnchantment != null)
        {
            List<EnchantmentInstance> enchantmentsWithRemotes = u.GetEnchantmentManager().GetEnchantmentsWithRemotes();
            bool flag = false;
            bool flag2 = false;
            foreach (EnchantmentInstance item in enchantmentsWithRemotes)
            {
                if (item.GetEnchantmentType() == EEnchantmentCategory.Positive)
                {
                    flag = true;
                }
                else if (item.GetEnchantmentType() == EEnchantmentCategory.Negative)
                {
                    flag2 = true;
                }
            }
            component.goodEnchantment.SetActive(value: false);
            component.badEnchantment.SetActive(value: false);
            component.goodBadEnchantment.SetActive(value: false);
            if (flag && flag2)
            {
                component.goodBadEnchantment.SetActive(value: true);
            }
            else if (flag)
            {
                component.goodEnchantment.SetActive(value: true);
            }
            else if (flag2)
            {
                component.badEnchantment.SetActive(value: true);
            }
        }
        Tag uType = ((u.dbSource.Get() is Hero) ? ((Tag)TAG.HERO_CLASS) : ((Tag)TAG.NORMAL_CLASS));
        UnitLvl unitLvl = DataBase.GetType<UnitLvl>().Find((UnitLvl o) => o.unitClass == uType && o.level == u.GetLevel());
        if (unitLvl != null)
        {
            component.unitLevel.gameObject.SetActive(value: true);
            component.unitLevel.texture = unitLvl.GetDescriptionInfo().GetTexture();
        }
        else
        {
            component.unitLevel.gameObject.SetActive(value: false);
        }
        component.button.onClick.RemoveAllListeners();
        component.button.onClick.AddListener(delegate
        {
            if (this.select != null)
            {
                this.select(source);
            }
            else
            {
                ScreenBase componentInParent2 = base.GetComponentInParent<ScreenBase>();
                UnitInfo unitInfo2 = UIManager.Open<UnitInfo>(UIManager.Layer.Popup, componentInParent2);
                List<object> list2 = null;
                if ((bool)this.gridUnits)
                {
                    foreach (object item2 in this.gridUnits.GetItems())
                    {
                        if (list2 == null)
                        {
                            list2 = new List<object>();
                        }
                        if (item2 is Reference<global::MOM.Unit>)
                        {
                            list2.Add((item2 as Reference<global::MOM.Unit>).Get());
                        }
                        else
                        {
                            list2.Add(item2);
                        }
                    }
                }
                unitInfo2.SetData(list2, u);
            }
        });
        component.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
        {
            ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
            UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Popup, componentInParent);
            List<object> list = null;
            if ((bool)this.gridUnits)
            {
                foreach (object item3 in this.gridUnits.GetItems())
                {
                    if (list == null)
                    {
                        list = new List<object>();
                    }
                    if (item3 is Reference<global::MOM.Unit>)
                    {
                        list.Add((item3 as Reference<global::MOM.Unit>).Get());
                    }
                    else
                    {
                        list.Add(item3);
                    }
                }
            }
            unitInfo.SetData(list, u);
        };
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btCancel)
        {
            UIManager.Close(this);
            if (this.cancel != null)
            {
                this.cancel(null);
            }
        }
    }
}
