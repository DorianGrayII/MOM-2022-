// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.TownListItem
using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TownListItem : ListItem, IPointerClickHandler, IEventSystemHandler
{
    [Tooltip("The city name")]
    public TextMeshProUGUI label;

    public TextMeshProUGUI labelRace;

    public RawImage iconRace;

    public EnchantmentGrid enchantmentGrid;

    public GameObject visibilityIsCapital;

    public GameObject visibilityIsSummoningCircle;

    public GameObject goUnrestWarning;

    public GameObject goStarvationWarning;

    public GameObject goOutpostNormal;

    public GameObject goOutpostPrimitive;

    public GameObject goOutpostNature;

    public GameObject goOutpostMagical;

    public GameObject goOutpostWarlike;

    public GameObject goOutpostTech;

    public GameObject goOutpostProgress;

    public GameObject goTownDetails;

    public GameObject currentProdFrame;

    public GameObject hoverEffect;

    public Toggle tgAutomanage;

    [Tooltip("This game object will be hidden if nothing is being produced")]
    public CraftingListItem craftingListItem;

    private float lastClikTime;

    private TownLocation town;

    [FormerlySerializedAs("townGold")]
    public TextMeshProUGUI labelGold;

    [FormerlySerializedAs("townFood")]
    public TextMeshProUGUI labelFood;

    [FormerlySerializedAs("townPower")]
    public TextMeshProUGUI labelPower;

    [Tooltip("cur population (+ increase per turn)")]
    [FormerlySerializedAs("townPopulation")]
    public TextMeshProUGUI labelPopulationAndIncrease;

    public TextMeshProUGUI labelPopulation;

    public TextMeshProUGUI labelMaxPopulation;

    public TextMeshProUGUI labelDefenders;

    [FormerlySerializedAs("townUnrest")]
    public TextMeshProUGUI labelUnrest;

    [FormerlySerializedAs("townProduction")]
    public TextMeshProUGUI labelProduction;

    [FormerlySerializedAs("townProduction")]
    public TextMeshProUGUI labelResearch;

    [FormerlySerializedAs("townBuildingsCount")]
    public TextMeshProUGUI labelBuildingsCount;

    public TextMeshProUGUI labelBuyCost;

    public Button btBuy;

    public void OnPointerClick(PointerEventData eventData)
    {
        CityManager screen = UIManager.GetScreen<CityManager>(UIManager.Layer.Standard);
        if (screen == null)
        {
            return;
        }
        if (FSMMapGame.Get().IsCasting())
        {
            FSMMapGame.Get().SetChosenTarget(this.town.GetPosition());
            if (CityManager.Get() != null)
            {
                CameraController.CenterAt(this.town.GetPosition());
                UIManager.Close(CityManager.Get());
            }
        }
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        if (realtimeSinceStartup - this.lastClikTime < 0.5f)
        {
            UIManager.Close(screen);
            bool flag = this.town.IsAnOutpost();
            string e = (flag ? "OutpostScreen" : "TownScreen");
            TownScreen townScreen = TownScreen.Get();
            if (townScreen != null)
            {
                if (flag)
                {
                    townScreen.Close();
                    FSMMapGame.Get()?.MapGameEvents(this, e);
                }
                else
                {
                    townScreen.SetTown(this.town);
                    townScreen.UpdateAll(ignoreTowns: true);
                    FSMSelectionManager.Get().Select(this.town, focus: true);
                }
            }
            else
            {
                FSMMapGame.Get()?.MapGameEvents(this, e);
            }
        }
        else
        {
            this.lastClikTime = realtimeSinceStartup;
        }
    }

    private void Awake()
    {
        MHEventSystem.RegisterListener<RolloverBase>(ConstructionQueueEvent, this);
    }

    private void OnDestroy()
    {
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
    }

    private void ConstructionQueueEvent(object sender, object e)
    {
        Toggle componentInChildren = base.gameObject.GetComponentInChildren<Toggle>();
        if (!(componentInChildren != null))
        {
            return;
        }
        RolloverBase rb = sender as RolloverBase;
        if (!(rb?.GetComponentInParent<CraftingListItem>() != null) || !Array.Exists(base.gameObject.GetComponentsInChildren<RolloverBase>(), (RolloverBase o) => o == rb))
        {
            return;
        }
        if (e as string == "OnPointerEnter")
        {
            componentInChildren.enabled = false;
            if (this.currentProdFrame != null)
            {
                this.currentProdFrame.SetActive(value: true);
            }
            if (this.hoverEffect != null)
            {
                this.hoverEffect.SetActive(value: false);
            }
        }
        else if (e as string == "OnPointerExit")
        {
            componentInChildren.enabled = true;
            if (this.currentProdFrame != null)
            {
                this.currentProdFrame.SetActive(value: false);
            }
            if (this.hoverEffect != null)
            {
                this.hoverEffect.SetActive(value: true);
            }
        }
    }

    public override void Set(object to, object data = null, int index = -1)
    {
        TownLocation townLocation = (this.town = to as TownLocation);
        bool flag = this.town.IsAnOutpost();
        if (this.label != null)
        {
            if (index >= 0)
            {
                this.label.text = index + 1 + ". " + townLocation.name;
            }
            else
            {
                this.label.text = townLocation.name;
            }
        }
        if (this.goTownDetails != null)
        {
            this.goTownDetails.SetActive(!flag);
        }
        if (this.goOutpostProgress != null)
        {
            this.goOutpostProgress.SetActive(flag);
        }
        if (this.labelDefenders != null)
        {
            this.labelDefenders.text = townLocation.GetUnits().Count.ToString();
        }
        if (!flag)
        {
            bool active = townLocation.GetRebels() > 0 && new FInt(townLocation.GetPopUnits()) / 2 <= townLocation.GetRebels();
            this.goUnrestWarning.SetActive(active);
            bool active2 = townLocation.CalculateFoodIncome() < townLocation.GetPopUnits();
            this.goStarvationWarning.SetActive(active2);
            if (this.labelGold != null)
            {
                this.labelGold.text = townLocation.CalculateMoneyIncome(includeUpkeep: true).ColorSInt();
            }
            if (this.labelPower != null)
            {
                this.labelPower.text = townLocation.CalculatePowerIncome().ColorSInt();
            }
            if (this.labelFood != null)
            {
                this.labelFood.text = townLocation.CalculateFoodFinalIncome().ColorSInt();
            }
            if (this.labelProduction != null)
            {
                this.labelProduction.text = townLocation.CalculateProductionIncome().ColorSInt();
            }
            if (this.labelResearch != null)
            {
                this.labelResearch.text = townLocation.CalculateResearchIncomeLimited().ToString();
            }
            if (this.labelPopulationAndIncrease != null)
            {
                this.labelPopulationAndIncrease.text = townLocation.Population + " (" + townLocation.PopulationIncreasePerTurn().ColorSInt() + ")";
            }
            if (this.labelPopulation != null)
            {
                this.labelPopulation.text = townLocation.Population.ToString();
            }
            if (this.labelMaxPopulation != null)
            {
                this.labelMaxPopulation.text = global::DBUtils.Localization.Get("UI_MAX", true) + townLocation.MaxPopulation() + "000";
            }
            if (this.labelUnrest != null)
            {
                this.labelUnrest.text = townLocation.GetUnrest().Percent() + "(" + townLocation.GetRebels() + ")";
            }
            if (this.craftingListItem != null)
            {
                this.craftingListItem.Set(townLocation);
            }
            if (this.tgAutomanage != null)
            {
                this.tgAutomanage.onValueChanged.RemoveAllListeners();
                this.tgAutomanage.isOn = this.town.autoManaged;
                this.tgAutomanage.onValueChanged.AddListener(delegate(bool b)
                {
                    this.town.autoManaged = b;
                    if (b)
                    {
                        this.town.craftingQueue.SanitizeQueue();
                        this.town.AutoManageUpdate();
                        TownScreen.Get()?.UpdateRightPanel();
                        CityManager.Get()?.UpdateTownList();
                    }
                });
            }
            CraftingItem craftingItem = this.town.craftingQueue.GetFirst();
            if (this.btBuy != null)
            {
                if (craftingItem == null)
                {
                    this.labelBuyCost.text = null;
                    this.btBuy.interactable = false;
                }
                else
                {
                    int num = Mathf.Max(craftingItem.BuyCost(), 0);
                    this.labelBuyCost.text = num.ToString();
                    PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
                    this.btBuy.interactable = craftingItem.progress < craftingItem.requirementValue && wizard.money >= craftingItem.BuyCost();
                    this.btBuy.onClick.RemoveAllListeners();
                    this.btBuy.onClick.AddListener(delegate
                    {
                        if (this.town.craftingQueue.craftingItems.Count > 0 && craftingItem.BuyCost() <= wizard.money)
                        {
                            wizard.money -= craftingItem.BuyCost();
                            craftingItem.progress = craftingItem.requirementValue;
                            TownScreen.Get()?.UpdateRightPanel();
                            CityManager.Get()?.UpdateTownList();
                        }
                    });
                }
            }
        }
        else
        {
            DBReference<Race> race = this.town.race;
            float fillAmount = this.town.OutpostProgression();
            string text = "";
            text = race.Get().visualGroup;
            if (string.IsNullOrEmpty(text))
            {
                if (race == (Race)RACE.GNOLLS || race == (Race)RACE.NOMADS || race == (Race)RACE.BARBARIANS)
                {
                    text = "Primitive";
                }
                if (race == (Race)RACE.TROLLS || race == (Race)RACE.KLACKONS || race == (Race)RACE.LIZARDMEN)
                {
                    text = "Nature";
                }
                if (race == (Race)RACE.DRACONIANS || race == (Race)RACE.HIGH_ELVES || race == (Race)RACE.DARK_ELVES)
                {
                    text = "Magical";
                }
                if (race == (Race)RACE.DWARVES || race == (Race)RACE.ORCS || race == (Race)RACE.BEASTMEN)
                {
                    text = "Warlike";
                }
                else if (race == (Race)RACE.HALFLINGS || race == (Race)RACE.HIGH_MEN || string.IsNullOrEmpty(text))
                {
                    text = "";
                }
            }
            this.goOutpostNormal.SetActive(text == "");
            this.goOutpostPrimitive.SetActive(text == "Primitive");
            this.goOutpostNature.SetActive(text == "Nature");
            this.goOutpostMagical.SetActive(text == "Magical");
            this.goOutpostWarlike.SetActive(text == "Warlike");
            this.goOutpostTech.SetActive(text == "Tech");
            GameObjectUtils.FindByNameGetComponent<Image>(this.goOutpostPrimitive, "Houses").fillAmount = fillAmount;
            GameObjectUtils.FindByNameGetComponent<Image>(this.goOutpostTech, "Houses").fillAmount = fillAmount;
            GameObjectUtils.FindByNameGetComponent<Image>(this.goOutpostNature, "Houses").fillAmount = fillAmount;
            GameObjectUtils.FindByNameGetComponent<Image>(this.goOutpostMagical, "Houses").fillAmount = fillAmount;
            GameObjectUtils.FindByNameGetComponent<Image>(this.goOutpostWarlike, "Houses").fillAmount = fillAmount;
            GameObjectUtils.FindByNameGetComponent<Image>(this.goOutpostNormal, "Houses").fillAmount = fillAmount;
        }
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        if ((bool)this.visibilityIsSummoningCircle)
        {
            this.visibilityIsSummoningCircle.SetActive(humanWizard.summoningCircle == townLocation);
        }
        if ((bool)this.visibilityIsCapital)
        {
            this.visibilityIsCapital.SetActive(humanWizard.wizardTower == townLocation);
        }
        DescriptionInfo descriptionInfo = townLocation.race.Get().GetDescriptionInfo();
        if ((bool)this.iconRace)
        {
            this.iconRace.texture = descriptionInfo.GetTexture();
        }
        if ((bool)this.labelRace)
        {
            this.labelRace.text = descriptionInfo.GetLocalizedName();
        }
        if ((bool)this.enchantmentGrid)
        {
            List<EnchantmentInstance> enchantments = townLocation.GetEnchantmentManager().enchantments;
            this.enchantmentGrid.gameObject.SetActive(enchantments.Count > 0);
            this.enchantmentGrid.SetEnchantments(enchantments);
        }
        if ((bool)this.labelBuildingsCount)
        {
            this.labelBuildingsCount.text = townLocation.buildings.Count.ToString();
        }
    }

    public TownLocation GetTown()
    {
        return this.town;
    }
}
