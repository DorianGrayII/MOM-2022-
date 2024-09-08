// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.MainMenu
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : ScreenBase
{
    private static bool clearedDebug;

    public Button btContinue;

    public Button btNewGame;

    public Button btEditor;

    public Button btHallOfFame;

    public Button btExit;

    public Button btSelectLanguage;

    public Button btMods;

    public Toggle tgDLC0;

    public Toggle tgDLC1;

    public Toggle tgDLC2;

    public Toggle tgDLC3;

    public RawImage languageFlag;

    public TextMeshProUGUI version;

    public Button btSelectLanguageCancel;

    public GameObject languageSelection;

    public GameObject dlcList;

    public CanvasGroup languageCanvas;

    public GridItemManager languageSelectionGrid;

    private Coroutine langSelectorAnim;

    public const string languageID = "Language";

    private Dictionary<TextMeshProUGUI, string> defaultString;

    private bool achievementTesting;

    public override IEnumerator PreStart()
    {
        this.version.text = GameVersion.GetGameVersionFull();
        yield return base.PreStart();
        this.btContinue.interactable = SaveManager.IsAnySaveAvailable();
        PlayMusic.Play("SOUND_LIST-MAIN_MENU", this);
        if (!MainMenu.clearedDebug)
        {
            MHZombieMemoryDetector.Clear();
            MainMenu.clearedDebug = true;
        }
        this.InitializeLanguageSelection();
        string text = (PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetString("Language") : null);
        if (text == null)
        {
            if (this.langSelectorAnim != null)
            {
                base.StopCoroutine(this.langSelectorAnim);
            }
            this.langSelectorAnim = base.StartCoroutine(this.OpenLanguageSelection());
        }
        else
        {
            DBClass dBClass = DataBase.Get(text, reportMissing: false);
            this.SetFlag(dBClass as Language);
        }
        if (Integration.IsReady())
        {
            Debug.Log("Platform integration ready for: " + Integration.GetName());
        }
        else
        {
            Debug.Log("Platform integration not ready");
        }
        this.dlcList.SetActive(value: true);
        if (PlayerPrefs.HasKey("UseDLC"))
        {
            int @int = PlayerPrefs.GetInt("UseDLC");
            this.tgDLC0.isOn = (@int & 0x40000000) > 0;
            this.tgDLC1.isOn = (@int & 1) > 0;
            this.tgDLC2.isOn = (@int & 2) > 0;
            this.tgDLC3.isOn = (@int & 4) > 0;
            DataBase.UpdateUse(@int, forced: true);
        }
        else
        {
            PlayerPrefs.SetInt("UseDLC", 1);
            PlayerPrefs.Save();
            DataBase.UpdateUse(-1, forced: true);
            this.tgDLC0.isOn = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc0);
            this.tgDLC1.isOn = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc1);
            this.tgDLC2.isOn = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc2);
            this.tgDLC3.isOn = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc3);
        }
        this.tgDLC0.interactable = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc0);
        this.tgDLC1.interactable = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc1);
        this.tgDLC2.interactable = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc2);
        this.tgDLC3.interactable = DLCManager.IsDlcOwned(DLCManager.DLCs.Dlc3);
        this.tgDLC0.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetInt("UseDLC", this.BuildDlcCode());
            PlayerPrefs.Save();
            DataBase.UpdateUse(-1, forced: true);
        });
        this.tgDLC1.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetInt("UseDLC", this.BuildDlcCode());
            PlayerPrefs.Save();
            DataBase.UpdateUse(-1, forced: true);
        });
        this.tgDLC2.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetInt("UseDLC", this.BuildDlcCode());
            PlayerPrefs.Save();
            DataBase.UpdateUse(-1, forced: true);
        });
        this.tgDLC3.onValueChanged.AddListener(delegate
        {
            PlayerPrefs.SetInt("UseDLC", this.BuildDlcCode());
            PlayerPrefs.Save();
            DataBase.UpdateUse(-1, forced: true);
        });
    }

    private int BuildDlcCode()
    {
        return 0 | (this.tgDLC0.isOn ? 1073741824 : 0) | (this.tgDLC1.isOn ? 1 : 0) | (this.tgDLC2.isOn ? 2 : 0) | (this.tgDLC3.isOn ? 4 : 0);
    }

    protected override void Start()
    {
        this.defaultString = new Dictionary<TextMeshProUGUI, string>();
        TextMeshProUGUI[] componentsInChildren = base.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren)
        {
            if (!(textMeshProUGUI == this.version) && (textMeshProUGUI.text.StartsWith("UI_") || textMeshProUGUI.text.StartsWith("DES_")))
            {
                this.defaultString[textMeshProUGUI] = textMeshProUGUI.text;
            }
        }
        base.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && Input.GetKeyDown(KeyCode.L) && Input.GetKeyDown(KeyCode.I))
        {
            this.achievementTesting = true;
        }
        if (this.achievementTesting && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                AchievementManager.Progress(AchievementManager.Achievement.WarmasterOfMagic);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                AchievementManager.Progress(AchievementManager.Achievement.MasterArcheologist);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                AchievementManager.Progress(AchievementManager.Achievement.NodeMaster);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                AchievementManager.Progress(AchievementManager.Achievement.WizardSupreme);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                AchievementManager.Progress(AchievementManager.Achievement.LookInTheMyrror);
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                AchievementManager.Progress(AchievementManager.Achievement.TasteOfDespair);
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                AchievementManager.Progress(AchievementManager.Achievement.CleansingTheLand);
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                AchievementManager.Progress(AchievementManager.Achievement.PowerOfKnowledge);
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                AchievementManager.Progress(AchievementManager.Achievement.JustGettingStarted);
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                AchievementManager.Progress(AchievementManager.Achievement.SpellmasterOfMagic);
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Backspace))
            {
                Integration.ResetAchievements();
            }
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btExit)
        {
            MHEventSystem.TriggerEvent(this, "ButtonExitAccepted");
        }
        if (s == this.btSelectLanguage)
        {
            if (this.langSelectorAnim != null)
            {
                base.StopCoroutine(this.langSelectorAnim);
            }
            this.langSelectorAnim = base.StartCoroutine(this.OpenLanguageSelection());
        }
        else if (s == this.btNewGame)
        {
            DataBase.UpdateUse();
        }
        else if (s == this.btMods)
        {
            UIManager.Open<Mods>(UIManager.Layer.Standard);
        }
    }

    private void InitializeLanguageSelection()
    {
        this.languageSelectionGrid.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            Language lng = source as Language;
            IconAndNameListItem component = itemSource.GetComponent<IconAndNameListItem>();
            component.icon.texture = lng.GetDescriptionInfo().GetTexture();
            component.label.text = lng.GetDescriptionInfo().GetName();
            itemSource.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (this.languageCanvas.interactable)
                {
                    global::DBUtils.Localization.LoadLibraryByName(lng.languageID);
                    this.StartClosingLanguageSelection();
                    this.SetFlag(lng);
                    PlayerPrefs.SetString("Language", lng.dbName);
                    PlayerPrefs.Save();
                    this.ReloadScreen();
                }
            });
        });
        this.languageSelectionGrid.UpdateGrid(DataBase.GetType<Language>());
        this.btSelectLanguageCancel.interactable = PlayerPrefs.HasKey("Language");
        this.btSelectLanguageCancel.onClick.AddListener(delegate
        {
            this.StartClosingLanguageSelection();
        });
    }

    private void StartClosingLanguageSelection()
    {
        if (this.langSelectorAnim != null)
        {
            base.StopCoroutine(this.langSelectorAnim);
        }
        base.StartCoroutine(this.CloseLanguageSelection());
    }

    private IEnumerator OpenLanguageSelection()
    {
        this.languageCanvas.alpha = 0f;
        this.languageCanvas.blocksRaycasts = true;
        this.languageSelection.SetActive(value: true);
        for (int i = 0; i <= 10; i++)
        {
            this.languageCanvas.alpha = 0.1f * (float)i;
            yield return null;
        }
        this.languageCanvas.interactable = true;
    }

    private IEnumerator CloseLanguageSelection()
    {
        this.languageCanvas.alpha = 1f;
        this.languageCanvas.interactable = false;
        for (int i = 0; i <= 10; i++)
        {
            this.languageCanvas.alpha = 1f - 0.1f * (float)i;
            yield return null;
        }
        this.languageSelection.SetActive(value: false);
    }

    private void SetFlag(Language lng)
    {
        if (lng != null)
        {
            Texture2D texture = lng.GetDescriptionInfo().GetTexture();
            this.languageFlag.texture = texture;
        }
    }

    private void ReloadScreen()
    {
        if (this.defaultString == null)
        {
            return;
        }
        foreach (KeyValuePair<TextMeshProUGUI, string> item in this.defaultString)
        {
            item.Key.text = global::DBUtils.Localization.Get(item.Value, true);
        }
    }
}
