using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class PauseMenu : ScreenBase
    {
        public Button buttonContinue;

        public Button buttonSaveGame;

        public Button buttonLoadGame;

        public Button buttonSettings;

        public Button buttonDifficulty;

        public Button buttonQuitToMenu;

        public Button buttonQuitGame;

        public Button buttonCloseDifficulty;

        public TextMeshProUGUI labelVersion;

        public TextMeshProUGUI scoreMultiplier;

        public GridItemManager difficultyGrid;

        public GameObject goDifficulty;

        private bool blockChanges;

        public override IEnumerator PreStart()
        {
            this.labelVersion.text = GameVersion.GetGameVersionFull();
            FSMSelectionManager.Get().Select(null, focus: false);
            this.goDifficulty.SetActive(value: false);
            this.UpdateButtons();
            yield return base.PreStart();
        }

        public override void OnStart()
        {
            base.OnStart();
            this.difficultyGrid.CustomDynamicItem(DifficultyItem, UpdateItems);
            this.UpdateItems();
            this.UpdateScoreMultiplier();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            this.UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (TurnManager.Get(allowNull: true) == null || TurnManager.Get().endTurn || !TurnManager.Get().playerTurn)
            {
                this.buttonSaveGame.interactable = false;
                this.buttonLoadGame.interactable = false;
                this.buttonSettings.interactable = false;
                this.buttonQuitToMenu.interactable = false;
                this.buttonQuitGame.interactable = false;
            }
            else
            {
                this.buttonSaveGame.interactable = true;
                this.buttonLoadGame.interactable = SaveManager.IsAnySaveAvailable();
                this.buttonSettings.interactable = true;
                this.buttonQuitToMenu.interactable = true;
                this.buttonQuitGame.interactable = true;
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.buttonQuitToMenu)
            {
                PopupGeneral.OpenPopup(this, "UI_QUIT_TO_MENU", "UI_QUIT_GAME_DESCRIPTION", "UI_QUIT_WITH_SAVE", delegate
                {
                    this.SaveAndGoToMenu();
                }, "UI_CANCEL", null, "UI_QUIT_WITHOUT_SAVE", delegate
                {
                    this.GoToMenu();
                });
            }
            else if (s == this.buttonQuitGame)
            {
                PopupGeneral.OpenPopup(this, "UI_QUIT_GAME", "UI_QUIT_GAME_DESCRIPTION", "UI_QUIT_WITH_SAVE", delegate
                {
                    this.SaveAndQuit();
                }, "UI_CANCEL", null, "UI_QUIT_WITHOUT_SAVE", delegate
                {
                    this.Quit();
                });
            }
            else if (s == this.buttonSaveGame)
            {
                SaveGame.Popup(isSave: true, this);
            }
            else if (s == this.buttonLoadGame)
            {
                SaveGame.Popup(isSave: false, this);
            }
            else if (s == this.buttonSettings)
            {
                Settings.Popup(this);
            }
            else if (s == this.buttonDifficulty)
            {
                this.goDifficulty.SetActive(value: true);
            }
            else if (s == this.buttonCloseDifficulty)
            {
                this.goDifficulty.SetActive(value: false);
            }
            else if (s == this.buttonContinue)
            {
                UIManager.Close(this);
            }
        }

        private void GoToMenu()
        {
            FSMGameplay.Get().HandleEvent("ExitGameplay");
            UIManager.Close(this);
        }

        private void SaveAndGoToMenu()
        {
            Exception ex = SaveManager.SaveGame(World.Get().seed, "save_and_quit");
            if (ex != null)
            {
                Debug.LogError(ex);
                return;
            }
            FSMGameplay.Get().HandleEvent("ExitGameplay");
            UIManager.Close(this);
        }

        private void SaveAndQuit()
        {
            Exception ex = SaveManager.SaveGame(World.Get().seed, "save_and_quit");
            if (ex != null)
            {
                Debug.LogError(ex);
                return;
            }
            Debug.LogWarning("Quitting application");
            Application.Quit();
        }

        private void Quit()
        {
            Debug.LogWarning("Quitting application");
            Application.Quit();
        }

        private void UpdateScoreMultiplier()
        {
            this.scoreMultiplier.text = DifficultySettingsData.GetCurentScoreMultiplier() + "%";
        }

        private void UpdateItems()
        {
            DifficultySettingsData.EnsureLoaded();
            DifficultySettingsData current = DifficultySettingsData.current;
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            if (type == null || type.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            List<Multitype<global::DBDef.Difficulty, DifficultyOption>> list = new List<Multitype<global::DBDef.Difficulty, DifficultyOption>>();
            for (int i = 0; i < type.Count; i++)
            {
          //      DifficultyOption difficultyOption = null;
                list.Add(new Multitype<global::DBDef.Difficulty, DifficultyOption>(t1: (current.settingsNamed == null || !current.settingsNamed.ContainsKey(type[i].name)) ? type[i].setting[0] : type[i].setting[current.settingsNamed[type[i].name]], t0: type[i]));
            }
            this.blockChanges = true;
            this.difficultyGrid.UpdateGrid(list);
            this.blockChanges = false;
        }

        private void DifficultyItem(GameObject item, object source, object data, int index)
        {
            DifficultyListItem component = item.GetComponent<DifficultyListItem>();
            Multitype<global::DBDef.Difficulty, DifficultyOption> d = source as Multitype<global::DBDef.Difficulty, DifficultyOption>;
            component.label.text = global::DBUtils.Localization.Get(d.t0.name, true);
            List<string> list = new List<string>();
            DifficultyOption[] setting = d.t0.setting;
            foreach (DifficultyOption difficultyOption in setting)
            {
                list.Add(difficultyOption.title);
            }
            component.dropdown.SetOptions(list);
            component.dropdown.SelectOption(Array.IndexOf(d.t0.setting, d.t1));
            component.dropdown.onChange = delegate(object obj)
            {
                if (!this.blockChanges)
                {
                    int orderIndex = Array.FindIndex(d.t0.setting, (DifficultyOption o) => o.title == obj as string);
                    DifficultySettingsData.SetValue(d.t0.name, orderIndex);
                    this.UpdateScoreMultiplier();
                }
            };
            RolloverSimpleTooltip orAddComponent = component.label.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent.title = global::DBUtils.Localization.Get(d.t0.tooltipName, true);
            orAddComponent.description = global::DBUtils.Localization.Get(d.t0.tooltipDescription, true);
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor = new Vector2(1.05f, 1f);
            orAddComponent.offset = new Vector2(0f, 50f);
        }
    }
}
