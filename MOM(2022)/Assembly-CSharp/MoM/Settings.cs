// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Settings
using System;
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : ScreenBase
{
    public enum Name
    {
        worldQualitySetting = 0,
        refreshCap = 1,
        musicVolume = 2,
        sfxVolume = 3,
        autoSaveFrequency = 4,
        aiAnimationSpeed = 5,
        playerAnimationSpeed = 6,
        experimentalLoading = 7,
        arcanusGamma = 8,
        myrrorGamma = 9
    }

    public enum WorldQualitySetting
    {
        High = 0,
        Medium = 1,
        Low = 2
    }

    public enum RefreshCap
    {
        UI_REFRESH_CAP_160 = 0,
        UI_REFRESH_CAP_144 = 1,
        UI_REFRESH_CAP_120 = 2,
        UI_REFRESH_CAP_60 = 3,
        UI_REFRESH_CAP_30 = 4,
        UI_REFRESH_CAP_NONE = 5
    }

    public enum KeyActions
    {
        None = 0,
        UI_NEXT_UNIT = 1,
        UI_SHOW_HIDE_SURVEYOR = 2,
        UI_END_TURN2 = 3,
        UI_MOVE_CAMERA_NORTH = 4,
        UI_MOVE_CAMERA_WEST = 5,
        UI_MOVE_CAMERA_SOUTH = 6,
        UI_MOVE_CAMERA_EAST = 7,
        UI_CHANGE_PLANE = 8,
        UI_OPEN_RESEARCH = 9,
        UI_OPEN_PAUSE_MENU = 10,
        UI_BUILD_OUTPOST2 = 11,
        UI_BUILD_ROAD2 = 12,
        UI_TOGGLE_GUARD2 = 13,
        UI_TOGGLE_SKIP2 = 14,
        UI_CONTINUE_MOVEMENT = 15,
        UI_COMBAT_LOGBOOK = 16,
        UI_CREATE_BUG_REPORT = 17,
        UI_OPEN_WIZARD_INFO = 18,
        UI_OPEN_CITY_MANAGER = 19,
        UI_OPEN_ARMY_MANAGER = 20,
        UI_OPEN_DIPLOMACY = 21,
        UI_OPEN_MAGIC = 22,
        UI_CAST_SPELLS4 = 23,
        UI_ZOOM_IN = 24,
        UI_ZOOM_OUT = 25,
        UI_PURIFY2 = 26,
        UI_MELD_WITH_NODE2 = 27,
        UI_OPEN_CARTOGRAPHER = 28,
        UI_QUICK_SAVE = 29,
        UI_QUICK_LOAD = 30,
        UI_SHOW_HIDE_RESOURCES = 31
    }

    public static int[] autoSaveFrequencies = new int[6] { 0, 1, 2, 3, 5, 10 };

    public static Dictionary<KeyActions, KeyCode> defaultMapping = new Dictionary<KeyActions, KeyCode>
    {
        [KeyActions.UI_NEXT_UNIT] = KeyCode.Space,
        [KeyActions.UI_SHOW_HIDE_SURVEYOR] = KeyCode.Tab,
        [KeyActions.UI_END_TURN2] = KeyCode.Return,
        [KeyActions.UI_MOVE_CAMERA_NORTH] = KeyCode.W,
        [KeyActions.UI_MOVE_CAMERA_WEST] = KeyCode.A,
        [KeyActions.UI_MOVE_CAMERA_SOUTH] = KeyCode.S,
        [KeyActions.UI_MOVE_CAMERA_EAST] = KeyCode.D,
        [KeyActions.UI_CHANGE_PLANE] = KeyCode.P,
        [KeyActions.UI_OPEN_RESEARCH] = KeyCode.R,
        [KeyActions.UI_OPEN_PAUSE_MENU] = KeyCode.Escape,
        [KeyActions.UI_BUILD_OUTPOST2] = KeyCode.O,
        [KeyActions.UI_BUILD_ROAD2] = KeyCode.Q,
        [KeyActions.UI_PURIFY2] = KeyCode.I,
        [KeyActions.UI_MELD_WITH_NODE2] = KeyCode.N,
        [KeyActions.UI_TOGGLE_GUARD2] = KeyCode.G,
        [KeyActions.UI_TOGGLE_SKIP2] = KeyCode.F,
        [KeyActions.UI_CONTINUE_MOVEMENT] = KeyCode.M,
        [KeyActions.UI_COMBAT_LOGBOOK] = KeyCode.L,
        [KeyActions.UI_CREATE_BUG_REPORT] = KeyCode.F8,
        [KeyActions.UI_OPEN_WIZARD_INFO] = KeyCode.Alpha1,
        [KeyActions.UI_OPEN_CITY_MANAGER] = KeyCode.Alpha2,
        [KeyActions.UI_OPEN_ARMY_MANAGER] = KeyCode.Alpha3,
        [KeyActions.UI_OPEN_MAGIC] = KeyCode.Alpha4,
        [KeyActions.UI_OPEN_DIPLOMACY] = KeyCode.Alpha5,
        [KeyActions.UI_CAST_SPELLS4] = KeyCode.C,
        [KeyActions.UI_ZOOM_IN] = KeyCode.PageDown,
        [KeyActions.UI_ZOOM_OUT] = KeyCode.PageUp,
        [KeyActions.UI_OPEN_CARTOGRAPHER] = KeyCode.K,
        [KeyActions.UI_QUICK_SAVE] = KeyCode.F5,
        [KeyActions.UI_QUICK_LOAD] = KeyCode.F6,
        [KeyActions.UI_SHOW_HIDE_RESOURCES] = KeyCode.Z
    };

    private static SettingsBlock dataBlock;

    private static int meshQuality;

    public Button btClose;

    public Button btResetTutorials;

    public Button btDisableTutorials;

    public Button btCancelAssignHotkey;

    public Button btDefaultHotkeys;

    public DropDownFilters ddWorldQuality;

    public DropDownFilters ddResolutionOptions;

    public DropDownFilters ddRefreshRate;

    public DropDownFilters ddAutoSaveFrequency;

    public DropDownFilters ddAIAnimationSpeed;

    public DropDownFilters ddPlayerAnimationSpeed;

    public DropDownFilters ddDisplay;

    public Slider sliderMusicVolume;

    public Slider sliderSfxVolume;

    public Slider sliderArcanusGamma;

    public Slider sliderMyrrorGamma;

    public Toggle toggleFollowEnemyMovement;

    public Toggle toggleFocusOnUnits;

    public Toggle toggleAutoEndTurn;

    public Toggle toggleEdgescrolling;

    public Toggle toggleAutoNextUnit;

    public Toggle toggleFasterLoading;

    public Toggle toggleTownMapAlwaysOn;

    public Toggle toggleFullscreen;

    public GameObject goArcanusGammaPreview;

    public GameObject goMyrrorGammaPreview;

    public CanvasGroup cgArcanusGammaPreview;

    public CanvasGroup cgMyrrorGammaPreview;

    public GridItemManager gridHotkeys;

    private bool gridInitialized;

    public GameObject goReadKey;

    private KeyActions readKeyAction;

    private bool dirty;

    private bool inGameMode;

    public static void Popup(State parent)
    {
        UIManager.Open<Settings>(UIManager.Layer.Popup, parent).inGameMode = true;
    }

    public static bool IsLoaded()
    {
        return Settings.dataBlock != null;
    }

    public static SettingsBlock GetData()
    {
        if (Settings.dataBlock == null)
        {
            Settings.dataBlock = SettingsBlock.Load();
        }
        return Settings.dataBlock;
    }

    public static void SetGameQualitySettings()
    {
        string[] names = QualitySettings.names;
        string q = Settings.GetData().Get<string>(Name.worldQualitySetting);
        int num = Array.FindIndex(names, (string o) => o == q);
        if (num > -1)
        {
            QualitySettings.SetQualityLevel(num, applyExpensiveChanges: true);
        }
    }

    public static int GetMeshQuality(bool force = false)
    {
        if (force || Settings.meshQuality < 1)
        {
            _ = QualitySettings.names;
            string text = Settings.GetData().Get<string>(Name.worldQualitySetting);
            if (text == WorldQualitySetting.Medium.ToString())
            {
                Settings.meshQuality = 6;
            }
            else if (text == WorldQualitySetting.Low.ToString())
            {
                Settings.meshQuality = 4;
            }
            else
            {
                Settings.meshQuality = 8;
            }
        }
        return Settings.meshQuality;
    }

    public override IEnumerator PreStart()
    {
        this.goArcanusGammaPreview.SetActive(value: false);
        this.goMyrrorGammaPreview.SetActive(value: false);
        this.InitializeVideoTab();
        this.InitializeAudioTab();
        this.UpdateOtherTab();
        yield return base.PreStart();
    }

    public void InitializeVideoTab()
    {
        string selectedOption = Settings.GetData().Get<string>(Name.worldQualitySetting);
        UIComponentFill.LinkDropdownEnum<WorldQualitySetting>(this.ddWorldQuality, delegate(object option)
        {
            Settings.dataBlock.Set(Name.worldQualitySetting, option as string);
            this.dirty = true;
        }, acceptDefaultOption: true, selectedOption, localize: true);
        UIComponentFill.LinkDropdownCustom(this.ddResolutionOptions, ListValidResolutions, null, CurrentResolutions, ResolutionsChanged);
        selectedOption = Settings.GetData().Get<string>(Name.refreshCap);
        UIComponentFill.LinkDropdownEnum<RefreshCap>(this.ddRefreshRate, delegate(object option)
        {
            Settings.dataBlock.Set(Name.refreshCap, option as string);
            Settings.ApplyVisualSettings();
            this.dirty = true;
        }, acceptDefaultOption: true, selectedOption, localize: true);
        UIComponentFill.LinkDropdownCustom(this.ddDisplay, ListValidDisplayMonitors, null, CurrentDisplayMonitor, ScreenChanged);
        this.toggleFullscreen.onValueChanged.RemoveAllListeners();
        this.toggleFullscreen.isOn = Screen.fullScreen;
        this.toggleFullscreen.onValueChanged.AddListener(delegate
        {
            this.ToggleFullScreen();
            Settings.ApplyVisualSettings();
            this.dirty = true;
        });
        int num = Settings.GetData().Get<int>(Name.arcanusGamma);
        this.sliderArcanusGamma.onValueChanged.RemoveAllListeners();
        this.sliderArcanusGamma.value = num;
        this.sliderArcanusGamma.onValueChanged.AddListener(delegate(float v)
        {
            int num4 = Mathf.RoundToInt(v);
            PosProcessingLibrary.ArcanusGamma(num4);
            this.cgArcanusGammaPreview.alpha = (float)num4 * 0.01f;
            Settings.dataBlock.Set(Name.arcanusGamma, num4.ToString());
            this.dirty = true;
        });
        int num2 = Settings.GetData().Get<int>(Name.myrrorGamma);
        this.sliderMyrrorGamma.onValueChanged.RemoveAllListeners();
        this.sliderMyrrorGamma.value = num2;
        this.sliderMyrrorGamma.onValueChanged.AddListener(delegate(float v)
        {
            int num3 = Mathf.RoundToInt(v);
            PosProcessingLibrary.MyrrorGamma(num3);
            this.cgMyrrorGammaPreview.alpha = (float)num3 * 0.01f;
            Settings.dataBlock.Set(Name.myrrorGamma, num3.ToString());
            this.dirty = true;
        });
    }

    public static void ApplyVisualSettings()
    {
        string text = Settings.GetData().Get<string>(Name.refreshCap);
        if (text == RefreshCap.UI_REFRESH_CAP_160.ToString())
        {
            Application.targetFrameRate = 160;
        }
        else if (text == RefreshCap.UI_REFRESH_CAP_144.ToString())
        {
            Application.targetFrameRate = 144;
        }
        else if (text == RefreshCap.UI_REFRESH_CAP_120.ToString())
        {
            Application.targetFrameRate = 120;
        }
        else if (text == RefreshCap.UI_REFRESH_CAP_60.ToString())
        {
            Application.targetFrameRate = 60;
        }
        else if (text == RefreshCap.UI_REFRESH_CAP_30.ToString())
        {
            Application.targetFrameRate = 30;
        }
        else if (text == RefreshCap.UI_REFRESH_CAP_NONE.ToString())
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = 60;
        }
    }

    public void InitializeAudioTab()
    {
        int num = Settings.GetData().Get<int>(Name.musicVolume);
        this.sliderMusicVolume.value = num;
        this.sliderMusicVolume.onValueChanged.AddListener(delegate(float v)
        {
            Settings.dataBlock.Set(Name.musicVolume, ((int)v).ToString());
            this.dirty = true;
        });
        num = Settings.GetData().Get<int>(Name.sfxVolume);
        this.sliderSfxVolume.value = num;
        this.sliderSfxVolume.onValueChanged.AddListener(delegate(float v)
        {
            Settings.dataBlock.Set(Name.sfxVolume, ((int)v).ToString());
            this.dirty = true;
        });
    }

    public void InitializeKeyMapTab()
    {
        GameObjectUtils.FindByNameGetComponent<Button>(this.goReadKey, "ButtonCancel").onClick.AddListener(delegate
        {
            this.goReadKey.SetActive(value: false);
        });
        this.gridHotkeys.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            HotkeyListItem component = itemSource.GetComponent<HotkeyListItem>();
            Multitype<KeyActions, KeyCode> d = source as Multitype<KeyActions, KeyCode>;
            component.labelFunction.text = Localization.Get(d.t0.ToString(), true);
            string text = ((d.t1 == KeyCode.None) ? Localization.Get("UI_NOT_ASSIGNED", true) : d.t1.ToString());
            component.labelHotkey.text = text;
            component.btReadHotkey.onClick.RemoveAllListeners();
            component.btReadHotkey.onClick.AddListener(delegate
            {
                this.goReadKey.SetActive(value: true);
                GameObjectUtils.FindByNameGetComponent<TextMeshProUGUI>(this.goReadKey, "LabelFunction2").text = Localization.Get(d.t0.ToString(), true);
                this.readKeyAction = d.t0;
            });
        }, UpdateKeyMap);
        this.UpdateKeyMap();
    }

    private void UpdateKeyMap()
    {
        List<Multitype<KeyActions, KeyCode>> list = new List<Multitype<KeyActions, KeyCode>>();
        foreach (KeyValuePair<KeyActions, KeyCode> item2 in Settings.defaultMapping)
        {
            Multitype<KeyActions, KeyCode> item = new Multitype<KeyActions, KeyCode>(item2.Key, SettingsBlock.GetKeyForAction(item2.Key));
            list.Add(item);
        }
        this.gridHotkeys.UpdateGrid(list);
    }

    public void UpdateOtherTab()
    {
        this.btResetTutorials.interactable = Settings.GetData().HavePrefix("TUT_");
        this.btDisableTutorials.interactable = !Settings.GetData().Get<bool>("TUT_HIDEALL");
        int autosaveFrequency = Settings.GetAutosaveFrequency();
        string text = null;
        List<string> options = new List<string>();
        int[] array = Settings.autoSaveFrequencies;
        foreach (int num in array)
        {
            switch (num)
            {
            case 0:
                options.Add(Localization.Get("UI_AUTOSAVE_FREQUENCY_OFF", true));
                break;
            case 1:
                options.Add(Localization.Get("UI_AUTOSAVE_FREQUENCY_ONE", true));
                break;
            default:
                options.Add(Localization.Get("UI_AUTOSAVE_FREQUENCY", true, num));
                break;
            }
            if (autosaveFrequency == num)
            {
                text = options[options.Count - 1];
            }
        }
        if (text == null)
        {
            text = options[0];
        }
        this.ddAutoSaveFrequency.onChange = null;
        this.ddAutoSaveFrequency.SetOptions(options, doUpdate: false, localize: false);
        this.ddAutoSaveFrequency.SelectOption(text, fallbackToFirst: true);
        this.ddAutoSaveFrequency.onChange = delegate(object o)
        {
            int num4 = options.FindIndex((string o2) => o2 == o);
            if (num4 >= 0)
            {
                Settings.dataBlock.Set(Name.autoSaveFrequency, Settings.autoSaveFrequencies[num4].ToString());
                this.dirty = true;
            }
        };
        autosaveFrequency = Settings.GetAiAnimationSpeed();
        text = null;
        List<string> options2 = new List<string>();
        for (int j = 1; j <= 3; j++)
        {
            string item = j.ToString();
            options2.Add(item);
            if (autosaveFrequency == j)
            {
                text = options2[options2.Count - 1];
            }
        }
        if (text == null)
        {
            text = options2[0];
        }
        this.ddAIAnimationSpeed.onChange = null;
        this.ddAIAnimationSpeed.SetOptions(options2, doUpdate: false, localize: false);
        this.ddAIAnimationSpeed.SelectOption(text, fallbackToFirst: true);
        this.ddAIAnimationSpeed.onChange = delegate(object o)
        {
            int num3 = options2.FindIndex((string o2) => o2 == o);
            if (num3 >= 0)
            {
                Settings.dataBlock.Set(Name.aiAnimationSpeed, options2[num3]);
                this.dirty = true;
            }
        };
        List<string> optionsP = new List<string>();
        optionsP.Add("1");
        optionsP.Add("2");
        optionsP.Add("4");
        optionsP.Add("UI_INSTANT");
        autosaveFrequency = Settings.GetPlayerAnimationSpeedIndex();
        text = ((optionsP.Count <= autosaveFrequency) ? optionsP[0] : optionsP[autosaveFrequency]);
        this.ddPlayerAnimationSpeed.onChange = null;
        this.ddPlayerAnimationSpeed.SetOptions(optionsP, doUpdate: false);
        this.ddPlayerAnimationSpeed.SelectOption(Localization.SimpleGet(text), fallbackToFirst: true);
        this.ddPlayerAnimationSpeed.onChange = delegate(object o)
        {
            int num2 = optionsP.FindIndex((string o2) => o2 == o);
            if (num2 >= 0)
            {
                Settings.dataBlock.Set(Name.playerAnimationSpeed, num2.ToString());
                Settings.GetData().UpdateAnimationSpeed();
                this.dirty = true;
            }
        };
        bool followAIMovement = Settings.GetData().GetFollowAIMovement();
        this.toggleFollowEnemyMovement.onValueChanged.RemoveAllListeners();
        this.toggleFollowEnemyMovement.isOn = followAIMovement;
        this.toggleFollowEnemyMovement.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetFollowEnemyMovement(active);
            this.dirty = true;
        });
        bool battleCameraFollow = Settings.GetData().GetBattleCameraFollow();
        this.toggleFocusOnUnits.onValueChanged.RemoveAllListeners();
        this.toggleFocusOnUnits.isOn = battleCameraFollow;
        this.toggleFocusOnUnits.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetBattleCameraFollow(active);
            this.dirty = true;
        });
        bool autoEndTurn = Settings.GetData().GetAutoEndTurn();
        this.toggleAutoEndTurn.onValueChanged.RemoveAllListeners();
        this.toggleAutoEndTurn.isOn = autoEndTurn;
        this.toggleAutoEndTurn.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetAutoEndTurn(active);
            this.dirty = true;
        });
        bool edgescrolling = Settings.GetData().GetEdgescrolling();
        this.toggleEdgescrolling.onValueChanged.RemoveAllListeners();
        this.toggleEdgescrolling.isOn = edgescrolling;
        this.toggleEdgescrolling.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetEdgescrolling(active);
            this.dirty = true;
        });
        bool autoNextUnit = Settings.GetData().GetAutoNextUnit();
        this.toggleAutoNextUnit.onValueChanged.RemoveAllListeners();
        this.toggleAutoNextUnit.isOn = autoNextUnit;
        this.toggleAutoNextUnit.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetAutoNextUnit(active);
            this.dirty = true;
        });
        bool isOn = Settings.GetData().GetExperimentalLoading() == 2;
        this.toggleFasterLoading.onValueChanged.RemoveAllListeners();
        this.toggleFasterLoading.isOn = isOn;
        this.toggleFasterLoading.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetExperimentalLoading((!active) ? 1 : 2);
            this.dirty = true;
        });
        bool townMap = Settings.GetData().GetTownMap();
        this.toggleTownMapAlwaysOn.onValueChanged.RemoveAllListeners();
        this.toggleTownMapAlwaysOn.isOn = townMap;
        this.toggleTownMapAlwaysOn.onValueChanged.AddListener(delegate(bool active)
        {
            Settings.dataBlock.SetTownMap(active);
            this.dirty = true;
        });
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (!this.gridInitialized && this.gridHotkeys.gameObject.activeInHierarchy)
        {
            this.InitializeKeyMapTab();
            this.gridInitialized = true;
        }
        if (!this.goReadKey.activeSelf)
        {
            return;
        }
        if (!this.goReadKey.activeInHierarchy)
        {
            this.goReadKey.SetActive(value: false);
            return;
        }
        KeyCode[] array = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        foreach (KeyCode keyCode in array)
        {
            if (keyCode != KeyCode.Mouse0 && keyCode != KeyCode.Mouse1 && keyCode != KeyCode.Mouse3 && Input.GetKeyDown(keyCode))
            {
                Settings.dataBlock.SetKey(this.readKeyAction, keyCode);
                this.goReadKey.SetActive(value: false);
                this.UpdateKeyMap();
            }
        }
    }

    public static int GetAutosaveFrequency()
    {
        string text = Settings.GetData().Get<string>(Name.autoSaveFrequency);
        if (text == null)
        {
            text = "1";
        }
        return int.Parse(text);
    }

    public static int GetPlayerAnimationSpeedIndex()
    {
        int b = Settings.GetData().Get<int>(Name.playerAnimationSpeed);
        return Mathf.Max(0, b);
    }

    public static int GetAiAnimationSpeed()
    {
        int b = Settings.GetData().Get<int>(Name.aiAnimationSpeed);
        return Mathf.Max(1, b);
    }

    public static int GetMusicVolume()
    {
        return Settings.GetData().Get<int>(Name.musicVolume);
    }

    public static int GetSFXVolume()
    {
        return Settings.GetData().Get<int>(Name.sfxVolume);
    }

    public static int GetArcanusGamma()
    {
        return Settings.GetData().Get<int>(Name.arcanusGamma);
    }

    public static int GetMyrrorGamma()
    {
        return Settings.GetData().Get<int>(Name.myrrorGamma);
    }

    private object ListValidDisplayMonitors(object data)
    {
        Display[] validDisplayMonitors = Settings.GetValidDisplayMonitors();
        List<string> list = new List<string>();
        for (int i = 0; i < validDisplayMonitors.Length; i++)
        {
            list.Add(Localization.Get("UI_DISPLAY " + i, true));
        }
        return list;
    }

    private object CurrentDisplayMonitor(object data)
    {
        int num = 0;
        if (PlayerPrefs.HasKey("UnitySelectMonitor"))
        {
            num = PlayerPrefs.GetInt("UnitySelectMonitor");
        }
        Display[] validDisplayMonitors = Settings.GetValidDisplayMonitors();
        if (validDisplayMonitors == null || num < 0 || num >= validDisplayMonitors.Length)
        {
            return 0;
        }
        Debug.Log("Current display: " + num);
        return num;
    }

    private void ScreenChanged(object data)
    {
        List<string> list = this.ListValidDisplayMonitors(null) as List<string>;
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = Localization.Get(list[i], true);
        }
        int num = list.IndexOf(data as string);
        if (num == -1)
        {
            Debug.LogWarning("Screen selection change invalid " + data);
            return;
        }
        PlayerPrefs.SetInt("UnitySelectMonitor", num);
        PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_CHANGE_NEED_RESTART", "UI_OK");
    }

    private object ListValidResolutions(object data)
    {
        Resolution[] validResolutions = Settings.GetValidResolutions();
        List<string> list = new List<string>();
        for (int i = 0; i < validResolutions.Length; i++)
        {
            list.Add(validResolutions[i].width + " x " + validResolutions[i].height + " @" + validResolutions[i].refreshRate);
        }
        return list;
    }

    private object CurrentResolutions(object data)
    {
        Resolution curent = Settings.GetResolution();
        Resolution[] validResolutions = Settings.GetValidResolutions();
        if (validResolutions == null)
        {
            return 0;
        }
        return Array.FindIndex(validResolutions, (Resolution o) => o.width == curent.width && o.height == curent.height && o.refreshRate == curent.refreshRate);
    }

    private void ResolutionsChanged(object data)
    {
        string text = data as string;
        Resolution[] validResolutions = Settings.GetValidResolutions();
        new List<string>();
        Resolution r = validResolutions[0];
        bool flag = false;
        for (int i = 0; i < validResolutions.Length; i++)
        {
            if (validResolutions[i].width + " x " + validResolutions[i].height + " @" + validResolutions[i].refreshRate == text)
            {
                r = validResolutions[i];
                flag = true;
            }
        }
        if (flag)
        {
            Settings.SetResolution(r, Screen.fullScreen);
            Settings.dataBlock.Set("Resolution W ", r.width.ToString());
            Settings.dataBlock.Set("Resolution H ", r.height.ToString());
            Settings.dataBlock.Set("Resolution Hz ", r.refreshRate.ToString());
            this.dirty = true;
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btClose)
        {
            if (this.dirty)
            {
                Settings.meshQuality = 0;
                SettingsBlock.Save(Settings.dataBlock);
            }
            Settings.SetGameQualitySettings();
            MHEventSystem.TriggerEvent(this, "FINISHED");
            UIManager.Close(this);
        }
        else if (s == this.btResetTutorials)
        {
            Settings.dataBlock.ClearPrefix("TUT_");
            SettingsBlock.Save(Settings.dataBlock);
            this.UpdateOtherTab();
        }
        else if (s == this.btDisableTutorials)
        {
            Settings.GetData().Set("TUT_HIDEALL", "true");
            SettingsBlock.Save(Settings.dataBlock);
            this.UpdateOtherTab();
        }
        else if (s == this.btDefaultHotkeys)
        {
            Settings.dataBlock.curentKeyMapping = null;
            this.UpdateKeyMap();
            SettingsBlock.Save(Settings.dataBlock);
        }
    }

    public static Display[] GetValidDisplayMonitors()
    {
        return Display.displays;
    }

    public static Display GetDisplayMonitor()
    {
        return Display.main;
    }

    public static void SetDisplayMonitor(Display d)
    {
        d?.Activate();
    }

    public static Resolution[] GetValidResolutions()
    {
        return Screen.resolutions;
    }

    public static Resolution GetResolution()
    {
        return Screen.currentResolution;
    }

    public static void SetResolution(Resolution r, bool fullScreen)
    {
        Screen.SetResolution(r.width, r.height, fullScreen);
    }

    private void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
