// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.UI.UIManager
using System;
using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnrealByte.EasyJira;

public class UIManager : StateMachine
{
    public enum Layer
    {
        TopLayer = 0,
        UIMarkersResources = 1,
        UIMarkers = 2,
        NonBlocking = 3,
        HUD = 4,
        Standard = 5,
        Popup = 6,
        MAX = 7
    }

    public GameObject layerSource;

    public List<GameObject> screens = new List<GameObject>();

    private ScreenBase topLayerForInputFrame;

    private bool exiting;

    public static UIManager instance;

    private Dictionary<Layer, HashSet<ScreenBase>> openScreensByLayer = new Dictionary<Layer, HashSet<ScreenBase>>();

    private Dictionary<Layer, GameObject> layerRoots = new Dictionary<Layer, GameObject>();

    private List<ScreenBase> interactionStackForKeyboard = new List<ScreenBase>();

    private bool inputConsumed;

    public static UIManager Get()
    {
        return UIManager.instance;
    }

    private void Awake()
    {
        UIManager.instance = this;
        for (int i = 0; i < 7; i++)
        {
            Layer key = (Layer)i;
            this.openScreensByLayer[key] = new HashSet<ScreenBase>();
            GameObject gameObject = null;
            gameObject = ((i != 0) ? global::UnityEngine.Object.Instantiate(this.layerSource, base.transform) : this.layerSource);
            gameObject.name = key.ToString();
            this.layerRoots[key] = gameObject;
        }
        this.layerRoots[Layer.TopLayer].transform.SetAsLastSibling();
        if (this.screens == null || this.screens.Count < 1)
        {
            Debug.LogWarning("No screens in UIManager");
        }
    }

    internal static void ClearScreens()
    {
        if (!(UIManager.Get() != null) || UIManager.Get().openScreensByLayer == null)
        {
            return;
        }
        foreach (KeyValuePair<Layer, HashSet<ScreenBase>> item in UIManager.Get().openScreensByLayer)
        {
            if (item.Value == null || item.Value.Count < 1)
            {
                continue;
            }
            foreach (ScreenBase item2 in new List<ScreenBase>(item.Value))
            {
                UIManager.Close(item2);
            }
        }
    }

    private void Update()
    {
        this.topLayerForInputFrame = null;
        this.TryInputConsumed();
        if (!TLog.Get().shownFirstLog && TLog.Get().showFirstLog)
        {
            if (TLog.firstCriticalHeader.Contains("Initializing Microsoft Media Foundation failed."))
            {
                TLog.ClearFirstError();
                return;
            }
            if (TLog.firstCriticalHeader.Contains("FMOD failed to initialize the output device"))
            {
                TLog.ClearFirstError();
                return;
            }
            if (TLog.firstCriticalHeader.Contains("FMOD ") || TLog.firstCriticalHeader.Contains("<RI.Hid>"))
            {
                TLog.ClearFirstError();
                return;
            }
            TLog.Get().shownFirstLog = true;
            PopupGeneral.OpenPopup(null, "UI_ISSUE_ENCOUNTERED", "UI_ISSUE_ENCOUNTERED_DES", "UI_CLOSE");
        }
    }

    public static bool IsOpen(Layer l, ScreenBase screen)
    {
        if (screen == null || screen.stateStatus > State.StateStatus.Active)
        {
            return false;
        }
        if (UIManager.Get().openScreensByLayer[l].Contains(screen))
        {
            return UIManager.Get().openScreensByLayer[l].Contains(screen);
        }
        return false;
    }

    public static bool IsOpen<T>(Layer l) where T : ScreenBase
    {
        if (UIManager.Get() == null || !UIManager.Get().openScreensByLayer.ContainsKey(l) || UIManager.Get().openScreensByLayer[l] == null)
        {
            return false;
        }
        foreach (ScreenBase item in UIManager.Get().openScreensByLayer[l])
        {
            if (item is T && item.stateStatus <= State.StateStatus.Active)
            {
                return true;
            }
        }
        return false;
    }

