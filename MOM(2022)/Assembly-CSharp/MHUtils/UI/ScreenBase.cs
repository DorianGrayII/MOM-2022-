// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.UI.ScreenBase
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBase : State
{
    private static float SCREEN_TRANSITION = 0.3f;

    public CanvasGroup cGroup;

    public UIManager.Layer layer;

    public Tutorial_Generic openTutorial;

    private Dictionary<string, Selectable> selectables = new Dictionary<string, Selectable>();

    private Coroutine openAnimation;

    protected bool instantOpen;

    protected bool delayedAnimateIn;

    public override IEnumerator PreClose()
    {
        foreach (KeyValuePair<string, Selectable> selectable in this.selectables)
        {
            if (!(selectable.Value == null) && selectable.Value.interactable)
            {
                selectable.Value.interactable = false;
            }
        }
        yield return base.PreClose();
    }

    public override IEnumerator PostClose()
    {
        yield return base.PostClose();
        UIManager.Get().Unregister(this);
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
        Object.Destroy(base.gameObject);
    }

    protected virtual void Awake()
    {
        if (this.cGroup == null)
        {
            this.cGroup = base.GetComponent<CanvasGroup>();
        }
        this.SetVisible(visible: false);
    }

    protected override void Start()
    {
        base.Start();
        this.LocalizeTextFields();
    }

    public override void OnStart()
    {
        base.OnStart();
        this.FindSelectables();
    }

    public override IEnumerator PostStart()
    {
        yield return base.PostStart();
        if (!this.delayedAnimateIn)
        {
            this.SetVisible(visible: true, animate: true);
        }
        this.openTutorial?.OpenIfNotSeen(this);
    }

    public void SetVisible(bool visible, bool animate = false)
    {
        if (this.cGroup == null)
        {
            return;
        }
        if (animate && !this.instantOpen && base.isActiveAndEnabled)
        {
            this.openAnimation = base.StartCoroutine(this.AnimateVisibility(visible));
        }
        else
        {
            this.cGroup.alpha = (visible ? 1f : 0f);
            if (this.openAnimation != null)
            {
                base.StopCoroutine(this.openAnimation);
            }
        }
        this.cGroup.interactable = visible;
        this.cGroup.blocksRaycasts = visible;
    }

    private IEnumerator AnimateVisibility(bool visible)
    {
        if (this.cGroup != null)
        {
            float time = 0f;
            while (time < ScreenBase.SCREEN_TRANSITION)
            {
                time += Time.fixedDeltaTime;
                if (time > ScreenBase.SCREEN_TRANSITION)
                {
                    time = ScreenBase.SCREEN_TRANSITION;
                }
                float num = time / ScreenBase.SCREEN_TRANSITION;
                this.cGroup.alpha = (visible ? num : (1f - num));
                yield return null;
            }
        }
        this.openAnimation = null;
    }

    protected void FindSelectables()
    {
        Selectable[] componentsInChildren = base.gameObject.GetComponentsInChildren<Selectable>(includeInactive: true);
        foreach (Selectable selectable in componentsInChildren)
        {
            this.selectables[selectable.name] = selectable;
            if (selectable is Button)
            {
                Button button = selectable as Button;
                button.onClick.AddListener(delegate
                {
                    if (base.stateStatus == StateStatus.Active)
                    {
                        this.ButtonClick(button);
                    }
                });
            }
            if (!(selectable is Toggle))
            {
                continue;
            }
            Toggle button2 = selectable as Toggle;
            button2.onValueChanged.AddListener(delegate
            {
                if (base.stateStatus == StateStatus.Active)
                {
                    this.ButtonClick(button2);
                }
            });
        }
    }

    protected void LocalizeTextFields()
    {
        ScreenBase.LocalizeTextFields(base.gameObject);
    }

    public static void LocalizeTextFields(GameObject parent)
    {
        if (DataBase.IsInitialized())
        {
            TextMeshProUGUI[] componentsInChildren = parent.GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
            foreach (TextMeshProUGUI obj in componentsInChildren)
            {
                obj.text = Localization.Get(obj.text, true);
            }
        }
    }

    protected virtual void ButtonClick(Selectable s)
    {
        if (base.stateStatus == StateStatus.Active)
        {
            MHEventSystem.TriggerEvent(this, s.name);
        }
    }

    public T GetSelectable<T>(string name) where T : Selectable
    {
        if (this.selectables.ContainsKey(name))
        {
            return this.selectables[name] as T;
        }
        return null;
    }

    public Dictionary<string, Selectable> GetSelectables()
    {
        return this.selectables;
    }

    public List<State> GetChildScreens()
    {
        return base.children;
    }

    public T GetParentAs<T>() where T : ScreenBase
    {
        return base.GetParent() as T;
    }

    public virtual bool BlockInteractionBelow()
    {
        return true;
    }
}
