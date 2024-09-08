using MHUtils;
using MOM;
using TMPro;
using UnityEngine;

public class TopStatsPanel : MonoBehaviour
{
    public TextMeshProUGUI coins;

    public TextMeshProUGUI mana;

    public TextMeshProUGUI food;

    public TextMeshProUGUI fame;

    public RolloverObject coinsRollover;

    public RolloverObject manaRollover;

    public RolloverObject foodRollover;

    private StatDetails moneyDetails = new StatDetails();

    private StatDetails manaDetails = new StatDetails();

    private StatDetails foodDetails = new StatDetails();

    private void Awake()
    {
        this.coinsRollover.source = this.moneyDetails;
        this.manaRollover.source = this.manaDetails;
        this.foodRollover.source = this.foodDetails;
    }

    public void Set(PlayerWizard w)
    {
        this.SetMoney(w);
        this.SetMana(w);
        this.SetFood(w);
        this.SetFame(w);
    }

    public void SetMoney(PlayerWizard w)
    {
        this.moneyDetails.Clear();
        this.coins.text = w.money + "(" + w.CalculateMoneyIncome(includeUpkeep: true, this.moneyDetails).SInt() + ")";
    }

    public void SetMana(PlayerWizard w)
    {
        this.manaDetails.Clear();
        this.mana.text = w.GetMana() + "(" + w.CalculateManaIncome(includeUpkeep: true, this.manaDetails).SInt() + ")";
    }

    public void SetFood(PlayerWizard w)
    {
        this.foodDetails.Clear();
        this.food.text = w.CalculateFoodIncome(includeUpkeep: true, this.foodDetails).SInt();
    }

    public void SetFame(PlayerWizard w)
    {
        this.fame.text = w.GetFame().ToString();
    }
}
