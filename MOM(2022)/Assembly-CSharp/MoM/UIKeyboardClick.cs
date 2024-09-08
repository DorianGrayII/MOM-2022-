namespace MOM
{
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIKeyboardClick : MonoBehaviour
    {
        public KeyCode button = KeyCode.Return;
        public Settings.KeyActions action;
        public int priority;
        private Selectable selectable;
        private ScreenBase parent;
        private CanvasGroup cg;
        private PlaySFX sfx;

        private void Start()
        {
            this.selectable = base.GetComponent<Selectable>();
            if (this.selectable == null)
            {
                Debug.LogError("[ERROR]UIKeyboardClick is not attached to gameobject with selectable to click!");
            }
            this.parent = base.GetComponentInParent<ScreenBase>();
            this.cg = base.GetComponentInParent<CanvasGroup>();
            this.sfx = base.GetComponentInParent<PlaySFX>();
            this.UpdateByRemap();
        }

        private void Update()
        {
            if ((this.selectable.interactable && ((this.cg == null) || (this.cg.interactable && (this.cg.alpha >= 0.9f)))) && (SettingsBlock.IsKeyUp(this.action) && UIManager.IsTopForInput(this.parent)))
            {
                ScreenBase componentInParent = base.gameObject.GetComponentInParent<ScreenBase>();
                if (componentInParent != null)
                {
                    UIKeyboardClick[] componentsInChildren = componentInParent.gameObject.GetComponentsInChildren<UIKeyboardClick>();
                    if (componentsInChildren.Length > 1)
                    {
                        int priority = -2147483648;
                        UIKeyboardClick[] clickArray2 = componentsInChildren;
                        int index = 0;
                        while (true)
                        {
                            if (index >= clickArray2.Length)
                            {
                                if (this.priority == priority)
                                {
                                    break;
                                }
                                return;
                            }
                            UIKeyboardClick click = clickArray2[index];
                            if (click.gameObject.activeInHierarchy && (priority < click.priority))
                            {
                                priority = click.priority;
                            }
                            index++;
                        }
                    }
                }
                if (this.selectable is Button)
                {
                    if (this.sfx != null)
                    {
                        AudioLibrary.RequestSFX(this.sfx.clickEffect, 0f, 0f, 1f);
                    }
                    (this.selectable as Button).onClick.Invoke();
                }
                else if (this.selectable is Toggle)
                {
                    if (this.sfx != null)
                    {
                        AudioLibrary.RequestSFX(this.sfx.clickEffect, 0f, 0f, 1f);
                    }
                    (this.selectable as Toggle).isOn = !(this.selectable as Toggle).isOn;
                }
            }
        }

        public void UpdateByRemap()
        {
            if (this.action == Settings.KeyActions.None)
            {
                foreach (KeyValuePair<Settings.KeyActions, KeyCode> pair in Settings.defaultMapping)
                {
                    if (((KeyCode) pair.Value) == this.button)
                    {
                        this.action = pair.Key;
                        break;
                    }
                }
            }
        }
    }
}

