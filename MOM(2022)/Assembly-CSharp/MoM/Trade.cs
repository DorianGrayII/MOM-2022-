// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Trade
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Trade : ScreenBase
{
    public Button btCancel;

    public Button btConfirm;

    public Button btConfirmTransfer;

    public Button btPlusOne;

    public Button btMinusOne;

    public Button btMatchOffer;

    public GridItemManager gridPlayerInventory;

    public GridItemManager gridPlayerSelection;

    public GridItemManager gridNPCInventory;

    public GridItemManager gridNPCSelection;

    public TextMeshProUGUI tfValuePlayer;

    public TextMeshProUGUI tfValueNPC;

    public TextMeshProUGUI tfRequirement;

    public TextMeshProUGUI tfPopupSliderMax;

    public TextMeshProUGUI tfPopupValue;

    public Toggle tgFilterAll;

    public Toggle tgFilterItems;

    public Toggle tgFilterUnits;

    public Toggle tgFilterResources;

    public Toggle tgFilterSpells;

    public GameObject goRequirementMet;

    public GameObject goTransfer;

    public GameObject goWizard1;

    public GameObject goWizard2;

    public GameObject goPlayerInventory;

    public GameObject goPlayerSelection;

    public GameObject goNPCInventory;

    public GameObject goNPCSelection;

    public Slider sliderTrasfer;

    public Slider sliderZero;

    public Slider tradeAffection;

    public TMP_InputField inputAmount;

    public RawImage riIcon;

    public RawImage riWizard1;

    public RawImage riWizard2;

    public RolloverSimpleTooltip wstWizard1;

    public RolloverSimpleTooltip wstWizard2;

    public TextMeshProUGUI tfValue;

    public Color32 colorPositive;

    public Color32 colorNegative;

    public Color32 colorNeutral;

    private int playerID;

    private int traderID;

    private List<object> playerWares = new List<object>();

    private List<object> traderWares = new List<object>();

    private List<object> playerOffer = new List<object>();

    private List<object> traderOffer = new List<object>();

    private int playerParameter;

    private int traderParameter;

    private int leftToRightTransferValue;

    private NodeTrade.TradeCurrency currency;

    private bool dataSet;

    private GridItemManager rolloverItem;

    private int playerOfferValue;

    private int traderOfferValue;

    private int totalValueTrader;

    private bool willAcceptOffer;

    private Dictionary<object, int> valueFinalDictionary = new Dictionary<object, int>();

    private bool fullTradeMode;

    private object wantedItem;

    public override IEnumerator PreStart()
    {
        this.goTransfer.SetActive(value: false);
        this.goRequirementMet.SetActive(value: false);
        this.btConfirm.interactable = false;
        this.gridPlayerInventory.CustomDynamicItem(Item, UpdateScreen);
        this.gridNPCInventory.CustomDynamicItem(Item, UpdateScreen);
        this.gridPlayerSelection.CustomDynamicItem(Item, UpdateScreen);
        this.gridNPCSelection.CustomDynamicItem(Item, UpdateScreen);
        this.goPlayerInventory.GetComponent<RollOverOutEvents>().data = this.gridPlayerInventory;
        this.goPlayerSelection.GetComponent<RollOverOutEvents>().data = this.gridPlayerSelection;
        this.goNPCInventory.GetComponent<RollOverOutEvents>().data = this.gridNPCInventory;
        this.goNPCSelection.GetComponent<RollOverOutEvents>().data = this.gridNPCSelection;
        MHEventSystem.RegisterListener<DragAndDropItem>(ItemDrop, this);
        MHEventSystem.RegisterListener<RollOverOutEvents>(EnterExitEvents, this);
        if (this.dataSet)
        {
            this.UpdateScreen();
        }
        yield return base.PreStart();
    }

    public override IEnumerator PreClose()
    {
        yield return base.PreClose();
        MHEventSystem.TriggerEvent<Trade>(this, "Closing");
    }

    private void Item(GameObject itemSource, object source, object data, int index)
    {
        TradeListItem component = itemSource.GetComponent<TradeListItem>();
        DragAndDrop componentInChildren = component.icon.gameObject.GetComponentInChildren<DragAndDrop>();
        RolloverObject orAddComponent = component.icon.gameObject.GetOrAddComponent<RolloverObject>();
        if (!(component != null))
        {
            return;
        }
        component.quantity.SetActive(value: false);
        component.value.SetActive(this.fullTradeMode);
        orAddComponent.source = source;
        if (source is global::MOM.Unit)
        {
            global::MOM.Unit unit = source as global::MOM.Unit;
            component.icon.texture = AssetManager.Get<Texture2D>(unit.GetDescriptionInfo().graphic);
            component.labelQuantity.text = "";
            if (componentInChildren != null)
            {
                componentInChildren.item = unit;
                componentInChildren.owner = data;
            }
        }
        else if (source is global::MOM.Artefact)
        {
            global::MOM.Artefact artefact = source as global::MOM.Artefact;
            component.icon.texture = AssetManager.Get<Texture2D>(artefact.graphic);
            component.labelQuantity.text = "";
            if (componentInChildren != null)
            {
                componentInChildren.item = artefact;
                componentInChildren.owner = data;
            }
        }
        else if (source is DBReference<Spell>)
        {
            DBReference<Spell> dBReference = source as DBReference<Spell>;
            component.icon.texture = dBReference.Get().GetDescriptionInfo().GetTexture();
            component.labelQuantity.text = "";
            if (componentInChildren != null)
            {
                componentInChildren.item = dBReference;
                componentInChildren.owner = data;
            }
        }
        else if (source is Multitype<NodeTrade.TradeCurrency, string, int> multitype)
        {
            component.quantity.SetActive(value: true);
            Texture2D texture2D = AssetManager.Get<Texture2D>(multitype.t1);
            component.icon.texture = texture2D;
            component.labelQuantity.text = multitype.t2.ToString();
            orAddComponent.source = null;
            switch (multitype.t0)
            {
            case NodeTrade.TradeCurrency.Gold:
                orAddComponent.overrideTitle = "UI_GOLD_INCOME";
                orAddComponent.overrideDescription = "UI_GOLD_TRADE_DES";
                orAddComponent.overrideTexture = texture2D;
                break;
            case NodeTrade.TradeCurrency.Mana:
                orAddComponent.overrideTitle = "UI_MANA_INCOME";
                orAddComponent.overrideDescription = "UI_MANA_TRADE_DES";
                orAddComponent.overrideTexture = texture2D;
                break;
            case NodeTrade.TradeCurrency.Both:
                orAddComponent.overrideTitle = "UI_BOTH_INCOME";
                orAddComponent.overrideDescription = "UI_BOTH_TRADE_DES";
                orAddComponent.overrideTexture = texture2D;
                break;
            default:
                orAddComponent.enabled = false;
                break;
            }
            if (componentInChildren != null)
            {
                componentInChildren.item = multitype;
                componentInChildren.owner = data;
            }
        }
        if (this.fullTradeMode)
        {
            GridItemManager gridItemManager = data as GridItemManager;
            if (gridItemManager == this.gridPlayerInventory || gridItemManager == this.gridPlayerSelection)
            {
                int num = GameManager.GetWizard(this.traderID).AdvantageIfAcquired(source, this.IsWantedItem(source));
                int valueCached = this.GetValueCached(source, this.traderID);
                component.labelValue.text = valueCached.ToString();
                component.aiInterested.SetActive(num > 0);
                component.aiNotInterested.SetActive(num < 0);
            }
            else
            {
                int valueCached2 = this.GetValueCached(source, this.playerID);
                component.labelValue.text = valueCached2.ToString();
                component.aiInterested.SetActive(value: false);
                component.aiNotInterested.SetActive(value: false);
            }
        }
        else
        {
            component.aiInterested.SetActive(value: false);
            component.aiNotInterested.SetActive(value: false);
        }
    }

    private void CheckRequirementsFullMode(int recipient = -1, object offer = null, int newValue = 0)
    {
        this.playerOfferValue = 0;
        this.traderOfferValue = 0;
        foreach (object item in this.playerOffer)
        {
            if (recipient == -1 || item != offer)
            {
                this.playerOfferValue += this.GetValueCached(item, this.traderID);
            }
        }
        if (recipient == -1 || recipient != PlayerWizard.HumanID())
        {
            this.playerOfferValue += newValue;
        }
        foreach (object item2 in this.traderOffer)
        {
            if (item2 != offer)
            {
                this.traderOfferValue += this.GetValueCached(item2, this.playerID);
            }
        }
        if (recipient == PlayerWizard.HumanID())
        {
            this.traderOfferValue += newValue;
        }
        this.traderOfferValue += this.GetTraderBribeForWillValue();
        this.RelationshipChange();
        if (this.willAcceptOffer)
        {
            this.goRequirementMet.SetActive(value: true);
            this.tfRequirement.text = global::DBUtils.Localization.Get("DES_TRADER_WILL_AGREE", true);
            this.btConfirm.interactable = true;
        }
        else
        {
            this.goRequirementMet.SetActive(value: false);
            this.tfRequirement.text = global::DBUtils.Localization.Get("DES_TRADER_WILL_NOT_AGREE", true);
            this.btConfirm.interactable = false;
        }
        this.tfValuePlayer.text = this.playerOfferValue.ToString();
        this.tfValueNPC.text = this.traderOfferValue.ToString();
    }

    private void CheckRequirements()
    {
        int num = 0;
        int num2 = 0;
        if (this.currency > NodeTrade.TradeCurrency.None)
        {
            foreach (object item in this.playerOffer)
            {
                Multitype<NodeTrade.TradeCurrency, string, int> multitype = item as Multitype<NodeTrade.TradeCurrency, string, int>;
                num += multitype.t2;
            }
            foreach (object item2 in this.traderOffer)
            {
                Multitype<NodeTrade.TradeCurrency, string, int> multitype2 = item2 as Multitype<NodeTrade.TradeCurrency, string, int>;
                num2 += multitype2.t2;
            }
            num2 += this.GetTraderBribeForWillValue();
        }
        else
        {
            num = this.playerOffer.Count;
            num2 = this.traderOffer.Count + this.GetTraderBribeForWillValue();
        }
        this.willAcceptOffer = num >= num2;
        if (this.willAcceptOffer)
        {
            this.goRequirementMet.SetActive(value: true);
            this.btConfirm.interactable = true;
        }
        else
        {
            this.goRequirementMet.SetActive(value: false);
            this.btConfirm.interactable = false;
        }
        this.tfValuePlayer.text = num.ToString();
        this.tfValueNPC.text = num2.ToString();
    }

    public static int GetValueFor(object o, int recipient, bool wantedItem)
    {
        PlayerWizard wizard = GameManager.GetWizard(recipient);
        int value = Trade.GetValue(o);
        if (recipient > PlayerWizard.HumanID())
        {
            int advantageIfAcquired = wizard.AdvantageIfAcquired(o, wantedItem);
            float num = wizard.ValueScale(o, advantageIfAcquired);
            return (int)((float)value * num);
        }
        return value;
    }

    private int GetValueCached(object o, int recipient, int currencyCount = -1)
    {
        PlayerWizard wizard = GameManager.GetWizard(recipient);
        if (o is Multitype<NodeTrade.TradeCurrency, string, int>)
        {
            int num = Trade.GetValue(o);
            if (currencyCount > -1)
            {
                num = currencyCount;
            }
            if (recipient > PlayerWizard.HumanID())
            {
                int advantageIfAcquired = wizard.AdvantageIfAcquired(o, this.IsWantedItem(o));
                float num2 = wizard.ValueScale(o, advantageIfAcquired);
                return (int)((float)num * num2);
            }
            return num;
        }
        if (this.valueFinalDictionary == null)
        {
            this.valueFinalDictionary = new Dictionary<object, int>();
        }
        if (!this.valueFinalDictionary.ContainsKey(o))
        {
            int num3 = Trade.GetValue(o);
            if (recipient > PlayerWizard.HumanID())
            {
                int advantageIfAcquired2 = wizard.AdvantageIfAcquired(o, this.IsWantedItem(o));
                float num4 = wizard.ValueScale(o, advantageIfAcquired2);
                num3 = (int)((float)num3 * num4);
            }
            this.valueFinalDictionary[o] = num3;
        }
        return this.valueFinalDictionary[o];
    }

    private static int GetValue(object o)
    {
        int result = 100;
        if (o is Multitype<NodeTrade.TradeCurrency, string, int>)
        {
            result = (o as Multitype<NodeTrade.TradeCurrency, string, int>).t2;
        }
        else if (o is DBReference<Spell>)
        {
            result = (o as DBReference<Spell>).Get().researchCost * 5;
        }
        else if (o is global::MOM.Artefact)
        {
            result = (o as global::MOM.Artefact).GetValue();
        }
        return result;
    }

    private void UpdateScreen()
    {
        this.goWizard1.SetActive(value: true);
        if (this.playerID > 0)
        {
            PlayerWizard wizard = GameManager.GetWizard(this.playerID);
            this.riWizard1.texture = wizard.Graphic;
            this.wstWizard1.title = wizard.GetName();
        }
        this.goWizard2.SetActive(this.traderID > 0);
        if (this.traderID > 0)
        {
            PlayerWizard wizard2 = GameManager.GetWizard(this.traderID);
            this.riWizard2.texture = wizard2.Graphic;
            this.wstWizard2.title = wizard2.GetName();
        }
        if (this.tgFilterAll.isOn)
        {
            this.gridPlayerInventory.UpdateGrid(this.playerWares, this.gridPlayerInventory);
            this.gridNPCInventory.UpdateGrid(this.traderWares, this.gridNPCInventory);
        }
        else if (this.tgFilterItems.isOn)
        {
            List<object> items = this.playerWares.FindAll((object o) => o is global::MOM.Artefact);
            this.gridPlayerInventory.UpdateGrid(items, this.gridPlayerInventory);
            items = this.traderWares.FindAll((object o) => o is global::MOM.Artefact);
            this.gridNPCInventory.UpdateGrid(items, this.gridNPCInventory);
        }
        else if (this.tgFilterUnits.isOn)
        {
            List<object> items2 = this.playerWares.FindAll((object o) => o is global::MOM.Unit);
            this.gridPlayerInventory.UpdateGrid(items2, this.gridPlayerInventory);
            items2 = this.traderWares.FindAll((object o) => o is global::MOM.Unit);
            this.gridNPCInventory.UpdateGrid(items2, this.gridNPCInventory);
        }
        else if (this.tgFilterResources.isOn)
        {
            List<object> items3 = this.playerWares.FindAll((object o) => o is Multitype<NodeTrade.TradeCurrency, string, int>);
            this.gridPlayerInventory.UpdateGrid(items3, this.gridPlayerInventory);
            items3 = this.traderWares.FindAll((object o) => o is Multitype<NodeTrade.TradeCurrency, string, int>);
            this.gridNPCInventory.UpdateGrid(items3, this.gridNPCInventory);
        }
        else if (this.tgFilterSpells.isOn)
        {
            List<object> items4 = this.playerWares.FindAll((object o) => o is DBReference<Spell>);
            this.gridPlayerInventory.UpdateGrid(items4, this.gridPlayerInventory);
            items4 = this.traderWares.FindAll((object o) => o is DBReference<Spell>);
            this.gridNPCInventory.UpdateGrid(items4, this.gridNPCInventory);
        }
        this.gridPlayerSelection.UpdateGrid(this.playerOffer, this.gridPlayerSelection);
        this.gridNPCSelection.UpdateGrid(this.traderOffer, this.gridNPCSelection);
        if (this.fullTradeMode)
        {
            this.CheckRequirementsFullMode();
        }
        else
        {
            this.CheckRequirements();
        }
        PlayerWizard wizard3 = GameManager.GetWizard(this.playerID);
        PlayerWizard wizard4 = GameManager.GetWizard(this.traderID);
        if (wizard3 == null || wizard4 == null)
        {
            this.tradeAffection.gameObject.SetActive(value: false);
        }
    }

    public void SetData(PlayerWizard player, PlayerWizard trader, object wantedItem)
    {
        List<object> possibleTradeWares = player.GetPossibleTradeWares();
        List<object> possibleTradeWares2 = trader.GetPossibleTradeWares();
        this.wantedItem = wantedItem;
        this.fullTradeMode = true;
        DiplomaticStatus statusToward = trader.GetDiplomacy().GetStatusToward(player);
        int num = Mathf.Max(0, -statusToward.willToTrade);
        this.SetData(player.GetID(), trader.GetID(), NodeTrade.TradeCurrency.Both, possibleTradeWares, possibleTradeWares2, 0, num);
    }

    public void SetData(int playerID, int traderID, NodeTrade.TradeCurrency currency, List<object> playerWares = null, List<object> traderWares = null, int playerParameter = 0, int traderParameter = 0)
    {
        this.dataSet = true;
        this.playerID = playerID;
        this.traderID = traderID;
        this.currency = currency;
        this.playerWares = playerWares;
        this.traderWares = traderWares;
        this.playerParameter = playerParameter;
        this.traderParameter = Mathf.Abs(traderParameter);
        if (this.fullTradeMode)
        {
            if (playerWares != null)
            {
                PlayerWizard w2 = GameManager.GetWizard(traderID);
                this.playerWares = playerWares.FindAll((object o) => w2.AdvantageIfAcquired(o, increasedWill: false) > -2);
                if (this.playerWares.Contains(this.wantedItem))
                {
                    this.playerOffer.Add(this.wantedItem);
                    this.playerWares.Remove(this.wantedItem);
                }
            }
            if (traderWares != null)
            {
                PlayerWizard w = GameManager.GetWizard(playerID);
                List<object> list = traderWares.FindAll((object o) => w.AdvantageIfAcquired(o, increasedWill: false) > -2);
                if (this.wantedItem is Multitype<NodeTrade.TradeCurrency, string, int> multitype)
                {
                    if (multitype.t0 == NodeTrade.TradeCurrency.Gold)
                    {
                        list = list.FindAll((object o) => !(o is Multitype<NodeTrade.TradeCurrency, string, int> multitype3) || multitype3.t0 == NodeTrade.TradeCurrency.Gold);
                    }
                    else if (multitype.t0 == NodeTrade.TradeCurrency.Mana)
                    {
                        list = list.FindAll((object o) => !(o is Multitype<NodeTrade.TradeCurrency, string, int> multitype2) || multitype2.t0 == NodeTrade.TradeCurrency.Mana);
                    }
                }
                this.traderWares = list;
            }
        }
        if (this.playerWares == null)
        {
            this.playerWares = new List<object>();
        }
        if (this.traderWares == null)
        {
            this.traderWares = new List<object>();
        }
        this.tgFilterItems.gameObject.SetActive(value: false);
        this.tgFilterUnits.gameObject.SetActive(value: false);
        this.tgFilterResources.gameObject.SetActive(value: false);
        this.tgFilterSpells.gameObject.SetActive(value: false);
        foreach (object playerWare in this.playerWares)
        {
            if (playerWare is Multitype<NodeTrade.TradeCurrency, string, int>)
            {
                this.tgFilterResources.gameObject.SetActive(value: true);
            }
            else if (playerWare is global::MOM.Artefact)
            {
                this.tgFilterItems.gameObject.SetActive(value: true);
            }
            else if (playerWare is global::MOM.Unit)
            {
                this.tgFilterUnits.gameObject.SetActive(value: true);
            }
            else if (playerWare is DBReference<Spell>)
            {
                this.tgFilterSpells.gameObject.SetActive(value: true);
            }
        }
        foreach (object traderWare in this.traderWares)
        {
            if (traderWare is Multitype<NodeTrade.TradeCurrency, string, int>)
            {
                this.tgFilterResources.gameObject.SetActive(value: true);
            }
            else if (traderWare is global::MOM.Artefact)
            {
                this.tgFilterItems.gameObject.SetActive(value: true);
            }
            else if (traderWare is global::MOM.Unit)
            {
                this.tgFilterUnits.gameObject.SetActive(value: true);
            }
            else if (traderWare is DBReference<Spell>)
            {
                this.tgFilterSpells.gameObject.SetActive(value: true);
            }
        }
        if (base.stateStatus >= StateStatus.PreStart)
        {
            this.UpdateScreen();
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btConfirm)
        {
            this.ConfirmTrade();
            Globals.SetDynamicData("TradeResult", "Accepted");
            UIManager.Close(this);
        }
        if (s == this.btCancel)
        {
            PlayerWizard wizard = GameManager.GetWizard(this.playerID);
            PlayerWizard wizard2 = GameManager.GetWizard(this.traderID);
            if (wizard != null)
            {
                wizard2?.GetDiplomacy().GetStatusToward(wizard).AddMessage(new DiplomaticMessage
                {
                    domination = DiplomaticMessage.Domination.AddToQueue,
                    messageType = DiplomaticMessage.MessageType.Greetings
                });
            }
            Globals.SetDynamicData("TradeResult", "Canceled");
            UIManager.Close(this);
        }
        if (s == this.btConfirmTransfer)
        {
            this.goTransfer.SetActive(value: false);
        }
        if ((s == this.tgFilterAll || s == this.tgFilterItems || s == this.tgFilterUnits || s == this.tgFilterResources || s == this.tgFilterSpells) && (s as Toggle).isOn)
        {
            this.UpdateScreen();
        }
    }

    private void EnterExitEvents(object sender, object e)
    {
        GridItemManager gridItemManager = (sender as RollOverOutEvents).data as GridItemManager;
        if (this.rolloverItem == gridItemManager && e.ToString() == "OnPointerExit")
        {
            this.rolloverItem = null;
        }
        else if (e.ToString() == "OnPointerEnter")
        {
            this.rolloverItem = gridItemManager;
        }
    }

    private void ItemDrop(object sender, object e)
    {
        object item = (sender as DragAndDropItem).source.item;
        GridItemManager gridItemManager = (e as DragAndDrop).owner as GridItemManager;
        if (this.rolloverItem != null && this.rolloverItem != gridItemManager)
        {
            if (gridItemManager == this.gridPlayerInventory && this.rolloverItem == this.gridPlayerSelection)
            {
                this.TransferItem(this.playerWares, this.playerOffer, item);
            }
            if (gridItemManager == this.gridPlayerSelection && this.rolloverItem == this.gridPlayerInventory)
            {
                this.TransferItem(this.playerOffer, this.playerWares, item);
            }
            if (gridItemManager == this.gridNPCInventory && this.rolloverItem == this.gridNPCSelection)
            {
                this.TransferItem(this.traderWares, this.traderOffer, item);
            }
            if (gridItemManager == this.gridNPCSelection && this.rolloverItem == this.gridNPCInventory)
            {
                this.TransferItem(this.traderOffer, this.traderWares, item);
            }
        }
    }

    private IEnumerator TransferResource(object item, List<object> sourceList, List<object> destinationList)
    {
        bool addToOffer = sourceList == this.playerWares || sourceList == this.traderWares;
        int recipient = this.traderID;
        if (sourceList == this.traderWares || sourceList == this.traderOffer)
        {
            recipient = this.playerID;
        }
        Multitype<NodeTrade.TradeCurrency, string, int> res = item as Multitype<NodeTrade.TradeCurrency, string, int>;
        Multitype<NodeTrade.TradeCurrency, string, int> destinationItem = null;
        foreach (object destination in destinationList)
        {
            if (destination is Multitype<NodeTrade.TradeCurrency, string, int>)
            {
                Multitype<NodeTrade.TradeCurrency, string, int> multitype = destination as Multitype<NodeTrade.TradeCurrency, string, int>;
                if (multitype.t0 == res.t0)
                {
                    destinationItem = multitype;
                    break;
                }
            }
        }
        bool alreadyInDestination = destinationItem != null;
        int max = res.t2;
        if (alreadyInDestination)
        {
            max += destinationItem.t2;
        }
        if (addToOffer)
        {
            this.OpenTransferPopup(max, max, item, recipient, res.t2);
        }
        else
        {
            this.OpenTransferPopup(max, 0, item, recipient, res.t2);
        }
        while (this.goTransfer.activeInHierarchy)
        {
            yield return null;
        }
        if (res != null)
        {
            int num = (addToOffer ? this.leftToRightTransferValue : (max - this.leftToRightTransferValue));
            if (max - num == 0)
            {
                sourceList.Remove(item);
            }
            else
            {
                res.t2 = max - num;
            }
            if (alreadyInDestination)
            {
                if (num == 0)
                {
                    destinationList.Remove(destinationItem);
                }
                else
                {
                    destinationItem.t2 = num;
                }
            }
            else if (num > 0)
            {
                Multitype<NodeTrade.TradeCurrency, string, int> item2 = new Multitype<NodeTrade.TradeCurrency, string, int>(res.t0, res.t1, num);
                destinationList.Add(item2);
            }
        }
        this.UpdateScreen();
    }

    private void TransferItem(List<object> sourceList, List<object> destinationList, object item)
    {
        if (sourceList.Contains(item))
        {
            if (item is Multitype<NodeTrade.TradeCurrency, string, int>)
            {
                base.StartCoroutine(this.TransferResource(item, sourceList, destinationList));
                return;
            }
            sourceList.Remove(item);
            destinationList.Add(item);
            this.UpdateScreen();
        }
        else
        {
            Debug.LogError("Transfer item error: source list doesn't contain item " + item);
        }
    }

    private void OpenTransferPopup(int maxValue, int startValue, object item, int recipient, int beforeTransfer)
    {
        this.goTransfer.SetActive(value: true);
        if (item is Multitype<NodeTrade.TradeCurrency, string, int> multitype && item is Multitype<NodeTrade.TradeCurrency, string, int>)
        {
            this.riIcon.texture = AssetManager.Get<Texture2D>(multitype.t1);
            this.sliderTrasfer.minValue = 0f;
            this.sliderTrasfer.maxValue = maxValue;
            this.UpdateFieldsOnPopup(maxValue, startValue, item, recipient, beforeTransfer);
        }
    }

    private void UpdateFieldsOnPopup(int max, int value, object item, int recipient, int beforeTransfer)
    {
        if (value > max || value < 0)
        {
            Debug.LogError("Value is out of bounds!");
        }
        this.leftToRightTransferValue = value;
        _ = item;
        int valueCached = this.GetValueCached(item, recipient, value);
        object waresItem = null;
        object offerItem = null;
        List<object> list;
        List<object> list2;
        int targetValue;
        if (recipient == this.traderID)
        {
            list = this.playerWares;
            list2 = this.playerOffer;
            targetValue = this.traderOfferValue + this.traderParameter;
        }
        else
        {
            list = this.traderWares;
            list2 = this.traderOffer;
            targetValue = this.playerOfferValue;
        }
        int baseLocalValue = 0;
        foreach (object item2 in list2)
        {
            if (item2 is Multitype<NodeTrade.TradeCurrency, string, int> multitype && item is Multitype<NodeTrade.TradeCurrency, string, int> multitype2 && multitype.t0 == multitype2.t0)
            {
                offerItem = item2;
            }
            else
            {
                baseLocalValue += this.GetValueCached(item2, recipient);
            }
        }
        foreach (object item3 in list)
        {
            if (item3 is Multitype<NodeTrade.TradeCurrency, string, int> multitype3 && item is Multitype<NodeTrade.TradeCurrency, string, int> multitype4 && multitype3.t0 == multitype4.t0)
            {
                waresItem = item3;
            }
        }
        this.CheckRequirementsFullMode(recipient, offerItem, valueCached);
        this.sliderTrasfer.onValueChanged.RemoveAllListeners();
        this.sliderTrasfer.minValue = 0f;
        this.sliderTrasfer.maxValue = max;
        this.sliderTrasfer.value = value;
        this.tfPopupSliderMax.text = max.ToString();
        this.tfPopupValue.text = valueCached.ToString();
        this.sliderTrasfer.onValueChanged.AddListener(delegate(float v)
        {
            int value6 = Mathf.RoundToInt(v);
            value6 = Mathf.Clamp(value6, 0, max);
            this.UpdateFieldsOnPopup(max, value6, item, recipient, beforeTransfer);
        });
        this.inputAmount.onValueChanged.RemoveAllListeners();
        this.inputAmount.text = value.ToString();
        this.inputAmount.onValueChanged.AddListener(delegate(string s)
        {
            try
            {
                int num3 = Convert.ToInt32(s);
                int num4 = 1;
                if (num3 < 0)
                {
                    num4 *= -1;
                }
                int value5 = Mathf.Clamp(num3 * num4, 0, max);
                this.UpdateFieldsOnPopup(max, value5, item, recipient, beforeTransfer);
            }
            catch (Exception message)
            {
                Debug.LogWarning(message);
            }
        });
        this.btPlusOne.onClick.RemoveAllListeners();
        this.btPlusOne.onClick.AddListener(delegate
        {
            int value4 = Mathf.RoundToInt(value + 1);
            value4 = Mathf.Clamp(value4, 0, max);
            this.UpdateFieldsOnPopup(max, value4, item, recipient, beforeTransfer);
        });
        this.btMinusOne.onClick.RemoveAllListeners();
        this.btMinusOne.onClick.AddListener(delegate
        {
            int value3 = Mathf.RoundToInt(value - 1);
            value3 = Mathf.Clamp(value3, 0, max);
            this.UpdateFieldsOnPopup(max, value3, item, recipient, beforeTransfer);
        });
        this.btMatchOffer.onClick.RemoveAllListeners();
        this.btMatchOffer.onClick.AddListener(delegate
        {
            int num = 0;
            if (waresItem != null)
            {
                num += this.GetValueCached(waresItem, recipient);
            }
            if (offerItem != null)
            {
                num += this.GetValueCached(offerItem, recipient);
            }
            if (num + baseLocalValue <= targetValue)
            {
                this.UpdateFieldsOnPopup(max, max, item, recipient, beforeTransfer);
            }
            else if (baseLocalValue >= targetValue)
            {
                this.UpdateFieldsOnPopup(max, 0, item, recipient, beforeTransfer);
            }
            else
            {
                PlayerWizard wizard = GameManager.GetWizard(recipient);
                float num2 = 1f;
                if (wizard != null && wizard.ID > PlayerWizard.HumanID())
                {
                    int advantageIfAcquired = wizard.AdvantageIfAcquired(item, this.IsWantedItem(item));
                    num2 = wizard.ValueScale(item, advantageIfAcquired);
                }
                int value2 = Mathf.CeilToInt((float)(targetValue - baseLocalValue) / num2);
                this.UpdateFieldsOnPopup(max, value2, item, recipient, beforeTransfer);
            }
        });
    }

    private void ConfirmTrade()
    {
        PlayerWizard wizard = GameManager.GetWizard(this.playerID);
        PlayerWizard wizard2 = GameManager.GetWizard(this.traderID);
        if (wizard != null)
        {
            this.ApplyTrade(wizard, this.traderOffer, this.playerOffer);
        }
        if (wizard2 != null)
        {
            this.ApplyTrade(wizard2, this.playerOffer, this.traderOffer);
        }
        if (wizard != null && wizard2 != null)
        {
            int num = this.RelationshipChange();
            DiplomaticStatus statusToward = wizard2.GetDiplomacy().GetStatusToward(wizard);
            statusToward.ChangeRelationshipBy(num, affectTreaties: true);
            statusToward.willToTrade = -100;
            DiplomaticMessage diplomaticMessage = new DiplomaticMessage
            {
                domination = DiplomaticMessage.Domination.AddToQueue
            };
            if (num < -5)
            {
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.PoorTrade;
            }
            else if (num > 5)
            {
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.ExcelentTrade;
            }
            else
            {
                diplomaticMessage.messageType = DiplomaticMessage.MessageType.SuccessfulTrade;
            }
            statusToward.AddMessage(diplomaticMessage);
        }
    }

    private int GetTraderBribeForWillValue()
    {
        if (GameManager.GetWizard(this.traderID) == null)
        {
            return this.traderParameter;
        }
        return (int)((float)this.GetTotalValue() * 0.05f * (float)this.traderParameter / 50f);
    }

    private int GetTotalValue()
    {
        if (this.totalValueTrader <= 0)
        {
            PlayerWizard wizard = GameManager.GetWizard(this.traderID);
            if (wizard == null)
            {
                return 1;
            }
            this.totalValueTrader = wizard.money + wizard.mana;
            foreach (DBReference<Spell> spell in wizard.GetSpellManager().GetSpells())
            {
                this.totalValueTrader += spell.Get().researchCost / 10;
            }
            if (this.totalValueTrader <= 0)
            {
                return 100;
            }
        }
        return this.totalValueTrader;
    }

    public int RelationshipChange()
    {
        PlayerWizard wizard = GameManager.GetWizard(this.traderID);
        int num = this.playerOfferValue - this.traderOfferValue - this.GetTraderBribeForWillValue();
        Debug.Log("po " + this.playerOfferValue + " to " + this.traderOfferValue + " jade " + this.GetTraderBribeForWillValue());
        if (wizard == null)
        {
            num = this.playerOfferValue - this.traderOfferValue - this.traderParameter;
            this.willAcceptOffer = num >= 0;
            return 0;
        }
        float num2 = (float)num / (float)this.GetTotalValue();
        DiplomaticStatus diplomaticStatus = wizard.GetDiplomacy()?.GetStatusToward(this.playerID);
        int num3 = 0;
        if (diplomaticStatus != null)
        {
            int relationship = diplomaticStatus.GetRelationship();
            int num4 = (int)Mathf.Clamp((float)diplomaticStatus.fear / 100f, 0f, 100f);
            num3 = Mathf.Clamp(Mathf.Max(0, num4 + relationship / 2) / 3, 0, 30);
            Debug.Log("Relationship: " + relationship + " fearModifier: " + num4);
        }
        this.willAcceptOffer = true;
        int num5 = 0;
        if (num2 < 0f)
        {
            float num6 = (float)num3 * 0.01f;
            if (num2 < 0f - num6)
            {
                this.willAcceptOffer = false;
                num2 = 0f - num6;
            }
            if (num6 == 0f)
            {
                this.tradeAffection.value = 0f;
                num5 = -30;
            }
            else
            {
                float num7 = num2 / num6;
                this.tradeAffection.value = Mathf.Clamp01(0.5f + num7 * 0.5f);
                num5 = (int)(num7 * 30f);
            }
        }
        else
        {
            this.willAcceptOffer = num2 != 0f || this.playerOfferValue != 0;
            float value = num2 / 0.6f;
            this.tradeAffection.value = Mathf.Clamp01(0.5f + Mathf.Clamp01(value) * 0.5f);
            num5 = (int)(Mathf.Clamp01(value) * 50f);
        }
        Debug.Log("Relationship change: " + num5 + " will trade: " + this.willAcceptOffer);
        return num5;
    }

    private bool IsWantedItem(object o)
    {
        if (o is Multitype<NodeTrade.TradeCurrency, string, int> && this.wantedItem is Multitype<NodeTrade.TradeCurrency, string, int>)
        {
            Multitype<NodeTrade.TradeCurrency, string, int> multitype = o as Multitype<NodeTrade.TradeCurrency, string, int>;
            Multitype<NodeTrade.TradeCurrency, string, int> multitype2 = this.wantedItem as Multitype<NodeTrade.TradeCurrency, string, int>;
            if (multitype.t0 == multitype2.t0 && multitype.t2 >= multitype2.t2)
            {
                return true;
            }
        }
        else if (o == this.wantedItem)
        {
            return true;
        }
        return false;
    }

    private void ApplyTrade(PlayerWizard wizard, List<object> gain, List<object> loss)
    {
        foreach (object item in loss)
        {
            if (item is global::MOM.Unit)
            {
                (item as global::MOM.Unit).Destroy();
                continue;
            }
            if (item is global::MOM.Artefact)
            {
                if (wizard.artefacts.Contains(item as global::MOM.Artefact))
                {
                    wizard.artefacts.Remove(item as global::MOM.Artefact);
                    continue;
                }
                List<global::MOM.Group> groupsOfWizard = GameManager.GetGroupsOfWizard(wizard.ID);
                List<Reference<global::MOM.Unit>> list = new List<Reference<global::MOM.Unit>>();
                foreach (global::MOM.Group item2 in groupsOfWizard)
                {
                    list.AddRange(item2.GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().dbSource.Get() is Hero));
                }
                foreach (Reference<global::MOM.Unit> item3 in list)
                {
                    if (item3.Get().artefactManager.equipmentSlots.Find((EquipmentSlot o) => o.item == item as global::MOM.Artefact) != null)
                    {
                        item3.Get().artefactManager.equipmentSlots.Find((EquipmentSlot o) => o.item == item as global::MOM.Artefact).item = null;
                        break;
                    }
                }
                continue;
            }
            if (item is Multitype<NodeTrade.TradeCurrency, string, int>)
            {
                Multitype<NodeTrade.TradeCurrency, string, int> multitype = item as Multitype<NodeTrade.TradeCurrency, string, int>;
                if (multitype.t0 == NodeTrade.TradeCurrency.Gold)
                {
                    wizard.money -= multitype.t2;
                    continue;
                }
                if (multitype.t0 == NodeTrade.TradeCurrency.Mana)
                {
                    wizard.mana -= multitype.t2;
                    continue;
                }
            }
            if (!(item is DBReference<Spell>))
            {
                Debug.LogError("Unknown item!" + item);
            }
        }
        foreach (object item4 in gain)
        {
            if (item4 is global::MOM.Unit)
            {
                global::MOM.Unit unit = item4 as global::MOM.Unit;
                ((global::MOM.Group)unit.group).AddUnit(unit);
                continue;
            }
            if (item4 is global::MOM.Artefact)
            {
                wizard.artefacts.Add(item4 as global::MOM.Artefact);
                continue;
            }
            if (item4 is Multitype<NodeTrade.TradeCurrency, string, int>)
            {
                Multitype<NodeTrade.TradeCurrency, string, int> multitype2 = item4 as Multitype<NodeTrade.TradeCurrency, string, int>;
                if (multitype2.t0 == NodeTrade.TradeCurrency.Gold)
                {
                    wizard.money += multitype2.t2;
                    continue;
                }
                if (multitype2.t0 == NodeTrade.TradeCurrency.Mana)
                {
                    wizard.mana += multitype2.t2;
                    continue;
                }
            }
            if (item4 is DBReference<Spell>)
            {
                Spell spell = (item4 as DBReference<Spell>).Get();
                wizard.GetSpellManager().Add(spell);
                wizard.GetMagicAndResearch().RemoveFromCurrentResearchOptions(spell);
            }
            else
            {
                Debug.LogError("Unknown item!" + item4);
            }
        }
    }
}
