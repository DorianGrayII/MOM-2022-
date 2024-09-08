namespace MOM
{
    using DBDef;
    using DBUtils;
    using MHUtils;
    using MHUtils.UI;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

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

        private void DifficultyItem(GameObject item, object source, object data, int index)
        {
            DifficultyListItem component = item.GetComponent<DifficultyListItem>();
            Multitype<DBDef.Difficulty, DifficultyOption> d = source as Multitype<DBDef.Difficulty, DifficultyOption>;
            component.label.text = DBUtils.Localization.Get(d.t0.name, true, Array.Empty<object>());
            List<string> options = new List<string>();
            foreach (DifficultyOption option in d.t0.setting)
            {
                options.Add(option.title);
            }
            component.dropdown.SetOptions(options, true, true);
            component.dropdown.SelectOption(Array.IndexOf<DifficultyOption>(d.t0.setting, d.t1), false, true);
            component.dropdown.onChange = delegate (object obj) {
                if (!this.blockChanges)
                {
                    int orderIndex = Array.FindIndex<DifficultyOption>(d.t0.setting, o => o.title == (obj as string));
                    DifficultySettingsData.SetValue(d.t0.name, orderIndex);
                    this.UpdatePresets();
                    this.UpdateScoreMultiplier();
                }
            };
            RolloverSimpleTooltip orAddComponent = GameObjectUtils.GetOrAddComponent<RolloverSimpleTooltip>(component.label.gameObject);
            orAddComponent.title = DBUtils.Localization.Get(d.t0.tooltipName, true, Array.Empty<object>());
            orAddComponent.description = DBUtils.Localization.Get(d.t0.tooltipDescription, true, Array.Empty<object>());
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor = new Vector2(1.05f, 1f);
            orAddComponent.offset = new Vector2(0f, 50f);
        }

        public override void OnStart()
        {
            base.OnStart();
            this.difficultyGrid.CustomDynamicItem(new CustomDynamicItemMethod(this.DifficultyItem), new SimpleCallback(this.UpdateItems));
            this.UpdateItems();
            this.UpdatePresets();
            this.UpdateScoreMultiplier();
            this.tgEasy.onValueChanged.AddListener(delegate (bool b) {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(1, false);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgNormal.onValueChanged.AddListener(delegate (bool b) {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(2, false);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgDifficult.onValueChanged.AddListener(delegate (bool b) {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(3, false);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgExtreme.onValueChanged.AddListener(delegate (bool b) {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(4, false);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
            this.tgQuick.onValueChanged.AddListener(delegate (bool b) {
                if (b)
                {
                    DifficultySettingsData.SetDefaults(2, true);
                    this.UpdateItems();
                    this.UpdateScoreMultiplier();
                }
            });
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

        private void UpdateItems()
        {
            DifficultySettingsData.EnsureLoaded(1);
            DifficultySettingsData current = DifficultySettingsData.current;
            List<DBDef.Difficulty> type = DataBase.GetType<DBDef.Difficulty>();
            if ((type == null) || (type.Count <= 0))
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            List<Multitype<DBDef.Difficulty, DifficultyOption>> items = new List<Multitype<DBDef.Difficulty, DifficultyOption>>();
            for (int i = 0; i < type.Count; i++)
            {
                DifficultyOption option = null;
                option = ((current.settingsNamed == null) || !current.settingsNamed.ContainsKey(type[i].name)) ? type[i].setting[0] : type[i].setting[current.settingsNamed[type[i].name]];
                items.Add(new Multitype<DBDef.Difficulty, DifficultyOption>(type[i], option));
            }
            this.blockChanges = true;
            this.difficultyGrid.UpdateGrid<Multitype<DBDef.Difficulty, DifficultyOption>>(items, null);
            this.blockChanges = false;
        }

        private void UpdatePresets()
        {
            switch (DifficultySettingsData.GetCurentDifficultyRank())
            {
                case 1:
                    this.tgEasy.isOn = true;
                    return;

                case 2:
                    this.tgNormal.isOn = true;
                    return;

                case 3:
                    this.tgDifficult.isOn = true;
                    return;

                case 4:
                    this.tgExtreme.isOn = true;
                    return;

                case 5:
                    this.tgQuick.isOn = true;
                    return;
            }
            this.tgCustom.isOn = true;
        }

        private void UpdateScoreMultiplier()
        {
            this.scoreMultiplier.text = DifficultySettingsData.GetCurentScoreMultiplier().ToString() + "%";
        }
    }
}

