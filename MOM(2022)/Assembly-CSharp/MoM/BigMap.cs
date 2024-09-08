using System;
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

namespace MOM
{
    public class BigMap : ScreenBase
    {
        public Button btClose;

        public Button btChangePlane1;

        public Button btChangePlane2;

        public Toggle tgGreenWizard;

        public Toggle tgBlueWizard;

        public Toggle tgRedWizard;

        public Toggle tgPurpleWizard;

        public Toggle tgYellowWizard;

        public Toggle tgNeutrals;

        public Toggle tgNodes;

        public Toggle tgTowers;

        public Toggle tgTerrainFeatures;

        public Toggle tgSoultrapped;

        public Toggle tgBossLair;

        public TextMeshProUGUI labelGreenWizardName;

        public TextMeshProUGUI labelBlueWizardName;

        public TextMeshProUGUI labelRedWizardName;

        public TextMeshProUGUI labelPurpleWizardName;

        public TextMeshProUGUI labelYellowWizardName;

        public TextMeshProUGUI labelMapName;

        public Sprite nodeIcon;

        public Sprite towerIcon;

        public Sprite soultrappedIcon;

        public Sprite bossLairIcon;

        public RawImage minimap;

        public global::WorldCode.Plane plane;

        private Dictionary<Toggle, int> switchToID = new Dictionary<Toggle, int>();

        public override void OnStart()
        {
            base.OnStart();
            this.tgTerrainFeatures.isOn = !MinimapManager.Get().simpleColor;
            this.switchToID[this.tgNeutrals] = 0;
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            foreach (Reference<PlayerWizard> item in ListUtils.MultiEnumerable(b: humanWizard.GetDiscoveredWizards().FindAll((Reference<PlayerWizard> o) => o.Get().isAlive), a: humanWizard))
            {
                PlayerWizard playerWizard = item.Get();
                switch (playerWizard.color)
                {
                case PlayerWizard.Color.Green:
                    this.switchToID[this.tgGreenWizard] = playerWizard.GetID();
                    this.labelGreenWizardName.text = playerWizard.name;
                    break;
                case PlayerWizard.Color.Purple:
                    this.switchToID[this.tgPurpleWizard] = playerWizard.GetID();
                    this.labelPurpleWizardName.text = playerWizard.name;
                    break;
                case PlayerWizard.Color.Red:
                    this.switchToID[this.tgRedWizard] = playerWizard.GetID();
                    this.labelRedWizardName.text = playerWizard.name;
                    break;
                case PlayerWizard.Color.Blue:
                    this.switchToID[this.tgBlueWizard] = playerWizard.GetID();
                    this.labelBlueWizardName.text = playerWizard.name;
                    break;
                case PlayerWizard.Color.Yellow:
                    this.switchToID[this.tgYellowWizard] = playerWizard.GetID();
                    this.labelYellowWizardName.text = playerWizard.name;
                    break;
                }
            }
            Action<Toggle, GameObject> obj = delegate(Toggle t, GameObject go)
            {
                if (!this.switchToID.ContainsKey(t))
                {
                    t.gameObject.SetActive(value: false);
                    go.gameObject.SetActive(value: false);
                }
            };
            obj(this.tgGreenWizard, this.labelGreenWizardName.gameObject);
            obj(this.tgPurpleWizard, this.labelPurpleWizardName.gameObject);
            obj(this.tgRedWizard, this.labelRedWizardName.gameObject);
            obj(this.tgBlueWizard, this.labelBlueWizardName.gameObject);
            obj(this.tgYellowWizard, this.labelYellowWizardName.gameObject);
            this.SetPlane(World.GetActivePlane());
            this.UpdateSettings();
        }

        private void SetPlane(global::WorldCode.Plane plane)
        {
            this.plane = plane;
            if (plane.arcanusType)
            {
                this.labelMapName.text = Localization.Get("UI_ARCANUS_PLANE", true);
            }
            else
            {
                this.labelMapName.text = Localization.Get("UI_MYRROR_PLANE", true);
            }
        }

        private void UpdateSettings()
        {
            MinimapManager.MapSettings settings = default(MinimapManager.MapSettings);
            if (this.tgGreenWizard.isOn && this.tgBlueWizard.isOn && this.tgRedWizard.isOn && this.tgPurpleWizard.isOn && this.tgYellowWizard.isOn && this.tgNeutrals.isOn)
            {
                settings.hideWizards = null;
            }
            else
            {
                foreach (KeyValuePair<Toggle, int> item in this.switchToID)
                {
                    if (!item.Key.isOn)
                    {
                        ref List<int> hideWizards = ref settings.hideWizards;
                        if (hideWizards == null)
                        {
                            hideWizards = new List<int>();
                        }
                        settings.hideWizards.Add(item.Value);
                    }
                }
            }
            settings.hideNeutrals = !this.tgNeutrals.isOn;
            settings.showMagicNodes = this.tgNodes.isOn;
            settings.showGates = this.tgTowers.isOn;
            settings.showBossLair = this.tgBossLair.isOn;
            int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_MID_GAME_AWAKE");
            this.tgSoultrapped.gameObject.SetActive(settingAsInt > 0);
            if (settingAsInt > 0)
            {
                settings.showSoultrapped = this.tgSoultrapped.isOn;
            }
            MinimapManager.Get().BigMapMode(b: true, settings);
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btClose)
            {
                UIManager.Close(this);
            }
            else if (s == this.btChangePlane1 || s == this.btChangePlane2)
            {
                this.SetPlane(World.GetOtherPlane(this.plane));
                MinimapManager.Get().SetPlane(this.plane);
                FOW.Get().SwitchFogMiniMapTo(this.plane.arcanusType);
            }
            else if (s == this.tgTerrainFeatures)
            {
                MinimapManager.Get().simpleColor = !this.tgTerrainFeatures.isOn;
                MinimapManager.Get().dirty = true;
            }
            else if (s == this.tgGreenWizard || s == this.tgBlueWizard || s == this.tgRedWizard || s == this.tgPurpleWizard || s == this.tgYellowWizard || s == this.tgNeutrals || s == this.tgNodes || s == this.tgTowers || s == this.tgSoultrapped || s == this.tgBossLair)
            {
                this.UpdateSettings();
            }
        }

        public override void UpdateState()
        {
            base.UpdateState();
            global::WorldCode.Plane activePlane = World.GetActivePlane();
            if (activePlane != null && activePlane.battlePlane)
            {
                UIManager.Close(this);
            }
        }

        public override IEnumerator PostClose()
        {
            yield return base.PostClose();
            MinimapManager.Get().BigMapMode(b: false, default(MinimapManager.MapSettings));
            MinimapManager.Get().SetPlane(World.GetActivePlane());
            FOW.Get().SwitchFogMiniMapTo(World.GetActivePlane().arcanusType);
        }
    }
}
