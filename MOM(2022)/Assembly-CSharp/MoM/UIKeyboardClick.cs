using System.Collections.Generic;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
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

        public void UpdateByRemap()
        {
            if (this.action != 0)
            {
                return;
            }
            foreach (KeyValuePair<Settings.KeyActions, KeyCode> item in Settings.defaultMapping)
            {
                if (item.Value == this.button)
                {
                    this.action = item.Key;
                    break;
                }
            }
        }

        private void Update()
        {
            if (!this.selectable.interactable || (this.cg != null && (!this.cg.interactable || this.cg.alpha < 0.9f)) || !SettingsBlock.IsKeyUp(this.action) || !UIManager.IsTopForInput(this.parent))
            {
                return;
            }
            ScreenBase componentInParent = base.gameObject.GetComponentInParent<ScreenBase>();
            if (componentInParent != null)
            {
                UIKeyboardClick[] componentsInChildren = componentInParent.gameObject.GetComponentsInChildren<UIKeyboardClick>();
                if (componentsInChildren.Length > 1)
                {
                    int num = int.MinValue;
                    UIKeyboardClick[] array = componentsInChildren;
                    foreach (UIKeyboardClick uIKeyboardClick in array)
                    {
                        if (uIKeyboardClick.gameObject.activeInHierarchy && num < uIKeyboardClick.priority)
                        {
                            num = uIKeyboardClick.priority;
                        }
                    }
                    if (this.priority != num)
                    {
                        return;
                    }
                }
            }
            if (this.selectable is Button)
            {
                if (this.sfx != null)
                {
                    AudioLibrary.RequestSFX(this.sfx.clickEffect);
                }
                (this.selectable as Button).onClick.Invoke();
            }
            else if (this.selectable is Toggle)
            {
                if (this.sfx != null)
                {
                    AudioLibrary.RequestSFX(this.sfx.clickEffect);
                }
                (this.selectable as Toggle).isOn = !(this.selectable as Toggle).isOn;
            }
        }
    }
}
