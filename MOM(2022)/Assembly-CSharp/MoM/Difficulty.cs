using System;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class Difficulty : ScreenBase
    {
        public Toggle tgEasy;

        public Toggle tgNormal;

        public Toggle tgDifficult;

        public Toggle tgExtreme;

        public Toggle tgCustom;

        public Toggle tgQuick;

        public TextMeshProUGUI scoreMultiplier;

        public Button btContinue;

        public Button btCancel;

        public GridItemManager difficultyGrid;

        private bool blockChanges;

        public override void OnStart()
        {
            base.OnStart();
            difficultyGrid.CustomDynamicItem(DifficultyItem, UpdateItems);
            UpdateItems();
            UpdatePresets();
            UpdateScoreMultiplier();
            tgEasy.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(1);
                    UpdateItems();
                    UpdateScoreMultiplier();
                }
            });
            tgNormal.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(2);
                    UpdateItems();
                    UpdateScoreMultiplier();
                }
            });
            tgDifficult.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(3);
                    UpdateItems();
                    UpdateScoreMultiplier();
                }
            });
            tgExtreme.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(4);
                    UpdateItems();
                    UpdateScoreMultiplier();
                }
            });
            tgQuick.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(2, quickStart: true);
                    UpdateItems();
                    UpdateScoreMultiplier();
                }
            });
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == btContinue)
            {
                MHEventSystem.TriggerEvent(this, "Advance");
            }
            else if (s == btCancel)
            {
                MHEventSystem.TriggerEvent(this, "Back");
            }
        }

        private void UpdatePresets()
        {
            switch (DifficultySettingsData.GetCurentDifficultyRank())
            {
            case 1:
                tgEasy.isOn = true;
                break;
            case 2:
                tgNormal.isOn = true;
                break;
            case 3:
                tgDifficult.isOn = true;
                break;
            case 4:
                tgExtreme.isOn = true;
                break;
            case 5:
                tgQuick.isOn = true;
                break;
            default:
                tgCustom.isOn = true;
                break;
            }
        }

        private void UpdateScoreMultiplier()
        {
            scoreMultiplier.text = DifficultySettingsData.GetCurentScoreMultiplier() + "%";
        }

        private void DifficultyItem(GameObject item, object source, object data, int index)
        {
            DifficultyListItem component = item.GetComponent<DifficultyListItem>();
            Multitype<global::DBDef.Difficulty, DifficultyOption> difficulty = source as Multitype<global::DBDef.Difficulty, DifficultyOption>;
            component.label.text = global::DBUtils.Localization.Get(difficulty.t0.name, true);
            List<string> list = new List<string>();
            DifficultyOption[] setting = difficulty.t0.setting;
            foreach (DifficultyOption difficultyOption in setting)
            {
                list.Add(difficultyOption.title);
            }
            component.dropdown.SetOptions(list);
            component.dropdown.SelectOption(Array.IndexOf(difficulty.t0.setting, difficulty.t1));
            component.dropdown.onChange = delegate(object obj)
            {
                if (!blockChanges)
                {
                    int orderIndex = Array.FindIndex(difficulty.t0.setting, (DifficultyOption o) => o.title == obj as string);
                    DifficultySettingsData.SetValue(difficulty.t0.name, orderIndex);
                    UpdatePresets();
                    UpdateScoreMultiplier();
                }
            };
            RolloverSimpleTooltip orAddComponent = component.label.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent.title = global::DBUtils.Localization.Get(difficulty.t0.tooltipName, true);
            orAddComponent.description = global::DBUtils.Localization.Get(difficulty.t0.tooltipDescription, true);
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor = new Vector2(1.05f, 1f);
            orAddComponent.offset = new Vector2(0f, 50f);
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
             //   DifficultyOption difficultyOption = null;
                list.Add(new Multitype<global::DBDef.Difficulty, DifficultyOption>(t1: (current.settingsNamed == null || !current.settingsNamed.ContainsKey(type[i].name)) ? type[i].setting[0] : type[i].setting[current.settingsNamed[type[i].name]], t0: type[i]));
            }
            blockChanges = true;
            difficultyGrid.UpdateGrid(list);
            blockChanges = false;
        }

        private void ResetToggles()
        {
            tgEasy.isOn = false;
            tgNormal.isOn = false;
            tgDifficult.isOn = false;
            tgExtreme.isOn = false;
            tgCustom.isOn = false;
            tgQuick.isOn = false;
        }
    }
}
