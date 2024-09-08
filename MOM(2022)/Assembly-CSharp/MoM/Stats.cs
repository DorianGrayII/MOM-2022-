// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Stats
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Stats : ScreenBase
{
    private enum State
    {
        Mirror = 0,
        History = 1,
        Victory = 2
    }

    public Button btClose;

    public Button btMirror;

    public Button btHistory;

    public Button btVictory;

    public GameObject goMirror;

    public GameObject goHistory;

    public GameObject goVictory;

    public GameObject goBanners;

    public GameObjectEnabler<PlayerWizard.Color> color;

    public GameObjectEnabler<PlayerWizard.Familiar> familiar;

    public TextMeshProUGUI labelYear;

    public UILineRenderer grid;

    public UILineRenderer line;

    private readonly List<UILineRenderer> lines = new List<UILineRenderer>();

    public Toggle tgGreenWizard;

    public Toggle tgBlueWizard;

    public Toggle tgRedWizard;

    public Toggle tgPurpleWizard;

    public Toggle tgYellowWizard;

    public DropDownFilters dropdownCategory;

    public GameObject vAxisTick;

    public TextMeshProUGUI vAxisLabel;

    public GameObject hAxisTick;

    public TextMeshProUGUI hAxisLabel;

    private List<GameObject> vAxisTicks = new List<GameObject>();

    private List<GameObject> hAxisTicks = new List<GameObject>();

    public RawImage riWizardPortrait;

    public GridItemManager gmTraits;

    public GridItemManager gmSpellbooks;

    public TextMeshProUGUI labelWizardName;

    public TextMeshProUGUI labelGold;

    public TextMeshProUGUI labelMana;

    public TextMeshProUGUI labelFame;

    public TextMeshProUGUI labelEnemyWizardHeading;

    public DiplomacyInfo diplomacyInfo;

    public RolloverObject spellBookRollover;

    public List<HeroListItem> heros;

    public GridItemManager wizardsGrid;

    public Image researchProgress;

    public Image castingProgress;

    public PlayerWizard wizard;

    private Tutorial_Generic tutorial;

    private State state;

    private List<Trait> traits;

    private readonly List<Tag> books = new List<Tag>();

    private static readonly Dictionary<StatHistory.Stats, (int minAxis, string Localization)> statConstants = new Dictionary<StatHistory.Stats, (int, string)>
    {
        {
            StatHistory.Stats.Research,
            (1000, "UI_RESEARCH_STAT")
        },
        {
            StatHistory.Stats.Army,
            (500, "UI_ARMY_STAT")
        },
        {
            StatHistory.Stats.Magic,
            (10, "UI_MAGIC_STAT")
        },
        {
            StatHistory.Stats.Towns,
            (10, "UI_TOWNS_STAT")
        },
        {
            StatHistory.Stats.Fame,
            (10, "UI_FAME_STAT")
        },
        {
            StatHistory.Stats.GoldIncome,
            (10, "UI_GOLD_INCOME_STAT")
        },
        {
            StatHistory.Stats.FoodIncome,
            (10, "UI_FOOD_INCOME_STAT")
        },
        {
            StatHistory.Stats.ManaIncome,
            (10, "UI_MANA_INCOME_STAT")
        }
    };

    private readonly Dictionary<PlayerWizard.Color, UILineRenderer> lineByWizardColor = new Dictionary<PlayerWizard.Color, UILineRenderer>();

    private readonly Dictionary<PlayerWizard.Color, Toggle> toggleByWizardColor = new Dictionary<PlayerWizard.Color, Toggle>();

    private StatHistory.Stats stat = StatHistory.Stats.Army;

    protected override void Awake()
    {
        base.Awake();
        this.lines.Add(this.line);
        this.hAxisTicks.Add(this.hAxisTick);
        this.vAxisTicks.Add(this.vAxisTick);
        for (int i = 0; i < GameManager.GetWizards().Count - 1; i++)
        {
            this.lines.Add(Object.Instantiate(this.line.gameObject, this.line.transform.parent).GetComponent<UILineRenderer>());
        }
        for (int j = 0; j < 5; j++)
        {
            this.hAxisTicks.Add(Object.Instantiate(this.hAxisTick.gameObject, this.hAxisTick.transform.parent));
            this.vAxisTicks.Add(Object.Instantiate(this.vAxisTick.gameObject, this.vAxisTick.transform.parent));
        }
        this.toggleByWizardColor.Add(PlayerWizard.Color.Green, this.tgGreenWizard);
        this.toggleByWizardColor.Add(PlayerWizard.Color.Blue, this.tgBlueWizard);
        this.toggleByWizardColor.Add(PlayerWizard.Color.Red, this.tgRedWizard);
        this.toggleByWizardColor.Add(PlayerWizard.Color.Purple, this.tgPurpleWizard);
        this.toggleByWizardColor.Add(PlayerWizard.Color.Yellow, this.tgYellowWizard);
        List<string> names = new List<string>();
        for (StatHistory.Stats stats = StatHistory.Stats.Magic; stats < StatHistory.Stats.MAX; stats++)
        {
            names.Add(global::DBUtils.Localization.Get(Stats.statConstants[stats].Localization, true));
        }
        this.dropdownCategory.SetOptions(names, doUpdate: true, localize: false);
        this.dropdownCategory.SelectOption((int)this.stat);
        this.dropdownCategory.onChange = delegate(object obj)
        {
            int num = names.FindIndex((string o) => o == obj as string);
            this.stat = (StatHistory.Stats)num;
            this.SetupGraphs();
        };
    }

    public override IEnumerator PreStart()
    {
        yield return base.PreStart();
        if (this.wizard == null)
        {
            this.wizard = GameManager.GetHumanWizard();
        }
        bool isHuman = this.wizard.IsHuman;
        int turnNumber = TurnManager.GetTurnNumber();
        this.labelYear.text = global::DBUtils.Localization.Get("UI_CURRENT_TURN", true) + " " + TurnManager.GetTurnNumber() + ", " + Stats.CalculateDate(turnNumber);
        this.labelWizardName.text = this.wizard.name;
        this.labelGold.text = this.wizard.money.ToString();
        this.labelMana.text = this.wizard.mana.ToString();
        this.labelFame.text = this.wizard.GetFame().ToString();
        this.riWizardPortrait.texture = this.wizard.Graphic;
        this.traits = this.wizard.GetTraits();
        this.gmTraits.CustomDynamicItem(TraitItem, delegate
        {
            this.gmTraits.UpdateGrid(this.traits);
        });
        this.gmTraits.UpdateGrid(this.traits);
        this.gmSpellbooks.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            SelectWizard.BookItem(this.wizard.GetBaseWizard(), itemSource, source, data, index);
        });
        this.gmSpellbooks.UpdateGrid(new List<Tag>());
        this.books.Clear();
        NetDictionary<DBReference<Tag>, FInt> finalDictionary = this.wizard.attributes.GetFinalDictionary();
        Tag tag = (Tag)TAG.MAGIC_BOOK;
        foreach (KeyValuePair<DBReference<Tag>, FInt> item in finalDictionary)
        {
            Tag tag2 = item.Key;
            if (tag2.parent == tag)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    this.books.Add(tag2);
                }
            }
        }
        this.gmSpellbooks.UpdateGrid(this.books);
        for (int j = 0; j < this.heros.Count; j++)
        {
            HeroListItem heroListItem = this.heros[j];
            heroListItem.gameObject.SetActive(this.wizard.heroes.Count > j);
            if (this.wizard.heroes.Count > j)
            {
                global::MOM.Unit unit = this.wizard.heroes[j].Get();
                string text = unit.GetName();
                if ((bool)heroListItem.heroName)
                {
                    heroListItem.heroName.text = text;
                }
                if ((bool)heroListItem.heroPortrait)
                {
                    heroListItem.heroPortrait.texture = unit.GetDescriptionInfo().GetTexture();
                }
                RolloverSimpleTooltip componentInChildren = heroListItem.GetComponentInChildren<RolloverSimpleTooltip>();
                if ((bool)componentInChildren)
                {
                    componentInChildren.title = text;
                    componentInChildren.description = global::DBUtils.Localization.Get(unit.GetDescriptionInfo().GetDescriptionKey(), true, unit.GetName());
                }
            }
        }
        this.diplomacyInfo.gameObject.SetActive(!isHuman);
        if (!isHuman)
        {
            this.familiar.Clear();
            this.diplomacyInfo.Set(this.wizard);
            this.labelEnemyWizardHeading.text = global::DBUtils.Localization.Get("UI_RIVAL_WIZARD", true);
        }
        else
        {
            this.familiar.Set(this.wizard.familiar);
        }
        this.spellBookRollover.source = this.wizard;
        this.color.Set(this.wizard.color);
        this.SetState(State.Mirror);
        this.SetupGraphs();
        this.SetupVictory();
        AudioLibrary.RequestSFX("OpenWizardInfoScreen");
    }

    private void SetState(State s)
    {
        this.state = s;
        this.btMirror.gameObject.SetActive((s == State.History || s == State.Victory) && this.wizard.IsHuman);
        this.btHistory.gameObject.SetActive((s == State.Mirror || s == State.Victory) && this.wizard.IsHuman);
        this.btVictory.gameObject.SetActive((s == State.History || s == State.Mirror) && this.wizard.IsHuman);
        this.goMirror.SetActive(s == State.Mirror);
        this.goHistory.SetActive(s == State.History);
        this.goVictory.SetActive(s == State.Victory);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        Toggle toggle = s as Toggle;
        if ((bool)toggle)
        {
            foreach (KeyValuePair<PlayerWizard.Color, Toggle> item in this.toggleByWizardColor)
            {
                if (item.Value == toggle)
                {
                    this.lineByWizardColor[item.Key].gameObject.SetActive(toggle.isOn);
                }
            }
        }
        if (s == this.btHistory)
        {
            this.SetState(State.History);
        }
        else if (s == this.btMirror)
        {
            this.SetState(State.Mirror);
        }
        else if (s == this.btVictory)
        {
            this.SetState(State.Victory);
        }
        if (s.name == "ButtonClose")
        {
            UIManager.Close(this);
        }
    }

    private void TraitItem(GameObject itemSource, object source, object data, int index)
    {
        TextMeshProUGUI componentInChildren = itemSource.GetComponentInChildren<TextMeshProUGUI>();
        if (source is Trait trait)
        {
            componentInChildren.text = trait.GetDescriptionInfo().GetLocalizedName();
            RolloverSimpleTooltip componentInChildren2 = itemSource.GetComponentInChildren<RolloverSimpleTooltip>();
            if ((bool)componentInChildren2)
            {
                componentInChildren2.sourceAsDbName = trait.dbName;
            }
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (this.tutorial == null && base.GetChildScreens() != null && base.GetChildScreens().Count > 0)
        {
            this.tutorial = base.GetChildScreens().Find((global::MHUtils.State o) => o is Tutorial_Generic) as Tutorial_Generic;
        }
        if (this.tutorial != null)
        {
            this.btHistory.interactable = false;
            this.btVictory.interactable = false;
            if (this.wizard != null && this.wizard != GameManager.GetHumanWizard())
            {
                this.tutorial.CloseIfOpen(openHidden: false);
            }
        }
        else
        {
            this.btHistory.interactable = true;
            this.btVictory.interactable = true;
        }
    }

    public static string CalculateDate(int turn)
    {
        int turnNumber = TurnManager.GetTurnNumber();
        int num = 1400 + (turnNumber - 1) / 12;
        int num2 = turnNumber % 12;
        string text = "";
        switch (num2)
        {
        case 1:
            text = global::DBUtils.Localization.Get("UI_JANUARY", true);
            break;
        case 2:
            text = global::DBUtils.Localization.Get("UI_FEBRUARY", true);
            break;
        case 3:
            text = global::DBUtils.Localization.Get("UI_MARCH", true);
            break;
        case 4:
            text = global::DBUtils.Localization.Get("UI_APRIL", true);
            break;
        case 5:
            text = global::DBUtils.Localization.Get("UI_MAY", true);
            break;
        case 6:
            text = global::DBUtils.Localization.Get("UI_JUNE", true);
            break;
        case 7:
            text = global::DBUtils.Localization.Get("UI_JULY", true);
            break;
        case 8:
            text = global::DBUtils.Localization.Get("UI_AUGUST", true);
            break;
        case 9:
            text = global::DBUtils.Localization.Get("UI_SEPTEMBER", true);
            break;
        case 10:
            text = global::DBUtils.Localization.Get("UI_OCTOBER", true);
            break;
        case 11:
            text = global::DBUtils.Localization.Get("UI_NOVEMBER", true);
            break;
        case 0:
            text = global::DBUtils.Localization.Get("UI_DECEMBER", true);
            break;
        }
        return text + " " + num;
    }

    private void SetupGraphs()
    {
        List<PlayerWizard> wizards = GameManager.GetWizards();
        int num = TurnManager.GetTurnNumber() - 1;
        Rect rect = this.line.rectTransform.rect;
        var (num2, id) = Stats.statConstants[this.stat];
        this.vAxisLabel.text = global::DBUtils.Localization.Get(id, true);
        this.lineByWizardColor.Clear();
        int num3 = 0;
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        List<Reference<PlayerWizard>> discoveredWizards = humanWizard.GetDiscoveredWizards();
        foreach (PlayerWizard item in wizards)
        {
            UILineRenderer uILineRenderer = this.lines[num3++];
            this.lineByWizardColor.Add(item.color, uILineRenderer);
            Vector2[] array = new Vector2[num];
            List<int> list = item.statHistory.stats[this.stat];
            int num4 = 0;
            foreach (int item2 in list)
            {
                array[num4].x = num4;
                array[num4].y = item2;
                if (item2 > num2)
                {
                    num2 = item2;
                }
                num4++;
            }
            if (array == null || array.Length == 0)
            {
                array = new Vector2[1];
            }
            uILineRenderer.Points = array;
            uILineRenderer.color = WizardColors.GetColor(item.color);
            bool flag = true;
            if (item != humanWizard)
            {
                flag = discoveredWizards.Contains(item);
            }
            Toggle toggle = this.toggleByWizardColor[item.color];
            toggle.isOn = flag;
            toggle.gameObject.SetActive(flag);
            uILineRenderer.gameObject.SetActive(flag);
            TextMeshProUGUI componentInChildren = toggle.GetComponentInChildren<TextMeshProUGUI>();
            if ((bool)componentInChildren)
            {
                componentInChildren.text = item.name;
            }
        }
        int length = num2.ToString().Length;
        float num5 = Mathf.Pow(10f, length);
        int num6 = 2;
        while (num5 / 2f > (float)num2 && num6 > 0)
        {
            num5 /= 2f;
            num6--;
        }
        int num7 = (num + 11) / 12;
        num7 = (num7 / 5 + 1) * 5;
        Vector2 scalePoints = new Vector2(rect.width / (float)(num7 * 12), rect.height / num5);
        foreach (UILineRenderer value in this.lineByWizardColor.Values)
        {
            value.scalePoints = scalePoints;
        }
        this.grid.scalePoints = scalePoints;
        List<Vector2> list2 = new List<Vector2>();
        Vector2 size = this.grid.rectTransform.rect.size;
        Vector2 vector = this.line.rectTransform.offsetMin - this.grid.rectTransform.offsetMin;
        this.SetGridAxis(0, this.hAxisTicks, list2, num7 * 12, 1400f, 1400 + num7, scalePoints.x, vector.y, size.y / scalePoints.y);
        this.SetGridAxis(1, this.vAxisTicks, list2, num5, 0f, num5, scalePoints.y, vector.x, size.x / scalePoints.x);
        this.grid.Points = list2.ToArray();
    }

    private void SetGridAxis(int axis, List<GameObject> ticks, List<Vector2> points, float max, float minLabel, float maxLabel, float scale, float gridOffset, float gridExtreme)
    {
        int num = ticks.Count - 1;
        for (int i = 0; i <= num; i++)
        {
            float num2 = (float)i / (float)num;
            GameObject obj = ticks[i];
            RectTransform obj2 = (RectTransform)obj.transform;
            Vector2 anchoredPosition = obj2.anchoredPosition;
            anchoredPosition[axis] = scale * max * num2;
            obj2.anchoredPosition = anchoredPosition;
            TextMeshProUGUI componentInChildren = obj.GetComponentInChildren<TextMeshProUGUI>();
            if ((bool)componentInChildren)
            {
                componentInChildren.text = Mathf.RoundToInt(Mathf.Lerp(minLabel, maxLabel, num2)).ToString();
            }
            Vector2 item = default(Vector2);
            item[axis] = max * num2 + gridOffset / scale;
            item[axis ^ 1] = 0f;
            points.Add(item);
            item[axis ^ 1] = gridExtreme;
            points.Add(item);
        }
    }

    private void SetupVictory()
    {
        List<PlayerWizard> items = GameManager.Get().wizards.FindAll((PlayerWizard o) => o.GetID() != this.wizard.GetID());
        this.wizardsGrid.CustomDynamicItem(WizardItem);
        this.wizardsGrid.UpdateGrid(items);
        MagicAndResearch magicAndResearch = this.wizard.GetMagicAndResearch();
        DBReference<Spell> dBReference = this.wizard.GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.SPELL_OF_MASTERY);
        Spell curentlyCastSpell = magicAndResearch.curentlyCastSpell;
        float curentStatus;
        float nextTurnStatus;
        int turnsLeft;
        if (magicAndResearch.curentlyResearched == (Spell)SPELL.SPELL_OF_MASTERY)
        {
            magicAndResearch.GetResearchProgress(out curentStatus, out nextTurnStatus, out turnsLeft);
            this.researchProgress.fillAmount = curentStatus;
        }
        else if (dBReference != null)
        {
            this.researchProgress.fillAmount = 1f;
        }
        else
        {
            this.researchProgress.fillAmount = 0f;
        }
        if (curentlyCastSpell == (Spell)SPELL.SPELL_OF_MASTERY)
        {
            magicAndResearch.GetCastingProgress(out curentStatus, out nextTurnStatus, out turnsLeft);
            this.castingProgress.fillAmount = curentStatus;
        }
        else
        {
            this.castingProgress.fillAmount = 0f;
        }
    }

    private void WizardItem(GameObject itemSource, object source, object data, int index)
    {
        PlayerWizard wizardTarget = source as PlayerWizard;
        WizardListItem2 component = itemSource.GetComponent<WizardListItem2>();
        component.gemBlue.SetActive(wizardTarget.color == PlayerWizard.Color.Blue);
        component.gemGreen.SetActive(wizardTarget.color == PlayerWizard.Color.Green);
        component.gemPurple.SetActive(wizardTarget.color == PlayerWizard.Color.Purple);
        component.gemRed.SetActive(wizardTarget.color == PlayerWizard.Color.Red);
        component.gemYellow.SetActive(wizardTarget.color == PlayerWizard.Color.Yellow);
        if (!wizardTarget.isAlive)
        {
            component.icon.texture = wizardTarget.Graphic;
            component.labelName.text = wizardTarget.name;
            component.icon.gameObject.SetActive(value: true);
            component.wizardDefeated.SetActive(value: true);
        }
        else if (this.wizard.GetDiscoveredWizards().Find((Reference<PlayerWizard> o) => o.ID == wizardTarget.ID) != null)
        {
            component.icon.texture = wizardTarget.Graphic;
            component.labelName.text = wizardTarget.name;
            component.icon.gameObject.SetActive(value: true);
            component.wizardDefeated.SetActive(value: false);
        }
        else
        {
            component.icon.gameObject.SetActive(value: false);
            component.wizardDefeated.SetActive(value: false);
            component.labelName.text = global::DBUtils.Localization.Get("UI_UNKNOWN_WIZARD", true);
        }
    }
}