    public static T Open<T>(Layer layer, State parent = null) where T : ScreenBase
    {
        return UIManager.OpenCore<T>(UIManager.Get().screens.Find((GameObject o) => o.GetComponent<ScreenBase>() is T), layer, parent);
    }

    public static T GetOrOpen<T>(Layer layer, State parent = null) where T : ScreenBase
    {
        return UIManager.GetOrOpen<T>(UIManager.Get().screens.Find((GameObject o) => o.GetComponent<ScreenBase>() is T), layer, parent);
    }

    public static T GetOrOpen<T>(GameObject source, Layer layer, State parent = null) where T : ScreenBase
    {
        if (UIManager.Get().openScreensByLayer.ContainsKey(layer))
        {
            ScreenBase component = source.GetComponent<ScreenBase>();
            if (component == null)
            {
                Debug.LogError("Source screen " + source.name + " does not contain ScreenBase type script on it");
                return null;
            }
            HashSet<ScreenBase> hashSet = UIManager.Get().openScreensByLayer[layer];
            Type type = component.GetType();
            foreach (ScreenBase item in hashSet)
            {
                if (item.GetType() == type)
                {
                    return item as T;
                }
            }
        }
        return UIManager.OpenCore<T>(source, layer, parent);
    }

    public static T OpenCore<T>(GameObject source, Layer layer, State parent = null, bool acceptKeyboardInput = true) where T : ScreenBase
    {
        GameObject obj = GameObjectUtils.Instantiate(source, UIManager.Get().layerRoots[layer].transform);
        RectTransform obj2 = obj.transform as RectTransform;
        obj2.localPosition = Vector3.zero;
        obj2.offsetMax = Vector2.zero;
        obj2.offsetMin = Vector2.zero;
        obj2.localScale = Vector3.one;
        ScreenBase component = obj.GetComponent<ScreenBase>();
        if (component == null)
        {
            return null;
        }
        component.layer = layer;
        UIManager.Get().openScreensByLayer[layer].Add(component);
        UIManager.Get().OpenState(component, closeUnrelevant: false, parent);
        if (acceptKeyboardInput && layer >= Layer.HUD)
        {
            UIManager.Get().interactionStackForKeyboard.Add(component);
        }
        return component as T;
    }

    public static T GetScreen<T>(Layer layer) where T : ScreenBase
    {
        if (UIManager.Get().openScreensByLayer.ContainsKey(layer))
        {
            foreach (ScreenBase item in UIManager.Get().openScreensByLayer[layer])
            {
                if (item.GetType().IsAssignableFrom(typeof(T)))
                {
                    return item as T;
                }
            }
        }
        return null;
    }

    public static void Close(ScreenBase screen)
    {
        Tutorial_Generic.CloseAllOnParent(screen);
        if (UIManager.IsOpen(screen.layer, screen) && !UIManager.Get().exiting)
        {
            UIManager.Get().CloseState(screen);
        }
    }

