using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class OutpostScreen : ScreenBase
    {
        public TextMeshProUGUI labelName;

        public TextMeshProUGUI labelRace;

        public Button btRaze;

        public Button btRename;

        public Button btClose;

        public EnchantmentGrid gridEnchantments;

        public ArmyGrid gridUnits;

        public GameObject goOutpostNormal;

        public GameObject goOutpostPrimitive;

        public GameObject goOutpostNature;

        public GameObject goOutpostMagical;

        public GameObject goOutpostWarlike;

        public GameObject goOutpostTech;

        public Image imgOutpostProgressNormal;

        public Image imgOutpostProgressPrimitive;

        public Image imgOutpostProgressNature;

        public Image imgOutpostProgressMagical;

        public Image imgOutpostProgressWarlike;

        public Image imgOutpostProgressTech;

        public RawImage riRace;

        private TownLocation town;

        public override IEnumerator PreStart()
        {
            yield return base.PreStart();
            this.town = FSMSelectionManager.Get().GetSelectedGroup() as TownLocation;
            this.UpdateText();
            this.UpdateVisuals();
            this.btRename.onClick.AddListener(delegate
            {
                PopupName.OpenPopup(this.town.name, delegate(object value)
                {
                    this.town.name = value as string;
                    this.UpdateText();
                }, null, this);
            });
            this.gridUnits.SetUnits(this.town.GetUnits());
            this.gridEnchantments.SetEnchantments(this.town.GetEnchantments());
            AudioLibrary.RequestSFX("OpenOutpostScreen");
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btRaze)
            {
                PopupGeneral.OpenPopup(this, this.town.GetName(), "UI_CONFIRM_RAZE_OUTPOST", "UI_RAZE", delegate
                {
                    this.town.Raze(GameManager.GetHumanWizard().GetID());
                    UIManager.Close(this);
                }, "UI_CANCEL");
            }
            else if (s == this.btClose)
            {
                UIManager.Close(this);
            }
        }

        public override IEnumerator PreClose()
        {
            yield return base.PreClose();
            List<Location> locationsOfWizard = GameManager.GetLocationsOfWizard(PlayerWizard.HumanID());
            if (locationsOfWizard == null)
            {
                yield break;
            }
            foreach (Location item in locationsOfWizard)
            {
                if (item is TownLocation)
                {
                    VerticalMarkerManager.Get().UpdateInfoOnMarker(item);
                }
            }
        }

        public void UpdateText()
        {
            this.labelName.text = this.town.name;
            this.labelRace.text = this.town.race.Get().GetDescriptionInfo().GetLocalizedName();
        }

        public void UpdateVisuals()
        {
            DBReference<Race> race = this.town.race;
            float fillAmount = this.town.OutpostProgression();
            this.riRace.texture = race.Get().GetDescriptionInfo().GetTexture();
            this.riRace.gameObject.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = race.Get().dbName;
            string text = "";
            text = race.Get().visualGroup;
            if (string.IsNullOrEmpty(text))
            {
                if (race == (Race)RACE.GNOLLS || race == (Race)RACE.NOMADS || race == (Race)RACE.BARBARIANS)
                {
                    text = "Primitive";
                }
                if (race == (Race)RACE.TROLLS || race == (Race)RACE.KLACKONS || race == (Race)RACE.LIZARDMEN)
                {
                    text = "Nature";
                }
                if (race == (Race)RACE.DRACONIANS || race == (Race)RACE.HIGH_ELVES || race == (Race)RACE.DARK_ELVES)
                {
                    text = "Magical";
                }
                if (race == (Race)RACE.DWARVES || race == (Race)RACE.ORCS || race == (Race)RACE.BEASTMEN)
                {
                    text = "Warlike";
                }
                else if (race == (Race)RACE.HALFLINGS || race == (Race)RACE.HIGH_MEN || string.IsNullOrEmpty(text))
                {
                    text = "";
                }
            }
            this.goOutpostNormal.SetActive(text == "");
            this.goOutpostPrimitive.SetActive(text == "Primitive");
            this.goOutpostNature.SetActive(text == "Nature");
            this.goOutpostMagical.SetActive(text == "Magical");
            this.goOutpostWarlike.SetActive(text == "Warlike");
            this.goOutpostTech.SetActive(text == "Tech");
            this.imgOutpostProgressTech.fillAmount = fillAmount;
            this.imgOutpostProgressPrimitive.fillAmount = fillAmount;
            this.imgOutpostProgressNature.fillAmount = fillAmount;
            this.imgOutpostProgressMagical.fillAmount = fillAmount;
            this.imgOutpostProgressWarlike.fillAmount = fillAmount;
            this.imgOutpostProgressNormal.fillAmount = fillAmount;
        }
    }
}
