using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnchantmentListItem : MonoBehaviour
{
    [Tooltip("The name of the enchantment")]
    public TextMeshProUGUI labelName;

    [Tooltip("The image that represents the enchantment")]
    public RawImage image;

    [Tooltip("This will be tinted by the casting wizard's colour")]
    public Graphic frame;

    [Tooltip("If true a roll over tooltip will be displayed when hovering")]
    public bool supportTooltip = true;

    public bool hideDescription;

    public bool detailedTooltip;

    public bool supportRightClick = true;

    public bool labelOwnerColored;

    public GameObjectEnabler<EEnchantmentCategory> type = new GameObjectEnabler<EEnchantmentCategory>();

    public Button btCancelEnchantment;

    private object enchantment;

    private RolloverObject tooltip;

    private Button button;

    private void Awake()
    {
        if (this.supportTooltip)
        {
            this.tooltip = base.gameObject.GetOrAddComponent<RolloverObject>();
        }
        this.button = base.GetComponentInChildren<Button>();
        if (!this.supportRightClick)
        {
            return;
        }
        base.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
        {
            if (this.enchantment != null)
            {
                ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
                UIManager.Open<PopupEnchantmentInfo>(UIManager.Layer.Popup, componentInParent).Set(this.enchantment);
            }
        };
    }

    public virtual void Set(object o)
    {
        this.enchantment = o;
        EnchantmentInstance instance = o as EnchantmentInstance;
        Enchantment enchantment = o as Enchantment;
        if (instance != null)
        {
            enchantment = instance.source.Get();
        }
        if (enchantment == null)
        {
            return;
        }
        DescriptionInfo descriptionInfo = enchantment.GetDescriptionInfo();
        if ((bool)this.image)
        {
            this.image.texture = AssetManager.Get<Texture2D>(descriptionInfo.graphic);
        }
        if ((bool)this.tooltip)
        {
            this.tooltip.source = this.enchantment;
            this.tooltip.hideDescription = this.hideDescription;
            this.tooltip.detail = this.detailedTooltip;
        }
        if ((bool)this.frame)
        {
            switch (enchantment.enchCategory)
            {
            case EEnchantmentCategory.Negative:
                this.frame.color = new Color(1f, 0f, 8f / 85f, 1f);
                break;
            case EEnchantmentCategory.Positive:
                this.frame.color = new Color(4f / 85f, 57f / 85f, 0f, 1f);
                break;
            default:
                this.frame.color = Color.white;
                break;
            }
        }
        if ((bool)this.labelName)
        {
            this.labelName.text = descriptionInfo.GetLocalizedName();
            if (this.labelOwnerColored)
            {
                Color color = WizardColors.GetColor((instance.owner?.GetEntity() is PlayerWizard playerWizard) ? playerWizard.color : PlayerWizard.Color.None);
                this.labelName.color = color;
            }
        }
        if (this.type != null)
        {
            if (instance != null)
            {
                this.type.Set(enchantment.enchCategory);
            }
            else
            {
                this.type.Clear();
            }
        }
        if (!this.btCancelEnchantment)
        {
            return;
        }
        this.btCancelEnchantment.gameObject.SetActive(instance.owner == GameManager.GetHumanWizard() && instance.source.Get().allowDispel);
        this.btCancelEnchantment.onClick.RemoveAllListeners();
        this.btCancelEnchantment.onClick.AddListener(delegate
        {
            if (instance.manager == null)
            {
                Debug.LogError("missing manager");
            }
            else
            {
                instance.manager.owner.RemoveEnchantment(instance);
            }
            TownScreen screen = UIManager.GetScreen<TownScreen>(UIManager.Layer.Standard);
            if (screen != null)
            {
                screen.UpdateTopPanel();
            }
            CityManager screen2 = UIManager.GetScreen<CityManager>(UIManager.Layer.Standard);
            if (screen2 != null)
            {
                screen2.UpdateTownList();
            }
        });
    }
}
