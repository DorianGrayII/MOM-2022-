using System.Collections;
using DBDef;
using DBUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupTownCaptured : ScreenBase
    {
        public Button btRaze;

        public Button btKeep;

        public GridItemManager gridResources;

        public TextMeshProUGUI labelConquestText;

        public TextMeshProUGUI labelKeepHeading;

        public TextMeshProUGUI labelKeepFame;

        public TextMeshProUGUI labelKeepGold;

        public TextMeshProUGUI labelKeepOutcome;

        public TextMeshProUGUI labelRazeHeading;

        public TextMeshProUGUI labelRazeFame;

        public TextMeshProUGUI labelRazeGold;

        public TextMeshProUGUI labelRazeOutcome;

        public TextMeshProUGUI labelTownName;

        public TextMeshProUGUI labelRace;

        public TextMeshProUGUI labelPopulation;

        public TextMeshProUGUI labelBuildingsCount;

        public RawImage raceIcon;

        public GameObject goKeepTown;

        public GameObjectEnabler<PlayerWizard.Familiar> familiar;

        private TownLocation town;

        public override IEnumerator PreStart()
        {
            yield return base.PreStart();
            this.town = TownLocation.lastBattle;
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            this.gridResources.SetListItems(this.town.GetResourceAndState());
            this.labelConquestText.text = global::DBUtils.Localization.Get("UI_" + this.town.GetTownSize().ToString().ToUpperInvariant() + "_CONQUERED", true);
            string text = this.town.GetName();
            this.labelTownName.text = text;
            this.labelKeepHeading.text = global::DBUtils.Localization.Get("UI_KEEP_TOWN", true, text);
            this.labelRace.text = this.town.race.Get().GetDescriptionInfo().GetLocalizedName();
            this.labelPopulation.text = this.town.Population.ToString();
            this.labelBuildingsCount.text = this.town.buildings.Count.ToString();
            this.labelKeepGold.text = this.town.ConquerGold().ToString();
            this.labelKeepOutcome.text = global::DBUtils.Localization.Get("UI_KEEP_OUTCOME", true, text);
            this.labelRazeHeading.text = global::DBUtils.Localization.Get("UI_RAZE_TOWN", true, text);
            if (humanWizard.traitThePirat > 0)
            {
                this.labelRazeFame.text = "0";
                this.labelRazeFame.text = this.town.ConquerFame().ToString();
                this.labelKeepFame.text = (-this.town.RazeFame()).ToString();
                this.labelRazeGold.text = (this.town.RazeGold() * humanWizard.traitThePirat + this.town.ConquerGold()).ToString();
            }
            else
            {
                this.labelKeepFame.text = this.town.ConquerFame().ToString();
                this.labelRazeFame.text = (-this.town.RazeFame()).ToString();
                this.labelRazeGold.text = (this.town.RazeGold() + this.town.ConquerGold()).ToString();
            }
            this.labelRazeOutcome.text = global::DBUtils.Localization.Get("UI_RAZE_OUTCOME", true, text);
            this.raceIcon.texture = this.town.race.Get().GetDescriptionInfo().GetTexture();
            this.familiar.Set(humanWizard.familiar);
            AudioLibrary.RequestSFX("PopupTownCaptured");
            if (this.town.IsAnOutpost())
            {
                this.btKeep.gameObject.SetActive(value: false);
                this.goKeepTown.SetActive(value: false);
            }
        }

        protected override void ButtonClick(Selectable s)
        {
            if (s == this.btRaze)
            {
                this.town.Raze(GameManager.GetHumanWizard().GetID());
            }
            else
            {
                this.town.Conquer();
                HUD.Get()?.UpdateHUD();
            }
            base.ButtonClick(s);
        }
    }
}
