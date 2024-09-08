using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class PopupPurchaseArtefact : ScreenBase
    {
        public Button btBuy;

        public Button btReject;

        public TextMeshProUGUI gold;

        public TextMeshProUGUI mana;

        public TextMeshProUGUI food;

        public TextMeshProUGUI fame;

        public GridItemManager gridBonuses;

        public RawImage riArtefactIcon;

        public TextMeshProUGUI labelDescription;

        public Artefact item;

        private Callback callback;

        public static void OpenPopup(ScreenBase parent, Artefact item, Callback cb)
        {
            PopupPurchaseArtefact popupPurchaseArtefact = UIManager.Open<PopupPurchaseArtefact>(UIManager.Layer.Popup, parent);
            popupPurchaseArtefact.labelDescription.text = global::DBUtils.Localization.Get("UI_A_MERCHANT_ARRIVES", true, item.name, item.GetValue());
            popupPurchaseArtefact.item = item;
            popupPurchaseArtefact.callback = cb;
            popupPurchaseArtefact.riArtefactIcon.texture = AssetManager.Get<Texture2D>(item.graphic);
            popupPurchaseArtefact.gridBonuses.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                DBReference<ArtefactPower> dBReference = source as DBReference<ArtefactPower>;
                TextMeshProUGUI textMeshProUGUI = GameObjectUtils.FindByNameGetComponent<TextMeshProUGUI>(itemSource, "LabelBonus");
                RolloverSimpleTooltip rolloverSimpleTooltip = GameObjectUtils.FindByNameGetComponent<RolloverSimpleTooltip>(itemSource, "LabelBonus");
                textMeshProUGUI.text = dBReference.Get().skill.descriptionInfo.GetLocalizedName();
                rolloverSimpleTooltip.sourceAsDbName = dBReference.Get().skill.dbName;
                rolloverSimpleTooltip.useMouseLocation = false;
                rolloverSimpleTooltip.anchor.x = 0.5f;
                rolloverSimpleTooltip.anchor.y = 1f;
                rolloverSimpleTooltip.offset.x = 220f;
            });
            popupPurchaseArtefact.gridBonuses.UpdateGrid(item.artefactPowers);
            popupPurchaseArtefact.UpdateResources();
            AudioLibrary.RequestSFX("OpenPurchaseArtefact");
        }

        protected override void ButtonClick(Selectable s)
        {
            base.ButtonClick(s);
            if (s == this.btReject)
            {
                UIManager.Close(this);
                if (this.callback != null)
                {
                    this.callback(null);
                }
            }
            else if (s == this.btBuy)
            {
                GameManager.GetHumanWizard().artefacts.Add(this.item);
                GameManager.GetHumanWizard().money -= this.item.GetValue();
                UIManager.Close(this);
                if (this.callback != null)
                {
                    this.callback(null);
                }
            }
        }

        private void UpdateResources()
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            this.gold.text = humanWizard.money + "(" + humanWizard.CalculateMoneyIncome(includeUpkeep: true).SInt() + ")";
            this.mana.text = humanWizard.GetMana() + "(" + humanWizard.CalculateManaIncome(includeUpkeep: true).SInt() + ")";
            this.food.text = humanWizard.CalculateFoodIncome(includeUpkeep: true).SInt();
            this.fame.text = humanWizard.GetFame().ToString();
        }
    }
}
