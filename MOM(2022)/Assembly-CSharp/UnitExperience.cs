using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UnitExperience : MonoBehaviour
{
    public void Set(BaseUnit u)
    {
        RawImage component = base.GetComponent<RawImage>();
        bool active = false;
        if (u != null)
        {
            Tag uType = ((u.dbSource.Get() is Hero) ? ((Tag)TAG.HERO_CLASS) : ((Tag)TAG.NORMAL_CLASS));
            UnitLvl unitLvl = DataBase.GetType<UnitLvl>().Find((UnitLvl o) => o.unitClass == uType && o.level == u.GetLevel());
            if (unitLvl != null && unitLvl.level != 1)
            {
                active = true;
                component.texture = unitLvl.GetDescriptionInfo().GetTexture();
                RolloverSimpleTooltip orAddComponent = component.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
                orAddComponent.sourceAsDbName = unitLvl.dbName;
                orAddComponent.useMouseLocation = false;
                orAddComponent.anchor.x = 0.4f;
                orAddComponent.anchor.y = 1f;
            }
        }
        component.gameObject.SetActive(active);
    }

    public void Set(BaseUnit u, TextMeshProUGUI labelExp)
    {
        RawImage component = base.GetComponent<RawImage>();
        bool active = false;
        if (u == null)
        {
            return;
        }
        Tag uType = ((u.dbSource.Get() is Hero) ? ((Tag)TAG.HERO_CLASS) : ((Tag)TAG.NORMAL_CLASS));
        UnitLvl unitLvl = DataBase.GetType<UnitLvl>().Find((UnitLvl o) => o.unitClass == uType && o.level == u.GetLevel());
        if (unitLvl != null && unitLvl.level != 1)
        {
            active = true;
            component.texture = unitLvl.GetDescriptionInfo().GetTexture();
            RolloverSimpleTooltip orAddComponent = component.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent.sourceAsDbName = unitLvl.dbName;
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor.x = 0.4f;
            orAddComponent.anchor.y = 1f;
        }
        component.gameObject.SetActive(active);
        int num = 0;
        int level = u.GetLevel();
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        int num2 = 0;
        if (humanWizard != null)
        {
            num2 = humanWizard.unitLevelIncrease;
        }
        if (u.GetAttFinal(TAG.NORMAL_CLASS) > 0 && u.canGainXP)
        {
            int[] expReq = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_UNIT).expReq;
            int num3 = Mathf.Clamp(level - num2, 1, expReq.Length - 1);
            num = expReq[num3];
        }
        else
        {
            int[] expReq2 = DataBase.Get<XpToLvl>(XP_TO_LVL.COST_HERO).expReq;
            int num4 = Mathf.Clamp(level - num2, 1, expReq2.Length - 1);
            num = expReq2[num4];
        }
        if (u.xp > 0 && u.canGainXP)
        {
            labelExp.gameObject.SetActive(value: true);
            if (num < u.xp)
            {
                labelExp.text = num + "/" + num;
            }
            else
            {
                labelExp.text = u.xp + "/" + num;
            }
        }
        else
        {
            labelExp.gameObject.SetActive(value: false);
        }
    }
}
