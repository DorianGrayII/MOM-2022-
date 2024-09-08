using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipBase : MonoBehaviour
{
    protected bool collapse;
    private static TooltipBase instance;
    protected static RolloverBase rollOver;
    public TextMeshProUGUI optionalMessage;
    public GameObject expandRightClick;
    public GameObject collapsible;

    public static void Close()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
        rollOver = null;
    }

    protected virtual void DoExpand()
    {
        this.collapse = false;
        if ((this.optionalMessage != null) && !string.IsNullOrEmpty(this.optionalMessage.text))
        {
            this.optionalMessage.gameObject.SetActive(true);
        }
        if (this.collapsible)
        {
            this.collapsible.gameObject.SetActive(true);
        }
        if (this.expandRightClick)
        {
            this.expandRightClick.gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
        this.Update();
    }

    public static void Expand()
    {
        if ((instance != null) && instance.collapse)
        {
            instance.DoExpand();
        }
    }

    public static void Open(object source, string message)
    {
        Close();
        TooltipBase component = null;
        GameObject layer = UIManager.GetLayer(UIManager.Layer.TopLayer);
        TooltipBase.rollOver = source as RolloverBase;
        RolloverObject obj3 = source as RolloverObject;
        if (obj3 == null)
        {
            if (source is RolloverSimpleTooltip)
            {
                component = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSimple, layer.transform).GetComponent<SimpleTooltip>();
            }
            else
            {
                RolloverUnitTooltip tooltip = source as RolloverUnitTooltip;
                if (tooltip != null)
                {
                    component = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipUnit, layer.transform).GetComponent<UnitTooltip>();
                    source = (tooltip.sourceAsUnit == null) ? ((object) tooltip.sourceFromDb) : ((object) tooltip.sourceAsUnit);
                }
            }
        }
        else
        {
            if (obj3.source != null)
            {
                source = obj3.source;
                DBRefBase base3 = source as DBRefBase;
                if (base3 != null)
                {
                    source = base3.GetObject();
                }
            }
            message = obj3.optionalMessage;
            if (obj3.overrideTooltipPrefab)
            {
                component = GameObjectUtils.Instantiate(obj3.overrideTooltipPrefab, layer.transform).GetComponent<TooltipBase>();
            }
            else if ((source is BaseUnit) || (source is Subrace))
            {
                component = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipUnit, layer.transform).GetComponent<UnitTooltip>();
            }
            else if (!(source is Enchantment) && !(source is EnchantmentInstance))
            {
                component = !(source is MOM.Artefact) ? (!(source is PlayerWizard) ? (!(source is Race) ? (!(source is StatDetails) ? ((TooltipBase) GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSimple, layer.transform).GetComponent<SimpleTooltip>()) : ((TooltipBase) GameObjectUtils.Instantiate(UIReferences.instance.goTooltipStatDetails, layer.transform).GetComponent<TooltipStatDetails>())) : ((TooltipBase) GameObjectUtils.Instantiate(UIReferences.instance.goTooltipCitizens, layer.transform).GetComponent<TooltipCitizens>())) : ((TooltipBase) GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSpellBlast, layer.transform).GetComponent<SpellBlastTooltip>())) : ((TooltipBase) GameObjectUtils.Instantiate(UIReferences.instance.goTooltipArtefact, layer.transform).GetComponent<TooltipArtefact>());
            }
            else
            {
                component = GameObjectUtils.Instantiate(obj3.detail ? UIReferences.instance.goTooltipEnchantmentDetail : UIReferences.instance.goTooltipEnchantment, layer.transform).GetComponent<EnchantmentTooltip>();
                ((EnchantmentTooltip) component).HideDescription(obj3.hideDescription);
            }
        }
        string instanceName = source as string;
        if (instanceName != null)
        {
            DBClass class2 = DataBase.Get(instanceName, false);
            if (class2 is Subrace)
            {
                component = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipUnit, layer.transform).GetComponent<UnitTooltip>();
                source = class2;
            }
        }
        if (component != null)
        {
            bool titleOnlyUntilRightClick;
            instance = component;
            if (TooltipBase.rollOver != null)
            {
                titleOnlyUntilRightClick = TooltipBase.rollOver.titleOnlyUntilRightClick;
            }
            else
            {
                RolloverBase rollOver = TooltipBase.rollOver;
                titleOnlyUntilRightClick = false;
            }
            component.collapse = titleOnlyUntilRightClick;
            if (component.expandRightClick)
            {
                component.expandRightClick.gameObject.SetActive(component.collapse);
            }
            if (component.optionalMessage != null)
            {
                component.optionalMessage.text = DBUtils.Localization.Get(message, true, Array.Empty<object>());
                component.optionalMessage.gameObject.SetActive(!string.IsNullOrEmpty(message) && !component.collapse);
            }
            if (component.collapsible)
            {
                component.collapsible.gameObject.SetActive(!component.collapse);
            }
            component.Populate(source);
            component.Update();
        }
    }

    public static void OpenSurveyor()
    {
        Close();
        GameObject layer = UIManager.GetLayer(UIManager.Layer.TopLayer);
        instance = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSurveyor, layer.transform).GetComponent<TooltipSurveyor>();
        instance.Refresh();
        instance.Update();
    }

    public virtual void Populate(object o)
    {
    }

    public virtual void Refresh()
    {
    }

    public static void RefreshInstance()
    {
        if (instance != null)
        {
            instance.Refresh();
        }
    }

    private unsafe void Update()
    {
        if (instance != this)
        {
            Destroy(base.gameObject);
        }
        else
        {
            RectTransform transform = base.transform as RectTransform;
            if (transform != null)
            {
                Rect rect = transform.rect;
                Vector2 vector = new Vector2(((float) Screen.width) / transform.lossyScale.x, ((float) Screen.height) / transform.lossyScale.y);
                Vector2 vector2 = new Vector2(Input.mousePosition.x / transform.lossyScale.x, Input.mousePosition.y / transform.lossyScale.y);
                float* singlePtr1 = &vector2.x;
                singlePtr1[0] -= vector.x / 2f;
                float* singlePtr2 = &vector2.y;
                singlePtr2[0] -= vector.y / 2f;
                if (rollOver && !rollOver.useMouseLocation)
                {
                    RectTransform transform2 = (!rollOver.optionalPosition || !rollOver.optionalPosition.activeInHierarchy) ? (rollOver.transform as RectTransform) : (rollOver.optionalPosition.transform as RectTransform);
                    Vector3[] fourCornersArray = new Vector3[4];
                    transform2.GetWorldCorners(fourCornersArray);
                    vector2 = new Vector2(((fourCornersArray[0].x + ((fourCornersArray[2].x - fourCornersArray[0].x) * rollOver.position.x)) / transform.lossyScale.x) - (vector.x / 2f), ((fourCornersArray[0].y + ((fourCornersArray[2].y - fourCornersArray[0].y) * rollOver.position.y)) / transform.lossyScale.y) - (vector.y / 2f));
                    float* singlePtr3 = &vector2.x;
                    singlePtr3[0] -= rect.width * rollOver.anchor.x;
                    float* singlePtr4 = &vector2.y;
                    singlePtr4[0] += rect.height * (1f - rollOver.anchor.y);
                    vector2 += rollOver.offset;
                }
                if (vector2.x > ((vector.x / 2f) - rect.width))
                {
                    vector2.x = (vector.x / 2f) - rect.width;
                }
                if (vector2.x < (-vector.x / 2f))
                {
                    vector2.x = -vector.x / 2f;
                }
                if (vector2.y < (rect.height - (vector.y / 2f)))
                {
                    vector2.y = rect.height - (vector.y / 2f);
                }
                if (vector2.y > (vector.y / 2f))
                {
                    vector2.y = vector.y / 2f;
                }
                base.transform.localPosition = (Vector3) vector2;
            }
        }
    }
}

