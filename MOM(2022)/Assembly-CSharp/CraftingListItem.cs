// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// CraftingListItem
using DBDef;
using MHUtils;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingListItem : MonoBehaviour
{
    public TextMeshProUGUI time;

    public new TextMeshProUGUI name;

    public RawImage icon;

    public Image progressFill;

    public Toggle tgRepeat;

    public Slider sliderProgress;

    [Tooltip("Label that shows the production progress x/y on top of the slider")]
    public TextMeshProUGUI production;

    [Tooltip("These game objects will be set active / inactive if craftingItem != null")]
    public GameObject[] visibility;

    private TownLocation town;

    public GameObject currentItem;

    private void Awake()
    {
        if ((bool)this.sliderProgress)
        {
            this.sliderProgress.minValue = 0f;
            this.sliderProgress.maxValue = 1f;
            this.sliderProgress.wholeNumbers = false;
        }
    }

    public void Set(TownLocation town)
    {
        this.town = town;
        CraftingItem first = town.craftingQueue.GetFirst();
        this.Set(first, town.CalculateProductionIncome());
    }

    public void Set(CraftingItem craftingItem, int craftingIncome = -1)
    {
        bool active = craftingItem != null;
        GameObject[] array = this.visibility;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(active);
        }
        if (craftingItem == null)
        {
            this.name.text = null;
            this.production.text = null;
            this.time.text = null;
            this.icon.texture = null;
            this.sliderProgress.value = 0f;
            this.progressFill.fillAmount = 0f;
            return;
        }
        int requirementValue = craftingItem.requirementValue;
        if ((bool)this.time)
        {
            if (craftingIncome <= 0)
            {
                this.time.text = "--";
            }
            else
            {
                float num = (float)(requirementValue - craftingItem.progress) / (float)craftingIncome;
                if (num <= 0f)
                {
                    this.time.text = "--";
                }
                else
                {
                    this.time.text = Mathf.CeilToInt(num).ToString();
                }
            }
        }
        if ((bool)this.name)
        {
            this.name.text = craftingItem.GetDI().GetLocalizedName();
        }
        if ((bool)this.production)
        {
            string text = ((requirementValue > 0) ? (craftingItem.progress + "/" + requirementValue) : "--");
            this.production.text = text;
        }
        if (this.tgRepeat != null)
        {
            bool flag = craftingItem.craftedUnit != null;
            this.tgRepeat.gameObject.SetActive(flag);
            if (flag)
            {
                this.tgRepeat.onValueChanged.RemoveAllListeners();
                this.tgRepeat.isOn = (this.town?.craftingQueue?.repeatUnit).GetValueOrDefault();
                this.tgRepeat.onValueChanged.AddListener(delegate(bool b)
                {
                    if (this.town?.craftingQueue != null)
                    {
                        this.town.craftingQueue.repeatUnit = b;
                        if (b)
                        {
                            this.town.autoManaged = false;
                            MHEventSystem.TriggerEvent<CraftingQueue>(this, null);
                        }
                    }
                });
            }
        }
        if ((bool)this.sliderProgress)
        {
            this.sliderProgress.value = craftingItem.Progress();
        }
        if (this.progressFill != null)
        {
            if (requirementValue > 0)
            {
                this.progressFill.fillAmount = 1f - craftingItem.Progress();
            }
            else
            {
                this.progressFill.fillAmount = 0f;
            }
        }
        if (!this.icon)
        {
            return;
        }
        this.icon.texture = craftingItem.GetDI().GetTexture();
        if (craftingItem.craftedBuilding != null)
        {
            RolloverUnitTooltip component = this.icon.GetComponent<RolloverUnitTooltip>();
            if (component != null)
            {
                Object.Destroy(component);
            }
            this.icon.gameObject.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = craftingItem.craftedBuilding.dbName;
        }
        else if (craftingItem.craftedUnit != null)
        {
            RolloverSimpleTooltip component2 = this.icon.GetComponent<RolloverSimpleTooltip>();
            if (component2 != null)
            {
                Object.Destroy(component2);
            }
            this.icon.gameObject.GetOrAddComponent<RolloverUnitTooltip>().sourceFromDb = (global::DBDef.Unit)craftingItem.craftedUnit;
        }
    }
}
