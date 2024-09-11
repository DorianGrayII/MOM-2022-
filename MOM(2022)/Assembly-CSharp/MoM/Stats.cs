using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace MOM
{
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
            lines.Add(line);
            hAxisTicks.Add(hAxisTick);
            vAxisTicks.Add(vAxisTick);
            for (int i = 0; i < GameManager.GetWizards().Count - 1; i++)
            {
                lines.Add(Object.Instantiate(line.gameObject, line.transform.parent).GetComponent<UILineRenderer>());
            }
            for (int j = 0; j < 5; j++)
            {
                hAxisTicks.Add(Object.Instantiate(hAxisTick.gameObject, hAxisTick.transform.parent));
                vAxisTicks.Add(Object.Instantiate(vAxisTick.gameObject, vAxisTick.transform.parent));
            }
            toggleByWizardColor.Add(PlayerWizard.Color.Green, tgGreenWizard);
            toggleByWizardColor.Add(PlayerWizard.Color.Blue, tgBlueWizard);
            toggleByWizardColor.Add(PlayerWizard.Color.Red, tgRedWizard);
            toggleByWizardColor.Add(PlayerWizard.Color.Purple, tgPurpleWizard);
            toggleByWizardColor.Add(PlayerWizard.Color.Yellow, tgYellowWizard);
            List<string> names = new List<string>();
            for (StatHistory.Stats stats = StatHistory.Stats.Magic; stats < StatHistory.Stats.MAX; stats++)
            {
                names.Add(global::DBUtils.Localization.Get(Stats.statConstants[stats].Localization, true));
            }
            dropdownCategory.SetOptions(names, doUpdate: true, localize: false);
            dropdownCategory.SelectOption((int)stat);
            dropdownCategory.onChange = delegate(object obj)
            {
                int num = names.FindIndex((string o) => o == obj as string);
                stat = (StatHistory.Stats)num;
                SetupGraphs();
            };
        }

        public override IEnumerator PreStart()
        {
            yield return base.PreStart();
            if (wizard == null)
            {
                wizard = GameManager.GetHumanWizard();
            }
            bool isHuman = wizard.IsHuman;
            int turnNumber = TurnManager.GetTurnNumber();
            labelYear.text = global::DBUtils.Localization.Get("UI_CURRENT_TURN", true) + " " + TurnManager.GetTurnNumber() + ", " + Stats.CalculateDate(turnNumber);
            labelWizardName.text = wizard.name;
            labelGold.text = wizard.money.ToString();
            labelMana.text = wizard.mana.ToString();
            labelFame.text = wizard.GetFame().ToString();
            riWizardPortrait.texture = wizard.Graphic;
            traits = wizard.GetTraits();
            gmTraits.CustomDynamicItem(TraitItem, delegate
            {
                gmTraits.UpdateGrid(traits);
            });
            gmTraits.UpdateGrid(traits);
            gmSpellbooks.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                SelectWizard.BookItem(wizard.GetBaseWizard(), itemSource, source, data, index);
            });
            gmSpellbooks.UpdateGrid(new List<Tag>());
            books.Clear();
            NetDictionary<DBReference<Tag>, FInt> finalDictionary = wizard.attributes.GetFinalDictionary();
            Tag tag = (Tag)TAG.MAGIC_BOOK;
            foreach (KeyValuePair<DBReference<Tag>, FInt> item in finalDictionary)
            {
                Tag tag2 = item.Key;
                if (tag2.parent == tag)
                {
                    for (int i = 0; i < item.Value; i++)
                    {
                        books.Add(tag2);
                    }
                }
            }
            gmSpellbooks.UpdateGrid(books);
            for (int j = 0; j < heros.Count; j++)
            {
                HeroListItem heroListItem = heros[j];
                heroListItem.gameObject.SetActive(wizard.heroes.Count > j);
                if (wizard.heroes.Count > j)
                {
                    Unit unit = wizard.heroes[j].Get();
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
            diplomacyInfo.gameObject.SetActive(!isHuman);
            if (!isHuman)
            {
                familiar.Clear();
                diplomacyInfo.Set(wizard);
                labelEnemyWizardHeading.text = global::DBUtils.Localization.Get("UI_RIVAL_WIZARD", true);
            }
            else
            {
                familiar.Set(wizard.familiar);
            }
            spellBookRollover.source = wizard;
            color.Set(wizard.color);
            SetState(State.Mirror);
            SetupGraphs();
            SetupVictory();
            AudioLibrary.RequestSFX("OpenWizardInfoScreen");
        }

        private void SetState(State s)
        {
            state = s;
            btMirror.gameObject.SetActive((s == State.History || s == State.Victory) && wizard.IsHuman);
            btHistory.gameObject.SetActive((s == State.Mirror || s == State.Victory) && wizard.IsHuman);
            btVictory.gameObject.SetActive((s == State.History || s == State.Mirror) && wizard.IsHuman);
            goMirror.SetActive(s == State.Mirror);
            goHistory.SetActive(s == State.History);
            goVictory.SetActive(s == State.Victory);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            Toggle toggle = s as Toggle;
            if ((bool)toggle)
            {
                foreach (KeyValuePair<PlayerWizard.Color, Toggle> item in toggleByWizardColor)
                {
                    if (item.Value == toggle)
                    {
                        lineByWizardColor[item.Key].gameObject.SetActive(toggle.isOn);
                    }
                }
            }
            if (s == btHistory)
            {
                SetState(State.History);
            }
            else if (s == btMirror)
            {
                SetState(State.Mirror);
            }
            else if (s == btVictory)
            {
                SetState(State.Victory);
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
            if (tutorial == null && base.GetChildScreens() != null && base.GetChildScreens().Count > 0)
            {
                tutorial = base.GetChildScreens().Find((global::MHUtils.State o) => o is Tutorial_Generic) as Tutorial_Generic;
            }
            if (tutorial != null)
            {
                btHistory.interactable = false;
                btVictory.interactable = false;
                if (wizard != null && wizard != GameManager.GetHumanWizard())
                {
                    tutorial.CloseIfOpen(openHidden: false);
                }
            }
            else
            {
                btHistory.interactable = true;
                btVictory.interactable = true;
            }
        }

        public static string CalculateDate(int turn)
        {
            int turnNumber = TurnManager.GetTurnNumber();
            int num = 1400 + (turnNumber - 1) / 12;
            int iMonth = turnNumber % 12;
            string text = "";
            switch (iMonth)
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
            List<PlayerWizard> lstWizards = GameManager.GetWizards();
            int iTurnNumber = TurnManager.GetTurnNumber() - 1;
            Rect rect = line.rectTransform.rect;
            (int num2, string id) = Stats.statConstants[stat];
            vAxisLabel.text = global::DBUtils.Localization.Get(id, true);
            lineByWizardColor.Clear();
            int num3 = 0;
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            List<Reference<PlayerWizard>> discoveredWizards = humanWizard.GetDiscoveredWizards();
            foreach (PlayerWizard playerWizard in lstWizards)
            {
                UILineRenderer uILineRenderer = lines[num3++];
                lineByWizardColor.Add(playerWizard.color, uILineRenderer);
                Vector2[] array = new Vector2[iTurnNumber];
                List<int> list = playerWizard.statHistory.stats[stat];
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
                uILineRenderer.color = WizardColors.GetColor(playerWizard.color);
                bool flag = true;
                if (playerWizard != humanWizard)
                {
                    flag = discoveredWizards.Contains(playerWizard);
                }
                Toggle toggle = toggleByWizardColor[playerWizard.color];
                toggle.isOn = flag;
                toggle.gameObject.SetActive(flag);
                uILineRenderer.gameObject.SetActive(flag);
                TextMeshProUGUI componentInChildren = toggle.GetComponentInChildren<TextMeshProUGUI>();
                if ((bool)componentInChildren)
                {
                    componentInChildren.text = playerWizard.name;
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
            int num7 = (iTurnNumber + 11) / 12;
            num7 = (num7 / 5 + 1) * 5;
            Vector2 scalePoints = new Vector2(rect.width / (float)(num7 * 12), rect.height / num5);
            foreach (UILineRenderer value in lineByWizardColor.Values)
            {
                value.scalePoints = scalePoints;
            }
            grid.scalePoints = scalePoints;
            List<Vector2> list2 = new List<Vector2>();
            Vector2 size = grid.rectTransform.rect.size;
            Vector2 vector = line.rectTransform.offsetMin - grid.rectTransform.offsetMin;
            SetGridAxis(0, hAxisTicks, list2, num7 * 12, 1400f, 1400 + num7, scalePoints.x, vector.y, size.y / scalePoints.y);
            SetGridAxis(1, vAxisTicks, list2, num5, 0f, num5, scalePoints.y, vector.x, size.x / scalePoints.x);
            grid.Points = list2.ToArray();
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
            List<PlayerWizard> items = GameManager.Get().wizards.FindAll((PlayerWizard o) => o.GetID() != wizard.GetID());
            wizardsGrid.CustomDynamicItem(WizardItem);
            wizardsGrid.UpdateGrid(items);
            MagicAndResearch magicAndResearch = wizard.GetMagicAndResearch();
            DBReference<Spell> dBReference = wizard.GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.SPELL_OF_MASTERY);
            Spell curentlyCastSpell = magicAndResearch.curentlyCastSpell;
            float curentStatus;
            float nextTurnStatus;
            int turnsLeft;
            if (magicAndResearch.curentlyResearched == (Spell)SPELL.SPELL_OF_MASTERY)
            {
                magicAndResearch.GetResearchProgress(out curentStatus, out nextTurnStatus, out turnsLeft);
                researchProgress.fillAmount = curentStatus;
            }
            else if (dBReference != null)
            {
                researchProgress.fillAmount = 1f;
            }
            else
            {
                researchProgress.fillAmount = 0f;
            }
            if (curentlyCastSpell == (Spell)SPELL.SPELL_OF_MASTERY)
            {
                magicAndResearch.GetCastingProgress(out curentStatus, out nextTurnStatus, out turnsLeft);
                castingProgress.fillAmount = curentStatus;
            }
            else
            {
                castingProgress.fillAmount = 0f;
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
            else if (wizard.GetDiscoveredWizards().Find((Reference<PlayerWizard> o) => o.ID == wizardTarget.ID) != null)
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
}
