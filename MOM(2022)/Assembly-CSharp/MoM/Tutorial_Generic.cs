using System;
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class Tutorial_Generic : ScreenBase
    {
        public const string SettingPrefix = "TUT_";

        public const string HideAllTutorials = "TUT_HIDEALL";

        public Button btClose;

        public GameObject[] pages;

        public GameObjectEnabler<PlayerWizard.Familiar> familiar;

        private static List<Tuple<Tutorial_Generic, object>> currentlyOpen = new List<Tuple<Tutorial_Generic, object>>();

        protected override void Awake()
        {
            base.Awake();
            this.familiar.Set(GameManager.GetHumanWizard().familiar);
            ScreenBase.LocalizeTextFields(base.gameObject);
            TutorialVisiblityByPage[] componentsInChildren = base.GetComponentsInChildren<TutorialVisiblityByPage>(includeInactive: true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].gameObject.SetActive(value: true);
            }
        }

        public override IEnumerator PreStart()
        {
            yield return base.PreStart();
            base.haveStarted = true;
        }

        public bool WouldShow()
        {
            foreach (Tuple<Tutorial_Generic, object> item in Tutorial_Generic.currentlyOpen)
            {
                if (item.Item1.name == base.name)
                {
                    return false;
                }
            }
            if (this.ShouldHideAll() || this.ShouldShowStart())
            {
                return false;
            }
            return Settings.GetData().Get<int>("TUT_" + base.name) < this.pages.Length;
        }

        private bool ShouldHideAll()
        {
            return Settings.GetData().Get<bool>("TUT_HIDEALL");
        }

        private bool ShouldShowStart()
        {
            if (!Settings.GetData().Get<bool>("TUT_Start") && TurnManager.Get().turnNumber == 1 && !Settings.GetData().HavePrefix("TUT_"))
            {
                return true;
            }
            return false;
        }

        public Tutorial_Generic OpenIfNotSeen(object parent)
        {
            ScreenBase screenBase = parent as ScreenBase;
            if (!screenBase && parent is GameObject gameObject)
            {
                screenBase = gameObject.GetComponentInParent<ScreenBase>();
            }
            if (this.ShouldHideAll())
            {
                return null;
            }
            if (this.ShouldShowStart())
            {
                string text = GameManager.GetHumanWizard().GetName();
                PopupGeneral.OpenPopup(screenBase, Localization.Get("UI_WELCOME", true, text), Localization.Get("UI_WELCOME_DES", true, text), "UI_YES", delegate
                {
                    Settings.GetData().Set("TUT_Start", "true");
                    SettingsBlock.Save(Settings.GetData());
                    this.OpenIfNotSeen(parent);
                }, "UI_CANCEL", delegate
                {
                    Settings.GetData().Set("TUT_HIDEALL", "true");
                    SettingsBlock.Save(Settings.GetData());
                });
                return null;
            }
            int num = Settings.GetData().Get<int>("TUT_" + base.name);
            if (num < this.pages.Length)
            {
                Tutorial_Generic tutorial_Generic = UIManager.OpenCore<Tutorial_Generic>(base.gameObject, UIManager.Layer.Popup, screenBase, acceptKeyboardInput: false);
                tutorial_Generic.name = base.name;
                for (int i = 0; i < tutorial_Generic.pages.Length; i++)
                {
                    tutorial_Generic.pages[i].SetActive(i == num);
                }
                Tutorial_Generic.currentlyOpen.Add(new Tuple<Tutorial_Generic, object>(tutorial_Generic, parent));
                Tutorial_Generic.Hide();
                return Tutorial_Generic.currentlyOpen[Tutorial_Generic.currentlyOpen.Count - 1].Item1;
            }
            return null;
        }

        private void MarkAsCompleted()
        {
            Settings.GetData().Set("TUT_" + base.name, this.pages.Length.ToString());
            SettingsBlock.Save(Settings.GetData());
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btClose)
            {
                this.CloseIfOpen(openHidden: false);
                this.MarkAsCompleted();
            }
        }

        private void SaveCurrentPage()
        {
            for (int i = 0; i < this.pages.Length; i++)
            {
                if (this.pages[i].activeSelf)
                {
                    Settings.GetData().Set("TUT_" + base.name, i.ToString());
                    SettingsBlock.Save(Settings.GetData());
                    break;
                }
            }
        }

        public void CloseIfOpen(bool openHidden = true)
        {
            for (int num = Tutorial_Generic.currentlyOpen.Count - 1; num >= 0; num--)
            {
                Tuple<Tutorial_Generic, object> tuple = Tutorial_Generic.currentlyOpen[num];
                if (tuple.Item1 == this)
                {
                    object item = tuple.Item2;
                    if (openHidden)
                    {
                        this.SaveCurrentPage();
                    }
                    UIManager.Close(tuple.Item1);
                    Tutorial_Generic.currentlyOpen.RemoveAt(num);
                    if (!openHidden && num > 0 && Tutorial_Generic.currentlyOpen[num - 1].Item2 == item)
                    {
                        Tutorial_Generic.currentlyOpen[num - 1].Item1.gameObject.SetActive(value: true);
                    }
                }
            }
            if (openHidden)
            {
                Tutorial_Generic.Hide();
            }
        }

        public static void CloseAllOnParent(object parent)
        {
            for (int num = Tutorial_Generic.currentlyOpen.Count - 1; num >= 0; num--)
            {
                Tuple<Tutorial_Generic, object> tuple = Tutorial_Generic.currentlyOpen[num];
                if (tuple.Item2 == parent)
                {
                    tuple.Item1.SaveCurrentPage();
                    UIManager.Close(tuple.Item1);
                    Tutorial_Generic.currentlyOpen.RemoveAt(num);
                }
            }
            Tutorial_Generic.Hide();
        }

        public static void Hide(bool excludeTop = true)
        {
            bool active = excludeTop;
            for (int num = Tutorial_Generic.currentlyOpen.Count - 1; num >= 0; num--)
            {
                Tutorial_Generic.currentlyOpen[num].Item1.gameObject.SetActive(active);
                active = false;
            }
        }

        public override bool BlockInteractionBelow()
        {
            return false;
        }

        public int GetPageNo(TutorialPage page)
        {
            for (int i = 0; i < this.pages.Length; i++)
            {
                if (this.pages[i] == page.gameObject)
                {
                    return i + 1;
                }
            }
            return 0;
        }
    }
}
