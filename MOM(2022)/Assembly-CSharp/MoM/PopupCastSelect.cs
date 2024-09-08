// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupCastSelect
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnityEngine.UI;

public class PopupCastSelect : ScreenBase
{
    private static PopupCastSelect instance;

    public Button btCancel;

    public GridItemManager gridUnits;

    private Spell spell;

    private Callback select;

    private Callback cancel;

    public static void OpenPopup(ScreenBase parent, object units, Callback cancel = null, Callback select = null, Spell spell = null)
    {
        PopupCastSelect.instance = UIManager.Open<PopupCastSelect>(UIManager.Layer.Popup, parent);
        PopupCastSelect.instance.cancel = cancel;
        PopupCastSelect.instance.select = select;
        PopupCastSelect.instance.spell = spell;
        PopupCastSelect.instance.gridUnits.CustomDynamicItem(PopupCastSelect.instance.UnitItem);
        if (units is List<Reference<global::MOM.Unit>>)
        {
            PopupCastSelect.instance.gridUnits.UpdateGrid(units as List<Reference<global::MOM.Unit>>);
        }
        else
        {
            PopupCastSelect.instance.gridUnits.UpdateGrid(units as List<BattleUnit>);
        }
    }

    public static bool IsOpen()
    {
        return PopupCastSelect.instance != null;
    }

    public override IEnumerator Closing()
    {
        PopupCastSelect.instance = null;
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
        if (this.spell != null)
        {
            bool flag3 = true;
            if (!string.IsNullOrEmpty(this.spell.targetingScript))
            {
                PlayerWizard humanWizard = GameManager.GetHumanWizard();
                flag3 = (bool)ScriptLibrary.Call(this.spell.targetingScript, new SpellCastData(humanWizard, null), u, this.spell);
            }
            component.portrait.material = (flag3 ? null : UIReferences.GetGrayscale());
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
            UIManager.Close(this);
            if (this.select != null)
            {
                this.select(source);
            }
        });
        component.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
        {
            ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
            UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Popup, componentInParent);
            List<object> list = null;
            if ((bool)this.gridUnits)
            {
                foreach (object item2 in this.gridUnits.GetItems())
                {
                    if (list == null)
                    {
                        list = new List<object>();
                    }
                    if (item2 is Reference<global::MOM.Unit>)
                    {
                        list.Add((item2 as Reference<global::MOM.Unit>).Get());
                    }
                    else
                    {
                        list.Add(item2);
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
