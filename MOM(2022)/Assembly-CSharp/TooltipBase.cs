using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
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

    public static void Open(object source, string message = null)
    {
        TooltipBase.Close();
        TooltipBase tooltipBase = null;
        GameObject layer = UIManager.GetLayer(UIManager.Layer.TopLayer);
        TooltipBase.rollOver = source as RolloverBase;
        if (source is RolloverObject rolloverObject)
        {
            if (rolloverObject.source != null)
            {
                source = rolloverObject.source;
                if (source is DBRefBase dBRefBase)
                {
                    source = dBRefBase.GetObject();
                }
            }
            message = rolloverObject.optionalMessage;
            if ((bool)rolloverObject.overrideTooltipPrefab)
            {
                tooltipBase = GameObjectUtils.Instantiate(rolloverObject.overrideTooltipPrefab, layer.transform).GetComponent<TooltipBase>();
            }
            else if (source is BaseUnit || source is Subrace)
            {
                tooltipBase = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipUnit, layer.transform).GetComponent<UnitTooltip>();
            }
            else if (source is Enchantment || source is EnchantmentInstance)
            {
                tooltipBase = GameObjectUtils.Instantiate(rolloverObject.detail ? UIReferences.instance.goTooltipEnchantmentDetail : UIReferences.instance.goTooltipEnchantment, layer.transform).GetComponent<EnchantmentTooltip>();
                ((EnchantmentTooltip)tooltipBase).HideDescription(rolloverObject.hideDescription);
            }
            else
            {
                tooltipBase = ((source is global::MOM.Artefact) ? GameObjectUtils.Instantiate(UIReferences.instance.goTooltipArtefact, layer.transform).GetComponent<TooltipArtefact>() : ((source is PlayerWizard) ? GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSpellBlast, layer.transform).GetComponent<SpellBlastTooltip>() : ((source is Race) ? GameObjectUtils.Instantiate(UIReferences.instance.goTooltipCitizens, layer.transform).GetComponent<TooltipCitizens>() : ((!(source is StatDetails)) ? ((TooltipBase)GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSimple, layer.transform).GetComponent<SimpleTooltip>()) : ((TooltipBase)GameObjectUtils.Instantiate(UIReferences.instance.goTooltipStatDetails, layer.transform).GetComponent<TooltipStatDetails>())))));
            }
        }
        else if (source is RolloverSimpleTooltip)
        {
            tooltipBase = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSimple, layer.transform).GetComponent<SimpleTooltip>();
        }
        else if (source is RolloverUnitTooltip rolloverUnitTooltip)
        {
            tooltipBase = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipUnit, layer.transform).GetComponent<UnitTooltip>();
            source = ((rolloverUnitTooltip.sourceAsUnit == null) ? ((object)rolloverUnitTooltip.sourceFromDb) : ((object)rolloverUnitTooltip.sourceAsUnit));
        }
        if (source is string instanceName)
        {
            DBClass dBClass = DataBase.Get(instanceName, reportMissing: false);
            if (dBClass is Subrace)
            {
                tooltipBase = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipUnit, layer.transform).GetComponent<UnitTooltip>();
                source = dBClass;
            }
        }
        if (tooltipBase != null)
        {
            TooltipBase.instance = tooltipBase;
            tooltipBase.collapse = TooltipBase.rollOver?.titleOnlyUntilRightClick ?? false;
            if ((bool)tooltipBase.expandRightClick)
            {
                tooltipBase.expandRightClick.gameObject.SetActive(tooltipBase.collapse);
            }
            if (tooltipBase.optionalMessage != null)
            {
                tooltipBase.optionalMessage.text = global::DBUtils.Localization.Get(message, true);
                tooltipBase.optionalMessage.gameObject.SetActive(!string.IsNullOrEmpty(message) && !tooltipBase.collapse);
            }
            if ((bool)tooltipBase.collapsible)
            {
                tooltipBase.collapsible.gameObject.SetActive(!tooltipBase.collapse);
            }
            tooltipBase.Populate(source);
            tooltipBase.Update();
        }
    }

    public virtual void Populate(object o)
    {
    }

    public static void Expand()
    {
        if (TooltipBase.instance != null && TooltipBase.instance.collapse)
        {
            TooltipBase.instance.DoExpand();
        }
    }

    protected virtual void DoExpand()
    {
        this.collapse = false;
        if (this.optionalMessage != null && !string.IsNullOrEmpty(this.optionalMessage.text))
        {
            this.optionalMessage.gameObject.SetActive(value: true);
        }
        if ((bool)this.collapsible)
        {
            this.collapsible.gameObject.SetActive(value: true);
        }
        if ((bool)this.expandRightClick)
        {
            this.expandRightClick.gameObject.SetActive(value: false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
        this.Update();
    }

    public static void Close()
    {
        if (TooltipBase.instance != null)
        {
            Object.Destroy(TooltipBase.instance.gameObject);
            TooltipBase.instance = null;
        }
        TooltipBase.rollOver = null;
    }

    private void Update()
    {
        if (TooltipBase.instance != this)
        {
            Object.Destroy(base.gameObject);
            return;
        }
        RectTransform rectTransform = base.transform as RectTransform;
        if (!(rectTransform == null))
        {
            Rect rect = rectTransform.rect;
            Vector2 vector = new Vector2((float)Screen.width / rectTransform.lossyScale.x, (float)Screen.height / rectTransform.lossyScale.y);
            Vector2 vector2 = new Vector2(Input.mousePosition.x / rectTransform.lossyScale.x, Input.mousePosition.y / rectTransform.lossyScale.y);
            vector2.x -= vector.x / 2f;
            vector2.y -= vector.y / 2f;
            if ((bool)TooltipBase.rollOver && !TooltipBase.rollOver.useMouseLocation)
            {
                RectTransform rectTransform2 = ((!TooltipBase.rollOver.optionalPosition || !TooltipBase.rollOver.optionalPosition.activeInHierarchy) ? (TooltipBase.rollOver.transform as RectTransform) : (TooltipBase.rollOver.optionalPosition.transform as RectTransform));
                Vector3[] array = new Vector3[4];
                rectTransform2.GetWorldCorners(array);
                vector2 = new Vector2((array[0].x + (array[2].x - array[0].x) * TooltipBase.rollOver.position.x) / rectTransform.lossyScale.x - vector.x / 2f, (array[0].y + (array[2].y - array[0].y) * TooltipBase.rollOver.position.y) / rectTransform.lossyScale.y - vector.y / 2f);
                vector2.x -= rect.width * TooltipBase.rollOver.anchor.x;
                vector2.y += rect.height * (1f - TooltipBase.rollOver.anchor.y);
                vector2 += TooltipBase.rollOver.offset;
            }
            if (vector2.x > vector.x / 2f - rect.width)
            {
                vector2.x = vector.x / 2f - rect.width;
            }
            if (vector2.x < (0f - vector.x) / 2f)
            {
                vector2.x = (0f - vector.x) / 2f;
            }
            if (vector2.y < rect.height - vector.y / 2f)
            {
                vector2.y = rect.height - vector.y / 2f;
            }
            if (vector2.y > vector.y / 2f)
            {
                vector2.y = vector.y / 2f;
            }
            base.transform.localPosition = vector2;
        }
    }

    public static void RefreshInstance()
    {
        if (TooltipBase.instance != null)
        {
            TooltipBase.instance.Refresh();
        }
    }

    public static void OpenSurveyor()
    {
        TooltipBase.Close();
        GameObject layer = UIManager.GetLayer(UIManager.Layer.TopLayer);
        TooltipBase.instance = GameObjectUtils.Instantiate(UIReferences.instance.goTooltipSurveyor, layer.transform).GetComponent<TooltipSurveyor>();
        TooltipBase.instance.Refresh();
        TooltipBase.instance.Update();
    }

    public virtual void Refresh()
    {
    }
}
