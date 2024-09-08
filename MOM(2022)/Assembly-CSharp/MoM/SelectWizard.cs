// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.SelectWizard
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coffee.UIEffects;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectWizard : ScreenBase
{
    private enum Stages
    {
        SelectWizard = 0,
        Portrait = 1,
        TraitsAndMagic = 2,
        Spells = 3
    }

    public Button btContinue;

    public Button btCancel;

    public Button btCustomise;

    public Button btNextWizard;

    public Button btPreviousWizard;

    public RawImage riWizardBg;

    public RawImage riWizardBgOld;

    public GridItemManager gmWizards;

    public GridItemManager gmSpellbooks;

    public TextMeshProUGUI textWizardName;

    public TextMeshProUGUI textWizardDesc;

    public TextMeshProUGUI textTrait1;

    public TextMeshProUGUI textTrait2;

    public TextMeshProUGUI textTrait3;

    public TextMeshProUGUI textTrait4;

    public GameObject goCustomWizard;

    public GameObject goSelectPortrait;

    public GameObject goSelectTraits;

    public GameObject goSelectSpells;

    public GameObject goTrait1;

    public GameObject goTrait2;

    public GameObject goTrait3;

    public GameObject goTrait4;

    public Animator screenAnimator;

    public List<Tag> books = new List<Tag>();

    private List<Trait> fixedTraits = new List<Trait>();

    private static SelectWizard instance;

    private bool customWizardMode;

    private bool animating;

    private Stages stage;

    public Button btBackToSelectWizard;

    public Button btNextToTraits;

    public Button btPrevPortrait;

    public Button btNextPortrait;

    public TMP_InputField inputWizardName;

    public RawImage riWizardPortrait;

    public TextMeshProUGUI textPicks;

    public Button btBackToPortrait;

    public Button btNextToSpells;

    public Button btLessLife;

    public Button btMoreLife;

    public Button btLessDeath;

    public Button btMoreDeath;

    public Button btLessChaos;

    public Button btMoreChaos;

    public Button btLessNature;

    public Button btMoreNature;

    public Button btLessSorcery;

    public Button btMoreSorcery;

    public CanvasGroup groupLife;

    public CanvasGroup groupDeath;

    public Slider slLife;

    public Slider slDeath;

    public Slider slChaos;

    public Slider slNature;

    public Slider slSorcery;

    public TextMeshProUGUI textTraitsNumber;

    public GridItemManager gmSelectableTraits;

    public GameObject goSelectTraitsAndMagic;

    private List<Wizard> wizards;

    private List<Trait> traits = new List<Trait>();

    private List<Trait> selectedTraits = new List<Trait>();

    private int picksLeft;

    private Dictionary<TAG, Slider> bookSliders = new Dictionary<TAG, Slider>();

    private Wizard customWizard = new Wizard();

    private Attributes attributes = new Attributes(null);

    public Button btConfirmWizard;

    public Button btBacktoTraits;

    public GridItemManager gridCommonSpells;

    public GridItemManager gridUncommonSpells;

    public GridItemManager gridRareSpells;

    public TextMeshProUGUI textCommonSpells;

    public TextMeshProUGUI textUncommonSpells;

    public TextMeshProUGUI textRareSpells;

    public TextMeshProUGUI textSpellPicks;

    public TextMeshProUGUI textDomainName;

    public Toggle tgLife;

    public Toggle tgDeath;

    public Toggle tgNature;

    public Toggle tgChaos;

    public Toggle tgSorcery;

    public GameObject goLifeNeedsAttention;

    public GameObject goDeathNeedsAttention;

    public GameObject goNatureNeedsAttention;

    public GameObject goChaosNeedsAttention;

    public GameObject goSorceryNeedsAttention;

    public GameObject goCommonNeedsAttention;

    public GameObject goUncommonNeedsAttention;

    public GameObject goRareNeedsAttention;

    public GameObject goUncommonInfo;

    public GameObject goRareInfo;

    private Dictionary<ERealm, Toggle> realmTabs = new Dictionary<ERealm, Toggle>();

    private Dictionary<ERealm, GameObject> realmAttention = new Dictionary<ERealm, GameObject>();

    private Dictionary<ERealm, Dictionary<ERarity, int>> realmPicks = new Dictionary<ERealm, Dictionary<ERarity, int>>();

    private Dictionary<ERealm, Dictionary<ERarity, List<Spell>>> realmSpells = new Dictionary<ERealm, Dictionary<ERarity, List<Spell>>>();

    private Dictionary<ERealm, Dictionary<ERarity, List<Spell>>> realmPickedSpells = new Dictionary<ERealm, Dictionary<ERarity, List<Spell>>>();

    private Dictionary<ERarity, GridItemManager> rarityGrids = new Dictionary<ERarity, GridItemManager>();

    private Dictionary<ERarity, TextMeshProUGUI> rarityTitles = new Dictionary<ERarity, TextMeshProUGUI>();

    private Dictionary<ERarity, GameObject> rarityAttention = new Dictionary<ERarity, GameObject>();

    private ERealm[] realms = new ERealm[5]
    {
        ERealm.Life,
        ERealm.Death,
        ERealm.Nature,
        ERealm.Chaos,
        ERealm.Sorcery
    };

    private Dictionary<ERealm, string> realmToUIText = new Dictionary<ERealm, string>
    {
        {
            ERealm.Life,
            "UI_LIFE"
        },
        {
            ERealm.Death,
            "UI_DEATH"
        },
        {
            ERealm.Nature,
            "UI_NATURE"
        },
        {
            ERealm.Chaos,
            "UI_CHAOS"
        },
        {
            ERealm.Sorcery,
            "UI_SORCERY"
        }
    };

    private Dictionary<ERarity, string> rarityToText = new Dictionary<ERarity, string>
    {
        {
            ERarity.Common,
            "UI_COMMON"
        },
        {
            ERarity.Uncommon,
            "UI_UNCOMMON"
        },
        {
            ERarity.Rare,
            "UI_RARE"
        }
    };

    private Dictionary<ERealm, TAG> realmToTag = new Dictionary<ERealm, TAG>
    {
        {
            ERealm.Life,
            TAG.LIFE_MAGIC_BOOK
        },
        {
            ERealm.Death,
            TAG.DEATH_MAGIC_BOOK
        },
        {
            ERealm.Nature,
            TAG.NATURE_MAGIC_BOOK
        },
        {
            ERealm.Chaos,
            TAG.CHAOS_MAGIC_BOOK
        },
        {
            ERealm.Sorcery,
            TAG.SORCERY_MAGIC_BOOK
        }
    };

    private ERealm currentRealm;

    private int imageIndex;

    private int errorOpen;

    private bool allSpellsPicked;

    public static SelectWizard Get()
    {
        return SelectWizard.instance;
    }

    protected override void Awake()
    {
        SelectWizard.instance = this;
        base.Awake();
        this.ClearWizards();
        this.btNextWizard = this.gmWizards.pageingNext;
        this.btPreviousWizard = this.gmWizards.pageingPrev;
        this.wizards = DataBase.GetType<Wizard>();
        this.goCustomWizard.SetActive(value: false);
        this.gmWizards.CustomDynamicItem(WizardItem, UpdateWizardGrid);
        this.gmWizards.onSelectionChange = SelectionChanged;
        this.gmWizards.UpdateGrid(new List<Wizard>());
        this.textTraitsNumber.text = global::DBUtils.Localization.Get("UI_NUMBER_OF_TRAITS_PICKED", true, this.selectedTraits.Count());
        this.gmSelectableTraits.CustomDynamicItem(TraitItem, delegate
        {
            this.gmSelectableTraits.UpdateGrid(this.traits);
        });
        this.gmSpellbooks.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            SelectWizard.BookItem(this.gmWizards.GetSelectedObject<Wizard>(), itemSource, source, data, index);
        });
        this.gmSpellbooks.UpdateGrid(new List<Tag>());
        this.bookSliders.Add(TAG.LIFE_MAGIC_BOOK, this.slLife);
        this.bookSliders.Add(TAG.DEATH_MAGIC_BOOK, this.slDeath);
        this.bookSliders.Add(TAG.CHAOS_MAGIC_BOOK, this.slChaos);
        this.bookSliders.Add(TAG.NATURE_MAGIC_BOOK, this.slNature);
        this.bookSliders.Add(TAG.SORCERY_MAGIC_BOOK, this.slSorcery);
        this.realmTabs.Add(ERealm.Life, this.tgLife);
        this.realmTabs.Add(ERealm.Death, this.tgDeath);
        this.realmTabs.Add(ERealm.Nature, this.tgNature);
        this.realmTabs.Add(ERealm.Chaos, this.tgChaos);
        this.realmTabs.Add(ERealm.Sorcery, this.tgSorcery);
        this.realmAttention.Add(ERealm.Life, this.goLifeNeedsAttention);
        this.realmAttention.Add(ERealm.Death, this.goDeathNeedsAttention);
        this.realmAttention.Add(ERealm.Nature, this.goNatureNeedsAttention);
        this.realmAttention.Add(ERealm.Chaos, this.goChaosNeedsAttention);
        this.realmAttention.Add(ERealm.Sorcery, this.goSorceryNeedsAttention);
        this.rarityGrids.Add(ERarity.Common, this.gridCommonSpells);
        this.rarityGrids.Add(ERarity.Uncommon, this.gridUncommonSpells);
        this.rarityGrids.Add(ERarity.Rare, this.gridRareSpells);
        this.rarityTitles.Add(ERarity.Common, this.textCommonSpells);
        this.rarityTitles.Add(ERarity.Uncommon, this.textUncommonSpells);
        this.rarityTitles.Add(ERarity.Rare, this.textRareSpells);
        this.rarityAttention.Add(ERarity.Common, this.goCommonNeedsAttention);
        this.rarityAttention.Add(ERarity.Uncommon, this.goUncommonNeedsAttention);
        this.rarityAttention.Add(ERarity.Rare, this.goRareNeedsAttention);
        foreach (KeyValuePair<ERarity, GridItemManager> grid in this.rarityGrids)
        {
            grid.Value.CustomDynamicItem(RaritySpellItem, delegate
            {
                grid.Value.UpdateGrid(this.realmSpells[this.currentRealm][grid.Key], grid.Key);
            });
        }
        foreach (KeyValuePair<ERealm, Toggle> kvp2 in this.realmTabs)
        {
            kvp2.Value.onValueChanged.AddListener(delegate(bool value)
            {
                if (value)
                {
                    this.RealmTabSelected(kvp2.Key);
                }
            });
        }
        foreach (KeyValuePair<TAG, Slider> kvp in this.bookSliders)
        {
            kvp.Value.onValueChanged.AddListener(delegate(float v)
            {
                this.SpellsChange(kvp.Key, v);
                this.EnsureSelectedTraits();
            });
        }
        this.stage = Stages.SelectWizard;
        this.UpdateVisibility();
        this.CyclePortrait(0);
        this.CalcPicksRemaining();
        this.gmSelectableTraits.UpdateGrid(this.traits);
        this.UpdateSpellInteractibility();
    }

    public override IEnumerator PreClose()
    {
        SelectWizard.instance = null;
        yield return base.PreClose();
    }

    private void ClearWizards()
    {
        foreach (PlayerWizard item in EntityManager.GetEntitiesType<PlayerWizard>())
        {
            item.Destroy();
        }
        if (GameManager.Get().wizards != null)
        {
            GameManager.Get().wizards.Clear();
        }
    }

    public static void BookItem(Wizard w, GameObject itemSource, object source, object data, int index)
    {
        GISpellbookItem component = itemSource.GetComponent<GISpellbookItem>();
        Tag bookOrRealm = source as Tag;
        component.Set(bookOrRealm, index, w);
    }

    public void FlipWizardToFront()
    {
        this.riWizardBg.transform.SetAsLastSibling();
    }

    public void AnimationFinished()
    {
        this.animating = false;
    }

    private void SelectionChanged(object o)
    {
        Wizard selectedObject = this.gmWizards.GetSelectedObject<Wizard>();
        if (selectedObject == null)
        {
            return;
        }
        this.textWizardName.text = selectedObject.descriptionInfo.GetLocalizedName();
        this.textWizardDesc.text = selectedObject.descriptionInfo.GetLocalizedDescription();
        this.goTrait1.gameObject.SetActive(value: false);
        this.goTrait2.gameObject.SetActive(value: false);
        this.goTrait3.gameObject.SetActive(value: false);
        this.goTrait4.gameObject.SetActive(value: false);
        if (selectedObject.traits != null)
        {
            for (int i = 0; i <= selectedObject.traits.Length - 1; i++)
            {
                if (i == 0)
                {
                    this.goTrait1.gameObject.SetActive(selectedObject.traits[i] != null);
                    this.textTrait1.text = selectedObject.traits[i].descriptionInfo.GetLocalizedName();
                    RolloverSimpleTooltip componentInChildren = this.textTrait1.GetComponentInChildren<RolloverSimpleTooltip>();
                    if ((bool)componentInChildren)
                    {
                        componentInChildren.sourceAsDbName = selectedObject.traits[i].dbName;
                    }
                }
                if (i == 1)
                {
                    this.goTrait2.gameObject.SetActive(selectedObject.traits[i] != null);
                    this.textTrait2.text = selectedObject.traits[i].descriptionInfo.GetLocalizedName();
                    RolloverSimpleTooltip componentInChildren2 = this.textTrait2.GetComponentInChildren<RolloverSimpleTooltip>();
                    if ((bool)componentInChildren2)
                    {
                        componentInChildren2.sourceAsDbName = selectedObject.traits[i].dbName;
                    }
                }
                if (i == 2)
                {
                    this.goTrait3.gameObject.SetActive(selectedObject.traits[i] != null);
                    this.textTrait3.text = selectedObject.traits[i].descriptionInfo.GetLocalizedName();
                    RolloverSimpleTooltip componentInChildren3 = this.textTrait3.GetComponentInChildren<RolloverSimpleTooltip>();
                    if ((bool)componentInChildren3)
                    {
                        componentInChildren3.sourceAsDbName = selectedObject.traits[i].dbName;
                    }
                }
                if (i == 3)
                {
                    this.goTrait4.gameObject.SetActive(selectedObject.traits[i] != null);
                    this.textTrait4.text = selectedObject.traits[i].descriptionInfo.GetLocalizedName();
                    RolloverSimpleTooltip componentInChildren4 = this.textTrait4.GetComponentInChildren<RolloverSimpleTooltip>();
                    if ((bool)componentInChildren4)
                    {
                        componentInChildren4.sourceAsDbName = selectedObject.traits[i].dbName;
                    }
                }
            }
        }
        Texture2D texture2D = AssetManager.Get<Texture2D>(selectedObject.background);
        if (texture2D != this.riWizardBg.texture)
        {
            if (!this.animating)
            {
                this.riWizardBgOld.texture = this.riWizardBg.texture;
            }
            this.riWizardBgOld.transform.SetAsLastSibling();
            this.riWizardBg.texture = texture2D;
            this.animating = true;
            this.screenAnimator.SetTrigger("SwitchWizard");
        }
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_WIZARD_PICKS");
        int num = 0;
        this.fixedTraits.Clear();
        if (selectedObject.traits != null)
        {
            Trait[] array = selectedObject.traits;
            foreach (Trait trait in array)
            {
                this.fixedTraits.Add(trait);
                num += trait.cost;
            }
        }
        this.books.Clear();
        if (selectedObject.tags != null)
        {
            CountedTag[] tags = selectedObject.tags;
            foreach (CountedTag countedTag in tags)
            {
                if (countedTag.tag.parent != (Tag)TAG.MAGIC_BOOK)
                {
                    continue;
                }
                for (int k = 0; k < countedTag.amount; k++)
                {
                    if (settingAsInt > num)
                    {
                        num++;
                        this.books.Add(countedTag.tag);
                    }
                }
            }
        }
        while (num < settingAsInt && this.books.Count > 0)
        {
            num++;
            this.books.Add(this.books[this.books.Count - 1]);
        }
        this.gmSpellbooks.UpdateGrid(this.books);
    }

    public override IEnumerator PreStart()
    {
        this.UpdateWizardGrid();
        this.traits = DataBase.GetType<Trait>().FindAll((Trait o) => o.cost != -1);
        this.gmSelectableTraits.UpdateGrid(this.traits);
        this.SelectionChanged(null);
        yield return base.PreStart();
    }

    private void UpdateWizardGrid()
    {
        this.gmWizards.UpdateGrid(this.wizards);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btCustomise)
        {
            this.customWizardMode = true;
            this.stage = Stages.Portrait;
            this.UpdateVisibility();
        }
        else if (s == this.btPrevPortrait)
        {
            this.CyclePortrait(-1);
        }
        else if (s == this.btNextPortrait)
        {
            this.CyclePortrait(1);
        }
        else if (s == this.btBackToSelectWizard)
        {
            this.customWizardMode = false;
            this.stage--;
            Wizard selectedObject = this.gmWizards.GetSelectedObject<Wizard>();
            this.SelectionChanged(selectedObject);
            this.UpdateVisibility();
        }
        else if (s == this.btBackToPortrait)
        {
            this.stage--;
            this.UpdateVisibility();
        }
        else if (s == this.btBacktoTraits)
        {
            if (this.customWizardMode)
            {
                if (this.BackSpells())
                {
                    this.stage--;
                    this.UpdateVisibility();
                }
                return;
            }
            this.stage = Stages.SelectWizard;
            this.UpdateVisibility();
            ERealm[] array = this.realms;
            foreach (ERealm key in array)
            {
                this.bookSliders[this.realmToTag[key]].value = 0f;
            }
            this.selectedTraits.Clear();
        }
        else if (s == this.btNextToTraits)
        {
            if (string.IsNullOrWhiteSpace(this.inputWizardName.text))
            {
                PopupGeneral.OpenPopup(this, global::DBUtils.Localization.Get("UI_REQUIRED", true), global::DBUtils.Localization.Get("UI_ENTER_NAME_ERROR", true), global::DBUtils.Localization.Get("UI_OKAY", true));
                return;
            }
            if (this.CalcPicksRemainingInt() < 0)
            {
                ERealm[] array = this.realms;
                foreach (ERealm key2 in array)
                {
                    this.bookSliders[this.realmToTag[key2]].value = 0f;
                }
                this.selectedTraits.Clear();
            }
            this.stage++;
            this.UpdateVisibility();
        }
        else if (s == this.btNextToSpells)
        {
            if (this.picksLeft != 0)
            {
                PopupGeneral.OpenPopup(this, global::DBUtils.Localization.Get("UI_REQUIRED", true), global::DBUtils.Localization.Get("UI_USE_PICKS_ERROR", true), global::DBUtils.Localization.Get("UI_OKAY", true));
                return;
            }
            this.stage++;
            if (this.GetNumBooks() == 0)
            {
                this.allSpellsPicked = true;
                this.ConfirmWizard();
            }
            else
            {
                this.UpdateVisibility();
                this.StartSpellSelection();
            }
        }
        else if (s == this.btConfirmWizard)
        {
            this.ConfirmWizard();
        }
        else if (s == this.btContinue)
        {
            Wizard selectedObject2 = this.gmWizards.GetSelectedObject<Wizard>();
            if (selectedObject2 == null)
            {
                Debug.LogError("Wizard selection contains no data!");
                return;
            }
            this.selectedTraits.Clear();
            ERealm[] array = this.realms;
            foreach (ERealm key3 in array)
            {
                this.bookSliders[this.realmToTag[key3]].value = 0f;
            }
            array = this.realms;
            foreach (ERealm key4 in array)
            {
                TAG r = this.realmToTag[key4];
                int count = this.books.FindAll((Tag o) => o == (Tag)r).Count;
                this.bookSliders[r].value = count;
            }
            foreach (Trait fixedTrait in this.fixedTraits)
            {
                this.selectedTraits.Add(fixedTrait);
            }
            int num = this.wizards.IndexOf(selectedObject2);
            if (num >= 0)
            {
                this.imageIndex = num;
            }
            this.stage = Stages.Spells;
            if (this.GetNumBooks() == 0)
            {
                this.allSpellsPicked = true;
                this.ConfirmWizard();
            }
            else
            {
                this.UpdateVisibility();
                this.StartSpellSelection();
            }
        }
        else if (s == this.btNextWizard || s == this.btPreviousWizard)
        {
            Wizard selectedObject3 = this.gmWizards.GetSelectedObject<Wizard>();
            this.SelectionChanged(selectedObject3);
        }
        else if (s == this.btCancel)
        {
            this.ClearWizards();
            MHEventSystem.TriggerEvent(this, "Back");
        }
    }

    private void WizardItem(GameObject itemSource, object source, object data, int index)
    {
        GIToggleWizardItem component = itemSource.GetComponent<GIToggleWizardItem>();
        Wizard wizard = source as Wizard;
        Texture2D texture = AssetManager.Get<Texture2D>(wizard.icon);
        component.image.texture = texture;
        component.goMagicTypeChaos.SetActive(this.HaveMagic(wizard, TAG.CHAOS_MAGIC_BOOK) > 0);
        component.goMagicTypeDeath.SetActive(this.HaveMagic(wizard, TAG.DEATH_MAGIC_BOOK) > 0);
        component.goMagicTypeLife.SetActive(this.HaveMagic(wizard, TAG.LIFE_MAGIC_BOOK) > 0);
        component.goMagicTypeNature.SetActive(this.HaveMagic(wizard, TAG.NATURE_MAGIC_BOOK) > 0);
        component.goMagicTypeSorcery.SetActive(this.HaveMagic(wizard, TAG.SORCERY_MAGIC_BOOK) > 0);
        RolloverSimpleTooltip orAddComponent = itemSource.GetOrAddComponent<RolloverSimpleTooltip>();
        orAddComponent.title = wizard.descriptionInfo.GetLocalizedName();
        orAddComponent.useMouseLocation = false;
        orAddComponent.offset.x = -12f;
        orAddComponent.offset.y = -40f;
    }

    private FInt HaveMagic(Wizard w, TAG t)
    {
        if (w.tags == null)
        {
            return FInt.ZERO;
        }
        Tag tg = (Tag)t;
        return Array.Find(w.tags, (CountedTag o) => o.tag == tg)?.amount ?? FInt.ZERO;
    }

    private void UpdateVisibility()
    {
        this.goCustomWizard.SetActive(this.stage != Stages.SelectWizard);
        this.goSelectPortrait.SetActive(this.stage == Stages.Portrait);
        this.goSelectTraits.SetActive(this.stage == Stages.TraitsAndMagic);
        this.goSelectSpells.SetActive(this.stage == Stages.Spells);
    }

    private void CyclePortrait(int dir)
    {
        int count = this.wizards.Count;
        this.imageIndex = (this.imageIndex + dir + count) % count;
        this.riWizardPortrait.texture = AssetManager.Get<Texture2D>(this.wizards[this.imageIndex].icon);
    }

    private void TraitItem(GameObject itemSource, object source, object data, int index)
    {
        TextMeshProUGUI componentInChildren = itemSource.GetComponentInChildren<TextMeshProUGUI>();
        Trait t = source as Trait;
        if (t == null)
        {
            return;
        }
        componentInChildren.text = t.GetDescriptionInfo().GetLocalizedName() + " (" + t.cost + ")";
        Toggle toggle = itemSource.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(delegate(bool value)
        {
            this.TraitSelectionChanged(t, toggle, value);
        });
        toggle.isOn = this.selectedTraits.Contains(t);
        RolloverSimpleTooltip componentInChildren2 = itemSource.GetComponentInChildren<RolloverSimpleTooltip>();
        if ((bool)componentInChildren2)
        {
            componentInChildren2.sourceAsDbName = t.dbName;
        }
        bool flag = true;
        if (!string.IsNullOrWhiteSpace(t.prerequisiteScript))
        {
            flag = (bool)ScriptLibrary.Call(t.prerequisiteScript, this.attributes, this.selectedTraits);
        }
        if (!toggle.isOn)
        {
            if (t.cost > this.picksLeft || this.selectedTraits.Count == 6)
            {
                flag = false;
            }
        }
        else if (!flag)
        {
            toggle.isOn = false;
        }
        toggle.interactable = flag;
    }

    private int CalcPicksRemainingInt()
    {
        if (DifficultySettingsData.current != null)
        {
            this.picksLeft = DifficultySettingsData.GetSettingAsInt("UI_DIFF_WIZARD_PICKS");
        }
        else
        {
            this.picksLeft = 11;
        }
        foreach (Trait selectedTrait in this.selectedTraits)
        {
            this.picksLeft -= selectedTrait.cost;
        }
        this.attributes.baseAttributes.Clear();
        foreach (KeyValuePair<TAG, Slider> bookSlider in this.bookSliders)
        {
            this.picksLeft -= (int)bookSlider.Value.value;
            this.attributes.baseAttributes[(Tag)bookSlider.Key] = new FInt(bookSlider.Value.value);
        }
        return this.picksLeft;
    }

    private void CalcPicksRemaining()
    {
        this.textPicks.text = global::DBUtils.Localization.Get("UI_PICKS", true) + " " + this.CalcPicksRemainingInt();
        this.gmSelectableTraits.UpdateGrid(this.traits);
    }

    private void UpdateSpellInteractibility()
    {
    }

    public int GetNumBooks(ERealm realm)
    {
        if (this.stage == Stages.SelectWizard)
        {
            Tag realmTag = MagicAndResearch.GetTagForRealm(realm);
            return this.books.FindAll((Tag o) => o == realmTag).Count();
        }
        return (int)this.bookSliders[this.realmToTag[realm]].value;
    }

    private void SpellsChange(TAG tag, float value)
    {
        if (tag == TAG.LIFE_MAGIC_BOOK && value > 0f && this.bookSliders[TAG.DEATH_MAGIC_BOOK].value > 0f)
        {
            this.bookSliders[tag].value = 0f;
            if (this.errorOpen == 0)
            {
                this.errorOpen++;
                PopupGeneral.OpenPopup(this, global::DBUtils.Localization.Get("UI_REQUIRED", true), global::DBUtils.Localization.Get("UI_LIFE_AND_DEATH_ERROR", true), global::DBUtils.Localization.Get("UI_OKAY", true), delegate
                {
                    this.errorOpen--;
                });
            }
            return;
        }
        if (tag == TAG.DEATH_MAGIC_BOOK && value > 0f && this.bookSliders[TAG.LIFE_MAGIC_BOOK].value > 0f)
        {
            this.bookSliders[tag].value = 0f;
            if (this.errorOpen == 0)
            {
                this.errorOpen++;
                PopupGeneral.OpenPopup(this, global::DBUtils.Localization.Get("UI_REQUIRED", true), global::DBUtils.Localization.Get("UI_LIFE_AND_DEATH_ERROR", true), global::DBUtils.Localization.Get("UI_OKAY", true), delegate
                {
                    this.errorOpen--;
                });
            }
            return;
        }
        this.CalcPicksRemaining();
        if (this.picksLeft < 0)
        {
            value += (float)this.picksLeft;
            this.bookSliders[tag].value = value;
        }
        this.gmSelectableTraits.UpdateGrid(this.traits);
        this.UpdateSpellInteractibility();
        TooltipSpellBook.Update();
    }

    private void TraitSelectionChanged(Trait trait, Toggle t, bool value)
    {
        bool flag = this.selectedTraits.Contains(trait);
        if (flag != t.isOn)
        {
            if (flag)
            {
                this.selectedTraits.Remove(trait);
            }
            else
            {
                this.selectedTraits.Add(trait);
            }
            this.CalcPicksRemaining();
            if (t.isOn && this.picksLeft < 0)
            {
                t.isOn = false;
                this.CalcPicksRemaining();
            }
            if (!this.EnsureSelectedTraits())
            {
                this.textTraitsNumber.text = global::DBUtils.Localization.Get("UI_NUMBER_OF_TRAITS_PICKED", true, this.selectedTraits.Count());
            }
            this.gmSelectableTraits.UpdateGrid(this.traits);
        }
    }

    private bool EnsureSelectedTraits()
    {
        bool flag = false;
        for (int num = this.selectedTraits.Count - 1; num >= 0; num--)
        {
            string prerequisiteScript = this.selectedTraits[num].prerequisiteScript;
            if (prerequisiteScript != null && !(bool)ScriptLibrary.Call(prerequisiteScript, this.attributes, this.selectedTraits))
            {
                this.selectedTraits.RemoveAt(num);
                flag = true;
            }
        }
        if (flag)
        {
            this.CalcPicksRemaining();
            this.textTraitsNumber.text = global::DBUtils.Localization.Get("UI_NUMBER_OF_TRAITS_PICKED", true, this.selectedTraits.Count());
        }
        return flag;
    }

    private int GetNumBooks()
    {
        int num = 0;
        ERealm[] array = this.realms;
        foreach (ERealm key in array)
        {
            num += Mathf.Max(0, (int)this.bookSliders[this.realmToTag[key]].value - 1);
        }
        return num;
    }

    private void StartSpellSelection()
    {
        this.UpdateSpellList();
        this.currentRealm = ERealm.None;
        this.realmPicks.Clear();
        this.realmPickedSpells.Clear();
        ERealm[] array = this.realms;
        for (int i = 0; i < array.Length; i++)
        {
            ERealm eRealm = (this.currentRealm = array[i]);
            Toggle toggle = this.realmTabs[eRealm];
            int bookCount = (int)this.bookSliders[this.realmToTag[eRealm]].value;
            int num = 0;
            BooksAdvantage booksAdvantage = MagicAndResearch.GetBooksAdvantage(eRealm, bookCount);
            Dictionary<ERarity, int> dictionary = new Dictionary<ERarity, int>
            {
                {
                    ERarity.Common,
                    0
                },
                {
                    ERarity.Uncommon,
                    0
                },
                {
                    ERarity.Rare,
                    0
                }
            };
            if (booksAdvantage != null && booksAdvantage.startingSpells != null)
            {
                SpellsSection[] startingSpells = booksAdvantage.startingSpells;
                foreach (SpellsSection spellsSection in startingSpells)
                {
                    if (spellsSection.rarity < ERarity.VeryRare)
                    {
                        List<Spell> list = new List<Spell>(this.realmSpells[eRealm][spellsSection.rarity]);
                        int num2 = Mathf.Min(spellsSection.count, list.Count);
                        dictionary[spellsSection.rarity] = num2;
                        num += num2;
                    }
                }
            }
            toggle.gameObject.SetActive(num > 0);
            this.realmPicks.Add(eRealm, dictionary);
            Dictionary<ERarity, List<Spell>> dictionary2 = new Dictionary<ERarity, List<Spell>>();
            foreach (KeyValuePair<ERarity, int> item in dictionary)
            {
                List<Spell> list2 = new List<Spell>(this.realmSpells[eRealm][item.Key]);
                List<Spell> list3 = new List<Spell>();
                for (int k = 0; k < item.Value; k++)
                {
                    int index = global::UnityEngine.Random.Range(0, list2.Count);
                    list3.Add(list2[index]);
                    list2.RemoveAt(index);
                }
                dictionary2.Add(item.Key, list3);
            }
            this.realmPickedSpells[eRealm] = dictionary2;
        }
        this.UpdateAttentions();
        array = this.realms;
        foreach (ERealm realm in array)
        {
            if (this.customWizardMode)
            {
                if (this.realmTabs[realm].isActiveAndEnabled)
                {
                    this.RealmTabSelected(realm);
                    break;
                }
            }
            else if (this.books.FindAll((Tag o) => o == (Tag)this.realmToTag[realm]).Count > 0)
            {
                this.RealmTabSelected(realm);
                break;
            }
        }
    }

    private void UpdateAttentions()
    {
        Dictionary<ERarity, int> dictionary = new Dictionary<ERarity, int>();
        foreach (ERarity key in this.rarityGrids.Keys)
        {
            dictionary[key] = 0;
        }
        ERealm[] array = this.realms;
        foreach (ERealm eRealm in array)
        {
            bool flag = false;
            foreach (ERarity key2 in this.rarityGrids.Keys)
            {
                int count = this.realmPickedSpells[eRealm][key2].Count;
                int num = this.realmPicks[eRealm][key2];
                dictionary[key2] += num - count;
                flag = flag || num > count;
                if (eRealm == this.currentRealm)
                {
                    this.rarityTitles[key2].text = global::DBUtils.Localization.Get(this.rarityToText[key2], true) + " " + count + "/" + num;
                    this.rarityAttention[key2].SetActive(num > count);
                }
            }
            this.realmAttention[eRealm].SetActive(flag);
        }
        StringBuilder stringBuilder = new StringBuilder(global::DBUtils.Localization.Get("UI_SPELLS_TO_PICK", true));
        int num2 = 0;
        bool flag2 = true;
        foreach (ERarity key3 in this.rarityGrids.Keys)
        {
            int num3 = dictionary[key3];
            if (num3 > 0)
            {
                if (flag2)
                {
                    flag2 = false;
                }
                else
                {
                    stringBuilder.Append(',');
                }
                stringBuilder.Append(' ');
                stringBuilder.Append(global::DBUtils.Localization.Get(this.rarityToText[key3], true));
                stringBuilder.Append(' ');
                num2 += num3;
                stringBuilder.Append(num3);
            }
        }
        stringBuilder.Append('.');
        this.textSpellPicks.text = stringBuilder.ToString();
        this.textSpellPicks.gameObject.SetActive(num2 > 0);
        this.allSpellsPicked = num2 == 0;
    }

    private void RealmTabSelected(ERealm realm)
    {
        if (realm != this.currentRealm && this.currentRealm != 0 && !this.CheckRealm())
        {
            this.realmTabs[this.currentRealm].isOn = true;
        }
        else
        {
            this.SelectRealm(realm);
        }
    }

    private void SelectRealm(ERealm realm, bool doCheck = true)
    {
        this.currentRealm = realm;
        if (!this.realmTabs[realm].isOn)
        {
            this.realmTabs[realm].isOn = true;
        }
        this.textDomainName.text = global::DBUtils.Localization.Get(this.realmToUIText[realm], true);
        foreach (ERarity key in this.rarityGrids.Keys)
        {
            this.UpdateRarity(key);
        }
        this.UpdateAttentions();
        int num = this.realmPicks[realm][ERarity.Uncommon];
        this.goUncommonInfo.SetActive(num == 0);
        num = this.realmPicks[realm][ERarity.Rare];
        this.goRareInfo.SetActive(num == 0);
    }

    private void UpdateRarity(ERarity rarity)
    {
        this.rarityGrids[rarity].UpdateGrid(this.realmSpells[this.currentRealm][rarity], rarity);
    }

    private void RaritySpellItem(GameObject itemSource, object source, object data, int index)
    {
        ERarity rarity = (ERarity)data;
        SimpleListItem component = itemSource.GetComponent<SimpleListItem>();
        Spell spell = (Spell)source;
        component.icon.texture = AssetManager.Get<Texture2D>(spell.GetDescriptionInfo().graphic);
        itemSource.GetComponent<RolloverSimpleTooltip>().sourceAsDbName = spell.dbName;
        Toggle component2 = itemSource.GetComponent<Toggle>();
        component2.onValueChanged.RemoveAllListeners();
        component2.isOn = this.realmPickedSpells[this.currentRealm][rarity].Contains(spell);
        component2.interactable = component2.isOn || this.realmPickedSpells[spell.realm][rarity].Count < this.realmPicks[spell.realm][rarity];
        component2.onValueChanged.AddListener(delegate(bool picked)
        {
            bool flag = this.realmPickedSpells[this.currentRealm][rarity].Contains(spell);
            if (picked != flag)
            {
                if (picked)
                {
                    this.realmPickedSpells[this.currentRealm][rarity].Add(spell);
                }
                else
                {
                    this.realmPickedSpells[this.currentRealm][rarity].Remove(spell);
                }
                this.RealmTabSelected(this.currentRealm);
            }
        });
        UIEffect componentInChildren = itemSource.GetComponentInChildren<UIEffect>();
        if ((bool)componentInChildren)
        {
            componentInChildren.enabled = !component2.interactable;
        }
        TextMeshProUGUI componentInChildren2 = itemSource.GetComponentInChildren<TextMeshProUGUI>();
        if ((bool)componentInChildren2)
        {
            componentInChildren2.text = spell.GetDescriptionInfo().GetLocalizedName();
        }
    }

    private bool CheckRealm()
    {
        if (this.currentRealm != 0 && this.realmAttention[this.currentRealm].activeInHierarchy)
        {
            PopupGeneral.OpenPopup(this, global::DBUtils.Localization.Get(this.realmToUIText[this.currentRealm], true), global::DBUtils.Localization.Get("UI_SELECT_MORE_SPELLS", true), global::DBUtils.Localization.Get("UI_OKAY", true));
            return false;
        }
        return true;
    }

    private bool BackSpells()
    {
        bool flag = false;
        for (int num = this.realms.Length - 1; num >= 0; num--)
        {
            ERealm eRealm = this.realms[num];
            if (flag && this.realmTabs[eRealm].isActiveAndEnabled)
            {
                this.SelectRealm(eRealm);
                return false;
            }
            if (eRealm == this.currentRealm)
            {
                flag = true;
            }
        }
        return true;
    }

    private void ConfirmWizard()
    {
        if (!this.CheckRealm())
        {
            return;
        }
        bool flag = false;
        for (int i = 0; i < this.realms.Length; i++)
        {
            ERealm eRealm = this.realms[i];
            if (this.realmAttention[eRealm].activeInHierarchy)
            {
                this.RealmTabSelected(eRealm);
                this.CheckRealm();
                return;
            }
            if (flag && this.realmTabs[eRealm].isActiveAndEnabled)
            {
                this.RealmTabSelected(eRealm);
                return;
            }
            if (eRealm == this.currentRealm)
            {
                flag = true;
            }
        }
        if (!this.allSpellsPicked)
        {
            return;
        }
        List<Tag> list = new List<Tag>();
        foreach (KeyValuePair<TAG, Slider> bookSlider in this.bookSliders)
        {
            for (int j = 0; (float)j < bookSlider.Value.value; j++)
            {
                list.Add((Tag)bookSlider.Key);
            }
        }
        List<Spell> list2 = new List<Spell>();
        foreach (KeyValuePair<ERealm, Dictionary<ERarity, List<Spell>>> realmPickedSpell in this.realmPickedSpells)
        {
            foreach (KeyValuePair<ERarity, List<Spell>> item in realmPickedSpell.Value)
            {
                list2.AddRange(item.Value);
            }
        }
        if (this.customWizardMode)
        {
            GameManager.SelectWizard(this.wizards[this.imageIndex], list, list2, this.selectedTraits, custom: true).name = this.inputWizardName.text;
        }
        else
        {
            GameManager.SelectWizard(this.wizards[this.imageIndex], list, list2, this.selectedTraits);
        }
        int num = 5;
        num = 1 + DifficultySettingsData.GetSettingAsInt("UI_AI_WIZARDS");
        if (num < 2)
        {
            num = 5;
        }
        GameManager.InitializeAllWizards(num);
        MHEventSystem.TriggerEvent(this, "Advance");
    }

    private void UpdateSpellList()
    {
        this.realmSpells.Clear();
        foreach (Spell item in DataBase.GetType<Spell>())
        {
            if (!this.RemoveSpellFromPick(item))
            {
                if (!this.realmSpells.TryGetValue(item.realm, out var value))
                {
                    value = new Dictionary<ERarity, List<Spell>>();
                    this.realmSpells.Add(item.realm, value);
                }
                if (!value.TryGetValue(item.rarity, out var value2))
                {
                    value2 = new List<Spell>();
                    value.Add(item.rarity, value2);
                }
                value2.Add(item);
            }
        }
    }

    private bool RemoveSpellFromPick(DBClass spell)
    {
        if (this.selectedTraits != null && this.selectedTraits.Count > 0)
        {
            foreach (Trait selectedTrait in this.selectedTraits)
            {
                if (selectedTrait.startingSpells == null || selectedTrait.startingSpells.Length == 0)
                {
                    continue;
                }
                string[] startingSpells = selectedTrait.startingSpells;
                for (int i = 0; i < startingSpells.Length; i++)
                {
                    DBClass dBClass = DataBase.Get(startingSpells[i], reportMissing: false);
                    if (spell == dBClass)
                    {
                        return true;
                    }
                }
            }
        }
        else if (this.fixedTraits != null && this.fixedTraits.Count > 0)
        {
            foreach (Trait fixedTrait in this.fixedTraits)
            {
                if (fixedTrait.startingSpells == null || fixedTrait.startingSpells.Length == 0)
                {
                    continue;
                }
                string[] startingSpells = fixedTrait.startingSpells;
                for (int i = 0; i < startingSpells.Length; i++)
                {
                    DBClass dBClass2 = DataBase.Get(startingSpells[i], reportMissing: false);
                    if (spell == dBClass2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
