using System;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MOM
{
    public class TooltipArtefact : TooltipBase
    {
        public TextMeshProUGUI artefactName;

        public TextMeshProUGUI RMBInfo;

        public GridItemManager gridEnchantments;

        public RawImage icon;

        public GameObject goRMBInfo;

        [NonSerialized]
        public Artefact artefact;

        public override void Populate(object o)
        {
            this.artefact = o as Artefact;
            this.artefactName.text = this.artefact.name;
            this.RMBInfo.text = global::DBUtils.Localization.Get("UI_CLICK_TO_DISMANTLE", true);
            this.icon.texture = AssetManager.Get<Texture2D>(this.artefact.graphic);
            this.gridEnchantments.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                itemSource.GetComponentInChildren<TextMeshProUGUI>().text = (source as DBReference<ArtefactPower>).Get().skill.GetDescriptionInfo().GetLocalizedName();
            }, delegate
            {
                this.gridEnchantments.UpdateGrid(this.artefact.artefactPowers);
            });
            this.gridEnchantments.UpdateGrid(this.artefact.artefactPowers);
            if (UIManager.GetScreen<AdventureScreen>(UIManager.Layer.Standard) != null || UIManager.GetScreen<Trade>(UIManager.Layer.Standard) != null)
            {
                this.goRMBInfo.SetActive(value: false);
            }
        }
    }
}