    public void Unregister(ScreenBase screen)
    {
        Layer layerFor = UIManager.GetLayerFor(screen);
        if (layerFor != Layer.MAX && this.openScreensByLayer[layerFor].Contains(screen))
        {
            this.openScreensByLayer[layerFor].Remove(screen);
        }
        if (layerFor >= Layer.HUD && layerFor < Layer.MAX && this.interactionStackForKeyboard.Contains(screen))
        {
            this.interactionStackForKeyboard.Remove(screen);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        this.exiting = true;
    }

    public static GameObject GetLayer(Layer l)
    {
        return UIManager.Get().layerRoots[l];
    }

    public static Layer GetLayerFor(ScreenBase sb)
    {
        foreach (KeyValuePair<Layer, HashSet<ScreenBase>> item in UIManager.Get().openScreensByLayer)
        {
            if (item.Value.Contains(sb))
            {
                return item.Key;
            }
        }
        return Layer.MAX;
    }

    public static bool AnyBlockingScreen()
    {
        if (UIManager.Get() == null)
        {
            return true;
        }
        for (int i = 5; i < 7; i++)
        {
            Layer key = (Layer)i;
            if (UIManager.Get().openScreensByLayer[key] == null || UIManager.Get().openScreensByLayer[key].Count <= 0)
            {
                continue;
            }
            foreach (ScreenBase item in UIManager.Get().openScreensByLayer[key])
            {
                if (item.BlockInteractionBelow())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool AnyPopupScreen()
    {
        for (int i = 6; i < 7; i++)
        {
            Layer key = (Layer)i;
            if (UIManager.Get().openScreensByLayer[key] == null || UIManager.Get().openScreensByLayer[key].Count <= 0)
            {
                continue;
            }
            foreach (ScreenBase item in UIManager.Get().openScreensByLayer[key])
            {
                if (item.BlockInteractionBelow())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool AnyScreenTransitioning()
    {
        for (int i = 5; i < 7; i++)
        {
            Layer key = (Layer)i;
            if (UIManager.Get().openScreensByLayer[key] == null || UIManager.Get().openScreensByLayer[key].Count <= 0)
            {
                continue;
            }
            foreach (ScreenBase item in UIManager.Get().openScreensByLayer[key])
            {
                if (item.stateStatus != State.StateStatus.Active)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static int ScreensOnLayerCount(Layer l)
    {
        if (UIManager.Get().openScreensByLayer[l] != null && UIManager.Get().openScreensByLayer[l].Count > 0)
        {
            return UIManager.Get().openScreensByLayer[l].Count;
        }
        return 0;
    }

    public static bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static void ClickFromScript(Selectable s)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(s.gameObject, eventData, ExecuteEvents.pointerClickHandler);
    }

    public static bool IsTopForInput(ScreenBase sb)
    {
        if (sb == null)
        {
            return false;
        }
        if (!(sb is BugReport) && UIManager.IsOpen<BugReport>(Layer.Popup))
        {
            return false;
        }
        if (UIManager.Get().interactionStackForKeyboard.Count > 0)
        {
            Layer layer = Layer.UIMarkers;
            ScreenBase screenBase = null;
            for (int num = UIManager.Get().interactionStackForKeyboard.Count - 1; num >= 0; num--)
            {
                ScreenBase screenBase2 = UIManager.Get().interactionStackForKeyboard[num];
                if (layer < screenBase2.layer)
                {
                    layer = screenBase2.layer;
                    screenBase = screenBase2;
                }
            }
            return screenBase == sb;
        }
        return false;
    }

    public static bool IsInputConsumed()
    {
        return UIManager.Get()?.inputConsumed ?? false;
    }

    public void TryInputConsumed()
    {
        this.inputConsumed = false;
        if (Selectable.allSelectablesArray == null)
        {
            return;
        }
        Selectable[] allSelectablesArray = Selectable.allSelectablesArray;
        for (int i = 0; i < allSelectablesArray.Length; i++)
        {
            if (allSelectablesArray[i] is TMP_InputField tMP_InputField && tMP_InputField.isFocused)
            {
                this.inputConsumed = true;
                break;
            }
        }
    }

    public void PopupEvents(object sender, object e)
    {
        base.StartCoroutine(this.PopupsQueue(sender, e));
    }

    private IEnumerator PopupsQueue(object sender, object e)
    {
        while (UIManager.AnyPopupScreen())
        {
            yield return null;
        }
        if (sender is TownLocation && e is List<AdventureOutcomeDelta.Outcome> outcomes)
        {
            RazingRewards.OpenPopup(null, outcomes);
        }
        if (sender is PlayerWizard && e is PlayerWizard banishedWizard)
        {
            PopupWizardBanished.OpenPopup(null, banishedWizard);
        }
    }
}
