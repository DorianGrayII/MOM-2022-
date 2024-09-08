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
            this.difficultyGrid.CustomDynamicItem(DifficultyItem, UpdateItems);
            this.UpdateItems();
            this.UpdatePresets();
            this.UpdateScoreMultiplier();
            this.tgEasy.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(1);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgNormal.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(2);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgDifficult.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(3);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgExtreme.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(4);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgQuick.onValueChanged.AddListener(delegate(bool b)
            {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(2, quickStart: true);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btContinue)
            {
                MHEventSystem.TriggerEvent(this, "Advance");
            }
            else if (s == this.btCancel)
            {
                MHEventSystem.TriggerEvent(this, "Back");
            }
        }

        private void UpdatePresets()
        {
            switch (DifficultySettingsData.GetCurentDifficultyRank())
            {
            case 1:
                this.tgEasy.isOn = true;
                break;
            case 2:
                this.tgNormal.isOn = true;
                break;
            case 3:
                this.tgDifficult.isOn = true;
                break;
            case 4:
                this.tgExtreme.isOn = true;
                break;
            case 5:
                this.tgQuick.isOn = true;
                break;
            default:
                this.tgCustom.isOn = true;
                break;
            }
        }

        private void UpdateScoreMultiplier()
        {
            this.scoreMultiplier.text = DifficultySettingsData.GetCurentScoreMultiplier() + "%";
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
                    this.UpdatePresets();
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
                DifficultyOption difficultyOption = null;
                list.Add(new Multitype<global::DBDef.Difficulty, DifficultyOption>(t1: (current.settingsNamed == null || !current.settingsNamed.ContainsKey(type[i].name)) ? type[i].setting[0] : type[i].setting[current.settingsNamed[type[i].name]], t0: type[i]));
            }
            this.blockChanges = true;
            this.difficultyGrid.UpdateGrid(list);
            this.blockChanges = false;
        }

        private void ResetToggles()
        {
            this.tgEasy.isOn = false;
            this.tgNormal.isOn = false;
            this.tgDifficult.isOn = false;
            this.tgExtreme.isOn = false;
            this.tgCustom.isOn = false;
            this.tgQuick.isOn = false;
        }
    }
}
