using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using System;
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
            this.tooltip = GameObjectUtils.GetOrAddComponent<RolloverObject>(base.gameObject);
        }
        this.button = base.GetComponentInChildren<Button>();
        if (this.supportRightClick)
        {
            GameObjectUtils.GetOrAddComponent<MouseClickEvent>(base.gameObject).mouseRightClick = delegate (object d) {
                if (this.enchantment != null)
                {
                    UIManager.Open<PopupEnchantmentInfo>(UIManager.Layer.Popup, base.GetComponentInParent<ScreenBase>()).Set(this.enchantment);
                }
            };
        }
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
        if (enchantment != null)
        {
            DescriptionInfo descriptionInfo = enchantment.GetDescriptionInfo();
            if (this.image)
            {
                this.image.texture = AssetManager.Get<Texture2D>(descriptionInfo.graphic, true);
            }
            if (this.tooltip)
            {
                this.tooltip.source = this.enchantment;
                this.tooltip.hideDescription = this.hideDescription;
                this.tooltip.detail = this.detailedTooltip;
            }
            if (this.frame)
            {
                EEnchantmentCategory enchCategory = enchantment.enchCategory;
                this.frame.color = (enchCategory == EEnchantmentCategory.Positive) ? new UnityEngine.Color(0.04705882f, 0.6705883f, 0f, 1f) : ((enchCategory != EEnchantmentCategory.Negative) ? UnityEngine.Color.white : new UnityEngine.Color(1f, 0f, 0.09411765f, 1f));
            }
            if (this.labelName)
            {
                this.labelName.text = descriptionInfo.GetLocalizedName();
                if (this.labelOwnerColored)
                {
                    Entity entity;
                    if (instance.owner != null)
                    {
                        entity = instance.owner.GetEntity();
                    }
                    else
                    {
                        Reference owner = instance.owner;
                        entity = null;
                    }
                    PlayerWizard wizard = entity as PlayerWizard;
                    UnityEngine.Color color = WizardColors.GetColor((wizard != null) ? wizard.color : PlayerWizard.Color.None);
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
            if (this.btCancelEnchantment)
            {
                this.btCancelEnchantment.gameObject.SetActive((instance.owner == GameManager.GetHumanWizard()) && instance.source.Get().allowDispel);
                this.btCancelEnchantment.onClick.RemoveAllListeners();
                this.btCancelEnchantment.onClick.AddListener(delegate {
                    if (instance.manager == null)
                    {
                        Debug.LogError("missing manager");
                    }
                    else
                    {
                        IEnchantableExtension.RemoveEnchantment(instance.manager.owner, instance);
                    }
                    TownScreen screen = UIManager.GetScreen<TownScreen>(UIManager.Layer.Standard);
                    if (screen != null)
                    {
                        screen.UpdateTopPanel(true);
                    }
                    CityManager manager = UIManager.GetScreen<CityManager>(UIManager.Layer.Standard);
                    if (manager != null)
                    {
                        manager.UpdateTownList();
                    }
                });
            }
        }
    }
}

