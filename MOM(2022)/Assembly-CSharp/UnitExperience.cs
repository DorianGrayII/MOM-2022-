using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UnitExperience : MonoBehaviour
{
    public void Set(BaseUnit u)
    {
        RawImage component = base.GetComponent<RawImage>();
        bool flag = false;
        if (u != null)
        {
            Tag uType = (u.dbSource.Get() is Hero) ? ((Tag) TAG.HERO_CLASS) : ((Tag) TAG.NORMAL_CLASS);
            UnitLvl lvl = DataBase.GetType<UnitLvl>().Find(o => ReferenceEquals(o.unitClass, uType) && (o.level == u.GetLevel()));
            if ((lvl != null) && (lvl.level != 1))
            {
                flag = true;
                component.texture = DescriptionInfoExtension.GetTexture(lvl.GetDescriptionInfo());
                RolloverSimpleTooltip orAddComponent = GameObjectUtils.GetOrAddComponent<RolloverSimpleTooltip>(component.gameObject);
                orAddComponent.sourceAsDbName = lvl.dbName;
                orAddComponent.useMouseLocation = false;
                orAddComponent.anchor.x = 0.4f;
                orAddComponent.anchor.y = 1f;
            }
        }
        component.gameObject.SetActive(flag);
    }

    public void Set(BaseUnit u, TextMeshProUGUI labelExp)
    {
        RawImage component = base.GetComponent<RawImage>();
        bool flag = false;
        if (u != null)
        {
            Tag uType = (u.dbSource.Get() is Hero) ? ((Tag) TAG.HERO_CLASS) : ((Tag) TAG.NORMAL_CLASS);
            UnitLvl lvl = DataBase.GetType<UnitLvl>().Find(o => ReferenceEquals(o.unitClass, uType) && (o.level == u.GetLevel()));
            if ((lvl != null) && (lvl.level != 1))
            {
                flag = true;
                component.texture = DescriptionInfoExtension.GetTexture(lvl.GetDescriptionInfo());
                RolloverSimpleTooltip orAddComponent = GameObjectUtils.GetOrAddComponent<RolloverSimpleTooltip>(component.gameObject);
                orAddComponent.sourceAsDbName = lvl.dbName;
                orAddComponent.useMouseLocation = false;
                orAddComponent.anchor.x = 0.4f;
                orAddComponent.anchor.y = 1f;
            }
            component.gameObject.SetActive(flag);
            int num = 0;
            int level = u.GetLevel();
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            int unitLevelIncrease = 0;
            if (humanWizard != null)
            {
                unitLevelIncrease = humanWizard.unitLevelIncrease;
            }
            if ((IAttributeableExtension.GetAttFinal(u, TAG.NORMAL_CLASS) > 0) && u.canGainXP)
            {
                int[] expReq = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT, false).expReq;
                num = expReq[Mathf.Clamp(level - unitLevelIncrease, 1, expReq.Length - 1)];
            }
            else
            {
                int[] expReq = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_HERO, false).expReq;
                num = expReq[Mathf.Clamp(level - unitLevelIncrease, 1, expReq.Length - 1)];
            }
            if ((u.xp > 0) && u.canGainXP)
            {
                labelExp.gameObject.SetActive(true);
                if (num < u.xp)
                {
                    labelExp.text = num.ToString() + "/" + num.ToString();
                }
                else
                {
                    labelExp.text = u.xp.ToString() + "/" + num.ToString();
                }
            }
            else
            {
                labelExp.gameObject.SetActive(false);
            }
        }
    }
}

