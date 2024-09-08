using System.Collections;
using DBDef;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupTownPreview : ScreenBase
    {
        public Button btClose;

        public ArmyGrid gridUnits;

        public EnchantmentGrid gridEnchantments;

        public RawImage riRaceIcon;

        public TextMeshProUGUI labelTownName;

        public TextMeshProUGUI labelFarmers;

        public TextMeshProUGUI labelWorkers;

        public TextMeshProUGUI labelRebels;

        public TextMeshProUGUI labelRace;

        public GameObject unitsNoInfo;

        public GameObject townMapArcanus;

        public GameObject townMapMyrror;

        public GameObject units;

        public GameObject pTownMapArcanus;

        public GameObject pTownMapMyrror;

        public Owner owner;

        private bool detailVisible;

        public override IEnumerator PreStart()
        {
            if (FSMSelectionManager.Get().GetSelectedGroup() is TownLocation townLocation)
            {
                this.detailVisible = townLocation.CanSeeInside();
                this.gridUnits.transform.parent.gameObject.SetActive(this.detailVisible);
                this.unitsNoInfo.SetActive(!this.detailVisible);
                if (this.detailVisible)
                {
                    this.gridUnits.SetUnits(townLocation.GetUnits());
                }
                this.labelTownName.text = townLocation.name;
                this.labelFarmers.text = townLocation.GetFarmers().ToString();
                this.labelWorkers.text = townLocation.GetWorkers().ToString();
                this.labelRebels.text = townLocation.GetRebels().ToString();
                this.riRaceIcon.texture = townLocation.race.Get().GetDescriptionInfo().GetTexture();
                this.labelRace.text = townLocation.race.Get().GetDescriptionInfo().GetLocalizedName();
                this.owner.SetWizard(townLocation.GetWizardOwner());
                bool arcanusType = townLocation.GetPlane().arcanusType;
                this.townMapArcanus.SetActive(arcanusType && this.detailVisible);
                this.townMapMyrror.SetActive(!arcanusType && this.detailVisible);
                GameObject gameObject = (arcanusType ? this.townMapArcanus : this.townMapMyrror);
                if (gameObject.transform.childCount < 1)
                {
                    GameObjectUtils.Instantiate(arcanusType ? this.pTownMapArcanus : this.pTownMapMyrror, gameObject.transform);
                }
                gameObject.GetComponentInChildren<TownMap>().SetTown(townLocation);
                this.gridEnchantments.SetEnchantments(townLocation.GetEnchantmentManager().GetEnchantments());
                AudioLibrary.RequestSFX("OpenTownPreview");
            }
            yield return base.PreStart();
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btClose)
            {
                UIManager.Close(this);
            }
        }
    }
}
